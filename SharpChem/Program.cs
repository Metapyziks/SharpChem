namespace SharpChem
{
    [StartPosition(2, 4)]
    class LittleLoop : WaldoProgram
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
                Move(Direction.Left, 2);
                Output(RegionLabel.OutputD);
                Move(Direction.Left, 3);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var reactor = Challenge.Get("Hello world!");

            reactor.RedWaldo.SetProgram<LittleLoop>();

            TimeControl.Start(StepSpeed.Fast);

            reactor.Display(1f);
            reactor.Dispose();
        }
    }
}
