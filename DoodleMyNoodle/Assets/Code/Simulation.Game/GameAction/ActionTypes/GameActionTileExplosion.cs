using static fixMath;
using Unity.Entities;
using Unity.Collections;
using System;
using CCC.Fix2D;

public class GameActionTileExplosion : GameAction
{
    public override Type[] GetRequiredSettingTypes() => new Type[]
    {
        typeof(GameActionSettingDamage),
        typeof(GameActionSettingRadius),
        typeof(GameActionSettingRange),
    };

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        var range = accessor.GetComponentData<GameActionSettingRange>(context.Item);
        return new UseContract(
                   new GameActionParameterPosition.Description()
                   {
                       MaxRangeFromInstigator = range
                   });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterPosition.Data paramPosition))
        {
            fix2 instigatorPos = accessor.GetComponentData<FixTranslation>(context.InstigatorPawn);
            int damage = accessor.GetComponentData<GameActionSettingDamage>(context.Item).Value;
            fix range = accessor.GetComponentData<GameActionSettingRange>(context.Item).Value;
            fix radius = accessor.GetComponentData<GameActionSettingRadius>(context.Item).Value;

            fix2 pos = Helpers.ClampPositionInsideRange(paramPosition.Position, instigatorPos, range);
            CommonWrites.RequestExplosion(accessor, context.InstigatorPawn, pos, radius, damage);

            return true;
        }

        return false;
    }
}