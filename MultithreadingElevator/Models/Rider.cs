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

        public Rider(int number)
        {
            Number = number;
        }

        public void Run(int riderThreadNumber, Floor floorFrom, Floor floorTo)
        {
            this.riderThreadNumber = riderThreadNumber;
            this.floorFrom = floorFrom;
            this.floorTo = floorTo;
            this.direction = floorFrom < floorTo ? Direction.Up : Direction.Down;

            AtFloor();
            AtElevator();
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
        }
    }
}
