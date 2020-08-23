﻿using Unity.Entities;
using static Unity.Mathematics.math;
using static fixMath;
using System.Collections.Generic;

public interface IStatInt
{
    int Value { get; set; }
}

public interface IStatFix
{
    fix Value { get; set; }
}

public struct MinimumInt<T> : IComponentData
    where T : IComponentData, IStatInt
{
    public int Value;
}

public struct MaximumInt<T> : IComponentData
    where T : IComponentData, IStatInt
{
    public int Value;
}

public struct MinimumFix<T> : IComponentData
    where T : IComponentData, IStatFix
{
    public fix Value;
}

public struct MaximumFix<T> : IComponentData
    where T : IComponentData, IStatFix
{
    public fix Value;
}

internal static partial class CommonWrites
{
    public static void ModifyStatInt<T>(ISimWorldReadWriteAccessor accessor, Entity entity, int value)
        where T : struct, IComponentData, IStatInt
    {
        int currentValue = accessor.GetComponentData<T>(entity).Value;
        int newValue = currentValue + value;

        SetStatInt(accessor, entity, new T { Value = newValue });
    }

    public static void ModifyStatFix<T>(ISimWorldReadWriteAccessor accessor, Entity entity, int value)
        where T : struct, IComponentData, IStatFix
    {
        fix currentValue = accessor.GetComponentData<T>(entity).Value;
        fix newValue = currentValue + value;

        SetStatFix(accessor, entity, new T { Value = newValue });
    }

    public static void SetStatInt<T>(ISimWorldReadWriteAccessor accessor, Entity entity, T compData)
        where T : struct, IComponentData, IStatInt
    {
        if (accessor.TryGetComponentData(entity, out MinimumInt<T> minimum))
        {
            compData.Value = max(minimum.Value, compData.Value);
        }

        if (accessor.TryGetComponentData(entity, out MaximumInt<T> maximum))
        {
            compData.Value = min(maximum.Value, compData.Value);
        }

        accessor.SetComponentData(entity, compData);
    }

    public static void SetStatFix<T>(ISimWorldReadWriteAccessor accessor, Entity entity, T compData)
        where T : struct, IComponentData, IStatFix
    {
        if (accessor.TryGetComponentData(entity, out MinimumFix<T> minimum))
        {
            compData.Value = max(minimum.Value, compData.Value);
        }

        if (accessor.TryGetComponentData(entity, out MaximumFix<T> maximum))
        {
            compData.Value = min(maximum.Value, compData.Value);
        }

        accessor.SetComponentData(entity, compData);
    }
}

