using MultithreadingElevator.Models;
using System;

namespace MultithreadingElevator
{
    public class Rider
    {
        private Floor floorFrom;
        private Floor floorTo;
        private Direction direction;
        private Elevator elevator;
        private int riderThreadNumber;

        public int Number { get; set; }

        public RiderState State { get; set; } = RiderState.AtFloor;

        public Rider(int number)
        {
            Number = number;
        }

        public void Run(int riderThreadNumber, Floor floorFrom, Floor floorTo)
        {
            this.riderThreadNumber = riderThreadNumber;
            this.floorFrom = floorFrom;
            this.floorTo = floorTo;
            this.direction = floorFrom.Number < floorTo.Number ? Direction.Up : Direction.Down;

            while (true)
            {
                switch (State)
                {
                    case RiderState.AtFloor:
                        AtFloor();
                        break;
                    case RiderState.AtElevator:
                        AtElevator();
                        break;
                    case RiderState.Exit:
                        return;
                }
            }
        }

        private void AtFloor()
        {
            Console.WriteLine($"T{riderThreadNumber}: R{Number} pushes {(direction == Direction.Up ? "U" : "D")}{floorFrom.Number}");
            floorFrom.SelectButton(direction);

            floorFrom.Events[direction].RidersCanEnterEvent.WaitOne();

            foreach (Elevator elevator in floorFrom.GetElevatorsToEnter(direction))
            {
                if (elevator.TryEnter())
                {
                    this.elevator = elevator;

                    Console.WriteLine($"T{riderThreadNumber}: R{Number} enters E{elevator.Number} on F{floorFrom.Number}");

                    State = RiderState.AtElevator;

                    return;
                }
            }

            floorFrom.Events[direction].ButtonReleasedEvent.WaitOne();
        }

        private void AtElevator()
        {
            Console.WriteLine($"T{riderThreadNumber}: R{Number} pushes E{elevator.Number}B{floorTo.Number}");
            elevator.SelectFloor(floorTo);

            elevator.RidersCanExitEvents[floorTo].WaitOne();

            Console.WriteLine($"T{riderThreadNumber}: R{Number} exits E{elevator.Number} on F{floorTo.Number}");
            elevator.Exit();

            State = RiderState.Exit;
        }
    }
}
