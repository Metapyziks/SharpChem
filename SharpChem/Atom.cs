using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private Text _text;

        public Element Element { get; private set; }

        public Atom(Element element)
        {
            Element = element;

            _text = new Text(new Font(new FontFamily("Century Gothic"), 18f));
            _text.Value = Element.ToString();
            _text.Colour = Color4.Black;
            _text.UseCentreAsOrigin = true;
        }

        public void Render(SpriteShader shader, Vector2 pos)
        {
            _atomSprite.Position = pos * 80f;
            _atomSprite.Colour = Element.GetColor();
            _atomSprite.Render(shader);

            _text.Position = (pos + new Vector2(0.5f, 0.5f)) * 80f;
            _text.Render(shader);
        }
    }
}
