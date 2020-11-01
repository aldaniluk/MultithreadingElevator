namespace MultithreadingElevator
{
    public static class GlobalFactory
    {
        public static Elevator CreateElevator(int number)
        {
            return new Elevator(number);
        }

        public static Rider CreateRider(int number)
        {
            return new Rider(number);
        }

        public static Floor CreateFloor(int number)
        {
            return new Floor(number);
        }
    }
}
