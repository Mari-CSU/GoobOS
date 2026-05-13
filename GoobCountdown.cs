using System;

namespace GoobOS
{
    // A class to represent a countdown timer that can be scheduled and updated over time, providing tick updates and a finished callback.
    public class GoobCountdown
    {
        public string Name { get; private set; }
        public int RemainingMilliseconds { get; private set; }
        public bool IsFinished { get; private set; }

        private int displayIntervalMilliseconds;
        private int displayTimerMilliseconds;

        private Action<int> onTick;
        private Action onFinished;

        public GoobCountdown(
            string name,
            int totalSeconds,
            Action<int> onTick,
            Action onFinished)
        {
            Name = name;
            RemainingMilliseconds = totalSeconds * 1000;
            displayIntervalMilliseconds = 1000;
            displayTimerMilliseconds = 1000;

            this.onTick = onTick;
            this.onFinished = onFinished;

            IsFinished = false;
        }

        public void Update(int deltaMilliseconds)
        {
            if (IsFinished)
            {
                return;
            }

            RemainingMilliseconds -= deltaMilliseconds;
            displayTimerMilliseconds -= deltaMilliseconds;

            if (displayTimerMilliseconds <= 0 && RemainingMilliseconds > 0)
            {
                int secondsLeft = RemainingMilliseconds / 1000;

                if (onTick != null)
                {
                    onTick(secondsLeft);
                }

                displayTimerMilliseconds = displayIntervalMilliseconds;
            }

            if (RemainingMilliseconds <= 0)
            {
                IsFinished = true;

                if (onFinished != null)
                {
                    onFinished();
                }
            }
        }
    }
}