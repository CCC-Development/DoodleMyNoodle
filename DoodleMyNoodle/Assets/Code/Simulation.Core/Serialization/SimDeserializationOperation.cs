﻿using CCC.Operations;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sim.Operations
{
    public class SimDeserializationOperation : CoroutineOperation
    {
        public bool PartialSuccess;

        string _serializedData;
        SimObjectJsonConverter _simObjectJsonConverter;
        JsonSerializerSettings _jsonSerializerSettings;

        internal SimDeserializationOperation(string serializedData, SimObjectJsonConverter simObjectJsonConverter, JsonSerializerSettings jsonSerializerSettings)
        {
            _serializedData = serializedData;
            _simObjectJsonConverter = simObjectJsonConverter;
            _jsonSerializerSettings = jsonSerializerSettings;
        }

        protected override IEnumerator ExecuteRoutine()
        {
            SimSerializableWorld serializableWorld = null;

            DebugService.Log("Start Deserialization Process...");
            // Deserialize the json data on a thread
            //  'yield return' will cause unity to continue to the next frame until the deserialization is done
            yield return JsonSerializationUtility.DeserializeThreaded<SimSerializableWorld>(_serializedData, _jsonSerializerSettings,
                (JsonSerializationUtility.DeserializationResult result) =>
                {
                    if (result.Success)
                    {
                        serializableWorld = (SimSerializableWorld)result.Object;
                        DebugService.Log($"SerializableSimWorld correctly deserialized");
                    }
                    else
                    {
                        DebugService.LogError($"Failed to deserialize SimSerializableWorld.\n{result.ErrorMessage}");
                        serializableWorld = null;
                    }
                });

            // early exit if world invalid
            if (serializableWorld == null)
            {
                TerminateWithFailure();
                yield break;
            }

            SimWorld world = SimModules._World;

            ////////////////////////////////////////////////////////////////////////////////////////
            //      Delete existing entites
            ////////////////////////////////////////////////////////////////////////////////////////
            DebugService.Log($"Destroying existing entities");
            ClearCurrentEntities(world);


            ////////////////////////////////////////////////////////////////////////////////////////
            //      Begin filling data in SimWorld
            ////////////////////////////////////////////////////////////////////////////////////////
            DebugService.Log($"Filling new world basic info");
            world.TickId = serializableWorld.TickId;
            world.Seed = serializableWorld.Seed;
            world.NextObjectId = serializableWorld.NextObjectId;

            SimModules._Ticker.OnTickUpdated();

            ////////////////////////////////////////////////////////////////////////////////////////
            //      Aquire necessary blueprints to reconstruct entites
            ////////////////////////////////////////////////////////////////////////////////////////
            DebugService.Log($"Aquiring necessary blueprints to reconstruct entites...");
            SimBlueprint[] resultBlueprints = new SimBlueprint[0];

            yield return SimModules._BlueprintManager.ProvideBlueprintBatched(serializableWorld.ReferencedBlueprints, (x) => resultBlueprints = x);

            ////////////////////////////////////////////////////////////////////////////////////////
            //      Reconstruct entities
            ////////////////////////////////////////////////////////////////////////////////////////
            DebugService.Log($"Reconstructing entites ({serializableWorld.Entities.Count})...");
            int reconstructCount = serializableWorld.Entities.Count;
            Dictionary<SimObjectId, SimObject> allSimObjects = new Dictionary<SimObjectId, SimObject>(reconstructCount * 4); // expecting ~4 components per entity
            List<SimComponent> reconstructedComponents = new List<SimComponent>();
            SimSerializableWorld deletethis = serializableWorld;

            for (int i = 0; i < serializableWorld.Entities.Count; i++)
            {
                SimEntity reconstructedEntity = null;
                SimSerializableWorld.Entity serializedEntity = serializableWorld.Entities[i];

                SimBlueprint blueprint = resultBlueprints.TryGetAt(serializedEntity.BlueprintIdIndex);

                if (blueprint.IsValid)
                {
                    reconstructedEntity = blueprint.Prefab.DuplicateGO();
                    reconstructedEntity.SimObjectId = serializedEntity.Id;
                    reconstructedEntity.gameObject.name = serializedEntity.Name;
                    reconstructedEntity.gameObject.SetActive(serializedEntity.Active);
                    reconstructedEntity.BlueprintId = blueprint.Id; // is this necessary ?
                    reconstructedEntity.GetComponents(reconstructedComponents);

                    for (int c = serializedEntity.Components.Count - 1; c >= 0; c--)
                    {
                        SimSerializableWorld.Entity.Component serializedComponent = serializedEntity.Components[c];

                        if(serializedComponent.Type == null)
                        {
                            PartialSuccess = true;
                            DebugService.LogWarning($"Failed to deserialize type for component[{c}] of entity '{serializableWorld.Entities[i].Name}'. Reconstruction skipped.");
                            continue;
                        }

                        int componentIndex = reconstructedComponents.IndexOf(serializedComponent.Type);
                        SimComponent comp;
                        if (componentIndex != -1)
                        {
                            comp = reconstructedComponents[componentIndex];
                            reconstructedComponents.RemoveAt(componentIndex);
                        }
                        else
                        {
                            // component was not on the original blueprint, add it
                            comp = (SimComponent)reconstructedEntity.gameObject.AddComponent(serializedComponent.Type);
                        }


                        comp.SimObjectId = serializedComponent.Id;
                        comp.enabled = serializedComponent.Enabled;

                        if (comp.SimObjectId.IsValid)
                        {
                            allSimObjects.Add(comp.SimObjectId, comp);
                        }
                    }

                    // remove components that were not found in the saved data
                    // (they are on the original blueprint, but they were probably removed in gameplay)
                    for (int c = 0; c < reconstructedComponents.Count; c++)
                    {
                        UnityEngine.Object.Destroy(reconstructedComponents[c]);
                    }

                    // cache SimObject
                    allSimObjects.Add(reconstructedEntity.SimObjectId, reconstructedEntity);
                }
                else
                {
                    PartialSuccess = true;
                    DebugService.LogWarning($"Invalid blueprint (index:{i}), entity '{serializableWorld.Entities[i].Name}' reconstruction skipped");
                }

                // We might add a null entity. We do that to maintain index parity between world.Entites and serializedWorld.Entities
                // We clear null entities afterwards.
                world.Entities.Add(reconstructedEntity);
            }

            ////////////////////////////////////////////////////////////////////////////////////////
            //      Release blueprints that were loaded for the reconstructions
            ////////////////////////////////////////////////////////////////////////////////////////
            DebugService.Log($"Releasing blueprints loaded for reconstruction...");
            SimModules._BlueprintManager.ReleaseBatchedBlueprints();

            ////////////////////////////////////////////////////////////////////////////////////////
            //      Deserialize all component data stack
            ////////////////////////////////////////////////////////////////////////////////////////
            DebugService.Log($"Deserializing component data stack...");
            _simObjectJsonConverter.SimObjectsReferenceTable = allSimObjects; // necessary for json to reassign references correctly
            _simObjectJsonConverter.AvailableBlueprints = resultBlueprints;

            SimComponentDataStack componentDataStack;
            {
                // NB: cannot be threaded for now because our json converter accesses Unity API which is main thread bound. Could probably be changed!
                JsonSerializationUtility.DeserializationResult result = JsonSerializationUtility.Deserialize<SimComponentDataStack>(
                    serializableWorld.SerializedComponentDataStack,
                    _jsonSerializerSettings);

                if (!result.Success)
                {
                    TerminateWithFailure();
                    DebugService.LogError($"Failed to deserialize SimComponentDataStack.\n{result.ErrorMessage}");
                    yield break;
                }

                componentDataStack = (SimComponentDataStack)result.Object;
            }

            ////////////////////////////////////////////////////////////////////////////////////////
            //      Refill components with data (from data stack)
            ////////////////////////////////////////////////////////////////////////////////////////
            DebugService.Log($"Refilling components' data...");
            for (int e = serializableWorld.Entities.Count - 1; e >= 0; e--)
            {
                SimEntity reconstructedEntity = world.Entities[e];
                if (reconstructedEntity != null) // skip over invalid entities
                {
                    reconstructedEntity.GetComponents(reconstructedComponents);
                    for (int c = reconstructedComponents.Count - 1; c >= 0; c--)
                    {
                        try
                        {
                            reconstructedComponents[c].PopFromDataStack(componentDataStack);
                        }
                        catch (Exception error)
                        {
                            DebugService.LogWarning($"Failed to deserialize {reconstructedEntity.gameObject.name}'s {reconstructedComponents[c].GetType()} component: {error.Message} / {error.StackTrace}." +
                                $"\nComponent data will stay at default values.");
                            PartialSuccess = true;
                        }
                    }
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////
            //      Remove null entities
            ////////////////////////////////////////////////////////////////////////////////////////
            DebugService.Log($"Clearing entities that failed to be reconstructed...");
            world.Entities.RemoveNulls();

            ////////////////////////////////////////////////////////////////////////////////////////
            //      Add all reconstructed entities to runtime
            ////////////////////////////////////////////////////////////////////////////////////////
            DebugService.Log($"Adding reconstructed entities to runtime...");
            world.Entities.ForEach((x) => SimModules._EntityManager.AddEntityToRuntime(x));

            // terminado!
            DebugService.Log($"Deserialization complete! sim at tick {world.TickId}");

            TerminateWithSuccess();
        }

        void ClearCurrentEntities(SimWorld world)
        {
            for (int i = 0; i < world.Entities.Count; i++)
            {
                SimModules._EntityManager.RemoveEntityFromRuntime(world.Entities[i]);
                world.Entities[i].DestroyGO();
            }
            world.Entities.Clear();
        }
    }
}