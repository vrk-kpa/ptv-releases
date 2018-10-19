namespace PTV.Database.DataAccess.Interfaces
{
    public class DataContextOptions
    {
        public string RetriesOnDeadlock { get; set; }
        public bool LogContextCalls { get; set; }
    }
}