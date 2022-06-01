using System.Collections.Generic;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Next.Validation
{
    public static class ServiceStatusValidation
    {
        private static readonly List<PublishingStatus> AllowedStatusesToArchive = new List<PublishingStatus>
            {PublishingStatus.Draft, PublishingStatus.Published, PublishingStatus.OldPublished, PublishingStatus.Modified};

        private static readonly List<PublishingStatus> AllowedStatusesToPublish = new List<PublishingStatus>
            {PublishingStatus.Draft, PublishingStatus.Modified, PublishingStatus.Published};

        private static readonly List<PublishingStatus> AllowedStatusesToRestore = new List<PublishingStatus>
            {PublishingStatus.Draft};

        private static readonly List<PublishingStatus> AllowedStatusesToRemove = new List<PublishingStatus>
            {PublishingStatus.Modified, PublishingStatus.Deleted};

        public static bool CanArchiveLanguageVersion(PublishingStatus status)
        {
            return AllowedStatusesToArchive.Contains(status);
        }

        public static bool CanPublishLanguageVersion(PublishingStatus status)
        {
            return AllowedStatusesToPublish.Contains(status);
        }

        public static bool CanRestoreLanguageVersion(PublishingStatus status)
        {
            return AllowedStatusesToRestore.Contains(status);
        }

        public static bool CanRemoveService(PublishingStatus status)
        {
            return AllowedStatusesToRemove.Contains(status);
        }
    }
}