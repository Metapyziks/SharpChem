using System;

namespace SharpChem
{
    public static class Challenge
    {
        private static readonly Molecule _water = new Molecule {
            { Element.O, 1, 1 },
            { Element.H, 2, 1 }, { 1, 1, 2, 1 },
            { Element.H, 1, 2 }, { 1, 1, 1, 2 }
        };

        private static readonly Molecule _formaldehyde = new Molecule {
            { Element.C, 2, 2 },
            { Element.H, 2, 1 }, { 2, 2, 2, 1 },
            { Element.H, 1, 2 }, { 2, 2, 1, 2 },
            { Element.O, 3, 2 }, { 2, 2, 3, 2 }, { 2, 2, 3, 2 },
        };

        public static Reactor Get(String passcode)
        {
            var reactor = new Reactor(ReactorBuilder.CreateDefault());

            switch (passcode) {
                case "Hello world!":
                    reactor[RegionLabel.InputA].AddBlueprint(_formaldehyde, 1);
                    reactor[RegionLabel.InputA].AddBlueprint(_water, 3);
                    break;
                default:
                    throw new InvalidOperationException("Bad passcode!");
            }

            return reactor;
        }
    }
}
