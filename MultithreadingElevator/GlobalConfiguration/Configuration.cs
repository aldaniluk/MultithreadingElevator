using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MultithreadingElevator
{
    public static class Configuration
    {
        private static readonly char separator = ' ';

        private static int configurationLine = 1;
        private static int configurationFloorsColumn = 0;
        private static int configurationElevatorsColumn = 1;
        private static int configurationRidersColumn = 2;
        private static int configurationRiderThreadsColumn = 3;
        private static int configurationRidersPerElevatorColumn = 4;

        private static int requestsStartLine = 3;
        private static int requestRiderColumn = 0;
        private static int requestFloorFromColumn = 1;
        private static int requestFloorToColumn = 2;

        public static void Initialize()
        {
            string[] lines = GetLines();

            ParseConfiguration(lines[configurationLine]);
            ParseRequests(lines.Skip(requestsStartLine));
        }

        private static void ParseConfiguration(string configurationLine)
        {
            int[] lineNumbers = configurationLine.Split(separator).Select(int.Parse).ToArray();

            int floorsCount = lineNumbers[configurationFloorsColumn];
            List<Floor> floors = Enumerable.Range(1, floorsCount).Select(GlobalFactory.CreateFloor).ToList();
            GlobalCache.SetFloors(floors);

            int elevatorsCount = lineNumbers[configurationElevatorsColumn];
            List<Elevator> elevators = Enumerable.Range(1, elevatorsCount).Select(GlobalFactory.CreateElevator).ToList();
            GlobalCache.SetElevators(elevators);

            int ridersCount = lineNumbers[configurationRidersColumn];
            List<Rider> riders = Enumerable.Range(1, ridersCount).Select(GlobalFactory.CreateRider).ToList();
            GlobalCache.SetRiders(riders);

            int riderThreadsCount = lineNumbers[configurationRiderThreadsColumn];
            GlobalCache.SetRiderThreadsCount(riderThreadsCount);

            int ridersPerElevatorCount = lineNumbers[configurationRidersPerElevatorColumn];
            GlobalCache.SetRidersPerElevatorCount(ridersPerElevatorCount);
        }

        private static void ParseRequests(IEnumerable<string> requestLines)
        {
            var requests = new List<Request>();
            foreach (string requestLine in requestLines)
            {
                int[] requestNumbers = requestLine.Split(separator).Select(s => int.Parse(s)).ToArray();

                requests.Add(new Request(
                    GlobalCache.Riders.First(r => r.Number == requestNumbers[requestRiderColumn]),
                    GlobalCache.Floors.First(f => f.Number == requestNumbers[requestFloorFromColumn]),
                    GlobalCache.Floors.First(f => f.Number == requestNumbers[requestFloorToColumn])));
            }

            GlobalCache.SetRequests(requests);
        }

        private static string[] GetLines()
        {
            return File.ReadAllLines($"{Directory.GetCurrentDirectory()}/data.txt");
        }
    }
}
