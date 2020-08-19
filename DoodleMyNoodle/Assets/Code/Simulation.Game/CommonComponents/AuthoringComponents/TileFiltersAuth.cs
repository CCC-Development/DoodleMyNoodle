﻿using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class TileFiltersAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public bool IsTileNavigable = false;
    public bool IsTileAscendable = false;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (!IsTileNavigable)
        {
            dstManager.AddComponentData(entity, new SolidWallTag());
        }

        if (IsTileAscendable)
        {
            dstManager.AddComponentData(entity, new AscendableTag());
        }
    }
}