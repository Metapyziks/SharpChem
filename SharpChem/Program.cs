namespace SharpChem
{
    [StartPosition(2, 4)]
    class RedProgram : WaldoProgram
    {
        protected override void OnThink()
        {
            while (true) {
                Move(Direction.Up, 2);
                Input(RegionLabel.InputA);
                Move(Direction.Up, 1);
                Grab();
                Move(Direction.Right, 5);
                Move(Direction.Down, 3);
                Drop();
                Move(Direction.Left, 1);
                BreakBond();
                Move(Direction.Left, 1);
                Output(RegionLabel.OutputD);
                Move(Direction.Left, 3);
            }
        }
    }

    [StartPosition(7, 1)]
    class BlueProgram : WaldoProgram
    {
        protected override void OnThink()
        {
            while (true) {
                Move(Direction.Down, 3);
                Drop();
                Move(Direction.Left, 2);
                Output(RegionLabel.OutputD);
                Move(Direction.Left, 3);
                Move(Direction.Up, 2);
                Input(RegionLabel.InputA);
                Move(Direction.Up, 1);
                Grab();
                Move(Direction.Right, 5);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var reactor = Challenge.Get("Hello world!");

            reactor.RedWaldo.SetProgram<RedProgram>();
            reactor.BlueWaldo.SetProgram<BlueProgram>();

            reactor.PlaceBonder(6, 4);
            reactor.PlaceBonder(7, 4);

            TimeControl.Start(StepSpeed.Medium);

            reactor.Display(1f);
            reactor.Dispose();
        }
    }
}
