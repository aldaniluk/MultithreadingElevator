using System.Collections.Generic;
using System.Threading;

namespace MultithreadingElevator.Models
{
    public class FloorDirectionButton
    {
        public bool IsPressed { get; set; }

        public List<Elevator> ElevatorsToEnter { get; set; }

        public object PressButtonLock { get; set; }

        public FloorDirectionButton()
        {
            IsPressed = false;
            ElevatorsToEnter = new List<Elevator>();
            PressButtonLock = new object();
        }
    }

    public class FloorDirectionButtonEvent
    {
        public ManualResetEvent ButtonReleasedEvent { get; set; }

        public ManualResetEvent RidersCanEnterEvent { get; set; }

        public FloorDirectionButtonEvent()
        {
            ButtonReleasedEvent = new ManualResetEvent(true);
            RidersCanEnterEvent = new ManualResetEvent(false);
        }
    }
}
