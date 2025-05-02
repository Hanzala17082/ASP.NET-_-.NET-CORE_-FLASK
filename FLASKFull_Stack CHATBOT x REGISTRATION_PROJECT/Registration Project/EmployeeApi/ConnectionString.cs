namespace EmployeeAPI
{
    public static class ConnectionString
    {
        private static string cs = "Server=HANZALA\\SQLEXPRESS; Database=CrudADOdb; Trusted_Connection=True";

        public static string dbcs => cs;
    }
}
