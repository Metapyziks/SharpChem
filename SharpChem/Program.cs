namespace SharpChem
{
    class Program
    {
        static void Main(string[] args)
        {
            var reactor = Challenge.Get("Hello world!");
            reactor.RedWaldo.SetProgram<LittleLoop>();

            TimeControl.Start(StepSpeed.Fast);

            reactor.Display(0.5f);
            reactor.Dispose();
        }
    }
}
