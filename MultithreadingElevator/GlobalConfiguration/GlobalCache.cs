using System.Collections.Generic;

namespace MultithreadingElevator
{
    public static class GlobalCache
    {
        public static List<Elevator> Elevators { get; private set; }
        public static void SetElevators(List<Elevator> elevators)
        {
            Elevators = elevators;
        }

        public static List<Rider> Riders { get; private set; }
        public static void SetRiders(List<Rider> riders)
        {
            Riders = riders;
        }

        public static List<Floor> Floors { get; private set; }
        public static void SetFloors(List<Floor> floors)
        {
            Floors = floors;
        }

        public static int RiderThreadsCount { get; private set; }
        public static void SetRiderThreadsCount(int riderThreadsCount)
        {
            RiderThreadsCount = riderThreadsCount;
        }

        public static int RidersPerElevatorCount { get; private set; }
        public static void SetRidersPerElevatorCount(int ridersPerElevatorCount)
        {
            RidersPerElevatorCount = ridersPerElevatorCount;
        }

        public static Queue<Request> Requests { get; private set; }
        public static void SetRequests(IEnumerable<Request> requests)
        {
            Requests = new Queue<Request>(requests);
        }
    }
}
