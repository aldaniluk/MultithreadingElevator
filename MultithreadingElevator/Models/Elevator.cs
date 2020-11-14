using MultithreadingElevator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MultithreadingElevator
{
    public class Elevator
    {
        private int ridersCount;
        private Dictionary<Floor, bool> floorsToStop = GlobalCache.Floors.ToDictionary(f => f, f => false);
        private int waitRidersForMilliSeconds = 2_000;
        private AutoResetEvent currentElevatorObtainedRequestEvent = new AutoResetEvent(false);
        private object enterRiderLock = new object();

        public Dictionary<Floor, ManualResetEvent> RidersCanExitEvents = GlobalCache.Floors
            .ToDictionary(f => f, f => new ManualResetEvent(false));

        public Floor CurrentFloor { get; private set; } = GlobalCache.Floors.First(f => f.Number == 1);

        public Direction? Direction { get; private set; }

        public int Number { get; }

        public ElevatorState State { get; set; } = ElevatorState.Wait;

        public Elevator(int number)
        {
            Number = number;
        }

        public void SelectFloor(Floor floor, Direction? direction = null)
        {
            lock (floorsToStop)
            {
                floorsToStop[floor] = true;
            }

            Direction = direction ?? Direction;

            currentElevatorObtainedRequestEvent.Set();
        }

        public bool TryEnter()
        {
            lock (enterRiderLock)
            {
                if (ridersCount < GlobalCache.RidersPerElevatorCount)
                {
                    Interlocked.Increment(ref ridersCount);

                    RidersCountChanged();

                    return true;
                }
            }

            return false;
        }

        public void Exit()
        {
            Interlocked.Decrement(ref ridersCount);

            RidersCountChanged();
        }

        public void Run()
        {
            while (true)
            {
                switch (State)
                {
                    case ElevatorState.Wait:
                        Wait();
                        break;
                    case ElevatorState.Move:
                        Move();
                        break;
                    case ElevatorState.Stop:
                        Stop();
                        break;
                    default:
                        throw new Exception("Unsupported ElevatorState");

                }
            }
        }

        private void Wait()
        {
            currentElevatorObtainedRequestEvent.WaitOne();

            State = ElevatorState.Move;
        }

        private void Move()
        {
            Floor lowestFloor, highestFloor;
            lock (floorsToStop)
            {
                lowestFloor = floorsToStop.FirstOrDefault(f => f.Value).Key;
                highestFloor = floorsToStop.LastOrDefault(f => f.Value).Key;
            }

            if (lowestFloor == null && highestFloor == null)
            {
                State = ElevatorState.Wait;

                return;
            }

            if (lowestFloor == CurrentFloor || highestFloor == CurrentFloor)
            {
                State = ElevatorState.Stop;

                return;
            }

            if (CurrentFloor < lowestFloor)
            {
                Console.WriteLine($"E{Number} moves up to F{CurrentFloor.Number + 1}");

                CurrentFloor = GlobalCache.Floors.First(f => f.Number == CurrentFloor.Number + 1);
            }
            else
            {
                Console.WriteLine($"E{Number} moves down to F{CurrentFloor.Number - 1}");

                CurrentFloor = GlobalCache.Floors.First(f => f.Number == CurrentFloor.Number - 1);
            }
        }

        private void Stop()
        {
            Console.WriteLine($"E{Number} on F{CurrentFloor} opens");

            RidersCanExitEvents[CurrentFloor].Set();
            WaitRiders();
            RidersCanExitEvents[CurrentFloor].Reset();

            CurrentFloor.ElevatorComes(this, Direction.Value);

            CurrentFloor.Events[Direction.Value].RidersCanEnterEvent.Set();
            WaitRiders();
            CurrentFloor.Events[Direction.Value].RidersCanEnterEvent.Reset();

            Console.WriteLine($"E{Number} on F{CurrentFloor} closes");

            //release buttons
            lock (floorsToStop)
            {
                floorsToStop[CurrentFloor] = false;
            }

            CurrentFloor.ElevatorLeaves(this, Direction.Value);

            State = ElevatorState.Move;
        }

        private void WaitRiders()
        {
            //wait riders while they are entering/exiting
            Thread.Sleep(waitRidersForMilliSeconds);
        }

        private void RidersCountChanged()
        {
            //when each new rider enters/exits elevator, it should prolongue its waiting on CurrentFloor with open doors
            waitRidersForMilliSeconds += 5_00;
        }
    }
}
