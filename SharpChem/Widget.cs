using System;
using System.Collections.Generic;
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
    public abstract class Widget
    {
        public Reactor Reactor { get; private set; }

        public int X { get; private set; }
        public int Y { get; private set; }

        public Widget(Reactor reactor, int x, int y)
        {
            Reactor = reactor;

            X = x;
            Y = y;
        }

        public abstract void Render(SpriteShader shader);
    }

    public class Bonder : Widget
    {
        private static readonly Sprite _bonderSprite = new Sprite(new BitmapTexture2D(Properties.Resources.bonder))
        { Colour = new Color4(0x80, 0x80, 0x80, 0xff) };
        private static readonly Sprite _bonderJoinSprite = new Sprite(new BitmapTexture2D(Properties.Resources.bonderjoin))
        { Colour = new Color4(0x80, 0x80, 0x80, 0xff), UseCentreAsOrigin = true };

        public Bonder(Reactor reactor, int x, int y)
            : base(reactor, x, y) { }

        public override void Render(SpriteShader shader)
        {
            _bonderSprite.Position = new Vector2(X, Y) * 80f;
            _bonderSprite.Render(shader);

            var neighbours = Reactor.Bonders.Where(x => (x.X == X) != (x.Y == Y) &&
                (Math.Abs(x.X - X) == 1 || Math.Abs(x.Y - Y) == 1));

            if (neighbours.Count() == 0) return;

            var pos = new Vector2(X + 0.5f, Y + 0.5f) * 80f;
            foreach (var neighbour in neighbours) {
                var diff = new Vector2(neighbour.X + 0.5f, neighbour.Y + 0.5f) * 80f - pos;
                _bonderJoinSprite.Position = pos;
                _bonderJoinSprite.Rotation = (float) Math.Atan2(diff.Y, diff.X);
                _bonderJoinSprite.Render(shader);
            }
        }
    }
}
