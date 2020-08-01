using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class ClearEmptyInteractableInventorySystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        Entities
        .WithAll<Interactable, Interacted>()
        .ForEach((Entity interactable, ref FixTranslation pos, DynamicBuffer<InventoryItemReference> inventory) =>
        {
            if (inventory.Length < 1)
            {
                int2 tilePos = Helpers.GetTile(pos);
                Entity tile = CommonReads.GetTileEntity(Accessor, tilePos);
                CommonWrites.RemoveTileAddon(Accessor, interactable, tile);
                PostUpdateCommands.DestroyEntity(interactable);
            }
        });

    }
}