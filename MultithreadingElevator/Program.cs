using MultithreadingElevator.SchedulingLogic;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultithreadingElevator
{
    class Program
    {
        static void Main()
        {
            Configuration.Initialize();

            RunElevatorThreads();
            RunAndWaitRiderThreads();
        }

        private static void RunElevatorThreads()
        {
            List<Task> elevatorThreads = GlobalCache.Elevators.Select(e => new Task(e.Run)).ToList();

            elevatorThreads.ForEach(t => t.Start());
        }

        private static void RunAndWaitRiderThreads()
        {
            List<Task> riderThreads = Enumerable.Range(1, GlobalCache.RiderThreadsCount)
                .Select(r => new Task(() => ProcessRequest(r))).ToList();

            riderThreads.ForEach(t => t.Start());

            Task.WhenAll(riderThreads).Wait();
        }

        private static void ProcessRequest(int riderThreadNumber)
        {
            Request request = RequestManager.GetNextRequest();

            if (request == null)
            {
                return;
            }

            request.Rider.Run(riderThreadNumber, request.FloorFrom, request.FloorTo);
        }
    }
}
