using System;
using System.Collections.Generic;

namespace GoobOS
{
    //Timer system for scheduling tasks and countdowns.
    //Uses deltaTime to update the remaining time for each task and countdown, executing thier assigned actions when finished. 
    public class GoobTimerSystem
    {
        private List<GoobTask> tasks = new List<GoobTask>();
        private List<GoobCountdown> countdownTasks = new List<GoobCountdown>();

        public void WaitSeconds(string name, int seconds, Action action)
        {
            WaitMilliseconds(name, seconds * 1000, action);
        }

        public void WaitMilliseconds(string name, int milliseconds, Action action)
        {
            if (milliseconds < 1)
            {
                milliseconds = 1;
            }

            tasks.Add(new GoobTask(name, milliseconds, action));
        }

        public void StartCountdown(
            string name,
            int seconds,
            Action<int> onTick,
            Action onFinished)
        {
            if (seconds < 1)
            {
                seconds = 1;
            }

            countdownTasks.Add(new GoobCountdown(name, seconds, onTick, onFinished));
        }

        public void Update(int deltaMilliseconds)
        {
            for (int i = tasks.Count - 1; i >= 0; i--)
            {
                tasks[i].Update(deltaMilliseconds);

                if (tasks[i].IsFinished)
                {
                    tasks.RemoveAt(i);
                }
            }

            for (int i = countdownTasks.Count - 1; i >= 0; i--)
            {
                countdownTasks[i].Update(deltaMilliseconds);

                if (countdownTasks[i].IsFinished)
                {
                    countdownTasks.RemoveAt(i);
                }
            }
        }

        public void ClearTasks()
        {
            tasks.Clear();
            countdownTasks.Clear();
        }
    }
}