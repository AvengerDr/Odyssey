using System.Diagnostics;

namespace Odyssey.Engine
{
    public class FpsTimer
    {
        private readonly Stopwatch clock;
        private long frameCount;
        private double measuredFPS;
        private double timeElapsed;
        private double totalTime;

        public FpsTimer()
        {
            clock = Stopwatch.StartNew();
        }

        public float FrameTime { get { return (float)timeElapsed; } }

        public double FrameTimeDouble { get { return timeElapsed; } }

        public double MeasuredFPS { get { return measuredFPS; } }

        public double TotalTime
        {
            get { return totalTime; }
        }

        public void Measure()
        {
            frameCount++;
            timeElapsed = (double)clock.ElapsedTicks / Stopwatch.Frequency;
            totalTime += timeElapsed;
            if (totalTime >= 1.0f)
            {
                measuredFPS = frameCount / totalTime;
                frameCount = 0;
                totalTime = 0.0;
            }

            clock.Restart();
        }
    }
}