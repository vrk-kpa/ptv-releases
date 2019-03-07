/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services.Import.Finto
{
    internal abstract class FintoImportServiceBase
    {
        protected static Dictionary<string, VmReplaceGroupServiceViewsJsonItem> GetMigratedFintoItems(List<VmServiceViewsJsonItem> sourceItems)
        {
            var replaced = sourceItems.Where(x => !string.IsNullOrEmpty(x.ReplacedBy)).GroupBy(x => x.ReplacedBy).ToList();
            var main = sourceItems.Where(x => string.IsNullOrEmpty(x.ReplacedBy))
                .ToDictionary(x => x.Id, x => new VmReplaceGroupServiceViewsJsonItem { Main = x });
            replaced.ForEach(x =>
            {
                var item = main.TryGet(x.Key);
                item?.Replaced.AddRange(x.Select(i => new VmReplaceItemServiceViewsJsonItem
                {
                    Item = i,
                    ReplacedBy = item.Main
                }));
            });
            return main;
        }
    }

    internal abstract class FintoImportServiceBase<TEntity> : FintoImportServiceBase, IFintoImportService<TEntity> where TEntity : FintoItemBase<TEntity>, new()
    {
        private readonly IContextManager contextManager;
        private readonly DataUtils dataUtils;
        private readonly ITranslationViewModel translationVmtoEnt;
        private ILogger<FintoImportServiceBase<TEntity>> logger;

        protected FintoImportServiceBase(ITranslationViewModel translationVmtoEnt, DataUtils dataUtils, IContextManager contextManager, ILoggerFactory loggerFactory)
        {
            this.translationVmtoEnt = translationVmtoEnt;
            this.dataUtils = dataUtils;
            this.contextManager = contextManager;
            logger = loggerFactory.CreateLogger<FintoImportServiceBase<TEntity>>();
        }

        public void SeedFintoItems(string content, string userName)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var sourceItems = JsonConvert.DeserializeObject<List<VmServiceViewsJsonItem>>(content);
                SeedFintoItems(sourceItems, unitOfWork);
                unitOfWork.Save(SaveMode.AllowAnonymous, userName: userName);
            });
        }

        public void SeedFintoItems(List<VmServiceViewsJsonItem> sourceItems, IUnitOfWorkWritable unitOfWork)
        {
            string userName = unitOfWork.GetUserNameForAuditing(SaveMode.AllowAnonymous);
            var main = GetMigratedFintoItems(sourceItems);
            var rep = unitOfWork.CreateRepository<IRepository<TEntity>>();
            rep.BatchUpdate(new TEntity(), ot => ot.IsValid, null, null, userName);
            
            var sourceItemsByKey = sourceItems.ToDictionary(x => x.Id);
            var entities = TranslateItems(main.Values.Where(x => x.Replaced.Count > 0).SelectMany(x => x.Replaced), unitOfWork);
            entities.AddRange(TranslateItems(
                main.Values.Where(x => x.Replaced.Count == 0).Select(x => x.Main), unitOfWork));
            dataUtils.JoinHierarchy(entities.GroupBy(x => x.Uri).ToDictionary(x => x.Key, x => x.FirstOrDefault()), item => sourceItemsByKey[item.Uri].BroaderURIs, (parent, child) =>
            {
                child.Parent = parent;
                parent.Children.Add(child);
            });
            logger.LogInformation($"Updating {typeof(TEntity).Name}: {entities.Count} items. ");
            //            RemoveOldFintoItems(entities, unitOfWork);
        }

        private List<TEntity> TranslateItems<TSource>(IEnumerable<TSource> source, ITranslationUnitOfWork unitOfWork) where TSource : class
        {
            return translationVmtoEnt.TranslateAll<TSource, TEntity>(source, unitOfWork).InclusiveToList();
        }
    }

    [RegisterService(typeof(IFintoImportService<ServiceClass>), RegisterType.Transient)]
    internal class ServiceClassImportService : FintoImportServiceBase<ServiceClass>
    {
        public ServiceClassImportService(ITranslationViewModel translationVmtoEnt, DataUtils dataUtils, IContextManager contextManager, ILoggerFactory loggerFactory) : base(translationVmtoEnt, dataUtils, contextManager, loggerFactory)
        {
        }
    }
    [RegisterService(typeof(IFintoImportService<LifeEvent>), RegisterType.Transient)]
    internal class LifeEventImportService : FintoImportServiceBase<LifeEvent>
    {
        public LifeEventImportService(ITranslationViewModel translationVmtoEnt, DataUtils dataUtils, IContextManager contextManager, ILoggerFactory loggerFactory) : base(translationVmtoEnt, dataUtils, contextManager, loggerFactory)
        {
        }
    }
    [RegisterService(typeof(IFintoImportService<TargetGroup>), RegisterType.Transient)]
    internal class TargetGroupImportService : FintoImportServiceBase<TargetGroup>
    {
        public TargetGroupImportService(ITranslationViewModel translationVmtoEnt, DataUtils dataUtils, IContextManager contextManager, ILoggerFactory loggerFactory) : base(translationVmtoEnt, dataUtils, contextManager, loggerFactory)
        {
        }
    }
    [RegisterService(typeof(IFintoImportService<IndustrialClass>), RegisterType.Transient)]
    internal class IndustrialClassImportService : FintoImportServiceBase<IndustrialClass>
    {
        public IndustrialClassImportService(ITranslationViewModel translationVmtoEnt, DataUtils dataUtils, IContextManager contextManager, ILoggerFactory loggerFactory) : base(translationVmtoEnt, dataUtils, contextManager, loggerFactory)
        {
        }
    }

    [RegisterService(typeof(IFintoImportService<OrganizationType>), RegisterType.Transient)]
    internal class OrganizationTypeImportService : FintoImportServiceBase<OrganizationType>
    {
        public OrganizationTypeImportService(ITranslationViewModel translationVmtoEnt, DataUtils dataUtils, IContextManager contextManager, ILoggerFactory loggerFactory) : base(translationVmtoEnt, dataUtils, contextManager, loggerFactory)
        {
        }
    }
}