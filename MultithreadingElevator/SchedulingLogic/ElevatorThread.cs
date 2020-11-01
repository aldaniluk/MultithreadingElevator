using MultithreadingElevator.Models;
using System;
using System.Threading;

namespace MultithreadingElevator
{
    public class ElevatorThread
    {
        private Elevator elevator;

        public ElevatorThread(Elevator elevator)
        {
            this.elevator = elevator;
        }

        public void Process()
        {
            while (true)
            {
                if (!elevator.HasSelectedFloors())
                {
                    Thread.Sleep(1_000);

                    continue;
                }

                StopAtCurrentFloor();
                MoveOneFloor();
            }
        }

        private void StopAtCurrentFloor()
        {
            if (!elevator.IsSelectedFloor(elevator.CurrentFloor))
            {
                return;
            }

            GetRiders();
        }

        private void GetRiders()
        {
            Console.WriteLine($"E{elevator.Number} on F{elevator.CurrentFloor} opens");
            elevator.OpenDoors();

            elevator.WaitRiders();

            ReleaseElevatorButton();
            ReleaseFloorButton();

            Console.WriteLine($"E{elevator.Number} on F{elevator.CurrentFloor} closes");
            elevator.CloseDoors();
        }

        private void ReleaseElevatorButton()
        {
            elevator.ReleaseFloor(elevator.CurrentFloor);
        }

        private void ReleaseFloorButton()
        {
            if (elevator.State == ElevatorState.MovingUp)
            {
                elevator.CurrentFloor.ReleaseUpButton();
            }

            if (elevator.State == ElevatorState.MovingDown)
            {
                elevator.CurrentFloor.ReleaseDownButton();
            }
        }

        private void MoveOneFloor()
        {
            if (elevator.State == ElevatorState.MovingUp)
            {
                Console.WriteLine($"E{elevator.Number} moves up to F{elevator.CurrentFloor.Number + 1}");
                elevator.MoveUpOneFloor();
            }

            if (elevator.State == ElevatorState.MovingDown)
            {
                Console.WriteLine($"E{elevator.Number} moves down to F{elevator.CurrentFloor.Number - 1}");
                elevator.MoveDownOneFloor();
            }
        }
    }
}
