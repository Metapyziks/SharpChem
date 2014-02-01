using System;
using System.Diagnostics;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTKTK.Shaders;

namespace SharpChem
{
    internal class ReactorWindow : GameWindow
    {
        private SpriteShader _shader;
        private float _scale;

        public Reactor Reactor { get; private set; }

        public ReactorWindow(Reactor reactor, float scale)
            : base(reactor.Width * (int) (80f * scale), reactor.Height * (int) (80f * scale), new GraphicsMode(new ColorFormat(8, 8, 8, 0), 0, 0, 4))
        {
            _scale = scale;

            Reactor = reactor;

            Title = "SharpChem Reactor View";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _shader = new SpriteShader((int) (Width / _scale), (int) (Height / _scale));

            GL.ClearColor(new Color4(0x06, 0x06, 0x06, 0xff));
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            
            Reactor.Update();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            _shader.Begin(true);
            Reactor.Render(_shader);
            _shader.End();

            SwapBuffers();
        }
    }
}
