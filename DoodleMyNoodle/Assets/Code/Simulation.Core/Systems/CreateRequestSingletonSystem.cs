using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using System;

public struct SystemRequestsSingletonTag : IComponentData { }
public interface ISystemRequestData : IBufferElementData { }

[UpdateInGroup(typeof(InitializationSystemGroup))]
[AlwaysUpdateSystem]
public class CreateRequestSingletonSystem : SimSystemBase
{
    public static ComponentType[] SystemRequestTypes;

    private EntityArchetype _singletonArchetype;

    protected override void OnCreate()
    {
        base.OnCreate();

        List<ComponentType> types = new List<ComponentType>();

        types.Add(typeof(SystemRequestsSingletonTag));

        foreach (var item in TypeManager.AllTypes)
        {
            if (typeof(ISystemRequestData).IsAssignableFrom(item.Type) && item.Category == TypeManager.TypeCategory.BufferData)
            {
                types.Add(new ComponentType(item.Type));
            }
        }

        _singletonArchetype = EntityManager.CreateArchetype(types.ToArray());
    }

    protected override void OnUpdate()
    {
        if (!HasSingleton<SystemRequestsSingletonTag>())
        {
            // create singleton
            EntityManager.CreateEntity(_singletonArchetype);
        }
        else if (_singletonArchetype.ChunkCount == 0)
        {
            // adjust singleton
            var entity = GetSingletonEntity<SystemRequestsSingletonTag>();
            EntityManager.SetArchetype(entity, _singletonArchetype);
        }
    }
}