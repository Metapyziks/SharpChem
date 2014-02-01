using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpChem
{
    public enum Element : byte
    {
        None = 0,
        
        H = 1,
        He = 2,

        Li = 3,
        Be = 4,
        B = 5,
        C = 6,
        N = 7,
        O = 8,
        F = 9,
        Ne = 10,

        Na = 11,
        Mg = 12,
        Al = 13,
        Si = 14,
        P = 15,
        S = 16,
        Cl = 17,
        Ar = 18
    }

    public static class ElementExtensions
    {
        private static readonly Tuple<String, int>[] _nameMaxBonds = new[] {
            Tuple.Create("Unknown",     0),

            Tuple.Create("Hydrogen",    1),
            Tuple.Create("Helium",      0),

            Tuple.Create("Lithium",     1),
            Tuple.Create("Beryllium",   2),
            Tuple.Create("Boron",       3),
            Tuple.Create("Carbon",      4),
            Tuple.Create("Nitrogen",    5),
            Tuple.Create("Oxygen",      2),
            Tuple.Create("Fluorine",    1),
            Tuple.Create("Neon",        0),

            Tuple.Create("Sodium",      1),
            Tuple.Create("Magnesium",   2),
            Tuple.Create("Aluminium",   4),
            Tuple.Create("Silicon",     4),
            Tuple.Create("Phosphorus",  5),
            Tuple.Create("Sulphur",     6),
            Tuple.Create("Chlorine",    7),
            Tuple.Create("Argon",       0)
        };

        public static String GetFullName(this Element elem)
        {
            return _nameMaxBonds[(int) elem > _nameMaxBonds.Length ? 0 : (int) elem].Item1;
        }

        public static int GetMaxBonds(this Element elem)
        {
            return _nameMaxBonds[(int) elem > _nameMaxBonds.Length ? 0 : (int) elem].Item2;
        }
    }

    internal class Atom
    {
        public Element Element { get; private set; }
    }
}
