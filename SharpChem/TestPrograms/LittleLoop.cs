using SharpChem;

[StartPosition(3, 2)]
class LittleLoop : WaldoProgram
{
    protected override void OnThink()
    {
        while (true) {
            Move(Direction.Right, 4);
            Move(Direction.Down, 2);
            Move(Direction.Left, 4);
            Move(Direction.Up, 2);
            GrabDrop();
        }
    }
}
