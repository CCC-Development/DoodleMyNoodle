using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using System;
using UnityEngine.Scripting.APIUpdating;

[Serializable]
[GameActionSettingAuth(typeof(GameActionSettingThrow))]
[MovedFrom(false, sourceClassName: "GameActionThrowSettingsAuth")]
public class GameActionSettingThrowAuth : GameActionSettingAuthBase
{
    public fix SpeedMin = 0;
    public fix SpeedMax = 10;
    public fix SpawnExtraDistance = 0;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionSettingThrow()
        {
            SpeedMax = SpeedMax,
            SpeedMin = SpeedMin,
            SpawnExtraDistance = SpawnExtraDistance,
        });
    }
}