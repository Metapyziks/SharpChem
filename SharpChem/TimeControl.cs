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

        static TimeControl()
        {
            _timer.AutoReset = true;
            _timer.Elapsed += (sender, e) => {
                ++Steps;
                _stopwatch.Restart();
                
                if (Step != null) Step(sender, e);
            };
        }

        public static int Steps { get; private set; }

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
                ++Steps;
            }
        }
    }
}
