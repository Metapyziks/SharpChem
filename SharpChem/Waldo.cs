using System;
using System.Linq;
using System.Reflection;

using OpenTK;
using OpenTK.Graphics;
using OpenTKTK.Scene;
using OpenTKTK.Shaders;
using OpenTKTK.Textures;

namespace SharpChem
{
    public enum WaldoColor
    {
        Red = 0,
        Blue = 1
    }

    public class Waldo : IDisposable
    {
        internal static readonly Sprite WaldoSprite = new Sprite(new BitmapTexture2D(Properties.Resources.waldo));

        private WaldoProgram _program;
        private int _oldX;
        private int _oldY;

        public Reactor Reactor { get; private set; }

        public int X { get; private set; }

        public int Y { get; private set; }

        public WaldoColor Color { get; private set; }

        public bool IsGrabbed { get; private set; }

        internal Molecule HeldMolecule { get; private set; }

        internal Waldo(Reactor reactor, WaldoColor color)
        {
            Reactor = reactor;
            Color = color;
            IsGrabbed = false;
        }

        public void Grab()
        {
            IsGrabbed = true;
            HeldMolecule = Reactor.GrabMolecule(X, Y);
        }

        public void Drop()
        {
            if (HeldMolecule != null) Reactor.DropMolecule(HeldMolecule);

            IsGrabbed = false;
            HeldMolecule = null;
        }

        public void GrabDrop()
        {
            if (IsGrabbed) Drop();
            else Grab();
        }

        public void SetProgram<T>()
            where T : WaldoProgram, new()
        {
            if (_program != null) {
                throw new InvalidOperationException("This waldo already has a program.");
            }

            var posAttrib = typeof(T).GetCustomAttribute<StartPositionAttribute>();

            if (posAttrib != null) {
                X = _oldX = posAttrib.X;
                Y = _oldY = posAttrib.Y;
            }

            if (X < 0 || X >= Reactor.Width || Y < 0 || Y >= Reactor.Height) {
                throw new IndexOutOfRangeException("Start position is out of bounds.");
            }

            _program = new T();
            _program.Waldo = this;

            _program.Begin();
        }

        public void Think()
        {
            if (_program == null) return;

            if (IsGrabbed && HeldMolecule != null && !HeldMolecule.Move(Reactor, X - _oldX, Y - _oldY)) {
                throw new InvalidOperationException("Molecule collision detected.");
            }

            _oldX = X;
            _oldY = Y;

            switch (_program.NextAction()) {
                case Action.MoveLeft:
                    if (X > 0) --X;
                    break;
                case Action.MoveUp:
                    if (Y > 0) --Y;
                    break;
                case Action.MoveRight:
                    if (X < Reactor.Width - 1) ++X;
                    break;
                case Action.MoveDown:
                    if (Y < Reactor.Height - 1) ++Y;
                    break;
            }
        }

        internal void Render(SpriteShader shader)
        {
            var delta = new Vector2(X - _oldX, Y - _oldY) * TimeControl.Delta;
            var pos = new Vector2(_oldX, _oldY) + delta;

            var xGap = IsGrabbed ? 0f : 4f;

            if (IsGrabbed && HeldMolecule != null) {
                HeldMolecule.Render(shader, delta);
            }

            WaldoSprite.X = pos.X * 80f - xGap;
            WaldoSprite.Y = pos.Y * 80f;
            WaldoSprite.Rotation = 0f;

            WaldoSprite.Colour = Color == WaldoColor.Red
                ? new Color4(0xff, 0x2f, 0x2f, 0xe0)
                : new Color4(0x5d, 0x8f, 0xff, 0xe0);

            WaldoSprite.Render(shader);

            WaldoSprite.X = (pos.X + 1f) * 80f + xGap;
            WaldoSprite.Y = (pos.Y + 1f) * 80f;
            WaldoSprite.Rotation = MathHelper.Pi;

            WaldoSprite.Render(shader);
        }

        public void Dispose()
        {
            if (_program != null) {
                _program.Dispose();
            }
        }
    }
}
