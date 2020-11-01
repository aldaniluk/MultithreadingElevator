using MultithreadingElevator.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MultithreadingElevator.SchedulingLogic
{
    public static class ElevatorManager
    {
        private static object findElevatorLock = new object();

        public static void FindAppropriateElevator(bool movingUp, Floor floorFrom)
        {
            new Task(() => FindAppropriateElevatorInternal(movingUp, floorFrom)).Start();
        }

        private static void FindAppropriateElevatorInternal(bool movingUp, Floor floorFrom)
        {
            lock (findElevatorLock)
            {
                Elevator elevator = null;

                while (elevator == null)
                {
                    elevator = GlobalCache.Elevators.FirstOrDefault(e => IsAppropriate(e, movingUp, floorFrom));
                }

                elevator.SelectFloor(floorFrom);
            }
        }

        private static bool IsAppropriate(Elevator elevator, bool movingUp, Floor floorFrom)
        {
            //elevator is moving in same side (up) and elevator's way contains floor where rider is waiting
            if (elevator.State == ElevatorState.MovingUp && movingUp && elevator.CurrentFloor < floorFrom)
            {
                return true;
            }

            //elevator is moving in same side (down) and elevator's way contains floor where rider is waiting
            if (elevator.State == ElevatorState.MovingDown && !movingUp && elevator.CurrentFloor > floorFrom)
            {
                return true;
            }

            //elevator is waiting
            if (elevator.State == ElevatorState.Waiting)
            {
                return true;
            }

            return false;
        }
    }
}
