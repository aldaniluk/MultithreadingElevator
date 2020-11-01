using System.Threading;

namespace MultithreadingElevator
{
    public class Rider
    {
        public int Number { get; set; }

        public Rider(int number)
        {
            Number = number;
        }

        public void PressFloorButton(Floor floorFrom, bool movingUp)
        {
            if (movingUp)
            {
                floorFrom.SelectUpButton();
            }
            else
            {
                floorFrom.SelectDownButton();
            }
        }

        public void PressElevatorButton(Elevator elevator, Floor floorTo)
        {
            elevator.SelectFloor(floorTo);
        }

        public void EnterElevator(Elevator elevator)
        {
            elevator.IncreaseRidersCount();

            Thread.Sleep(500);
        }

        public void ExitElevator(Elevator elevator)
        {
            elevator.DecreaseRidersCount();

            Thread.Sleep(500);
        }

        public override string ToString()
        {
            return Number.ToString();
        }
    }
}
