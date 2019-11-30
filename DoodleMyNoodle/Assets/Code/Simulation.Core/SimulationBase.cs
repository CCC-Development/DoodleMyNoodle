﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class SimulationBase
{
    /// <summary>
    /// Is the simulation running or ready to run ? 
    /// </summary>
    public static bool IsRunningOrReadyToRun => 
        SimModules._World != null
        && SimModules._Serializer.IsInDeserializationProcess == false;

    /// <summary>
    /// The simulation tick delta time. Should be constant for the simulation to stay deterministic
    /// </summary>
    public static readonly Fix64 DeltaTime = SimulationConstants.TIME_STEP; // 50 ticks per seconds

    /// <summary>
    /// The current simulation tick id. Increments by 1 every Tick()
    /// </summary>
    public static uint TickId => SimModules._World.TickId;
    public static void Tick(in SimTickData tickData) => SimModules._Ticker.Tick(tickData);

    public static SimBlueprint GetBlueprint(in SimBlueprintId blueprintId) => SimModules._BlueprintManager.GetBlueprint(blueprintId);

    public static SimEntity FindEntityWithName(string name) => SimModules._WorldSearcher.FindEntityWithName(name);
    public static SimEntity FindEntityWithComponent<T>() => SimModules._WorldSearcher.FindEntityWithComponent<T>();
    public static SimEntity FindEntityWithComponent<T>(out T comp) => SimModules._WorldSearcher.FindEntityWithComponent(out comp);

    public static ReadOnlyList<SimEntity>          Entities                                 => new ReadOnlyList<SimEntity>(SimModules._World.Entities);
    public static EntityEnumerable<T1>             EntitiesWithComponent<T1>()              => new EntityEnumerable<T1>(SimModules._World.Entities);
    public static EntityEnumerable<T1, T2>         EntitiesWithComponents<T1, T2>()         => new EntityEnumerable<T1, T2>(SimModules._World.Entities);
    public static EntityEnumerable<T1, T2, T3>     EntitiesWithComponents<T1, T2, T3>()     => new EntityEnumerable<T1, T2, T3>(SimModules._World.Entities);
    public static EntityEnumerable<T1, T2, T3, T4> EntitiesWithComponents<T1, T2, T3, T4>() => new EntityEnumerable<T1, T2, T3, T4>(SimModules._World.Entities);
}