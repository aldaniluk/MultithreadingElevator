using MultithreadingElevator.Models;
using MultithreadingElevator.SchedulingLogic;
using System;
using System.Linq;
using System.Threading;

namespace MultithreadingElevator
{
    public class RiderThread
    {
        private int number;

        public RiderThread(int number)
        {
            this.number = number;
        }

        public void Process()
        {
            while (true)
            {
                Request request = RequestManager.GetNextRequest();
                if (request == null)
                {
                    return;
                }

                ProcessInternal(request);
            }
        }

        private void ProcessInternal(Request request)
        {
            bool movingUp = request.FloorFrom < request.FloorTo;

            Elevator elevator;
            while (true)
            {
                Console.WriteLine($"T{number}: R{request.Rider} pushes {(movingUp ? "U" : "D")}{request.FloorFrom}");
                request.Rider.PressFloorButton(request.FloorFrom, movingUp);

                elevator = WaitForElevatorComesToFloorFrom(request.FloorFrom, movingUp);

                if (EnterElevator(elevator, request))
                {
                    break;
                }
            }

            Console.WriteLine($"T{number}: R{request.Rider} pushes E{elevator.Number}B{request.FloorTo}");
            request.Rider.PressElevatorButton(elevator, request.FloorTo);

            WaitForElevatorComesToFloorTo(elevator, request.FloorTo);

            Console.WriteLine($"T{number}: R{request.Rider} exits E{elevator.Number} on F{request.FloorTo}");
            request.Rider.ExitElevator(elevator);
        }

        private bool EnterElevator(Elevator elevator, Request request)
        {
            while (elevator.AreDoorsOpen)
            {
                lock (elevator)
                {
                    if (elevator.RidersCount < GlobalCache.RidersPerElevatorCount)
                    {
                        Console.WriteLine($"T{number}: R{request.Rider} enters E{elevator.Number} on F{request.FloorFrom.Number}");
                        request.Rider.EnterElevator(elevator);

                        return true;
                    }
                }
            }

            return false;
        }

        private Elevator WaitForElevatorComesToFloorFrom(Floor floorFrom, bool movingUp)
        {
            Elevator elevator = null;

            while (elevator == null)
            {
                elevator = GlobalCache.Elevators.FirstOrDefault(e => ElevatorComes(e, floorFrom, movingUp));
            }

            return elevator;
        }

        private bool ElevatorComes(Elevator elevator, Floor floorFrom, bool movingUp)
        {
            //elevator is moving to rider's floor
            if (elevator.CurrentFloor != floorFrom)
            {
                return false;
            }

            //elevator doesn't open doors
            if (!elevator.AreDoorsOpen)
            {
                return false;
            }

            //elevator is moving in opposite direction
            if ((elevator.State == ElevatorState.MovingUp && !movingUp) ||
                (elevator.State == ElevatorState.MovingDown && movingUp))
            {
                return false;
            }

            return true;
        }

        private void WaitForElevatorComesToFloorTo(Elevator elevator, Floor floorTo)
        {
            SpinWait.SpinUntil(() => elevator.CurrentFloor == floorTo && elevator.AreDoorsOpen);
        }
    }
}
