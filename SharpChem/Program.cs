namespace SharpChem
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var reactor = new Reactor(ReactorBuilder.CreateDefault())) {
                reactor.RedWaldo.SetProgram<LittleLoop>(2, 2);

                TimeControl.Start(StepSpeed.Fast);

                reactor.Display();
            }
        }
    }
}
