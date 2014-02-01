using System;
using System.Diagnostics;
using System.Timers;

namespace SharpChem
{
    public enum StepSpeed
    {
        Slow = 1000,
        Medium = 500,
        Fast = 100,
        Turbo = 10
    }

    public static class TimeControl
    {
        private static Timer _timer = new Timer();
        private static Stopwatch _stopwatch = new Stopwatch();

        private static StepSpeed _speed;
        private static int _steps;

        static TimeControl()
        {
            _timer.AutoReset = true;
            _timer.Elapsed += (sender, e) => {
                ++_steps;

                if (Step != null) Step(sender, e);

                _stopwatch.Restart();
            };
        }

        public static int Steps
        {
            get
            {
                return _steps;
            }
        }

        public static float Delta
        {
            get
            {
                return (float) Math.Min(_stopwatch.ElapsedMilliseconds, (float) _speed) / (int) _speed;
            }
        }

        public static event EventHandler Step;
        
        public static void Start(StepSpeed speed)
        {
            _speed = speed;
            _stopwatch.Start();

            _timer.Interval = (double) speed;
            _timer.Start();
        }

        public static void Stop()
        {
            _stopwatch.Stop();
            _timer.Stop();

            if (_stopwatch.ElapsedMilliseconds > 0) {
                _stopwatch.Reset();
                ++_steps;
            }
        }
    }
}
