using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Service;
using PTV.Framework;

namespace PTV.Database.DataAccess.Interfaces.Services.V2
{
    public interface IServiceServiceNext
    {
        Guid? SaveServiceSimple(VmServiceInput model);
        
        Dictionary<string, List<ValidationMessage>> GetValidatedEntitySimple(Guid id);
        
        Guid? ArchiveLanguageSimple(VmEntityBasic model);
        
        Guid? DeleteServiceSimple(Guid id);

        Guid? WithdrawLanguageSimple(VmServiceBasic model);

        Guid? WithdrawServiceSimple(Guid id);

        Guid? RestoreLanguageSimple(VmServiceBasic model);

        Guid? RestoreServiceSimple(Guid id);

        Guid? RemoveServiceSimple(Guid id);
    }
}