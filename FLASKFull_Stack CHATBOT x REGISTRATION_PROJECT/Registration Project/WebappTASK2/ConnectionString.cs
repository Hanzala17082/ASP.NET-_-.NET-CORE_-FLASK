namespace WebappTASK2
{
    public static class ConnectionString
    {
        // Corrected connection string with the correct path
        private static string CS = "Server=(localdb)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\muham\\Registration.mdf;Trusted_Connection=True;";

        public static string dbcs { get => CS; }
    }
}
