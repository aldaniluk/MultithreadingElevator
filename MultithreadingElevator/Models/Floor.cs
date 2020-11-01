using MultithreadingElevator.SchedulingLogic;

namespace MultithreadingElevator
{
    public class Floor
    {
        private object buttonUpLock = new object();
        private volatile bool isButtonUpPressed;

        private object buttonDownLock = new object();
        private volatile bool isButtonDownPressed;

        public int Number { get; }

        public Floor(int number)
        {
            Number = number;
        }

        public void SelectUpButton()
        {
            lock (buttonUpLock)
            {
                if (!isButtonUpPressed)
                {
                    ElevatorManager.FindAppropriateElevator(movingUp: true, this);
                }

                isButtonUpPressed = true;
            }
        }

        public void SelectDownButton()
        {
            lock (buttonDownLock)
            {
                if (!isButtonDownPressed)
                {
                    ElevatorManager.FindAppropriateElevator(movingUp: false, this);
                }

                isButtonDownPressed = true;
            }
        }

        public void ReleaseUpButton()
        {
            lock (buttonUpLock)
            {
                isButtonUpPressed = false;
            }
        }

        public void ReleaseDownButton()
        {
            lock (buttonDownLock)
            {
                isButtonDownPressed = false;
            }
        }

        public static bool operator <(Floor lhs, Floor rhs)
        {
            return lhs.Number < rhs.Number;
        }

        public static bool operator <=(Floor lhs, Floor rhs)
        {
            return lhs.Number <= rhs.Number;
        }

        public static bool operator >(Floor lhs, Floor rhs)
        {
            return lhs.Number > rhs.Number;
        }

        public static bool operator >=(Floor lhs, Floor rhs)
        {
            return lhs.Number >= rhs.Number;
        }

        public override string ToString()
        {
            return Number.ToString();
        }
    }
}