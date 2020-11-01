using System.Linq;

namespace MultithreadingElevator.SchedulingLogic
{
    public static class RequestManager
    {
        private static object requestsLock = new object();

        public static Request GetNextRequest()
        {
            lock (requestsLock)
            {
                if (GlobalCache.Requests.Any())
                {
                    return GlobalCache.Requests.Dequeue();
                }

                return null;
            }
        }
    }
}
