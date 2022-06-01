using System;
using System.Collections.Generic;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Interfaces.Next
{
    public interface IServiceCommands
    {
        Guid? Save(ServiceModel model);
        Guid? Publish(Guid id, Dictionary<LanguageEnum, PublishingModel> model);
        Guid? Archive(Guid id, LanguageEnum? language);
        Guid? Withdraw(Guid id, LanguageEnum? language);
        Guid? Restore(Guid id, LanguageEnum? language);
        Guid? Remove(Guid id);
    }
}