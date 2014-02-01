using System;

namespace SharpChem
{
    public static class Challenge
    {
        public static Reactor Get(String passcode)
        {
            switch (passcode) {
                case "Hello world!":
                    return new Reactor(ReactorBuilder.CreateDefault());
            }

            throw new InvalidOperationException("Bad passcode!");
        }
    }
}
