namespace SharpChem
{
    [StartPosition(2, 2)]
    class LittleLoop : WaldoProgram
    {
        protected override void OnThink()
        {
            while (true) {
                Move(Direction.Right, 5);
                Move(Direction.Down, 3);
                Move(Direction.Left, 5);
                Move(Direction.Up, 3);
                GrabDrop();
            }
        }
    }

    [StartPosition(1, 1)]
    class BigLoop : WaldoProgram
    {
        protected override void OnThink()
        {
            while (true) {
                Move(Direction.Right, 7);
                Move(Direction.Down, 5);
                Move(Direction.Left, 7);
                Move(Direction.Up, 5);
                GrabDrop();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var reactor = Challenge.Get("Hello world!");

            reactor.RedWaldo.SetProgram<LittleLoop>();
            reactor.BlueWaldo.SetProgram<BigLoop>();

            TimeControl.Start(StepSpeed.Fast);

            reactor.Display(0.5f);
            reactor.Dispose();
        }
    }
}
