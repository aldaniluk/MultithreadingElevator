using MultithreadingElevator.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MultithreadingElevator
{
    public class Elevator
    {
        private volatile int ridersCount;
        private bool areDoorsOpen;
        private Floor currentFloor = GlobalCache.Floors.First(f => f.Number == 1);
        private Dictionary<Floor, bool> floorsToStop = GlobalCache.Floors.ToDictionary(f => f, f => false);
        private int waitRidersForMilliSeconds = 2_000;

        public int Number { get; }

        public Elevator(int number)
        {
            Number = number;
        }

        #region state
        public ElevatorState State => GetState();

        private ElevatorState GetState()
        {
            Floor lowestFloorToStop, highestFloorToStop;

            lock (floorsToStop)
            {
                if (!floorsToStop.Any(f => f.Value))
                {
                    return ElevatorState.Waiting;
                }

                lowestFloorToStop = floorsToStop.FirstOrDefault(f => f.Value).Key;
                highestFloorToStop = floorsToStop.LastOrDefault(f => f.Value).Key;
            }

            if (CurrentFloor < lowestFloorToStop ||
                CurrentFloor == lowestFloorToStop && CurrentFloor < highestFloorToStop)
            {
                return ElevatorState.MovingUp;
            }

            if (CurrentFloor > highestFloorToStop ||
                CurrentFloor == highestFloorToStop && CurrentFloor > lowestFloorToStop)
            {
                return ElevatorState.MovingDown;
            }

            return ElevatorState.ObtainigRequests;
        }
        #endregion

        #region selected floors
        public void SelectFloor(Floor floor)
        {
            lock (floorsToStop)
            {
                floorsToStop[floor] = true;
            }
        }

        public void ReleaseFloor(Floor floor)
        {
            lock (floorsToStop)
            {
                floorsToStop[floor] = false;
            }
        }

        public bool IsSelectedFloor(Floor floor)
        {
            lock (floorsToStop)
            {
                return floorsToStop[floor];
            }
        }

        public bool HasSelectedFloors()
        {
            lock (floorsToStop)
            {
                return floorsToStop.Any(f => f.Value);
            }
        }
        #endregion

        #region doors
        public bool AreDoorsOpen => areDoorsOpen;

        public void OpenDoors()
        {
            areDoorsOpen = true;

            Thread.Sleep(500);
        }

        public void CloseDoors()
        {
            areDoorsOpen = false;

            Thread.Sleep(500);
        }
        #endregion

        #region riders
        public int RidersCount => ridersCount;

        public void IncreaseRidersCount()
        {
            ridersCount++;
            RidersCountChanged();
        }

        public void DecreaseRidersCount()
        {
            ridersCount--;
            RidersCountChanged();
        }

        public void WaitRiders()
        {
            //wait riders while they are entering/exiting
            Thread.Sleep(waitRidersForMilliSeconds);
        }

        private void RidersCountChanged()
        {
            //when each new rider enters/exits elevator, it should prolongue its waiting on CurrentFloor with open doors
            waitRidersForMilliSeconds += 1_000;
        }
        #endregion

        #region moving up-down one floor
        public Floor CurrentFloor => currentFloor;

        public void MoveUpOneFloor()
        {
            currentFloor = GlobalCache.Floors.First(f => f.Number == CurrentFloor.Number + 1);

            Thread.Sleep(500);
        }

        public void MoveDownOneFloor()
        {
            currentFloor = GlobalCache.Floors.First(f => f.Number == CurrentFloor.Number - 1);

            Thread.Sleep(500);
        }
        #endregion
    }
}
