using System;
using System.Collections.Generic;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Interfaces.Next
{
    public interface ITranslationCommands
    {
        bool OrderService(Guid serviceId, TranslationOrderModel model, out Guid newId);
    }
}