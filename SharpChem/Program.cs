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
                GrabDrop();
                Move(Direction.Left, 5);
                GrabDrop();
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
                if ((Waldo.X & 1) == 0) {
                    if (Waldo.Y > 0) {
                        Move(Direction.Up);
                    } else {
                        Move(Direction.Right);
                    }
                } else {
                    if (Waldo.Y < Reactor.Height - 1) {
                        Move(Direction.Down);
                    } else if (Waldo.X == Reactor.Width - 1) {
                        Move(Direction.Left, Reactor.Width - 1);
                    } else {
                        Move(Direction.Right);
                    }
                }
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

            TimeControl.Start(StepSpeed.Medium);

            reactor.Display(1f);
            reactor.Dispose();
        }
    }
}
