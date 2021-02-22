using MultithreadingElevator.Models;
using MultithreadingElevator.SchedulingLogic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultithreadingElevator
{
    public class Floor
    {
        private Dictionary<Direction, FloorDirectionButton> Buttons =
            new List<Direction>((Direction[])Enum.GetValues(typeof(Direction)))
                .ToDictionary(d => d, d => new FloorDirectionButton());

        public Dictionary<Direction, FloorDirectionButtonEvent> Events =
            new List<Direction>((Direction[])Enum.GetValues(typeof(Direction)))
                .ToDictionary(d => d, d => new FloorDirectionButtonEvent());

        public int Number { get; }

        public Floor(int number)
        {
            Number = number;
        }

        public void ElevatorComes(Elevator elevator, Direction direction)
        {
            lock (Buttons[direction].ElevatorsToEnter)
            {
                Events[direction].RidersCanEnterEvent.Set();

                Buttons[direction].ElevatorsToEnter.Add(elevator);
            }
        }

        public void ElevatorLeaves(Elevator elevator, Direction direction)
        {
            lock (Buttons[direction].ElevatorsToEnter)
            {
                Buttons[direction].ElevatorsToEnter.Remove(elevator);

                if (!Buttons[direction].ElevatorsToEnter.Any())
                {
                    Buttons[direction].IsPressed = false;

                    Events[direction].RidersCanEnterEvent.Reset();
                    Events[direction].ButtonReleasedEvent.Set();
                }
            }
        }

        public List<Elevator> GetElevatorsToEnter(Direction direction)
        {
            lock (Buttons[direction].ElevatorsToEnter)
            {
                return Buttons[direction].ElevatorsToEnter.Select(e => e).ToList();
            }
        }

        public void SelectButton(Direction direction)
        {
            lock (Buttons[direction].PressButtonLock)
            {
                if (!Buttons[direction].IsPressed)
                {
                    ElevatorManager.FindAppropriateElevator(direction, this);
                }

                Buttons[direction].IsPressed = true;

                Events[direction].ButtonReleasedEvent.Reset();
            }
        }
    }
}