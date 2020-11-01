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
            List<Task> elevatorThreads = GlobalCache.Elevators
                .Select(e => new Task(new ElevatorThread(e).Process)).ToList();

            elevatorThreads.ForEach(t => t.Start());

            Task.WhenAll(elevatorThreads);
        }

        private static void RunAndWaitRiderThreads()
        {
            List<Task> riderThreads = Enumerable.Range(1, GlobalCache.RiderThreadsCount)
                .Select(r => new Task(new RiderThread(r).Process)).ToList();

            riderThreads.ForEach(t => t.Start());

            Task.WhenAll(riderThreads).Wait();
        }
    }
}
