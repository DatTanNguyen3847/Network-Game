namespace GameEngine
{
    public static class Events
    {
        public const string Connection = "connection";
        public const string Disconnect = "disconnect";
        public const string Close = "close";

        public const string Connected = "ct";
        public const string AddPlayer = "ap";
        public const string RemovePlayer = "rp";
        // network stats
        public const string Ping = "pi";
        public const string Pong = "po";

        public const string ClientUpdate = "cup";

        public const string ServerUpdate = "sup";
        // CPU usage
        public const string CpuUsage = "cpu";
    }
}