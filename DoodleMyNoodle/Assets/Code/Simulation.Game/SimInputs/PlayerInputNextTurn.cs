using Unity.Entities;

public class PlayerInputNextTurn : SimPlayerInput
{
    public bool ReadyForNextTurn = false;

    public PlayerInputNextTurn() { }

    public PlayerInputNextTurn(bool isReady) 
    {
        ReadyForNextTurn = isReady;
    }
}

public class PawnInputNextTurn : PawnControllerInputBase
{
    public bool ReadyForNextTurn = false;

    public PawnInputNextTurn(Entity pawnController) : base(pawnController) { }

    public PawnInputNextTurn(Entity pawnController, bool isReady) : base(pawnController)
    {
        ReadyForNextTurn = isReady;
    }
}