namespace PTV.Framework.Interfaces
{
    public interface IMessage
    {
        string SubCode { get; set; }
        string Code { get; set; }
        string Params { get; set; }
    }

    public interface IError : IMessage
    {
        string StackTrace { get; }
    }
}