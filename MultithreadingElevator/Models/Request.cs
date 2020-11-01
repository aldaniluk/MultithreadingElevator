namespace MultithreadingElevator
{
    public class Request
    {
        public Rider Rider { get; }

        public Floor FloorFrom { get; }

        public Floor FloorTo { get; }

        public Request(Rider rider, Floor floorFrom, Floor floorTo)
        {
            Rider = rider;
            FloorFrom = floorFrom;
            FloorTo = floorTo;
        }
    }
}
