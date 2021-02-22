using MultithreadingElevator.Models;
using System.Linq;

namespace MultithreadingElevator.SchedulingLogic
{
    public static class ElevatorManager
    {
        private static object findElevatorLock = new object();

        public static void FindAppropriateElevator(Direction direction, Floor floorFrom)
        {
            FindAppropriateElevatorInternal(direction, floorFrom);
        }

        private static void FindAppropriateElevatorInternal(Direction direction, Floor floorFrom)
        {
            lock (findElevatorLock)
            {
                Elevator elevator = null;

                while (elevator == null)
                {
                    elevator = GlobalCache.Elevators.FirstOrDefault(e => IsAppropriate(e, direction, floorFrom));
                }

                elevator.SelectFloor(floorFrom, direction);
            }
        }

        private static bool IsAppropriate(Elevator elevator, Direction direction, Floor floorFrom)
        {
            //elevator is waiting
            if (elevator.State == ElevatorState.Wait)
            {
                return true;
            }

            if (elevator.Direction != direction)
            {
                return false;
            }

            //elevator is moving in same side (up) and elevator's way contains floor where rider is waiting
            if (direction == Direction.Up && elevator.CurrentFloor.Number < floorFrom.Number)
            {
                return true;
            }

            //elevator is moving in same side (down) and elevator's way contains floor where rider is waiting
            if (direction == Direction.Down && elevator.CurrentFloor.Number > floorFrom.Number)
            {
                return true;
            }

            return false;
        }
    }
}
