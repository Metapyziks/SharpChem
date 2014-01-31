namespace SharpChem
{
    class Program
    {
        static void Main(string[] args)
        {
            var reactor = new Reactor(ReactorBuilder.CreateDefault());
            reactor.Display();            
        }
    }
}
