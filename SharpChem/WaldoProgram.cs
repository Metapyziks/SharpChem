using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SharpChem
{
    public enum Direction
    {
        Left = 0,
        Up = 1,
        Right = 2,
        Down = 3
    }

    internal enum Action
    {
        Wait = 0,
        MoveLeft = 1,
        MoveUp = 2,
        MoveRight = 3,
        MoveDown = 4,
        Grab = 5,
        Drop = 6,
        GrabDrop = 7
    }

    public abstract class WaldoProgram : IDisposable
    {
        public const int TimeoutMillis = 1000;

        private Thread _thread;
        private AutoResetEvent _waitSignal;
        private AutoResetEvent _actSignal;

        private Action _nextAction;

        public Waldo Waldo { get; internal set; }

        public Reactor Reactor { get { return Waldo.Reactor; } }

        internal WaldoProgram()
        {
            _waitSignal = new AutoResetEvent(false);
            _actSignal = new AutoResetEvent(false);
        }

        internal void Begin()
        {
            _thread = new Thread(Think);
            _thread.Start();
        }

        private void Think()
        {
            _waitSignal.WaitOne();
            OnThink();
        }

        internal Action NextAction()
        {
            _waitSignal.Set();

#if DEBUG
            _actSignal.WaitOne();
#else
            if (!_actSignal.WaitOne(TimeoutMillis)) {
                throw new TimeoutException(String.Format("{0} took too long to act.", GetType().Name));
            }
#endif

            return _nextAction;
        }
        
        protected abstract void OnThink();

        private void FinishAction()
        {
            _actSignal.Set();
            _waitSignal.WaitOne();
        }

        protected void Wait()
        {
            _nextAction = Action.Wait;

            FinishAction();
        }

        protected void Wait(int steps)
        {
            for (int i = 0; i < steps; ++i) Wait();
        }

        protected void Move(Direction dir)
        {
            switch (dir) {
                case Direction.Left:
                    _nextAction = Action.MoveLeft; break;
                case Direction.Up:
                    _nextAction = Action.MoveUp; break;
                case Direction.Right:
                    _nextAction = Action.MoveRight; break;
                case Direction.Down:
                    _nextAction = Action.MoveDown; break;
            }

            FinishAction();
        }

        protected void Move(Direction dir, int steps)
        {
            for (int i = 0; i < steps; ++i) Move(dir);
        }

        public void Dispose()
        {
            _thread.Abort();
        }
    }
}
