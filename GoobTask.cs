using System;

namespace GoobOS
{
    // A class to represent a task that can be scheduled and executed after a certain amount of time has passed.

    public class GoobTask
    {
        //Every task is given a name, remaining time, and an action. The action is a delegate that represents the code to be executed when the task is finished.
        public string Name { get; private set; }
        public int RemainingMilliseconds { get; private set; }
        public bool IsFinished { get; private set; }

        private Action action;

        public GoobTask(string name, int milliseconds, Action action)
        {
            Name = name;

            if (milliseconds < 1)
            {
                milliseconds = 1;
            }

            RemainingMilliseconds = milliseconds;
            this.action = action;
            IsFinished = false;
        }

        public void Update(int deltaMilliseconds)
        {
            if (IsFinished)
            {
                return;
            }

            RemainingMilliseconds -= deltaMilliseconds;

            if (RemainingMilliseconds <= 0)
            {
                Execute();
            }
        }

        private void Execute()
        {
            if (action != null)
            {
                action();
                action = null;
            }

            IsFinished = true;
        }
    }
}