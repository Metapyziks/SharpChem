using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using OpenTK;
using OpenTK.Graphics;
using OpenTKTK.Scene;
using OpenTKTK.Shaders;
using OpenTKTK.Textures;

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
        private static readonly Tuple<String, int, Color4>[] _nameMaxBonds = new[] {
            Tuple.Create("Unknown",     0,      new Color4(0x00, 0x00, 0x00, 0x00)),

            Tuple.Create("Hydrogen",    1,      new Color4(0xd2, 0xd2, 0xd2, 0xff)),
            Tuple.Create("Helium",      0,      new Color4(0xbc, 0xbc, 0xbc, 0xff)),

            Tuple.Create("Lithium",     1,      new Color4(0xd2, 0xd2, 0xd2, 0xff)),
            Tuple.Create("Beryllium",   2,      new Color4(0xd2, 0xa7, 0xa7, 0xff)),
            Tuple.Create("Boron",       3,      new Color4(0xa7, 0xd2, 0xd2, 0xff)),
            Tuple.Create("Carbon",      4,      new Color4(0xb6, 0xd2, 0x9d, 0xff)),
            Tuple.Create("Nitrogen",    5,      new Color4(0xbf, 0xb4, 0xd2, 0xff)),
            Tuple.Create("Oxygen",      2,      new Color4(0xd2, 0xa7, 0xa7, 0xff)),
            Tuple.Create("Fluorine",    1,      new Color4(0xd2, 0xd2, 0xd2, 0xff)),
            Tuple.Create("Neon",        0,      new Color4(0xbc, 0xbc, 0xbc, 0xff)),

            Tuple.Create("Sodium",      1,      new Color4(0xd2, 0xd2, 0xd2, 0xff)),
            Tuple.Create("Magnesium",   2,      new Color4(0xd2, 0xa7, 0xa7, 0xff)),
            Tuple.Create("Aluminium",   4,      new Color4(0xb6, 0xd2, 0x9d, 0xff)),
            Tuple.Create("Silicon",     4,      new Color4(0xb6, 0xd2, 0x9d, 0xff)),
            Tuple.Create("Phosphorus",  5,      new Color4(0xbf, 0xb4, 0xd2, 0xff)),
            Tuple.Create("Sulphur",     6,      new Color4(0xd2, 0xc5, 0xa7, 0xff)),
            Tuple.Create("Chlorine",    7,      new Color4(0xb6, 0xd2, 0x9d, 0xff)),
            Tuple.Create("Argon",       0,      new Color4(0xbc, 0xbc, 0xbc, 0xff))
        };

        public static String GetFullName(this Element elem)
        {
            return _nameMaxBonds[(int) elem > _nameMaxBonds.Length ? 0 : (int) elem].Item1;
        }

        public static int GetMaxBonds(this Element elem)
        {
            return _nameMaxBonds[(int) elem > _nameMaxBonds.Length ? 0 : (int) elem].Item2;
        }

        internal static Color4 GetColor(this Element elem)
        {
            return _nameMaxBonds[(int) elem > _nameMaxBonds.Length ? 0 : (int) elem].Item3;
        }
    }

    internal class Atom
    {
        private static readonly Sprite _atomSprite = new Sprite(new BitmapTexture2D(Properties.Resources.atom));
        private static readonly Sprite _bondSprite = new Sprite(new BitmapTexture2D(Properties.Resources.bond))
        { UseCentreAsOrigin = true, Colour = new Color4(0xe6, 0xe6, 0xe6, 0xff) };
        
        private static int _nextID = 0;

        private Dictionary<Atom, int> _bonds;
        private Text _text;

        public int ID { get; private set; }

        public int XOffset { get; private set; }

        public int YOffset { get; private set; }

        public int X { get { return Molecule.OriginX + XOffset; } }
        public int Y { get { return Molecule.OriginY + YOffset; } }

        public Molecule Molecule { get; private set; }

        public Element Element { get; private set; }

        public IEnumerable<Atom> Bonds { get { return _bonds.SelectMany(x => Enumerable.Repeat(x.Key, x.Value)); } }

        public Atom(Element element, Molecule molecule, int xoffset, int yoffset)
        {
            ID = ++_nextID;

            Element = element;
            Molecule = molecule;

            XOffset = xoffset;
            YOffset = yoffset;

            _bonds = new Dictionary<Atom, int>();

            _text = new Text(new Font(new FontFamily("Century Gothic"), 18f));
            _text.Value = Element.ToString();
            _text.Colour = Color4.Black;
            _text.UseCentreAsOrigin = true;
        }

        public bool AddBond(Atom other)
        {
            if (_bonds.Count >= Element.GetMaxBonds()) {
                return false;
            }

            if (!_bonds.ContainsKey(other)) {
                _bonds.Add(other, 1);
            } else {
                _bonds[other] += 1;
            }

            return true;
        }

        public bool BreakBond(Atom other)
        {
            if (!_bonds.ContainsKey(other)) {
                return false;
            }
            
            if (_bonds[other] <= 1) {
                _bonds.Remove(other);
            } else {
                _bonds[other] -= 1;
            }

            return true;
        }

        public void RenderBonds(SpriteShader shader, Vector2 delta = default(Vector2))
        {
            var pos = new Vector2(X + 0.5f, Y + 0.5f) * 80f;

            foreach (var bond in _bonds.Keys) {
                if (bond.ID < this.ID) continue;

                int count = _bonds[bond];

                var mid = new Vector2(this.X + bond.X + 1f, this.Y + bond.Y + 1f) * 40f;
                var dir = mid - pos;
                var lft = new Vector2(dir.Y, -dir.X).Normalized() * 12f;

                _bondSprite.Rotation = (float) Math.Atan2(dir.Y, dir.X);

                for (int i = 0; i < count; ++i) {
                    var offset = i - (count - 1) * 0.5f;

                    _bondSprite.Position = mid + lft * offset + delta * 80f;
                    _bondSprite.Render(shader);
                }
            }
        }

        public void Render(SpriteShader shader, Vector2 delta = default(Vector2))
        {
            var pos = new Vector2(X, Y) * 80f;

            _atomSprite.Position = pos + delta * 80f;
            _atomSprite.Colour = Element.GetColor();
            _atomSprite.Render(shader);

            pos += new Vector2(40f, 40f);

            _text.Position = pos + delta * 80f;
            _text.Render(shader);
        }

        public override string ToString()
        {
            return Element.ToString();
        }
    }
}
