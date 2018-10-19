namespace PTV.Framework
{
    public class MainConnectionString
    {
        public string ConnectionString { get; }

        public MainConnectionString(string connectionString)
        {
            this.ConnectionString = connectionString;
        }
    }
}