using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTKTK.Scene;
using OpenTKTK.Shaders;
using OpenTKTK.Textures;

namespace SharpChem
{
    public enum ReactorLabel : byte
    {
        None = 0,

        Input = 2,
        Output = 4,

        InputA = 0 | Input,
        InputB = 1 | Input,

        OutputC = 0 | Output,
        OutputD = 1 | Output
    }

    public class ReactorRegion
    {
        internal static readonly Color4 ColorDefault = new Color4(0x14, 0x14, 0x14, 0xff);
        internal static readonly Color4 ColorAD = new Color4(0x20, 0x20, 0x20, 0xff);
        internal static readonly Color4 ColorBC = new Color4(0x1a, 0x1a, 0x1a, 0xff);

        internal static readonly Sprite _inputASprite = new Sprite(new BitmapTexture2D(Properties.Resources.inputa));
        internal static readonly Sprite _inputBSprite = new Sprite(new BitmapTexture2D(Properties.Resources.inputb));
        internal static readonly Sprite _outputCSprite = new Sprite(new BitmapTexture2D(Properties.Resources.outputc));
        internal static readonly Sprite _outputDSprite = new Sprite(new BitmapTexture2D(Properties.Resources.outputd));

        public int X { get; private set; }

        public int Y { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        internal Color4 TileColor { get; set; }

        public ReactorLabel Label { get; set; }
        
        public bool IsInput { get { return Label.HasFlag(ReactorLabel.Input); } }
        public bool IsOutput { get { return Label.HasFlag(ReactorLabel.Output); } }

        internal ReactorRegion(int x, int y, int width, int height)
        {
            X = x; Y = y; Width = width; Height = height;
        }

        internal void Render(SpriteShader shader)
        {
            Reactor.TileSprite.Colour = TileColor;

            for (int x = X; x < X + Width; ++x) {
                for (int y = Y; y < Y + Height; ++y) {
                    Reactor.TileSprite.Position = new Vector2(x, y) * 80f;
                    Reactor.TileSprite.Render(shader);
                }
            }

            Sprite labelSprite = null;

            switch (Label) {
                case ReactorLabel.InputA:
                    labelSprite = _inputASprite; break;
                case ReactorLabel.InputB:
                    labelSprite = _inputBSprite; break;
                case ReactorLabel.OutputC:
                    labelSprite = _outputCSprite; break;
                case ReactorLabel.OutputD:
                    labelSprite = _outputDSprite; break;
            }

            if (labelSprite != null) {
                labelSprite.Position = 80f * (new Vector2(X, Y) + 0.5f * new Vector2(Width, Height)
                    - new Vector2(labelSprite.Width, labelSprite.Height) / 160f);
                labelSprite.Colour = new Color4(0xff, 0xff, 0xff, 0x40);
                labelSprite.Render(shader);
            }
        }
    }

    internal class ReactorBuilder
    {
        internal static ReactorBuilder CreateDefault()
        {
            return new ReactorBuilder(10, 8)
                .AddRegion(new ReactorRegion(0, 0, 4, 4) {
                    TileColor = ReactorRegion.ColorAD,
                    Label = ReactorLabel.InputA
                }).AddRegion(new ReactorRegion(0, 4, 4, 4) {
                    TileColor = ReactorRegion.ColorBC,
                    Label = ReactorLabel.InputB
                }).AddRegion(new ReactorRegion(6, 0, 4, 4) {
                    TileColor = ReactorRegion.ColorBC,
                    Label = ReactorLabel.OutputC
                }).AddRegion(new ReactorRegion(6, 4, 4, 4) {
                    TileColor = ReactorRegion.ColorAD,
                    Label = ReactorLabel.OutputD
                });
        }

        private List<ReactorRegion> _regions;

        internal int Width { get; private set; }

        internal int Height { get; private set; }

        internal IEnumerable<ReactorRegion> Regions { get { return _regions; } }

        internal ReactorBuilder(int width, int height)
        {
            _regions = new List<ReactorRegion>();

            Width = width;
            Height = height;
        }

        internal ReactorBuilder AddRegion(ReactorRegion region)
        {
            _regions.Add(region);
            return this;
        }
    }

    public class Reactor : IDisposable
    {
        private static readonly BitmapTexture2D _tileTexture = new BitmapTexture2D(Properties.Resources.tile);
        internal static readonly Sprite TileSprite = new Sprite(_tileTexture);

        private int _steps;

        private ReactorRegion _baseRegion;
        private IEnumerable<ReactorRegion> _regions;
        private List<Atom> _atoms;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Waldo RedWaldo { get; private set; }
        public Waldo BlueWaldo { get; private set; }

        public IEnumerable<ReactorRegion> Inputs { get { return _regions.Where(x => x.IsInput); } }
        public IEnumerable<ReactorRegion> Outputs { get { return _regions.Where(x => x.IsOutput); } }

        internal Reactor(ReactorBuilder builder)
        {
            Width = builder.Width;
            Height = builder.Height;

            _baseRegion = new ReactorRegion(0, 0, Width, Height) {
                Label = ReactorLabel.None, TileColor = ReactorRegion.ColorDefault
            };

            _regions = builder.Regions;
            _atoms = new List<Atom>();

            RedWaldo = new Waldo(this, WaldoColor.Red);
            BlueWaldo = new Waldo(this, WaldoColor.Blue);

            _atoms.Add(new Atom(Element.C));
            _atoms.Add(new Atom(Element.Mg));
            _atoms.Add(new Atom(Element.H));
            _atoms.Add(new Atom(Element.He));

            _steps = 0;
        }

        public void Display(float scale = 1f)
        {
            using (var window = new ReactorWindow(this, scale)) {
                window.Run();
            }
        }

        internal void Update()
        {
            while (_steps < TimeControl.Steps) {
                ++_steps;
                RedWaldo.Think();
                BlueWaldo.Think();
            }
        }

        internal void Render(SpriteShader shader)
        {
            _baseRegion.Render(shader);

            foreach (var region in _regions) {
                region.Render(shader);
            }

            for (int i = 0; i < _atoms.Count; ++i) {
                _atoms[i].Render(shader, new Vector2(1f + i, 1f));
            }

            RedWaldo.Render(shader);
            BlueWaldo.Render(shader);
        }

        public void Dispose()
        {
            RedWaldo.Dispose();
            BlueWaldo.Dispose();
        }
    }
}
