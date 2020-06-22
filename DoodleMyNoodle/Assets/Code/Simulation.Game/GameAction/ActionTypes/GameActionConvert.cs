using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Entities;
using Unity.Collections;

public class GameActionConvert : GameAction
{
    // TODO: add settings on the item itself
    const int AP_COST = 6;
    const int RANGE = 5;
    const int DURATION = 2;

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return new UseContract(
            new GameActionParameterTile.Description()
            {
                Filter = TileFilterFlags.Occupied | TileFilterFlags.NotEmpty,
                RangeFromInstigator = RANGE
            });
    }

    public override bool IsContextValid(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return accessor.HasComponent<ActionPoints>(context.InstigatorPawn)
            && accessor.HasComponent<FixTranslation>(context.InstigatorPawn);
    }

    public override void Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            if (accessor.GetComponentData<ActionPoints>(context.InstigatorPawn).Value < AP_COST)
            {
                return;
            }

            // reduce instigator AP
            CommonWrites.ModifyStatInt<ActionPoints>(accessor, context.InstigatorPawn, -AP_COST);

            // find target
            NativeList<Entity> victims = new NativeList<Entity>(Allocator.Temp);
            CommonReads.FindEntitiesOnTileWithComponent<ControllableTag>(accessor, paramTile.Tile, victims);
            foreach (Entity entity in victims)
            {
                Entity pawnController = CommonReads.GetPawnController(accessor, entity);
                if (pawnController != Entity.Null && accessor.TryGetComponentData(pawnController, out Team currentTeam))
                {
                    var newTeam = currentTeam.Value == 0 ? 1 : 0;

                    accessor.SetComponentData(pawnController, new Team() { Value = newTeam });
                    if (accessor.HasComponent<Converted>(pawnController))
                    {
                        accessor.RemoveComponent<Converted>(pawnController);
                    }
                    else
                    {
                        accessor.AddComponentData(pawnController, new Converted() { Duration = DURATION });
                    }
                }
            }
        }
    }
}
