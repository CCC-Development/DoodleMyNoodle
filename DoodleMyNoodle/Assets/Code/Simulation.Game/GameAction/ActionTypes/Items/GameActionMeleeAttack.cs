﻿using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;
using static fixMath;
using System.Collections.Generic;
using UnityEngine;

public class GameActionMeleeAttack : GameAction
{
    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        GameActionParameterTile.Description tileParam = new GameActionParameterTile.Description(accessor.GetComponentData<ItemRangeData>(context.Entity).Value)
        {
            IncludeSelf = false,
            RequiresAttackableEntity = true,
        };

        int2 position = Helpers.GetTile(accessor.GetComponentData<FixTranslation>(context.InstigatorPawn).Value);
        GameActionParameterMiniGame.Description miniGameParam = new GameActionParameterMiniGame.Description(position);

        return new UseContract(tileParam, miniGameParam);
    }

    protected override bool CanBeUsedInContextSpecific(ISimWorldReadAccessor accessor, in UseContext context, DebugReason debugReason)
    {
        return true;
    }

    protected override int GetMinimumActionPointCost(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return accessor.GetComponentData<ItemActionPointCostData>(context.Entity).Value;
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            if (useData.TryGetParameter(1, out GameActionParameterMiniGame.Data paramMiniGame))
            {
                int2 instigatorTile = Helpers.GetTile(accessor.GetComponentData<FixTranslation>(context.InstigatorPawn));

                // melee attack has a range of RANGE
                if (lengthmanhattan(paramTile.Tile - instigatorTile) > accessor.GetComponentData<ItemRangeData>(context.Entity).Value)
                {
                    LogGameActionInfo(context, $"Melee attack at {paramTile.Tile} out of range. Ignoring.");
                    return false;
                }

                // reduce target health
                NativeList<Entity> victims = new NativeList<Entity>(Allocator.Temp);
                CommonReads.FindTileActorsWithComponents<Health>(accessor, paramTile.Tile, victims);

                int damageValue = accessor.GetComponentData<ItemDamageData>(context.Entity).Value;

                if (((int)paramMiniGame.SuccessRate) < 3)
                {
                    damageValue = 0;
                } 

                foreach (Entity entity in victims)
                {
                    CommonWrites.RequestDamageOnTarget(accessor, context.InstigatorPawn, entity, damageValue);
                }

                int2 attackDirection = paramTile.Tile - instigatorTile;
                CommonWrites.SetEntityAnimation(accessor, context.InstigatorPawn, CommonReads.AnimationTypes.Attack, new KeyValuePair<string, object>("Direction", attackDirection));

                return true;
            }
        }

        return false;
    }
}
