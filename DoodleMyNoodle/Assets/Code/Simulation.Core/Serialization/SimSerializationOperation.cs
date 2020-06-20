﻿using CCC.Operations;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Entities.Serialization;

namespace Sim.Operations
{
    // This operation launches the real 'SimSerializationOperation' and caches it.
    // If there is already one for the current tick id, it'll take the existing cached operation insteading of launching a new one
    public class SimSerializationOperationWithCache : CoroutineOperation
    {
        internal static SimSerializationOperation s_CachedSerializationOp;
        internal static uint s_CachedSerializationTickId = uint.MaxValue;

        public byte[] SerializationData;

        private World _world;

        internal SimSerializationOperationWithCache(World world)
        {
            _world = world;
        }

        protected override IEnumerator ExecuteRoutine()
        {
            if (_world is SimulationWorld simWorld && simWorld.GetLastedTickIdFromEntity() != s_CachedSerializationTickId)
            {
                s_CachedSerializationOp = new SimSerializationOperation(_world);
                s_CachedSerializationOp.Execute();
                s_CachedSerializationTickId = simWorld.GetLastedTickIdFromEntity();
            }

            if (s_CachedSerializationOp.IsRunning)
            {
                DebugService.Log($"ExecuteSubOperationAndWaitForSuccess(s_CachedSerializationOp)...");
                yield return ExecuteSubOperationAndWaitForSuccess(s_CachedSerializationOp);
            }

            DebugService.Log($"SerializationData = s_CachedSerializationOp.SerializationData");
            SerializationData = s_CachedSerializationOp.SerializationData;

            if (s_CachedSerializationOp.HasSucceeded)
                TerminateWithSuccess(s_CachedSerializationOp.Message);
            else
                TerminateWithAbnormalFailure(s_CachedSerializationOp.Message);
        }
    }


    public class SimSerializationOperation : CoroutineOperation
    {
        public byte[] SerializationData;

        World _simulationWorld;

        internal SimSerializationOperation(World simulationWorld)
        {
            _simulationWorld = simulationWorld;
        }

        byte[] GetByteArrayFromBinaryWriter(MemoryBinaryWriter binaryWriter)
        {
            byte[] arr = new byte[binaryWriter.Length];

            unsafe
            {
                Marshal.Copy((IntPtr)binaryWriter.Data, arr, 0, binaryWriter.Length);
            }

            return arr;
        }

        protected override IEnumerator ExecuteRoutine()
        {
            DebugService.Log("Start Serialization Process...");

            using (var binaryWriter = new MemoryBinaryWriter())
            {
                SerializeUtility.SerializeWorld(_simulationWorld.EntityManager, binaryWriter, out object[] referencedObjects);

                foreach (var obj in referencedObjects)
                {
                    DebugService.LogWarning($"The ECS simulation references {obj}, which is a managed object. " +
                        $"This is not allowed for now due to serialization");
                }

                SerializationData = GetByteArrayFromBinaryWriter(binaryWriter);
            }

            TerminateWithSuccess();
            yield break;
        }
    }
}