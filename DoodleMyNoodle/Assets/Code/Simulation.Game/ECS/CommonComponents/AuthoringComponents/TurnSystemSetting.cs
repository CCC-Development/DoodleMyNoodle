﻿using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class TurnSystemSetting : MonoBehaviour, IConvertGameObjectToEntity
{
    public enum Team
    {
        Players = 0,
        AI = 1
    }

    public Team StartingTeam = Team.Players;
    public fix TurnDuration = 10;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new TurnDuration { Value = TurnDuration });

        dstManager.AddComponentData(entity, new TurnCurrentTeam { Value = (int)StartingTeam });

        dstManager.AddComponentData(entity, new TurnTeamCount { Value = (int)Enum.GetValues(typeof(Team)).Length - 1 });

        dstManager.AddComponentData(entity, new TurnTimer { Value = TurnDuration });
    }
}