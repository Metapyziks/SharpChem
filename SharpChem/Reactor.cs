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
    public enum RegionLabel : byte
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

        private Dictionary<Molecule, int> _blueprints;
        private Random _rand;
        private int _lastPulse;

        public int X { get; private set; }

        public int Y { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        internal Color4 TileColor { get; set; }

        public RegionLabel Label { get; set; }
        
        public bool IsInput { get { return Label.HasFlag(RegionLabel.Input); } }
        public bool IsOutput { get { return Label.HasFlag(RegionLabel.Output); } }

        internal ReactorRegion(int x, int y, int width, int height)
        {
            X = x; Y = y; Width = width; Height = height;

            _rand = new Random(0x198e7f0f);
            _blueprints = new Dictionary<Molecule, int>();
        }

        internal void AddBlueprint(Molecule molecule, int weighting = 1)
        {
            _blueprints.Add(molecule, weighting);
        }

        internal Molecule Input()
        {
            if (!Label.HasFlag(RegionLabel.Input)) {
                throw new InvalidOperationException("Can't input a molecule from an output.");
            }

            int sum = _blueprints.Sum(x => x.Value);
            int val = _rand.Next(sum);

            Molecule input = null;
            foreach (var molecule in _blueprints.Keys) {
                val -= _blueprints[molecule];

                if (val <= 0) {
                    input = molecule;
                    break;
                }
            }

            _lastPulse = TimeControl.Steps;

            if (input == null) return null;
            
            return input.Clone(X, Y);
        }

        internal bool Output(Molecule molecule)
        {
            _lastPulse = TimeControl.Steps;

            if (molecule == null) return true;

            return true;
        }

        internal void Render(SpriteShader shader)
        {
            float pulse = Math.Max(0f, 2f - (TimeControl.Steps + TimeControl.Delta - _lastPulse)) * 0.5f;

            var pulseClr = new Color4(0x60, 0x60, 0x60, 0xff);

            Reactor.TileSprite.Colour = new Color4(
                TileColor.R + (pulseClr.R - TileColor.R) * pulse,
                TileColor.G + (pulseClr.G - TileColor.G) * pulse,
                TileColor.B + (pulseClr.B - TileColor.B) * pulse, 
                1f);

            for (int x = X; x < X + Width; ++x) {
                for (int y = Y; y < Y + Height; ++y) {
                    Reactor.TileSprite.Position = new Vector2(x, y) * 80f;
                    Reactor.TileSprite.Render(shader);
                }
            }

            Sprite labelSprite = null;

            switch (Label) {
                case RegionLabel.InputA:
                    labelSprite = _inputASprite; break;
                case RegionLabel.InputB:
                    labelSprite = _inputBSprite; break;
                case RegionLabel.OutputC:
                    labelSprite = _outputCSprite; break;
                case RegionLabel.OutputD:
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
                    Label = RegionLabel.InputA
                }).AddRegion(new ReactorRegion(0, 4, 4, 4) {
                    TileColor = ReactorRegion.ColorBC,
                    Label = RegionLabel.InputB
                }).AddRegion(new ReactorRegion(6, 0, 4, 4) {
                    TileColor = ReactorRegion.ColorBC,
                    Label = RegionLabel.OutputC
                }).AddRegion(new ReactorRegion(6, 4, 4, 4) {
                    TileColor = ReactorRegion.ColorAD,
                    Label = RegionLabel.OutputD
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
        private List<Molecule> _molecules;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Waldo RedWaldo { get; private set; }
        public Waldo BlueWaldo { get; private set; }

        public IEnumerable<ReactorRegion> Inputs { get { return _regions.Where(x => x.IsInput); } }
        public IEnumerable<ReactorRegion> Outputs { get { return _regions.Where(x => x.IsOutput); } }

        public ReactorRegion this[RegionLabel label]
        {
            get { return _regions.First(x => x.Label == label); }
        }

        internal Reactor(ReactorBuilder builder)
        {
            Width = builder.Width;
            Height = builder.Height;

            _baseRegion = new ReactorRegion(0, 0, Width, Height) {
                Label = RegionLabel.None, TileColor = ReactorRegion.ColorDefault
            };

            _regions = builder.Regions;
            _molecules = new List<Molecule>();

            RedWaldo = new Waldo(this, WaldoColor.Red);
            BlueWaldo = new Waldo(this, WaldoColor.Blue);

            TimeControl.Step += (sender, e) => Update();
        }

        internal Molecule GrabMolecule(int x, int y)
        {
            lock (_molecules) {
                var molecule = _molecules.FirstOrDefault(m => m.HitTest(x, y));
                if (molecule != null) _molecules.Remove(molecule);

                return molecule;
            }
        }

        internal void DropMolecule(Molecule molecule)
        {
            if (_molecules.Contains(molecule)) {
                throw new InvalidOperationException("Can't drop already dropped molecule.");
            }

            lock (_molecules) {
                _molecules.Add(molecule);
            }
        }

        internal bool Input(RegionLabel region)
        {
            if (!region.HasFlag(RegionLabel.Input)) {
                throw new ArgumentException("Can't input from an output.");
            }

            lock (_molecules) {
                var molecule = this[region].Input();

                if (molecule != null) {
                    DropMolecule(molecule);
                    return true;
                } else {
                    return false;
                }
            }
        }

        internal bool Output(RegionLabel region)
        {
            if (!region.HasFlag(RegionLabel.Output)) {
                throw new ArgumentException("Can't output to an input.");
            }

            var reg = this[region];

            lock (_molecules) {
                var molecule = _molecules.FirstOrDefault(x => x.IsWithinRegion(reg));
                _molecules.Remove(molecule);
                reg.Output(molecule);

                return _molecules.Count(x => x.IsWithinRegion(reg)) == 0;
            }
        }

        public void Display(float scale = 1f)
        {
            using (var window = new ReactorWindow(this, scale)) {
                window.Run();
            }
        }

        internal void Update()
        {
            ++_steps;
            RedWaldo.Think();
            BlueWaldo.Think();
        }

        internal void Render(SpriteShader shader)
        {
            _baseRegion.Render(shader);

            foreach (var region in _regions) {
                region.Render(shader);
            }

            lock (_molecules) {
                foreach (var molecule in _molecules) {
                    molecule.Render(shader);
                }
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
