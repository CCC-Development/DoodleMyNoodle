﻿using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;
using static fixMath;

public class GameActionMeleeAttack : GameAction
{
    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return new UseContract(
            new GameActionParameterTile.Description(accessor.GetComponentData<ItemRangeData>(context.Entity).Value)
            {
                IncludeSelf = false,
                RequiresAttackableEntity = true,
            });
    }

    public override bool IsContextValid(ISimWorldReadAccessor accessor, in UseContext context)
    {
        // Cooldown
        if (accessor.HasComponent<ItemCooldownCounter>(context.Entity) && accessor.GetComponentData<ItemCooldownCounter>(context.Entity).Value > 0)
        {
            return false;
        }

        return accessor.HasComponent<ActionPoints>(context.InstigatorPawn)
            && accessor.HasComponent<FixTranslation>(context.InstigatorPawn);
    }

    public override void Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            int2 instigatorTile = roundToInt(accessor.GetComponentData<FixTranslation>(context.InstigatorPawn).Value).xy;

            // melee attack has a range of RANGE
            if (lengthmanhattan(paramTile.Tile - instigatorTile) > accessor.GetComponentData<ItemRangeData>(context.Entity).Value)
            {
                LogGameActionInfo(context, $"Melee attack at {paramTile.Tile} out of range. Ignoring.");
                return;
            }

            accessor.SetOrAddComponentData(context.Entity, new ItemCooldownCounter() { Value = accessor.GetComponentData<ItemCooldownData>(context.Entity).Value });

            // reduce target health
            NativeList<Entity> victims = new NativeList<Entity>(Allocator.Temp);
            CommonReads.FindTileActorsWithComponents<Health>(accessor, paramTile.Tile, victims);
            foreach (Entity entity in victims)
            {
                CommonWrites.RequestDamageOnTarget(accessor, context.InstigatorPawn, entity, accessor.GetComponentData<ItemDamageData>(context.Entity).Value);
            }
        }
    }
}
