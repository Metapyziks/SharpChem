using System;
using System.Diagnostics;

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
        private static Stopwatch _stopwatch = new Stopwatch();
        private static int _steps = 0;
        private static StepSpeed _speed;

        public static int Steps
        {
            get
            {
                if (_stopwatch.ElapsedMilliseconds >= (int) _speed) {
                    _stopwatch.Restart();
                    ++_steps;
                }

                return _steps;
            }
        }

        public static float Delta
        {
            get
            {
                return (float) Math.Min(_stopwatch.ElapsedMilliseconds, (int) _speed) / (int) _speed;
            }
        }
        
        public static void Start(StepSpeed speed)
        {
            _speed = speed;
            _stopwatch.Start();
        }

        public static void Stop()
        {
            _stopwatch.Stop();

            if (_stopwatch.ElapsedMilliseconds > 0) {
                _stopwatch.Reset();
                ++_steps;
            }
        }
    }
}
