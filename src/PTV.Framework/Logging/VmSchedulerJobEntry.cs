namespace PTV.Framework.Logging
{
    /// <summary>
    /// Job log entry
    /// </summary>
    public class VmJobLogEntry
    {
        public string OperationId { get; set; }
        public string JobStatus { get; set; }
        public string JobType { get; set; }
        public string ExecutionType { get; set; }
        public string UserName { get; set; }
    }
}