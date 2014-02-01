using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTKTK.Shaders;

namespace SharpChem
{
    internal class Molecule : IEnumerable<Atom>
    {
        private List<Atom> _atoms;

        public int OriginX { get; private set; }
        public int OriginY { get; private set; }

        public int Left { get { return OriginX + _atoms.Min(x => x.XOffset); } }
        public int Top { get { return OriginY + _atoms.Min(x => x.YOffset); } }
        public int Right { get { return OriginX + _atoms.Max(x => x.XOffset) + 1; } }
        public int Bottom { get { return OriginY + _atoms.Max(x => x.YOffset) + 1; } }

        public int Width { get { return Right - Left; } }
        public int Height { get { return Bottom - Top; } }

        public Molecule()
        {
            _atoms = new List<Atom>();
        }

        public Molecule Clone(int dx, int dy)
        {
            var molecule = new Molecule();
            var clones = _atoms.ToDictionary(x => x,
                x => molecule.Add(x.Element, x.X + dx, x.Y + dy));

            foreach (var key in clones.Keys) {
                var clone = clones[key];
                foreach (var bond in key.Bonds) {
                    var other = clones[bond];
                    clone.AddBond(other);
                }
            }

            return molecule;
        }

        public bool Move(Reactor reactor, int dx, int dy)
        {
            OriginX += dx;
            OriginY += dy;

            if (Left < 0 || Top < 0 || Right > reactor.Width || Bottom > reactor.Height) {
                return false;
            }

            return true;
        }

        public Atom Add(Element elem, int x, int y)
        {
            if (_atoms.Count == 0) {
                OriginX = x;
                OriginY = y;
            }

            var atom = new Atom(elem, this, x - OriginX, y - OriginY);

            _atoms.Add(atom);

            return atom;
        }

        public void Add(int ax, int ay, int bx, int by)
        {
            var a = _atoms.FirstOrDefault(x => x.X == ax && x.Y == ay);
            var b = _atoms.FirstOrDefault(x => x.X == bx && x.Y == by);

            if (a == null || b == null || (ax != bx) == (ay != by) ||
                (Math.Abs(ax - bx) != 1 && Math.Abs(ay - by) != 1)) {
                throw new InvalidOperationException("Invalid atom positions given for bond.");
            }

            a.AddBond(b);
            b.AddBond(a);
        }

        public bool HitTest(int x, int y)
        {
            return _atoms.Any(a => a.X == x && a.Y == y);
        }

        public void Render(SpriteShader shader, Vector2 delta = default(Vector2))
        {
            foreach (var atom in _atoms) atom.RenderBonds(shader, delta);
            foreach (var atom in _atoms) atom.Render(shader, delta);
        }

        public IEnumerator<Atom> GetEnumerator()
        {
            return _atoms.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _atoms.GetEnumerator();
        }
    }
}
