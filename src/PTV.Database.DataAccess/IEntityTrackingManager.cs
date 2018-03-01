namespace PTV.Database.DataAccess
{
    internal interface IEntityTrackingManager
    {
        void ProcessKnownEntities(TrackingContextInfo trackingInformation);
    }
}