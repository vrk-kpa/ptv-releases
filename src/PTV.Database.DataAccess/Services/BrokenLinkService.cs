/**
 * The MIT License
 * Copyright (c) 2020 Finnish Digital Agency (DVV)
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
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using PTV.Domain.Model.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Database.Model.Views;
using PTV.Domain.Model.Enums;
using PTV.Framework.Extensions;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IBrokenLinkService), RegisterType.Transient)]
    internal class BrokenLinkService : IBrokenLinkService
    {
        private readonly IContextManager contextManager;
        private readonly ITranslationViewModel translationViewModel;
        private readonly ITranslationEntity translationEntity;
        private readonly IOrganizationTreeDataCache organizationCache;
        private readonly LinkValidatorConfiguration configuration;
        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;
        private readonly IServiceUtilities serviceUtilities;
        private const int MAX_RESULTS = 10;

        public BrokenLinkService(
            ApplicationConfiguration configuration,
            IContextManager contextManager,
            ITranslationEntity translationEntity,
            ITranslationViewModel translationViewModel,
            IOrganizationTreeDataCache organizationCache,
            ILanguageCache languageCache,
            ICacheManager cacheManager,
            IServiceUtilities serviceUtilities)
        {
            this.contextManager = contextManager;
            this.translationEntity = translationEntity;
            this.translationViewModel = translationViewModel;
            this.organizationCache = organizationCache;
            this.configuration = configuration.GetLinkValidatorConfiguration();
            this.languageCache = languageCache;
            this.typesCache = cacheManager.TypesCache;
            this.serviceUtilities = serviceUtilities;
        }

        public async Task<VmBrokenLink> Check(VmBrokenLink link)
        {
            var isException = contextManager.ExecuteReader(unitOfWork =>
            {
                var webPageRepo = unitOfWork.CreateRepository<IWebPageRepository>();
                var webPage = webPageRepo.All().FirstOrDefault(w => w.Url == link.Url);
                return webPage?.IsException ?? false;
            });

            if (isException)
            {
                link.IsException = true;
                link.IsBroken = false;
                return link;
            }

            return await PtvHttpClient.UseAsync(async client =>
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(configuration.Timeout));
                return await VerifyWebPage(link, client, cts.Token);
            });
        }

        public async Task<string> CheckAllWebPages(HttpClient client)
        {
            var counter = 0;
            var log = new StringBuilder();
            foreach (var batch in GetWebPages().Batch(configuration.BatchSize))
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(configuration.Timeout));
                log.AppendLine($"{DateTime.UtcNow} - Processing batch {counter}");

                var changedBrokenLinks = await GetUpdatedBrokenLinks(batch, client, cts.Token, log);

                log.AppendLine($"{DateTime.UtcNow} - Saving batch {counter++}");
                SaveChanges(changedBrokenLinks);
            }

            return log.ToString();
        }

        private async Task<Dictionary<string, bool>> GetUpdatedBrokenLinks(IEnumerable<VmBrokenLink> webPages,
            HttpClient client, CancellationToken token, StringBuilder log)
        {
            var changed = new Dictionary<string, bool>();

            try
            {
                var dictionary = webPages.ToDictionary(w => w.Url);
                var jsonString = JsonConvert.SerializeObject(dictionary.Select(x => x.Key));
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(configuration.ValidationUrl, content, token);
                var stringResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<LinkValidatorResponse>(stringResponse);

                if (result?.Results == null)
                {
                    log.AppendLine($"NULL RESPONSE ({response.StatusCode.ToString()}): {stringResponse}");
                    return changed;
                }

                foreach (var link in result.Results)
                {
                    if (link.InternalError)
                    {
                        log.AppendLine($"INTERNAL ERROR for link {link.Url}");
                    }

                    if (link.IsBroken != dictionary[link.Url].IsBroken)
                    {
                        log.AppendLine($"Switching BROKEN flag for {link.Url} to {link.IsBroken}");
                        changed.Add(link.Url, link.IsBroken);
                    }
                }

                response.Dispose();
                content.Dispose();
            }
            catch (OperationCanceledException oce)
            {
                log.AppendLine("TIMEOUT");
                log.AppendLine(oce.FlattenWithInnerExceptions());
            }
            catch (Exception e)
            {
                log.AppendLine("ERROR");
                log.AppendLine(e.FlattenWithInnerExceptions());
            }

            return changed;
        }

        private async Task<VmBrokenLink> VerifyWebPage(VmBrokenLink webPage, HttpClient client, CancellationToken token)
        {
            if (webPage.Url.IsNullOrWhitespace())
            {
                return webPage;
            }

            try
            {
                var jsonString = JsonConvert.SerializeObject(new List<string> {webPage.Url});
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(configuration.ValidationUrl, content, token);
                var result = JsonConvert.DeserializeObject<LinkValidatorResponse>(await response.Content.ReadAsStringAsync());

                webPage.IsBroken = result.Results.First().IsBroken;

                response.Dispose();
                content.Dispose();
            }
            catch (Exception)
            {
                // If link validator service cannot be reached, display link as ok to avoid false positives.
                webPage.IsBroken = false;
            }

            return webPage;
        }

        public int GetUnstableLinksCount(IEnumerable<Guid> forOrganizations, bool isException, IUnitOfWork unitOfWork)
        {
            return GetBrokenLinksInternal(unitOfWork, isException, forOrganizations).Select(x => x.Url).Distinct()
                .Count();
        }

        public VmOrganizationBrokenLink Update(VmOrganizationBrokenLink model)
        {
            if (model?.Url.IsNullOrWhitespace() ?? true)
            {
                throw new ArgumentNullException($"{nameof(model)} and {nameof(VmOrganizationBrokenLink.Url)} cannot be null.");
            }

            var mainOrganizationId = serviceUtilities.GetUserMainOrganization();
            var organizationTree = organizationCache.GetAllSubOrganizationIds(mainOrganizationId);

            var result = contextManager.ExecuteWriter(unitOfWork =>
            {
                var webPageRepo = unitOfWork.CreateRepository<IWebPageRepository>();
                var oldWebPage = webPageRepo.All().First(x => x.Id == model.Id);
                var newWebPage = translationViewModel.Translate<string, WebPage>(model.Url, unitOfWork);

                if (newWebPage.Url != oldWebPage.Url)
                {
                    RelinkServices(unitOfWork, model, newWebPage, organizationTree);
                    RelinkChannels(unitOfWork, model, newWebPage, organizationTree);
                    RelinkConnections(unitOfWork, model, newWebPage, organizationTree);
                    RelinkOrganizations(unitOfWork, model, newWebPage, organizationTree);
                    RelinkLaws(unitOfWork, model, newWebPage, organizationTree);
                    RelinkEChannels(unitOfWork, model, newWebPage, organizationTree);
                    RelinkPrintableForms(unitOfWork, model, newWebPage, organizationTree);
                    RelinkWebChannels(unitOfWork, model, newWebPage, organizationTree);
                }
                else
                {
                    // If they are not different, point them to the same object, so that
                    // there are no conflicts for EF.
                    newWebPage = oldWebPage;
                }

                if (model.IsException != newWebPage.IsException ||
                         model.ExceptionComment != newWebPage.ExceptionComment)
                {
                    newWebPage.IsException = model.IsException;
                    newWebPage.ExceptionComment = model.ExceptionComment;
                }

                unitOfWork.Save();

                return GetBrokenLinks(unitOfWork, newWebPage.IsException, organizationTree, 0, newWebPage.Id).ToList();
            });

            if (result.Count > 1)
                throw new OverflowException("Multiple results not supported");
            return result.FirstOrDefault();
        }

        private void RelinkWebChannels(IUnitOfWorkWritable unitOfWork, VmOrganizationBrokenLink model, WebPage newWebPage, List<Guid> organizationTree)
        {
            var webUrlRepo = unitOfWork.CreateRepository<IWebpageChannelUrlRepository>();
            var webChannels = webUrlRepo.All()
                .Where(x => x.WebPageId == model.Id &&
                            organizationTree.Contains(x.WebpageChannel.ServiceChannelVersioned.OrganizationId));

            foreach (var webChannel in webChannels)
            {
                webChannel.WebPage = newWebPage;
            }

            unitOfWork.Save();
        }

        private void RelinkPrintableForms(IUnitOfWorkWritable unitOfWork, VmOrganizationBrokenLink model, WebPage newWebPage, List<Guid> organizationTree)
        {
            var printableFormUrlRepo = unitOfWork.CreateRepository<IPrintableFormChannelUrlRepository>();
            var printableForms = printableFormUrlRepo.All()
                .Where(x => x.WebPageId == model.Id &&
                            organizationTree.Contains(x.PrintableFormChannel.ServiceChannelVersioned.OrganizationId));

            foreach (var printableForm in printableForms)
            {
                printableForm.WebPage = newWebPage;
            }

            unitOfWork.Save();
        }

        private void RelinkEChannels(IUnitOfWorkWritable unitOfWork, VmOrganizationBrokenLink model, WebPage newWebPage, List<Guid> organizationTree)
        {
            var eChannelUrlRepo = unitOfWork.CreateRepository<IElectronicChannelUrlRepository>();
            var eChannels = eChannelUrlRepo.All()
                .Where(x => x.WebPageId == model.Id &&
                            organizationTree.Contains(x.ElectronicChannel.ServiceChannelVersioned.OrganizationId));

            foreach (var eChannel in eChannels)
            {
                eChannel.WebPage = newWebPage;
            }

            unitOfWork.Save();
        }

        private void RelinkLaws(IUnitOfWorkWritable unitOfWork, VmOrganizationBrokenLink model, WebPage newWebPage, List<Guid> organizationTree)
        {
            var serviceLawRepo = unitOfWork.CreateRepository<IServiceLawRepository>();
            var laws = serviceLawRepo.All()
                .Include(x => x.Law)
                .ThenInclude(x => x.WebPages)
                .Include(x => x.ServiceVersioned)
                .Where(x => organizationTree.Contains(x.ServiceVersioned.OrganizationId))
                .SelectMany(x => x.Law.WebPages)
                .Where(x => x.WebPageId == model.Id)
                .Distinct();

            foreach (var law in laws)
            {
                law.WebPage = newWebPage;
            }

            unitOfWork.Save();
        }

        private void RelinkOrganizations(IUnitOfWorkWritable unitOfWork, VmOrganizationBrokenLink model, WebPage newWebPage, List<Guid> organizationTree)
        {
            var repo = unitOfWork.CreateRepository<IOrganizationWebPageRepository>();
            var organizations =  repo.All()
                .Where(x => x.WebPageId == model.Id && organizationTree.Contains(x.OrganizationVersioned.UnificRootId))
                .Distinct();

            foreach (var organization in organizations)
            {
                organization.WebPage = newWebPage;
            }

            unitOfWork.Save();
        }

        private void RelinkConnections(IUnitOfWorkWritable unitOfWork, VmOrganizationBrokenLink model, WebPage newWebPage, List<Guid> organizationTree)
        {
            var serviceVersionedRepo = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var connections = new List<ServiceServiceChannelWebPage>();
            connections.AddRange(serviceVersionedRepo.All()
                .Include(w => w.UnificRoot)
                .ThenInclude(x => x.ServiceServiceChannels)
                .ThenInclude(x => x.ServiceServiceChannelWebPages)
                .Where(x => organizationTree.Contains(x.OrganizationId))
                .SelectMany(x => x.UnificRoot.ServiceServiceChannels.SelectMany(y => y.ServiceServiceChannelWebPages))
                .Where(x => x.WebPageId == model.Id)
                .Distinct());

            var channelVersionedRepo = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            connections.AddRange(channelVersionedRepo.All()
                .Include(w => w.UnificRoot)
                .ThenInclude(x => x.ServiceServiceChannels)
                .ThenInclude(x => x.ServiceServiceChannelWebPages)
                .Where(x => organizationTree.Contains(x.OrganizationId))
                .SelectMany(x => x.UnificRoot.ServiceServiceChannels.SelectMany(y => y.ServiceServiceChannelWebPages))
                .Where(x => x.WebPageId == model.Id)
                .Distinct());

            foreach (var connection in connections.Distinct())
            {
                connection.WebPage = newWebPage;
            }

            unitOfWork.Save();
        }

        private void RelinkChannels(IUnitOfWorkWritable unitOfWork, VmOrganizationBrokenLink model, WebPage newWebPage, List<Guid> organizationTree)
        {
            var repo = unitOfWork.CreateRepository<IServiceChannelWebPageRepository>();
            var channels = repo.All()
                .Where(x => x.WebPageId == model.Id && organizationTree.Contains(x.ServiceChannelVersioned.OrganizationId))
                .Distinct();

            foreach (var channel in channels)
            {
                channel.WebPage = newWebPage;
            }

            unitOfWork.Save();
        }

        private void RelinkServices(IUnitOfWorkWritable unitOfWork, VmOrganizationBrokenLink model, WebPage newWebPage, List<Guid> organizationTree)
        {
            var repo = unitOfWork.CreateRepository<IServiceWebPageRepository>();
            var services = repo.All()
                .Where(x => x.WebPageId == model.Id && organizationTree.Contains(x.ServiceVersioned.OrganizationId))
                .Distinct();

            foreach (var service in services)
            {
                service.WebPage = newWebPage;
            }

            unitOfWork.Save();
        }

        public IEnumerable<VmOrganizationBrokenLink> GetBrokenLinks(IUnitOfWork unitOfWork, bool isException,
            IEnumerable<Guid> organizationTree,int pageNumber = 0, params Guid[] webPageIds)
        {
            var entities = GetBrokenLinksInternal(unitOfWork, isException, organizationTree, webPageIds).ToList();
            var groups = entities.GroupBy(x => x.Url)
                .ToDictionary(x => x.Key, x => x.ToList())
                .OrderBy(x => x.Key);
            var namesCache = GetBrokenLinkNames(entities, unitOfWork);
            var organizationNames = GetOrganizationNames(entities.Select(x => x.OrganizationId).Distinct());

            var page = pageNumber.PositiveOrZero();
            var take = (page + 1) * MAX_RESULTS;
            foreach (var (key, value) in groups)
            {
                var first = value.First();
                yield return new VmOrganizationBrokenLink
                {
                    Id = first.WebPageId,
                    Url = key,
                    ExceptionComment = first.ExceptionComment,
                    IsBroken = true,
                    IsException = first.IsException,
                    ValidationDate = first.ValidationDate,
                    Content = GetBrokenLinkDetails(value.OrderBy(x=>x.WebPageId).Take(take).ToList(), namesCache, organizationNames).ToList(),
                    Count = value.Count,
                    MoreAvailable = value.Count.MoreResultsAvailable(page, MAX_RESULTS),
                    PageNumber = page + 1
                };
            }
        }

        private BrokenLinkNamesCache GetBrokenLinkNames(List<VBrokenLink> entities, IUnitOfWork unitOfWork)
        {
            var result = new BrokenLinkNamesCache();
            var mainNameType = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());

            // Channels
            var channelIds = entities.Where(e => e.EntityType == SearchEntityTypeEnum.Channel.ToString())
                .Select(e => e.EntityId);
            var channelNameRepo = unitOfWork.CreateRepository<IServiceChannelNameRepository>();
            var channelNames = channelNameRepo.All()
                .Where(x => channelIds.Contains(x.ServiceChannelVersionedId) && x.TypeId == mainNameType)
                .Select(x => new {x.LocalizationId, x.Name, x.ServiceChannelVersionedId})
                .ToList()
                .GroupBy(x => x.ServiceChannelVersionedId);
            foreach (var channelName in channelNames)
            {
                result.Add((SearchEntityTypeEnum.Channel, channelName.Key, null),
                    channelName.ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name));
            }

            // Services
            var serviceIds = entities.Where(e => e.EntityType == SearchEntityTypeEnum.Service.ToString())
                .Select(e => e.EntityId);
            var serviceNameRepo = unitOfWork.CreateRepository<IServiceNameRepository>();
            var serviceNames = serviceNameRepo.All()
                .Where(x => serviceIds.Contains(x.ServiceVersionedId) && x.TypeId == mainNameType)
                .Select(x => new {x.LocalizationId, x.Name, x.ServiceVersionedId})
                .ToList()
                .GroupBy(x => x.ServiceVersionedId);
            foreach (var serviceName in serviceNames)
            {
                result.Add((SearchEntityTypeEnum.Service, serviceName.Key, null),
                    serviceName.ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name));
            }

            // Organizations
            var organizationIds = entities.Where(e => e.EntityType == SearchEntityTypeEnum.Organization.ToString())
                .Select(e => e.EntityId);
            // Cannot use organization cache because it does not contain all organization versions
            var organizationNameRepo = unitOfWork.CreateRepository<IOrganizationNameRepository>();
            var organizationNames = organizationNameRepo.All()
                .Where(x => organizationIds.Contains(x.OrganizationVersionedId) && x.TypeId == mainNameType)
                .Select(x => new {x.LocalizationId, x.Name, x.OrganizationVersionedId})
                .ToList()
                .GroupBy(x => x.OrganizationVersionedId);
            foreach (var organizationName in organizationNames)
            {
                result.Add((SearchEntityTypeEnum.Organization, organizationName.Key, null),
                    organizationName.ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name));
            }

            // Connections
            var connectionIds = entities.Where(e => e.EntityType == SearchEntityTypeEnum.Connection.ToString())
                .Select(e => ( ServiceId: e.EntityId,  ChannelId: e.ConnectedChannelId.Value))
                .Distinct()
                .ToList();
            var connectionChannelIds = connectionIds.Select(c => c.ChannelId).ToList();
            var connectionServiceIds = connectionIds.Select(c => c.ServiceId).ToList();
            var connectionChannelNames = channelNameRepo.All()
                .Where(x => connectionChannelIds.Contains(x.ServiceChannelVersionedId) && x.TypeId == mainNameType)
                .Select(x => new {x.LocalizationId, x.Name, x.ServiceChannelVersionedId})
                .ToList()
                .GroupBy(x => x.ServiceChannelVersionedId)
                .ToDictionary(x => x.Key, x => x.ToList());
            var connectionServiceNames = serviceNameRepo.All()
                .Where(x => connectionServiceIds.Contains(x.ServiceVersionedId) && x.TypeId == mainNameType)
                .Select(x => new {x.LocalizationId, x.Name, x.ServiceVersionedId})
                .ToList()
                .GroupBy(x => x.ServiceVersionedId)
                .ToDictionary(x => x.Key, x => x.ToList());

            foreach (var connectionId in connectionIds)
            {
                connectionChannelNames.TryGetValue(connectionId.ChannelId, out var connectionChannelName);
                connectionServiceNames.TryGetValue(connectionId.ServiceId, out var connectionServiceName);

                if (connectionChannelName != null && connectionServiceName != null)
                {
                    var connectionName =
                        connectionChannelName.Join(connectionServiceName, c => c.LocalizationId, s => s.LocalizationId,
                            (c, s) => new
                            {
                                c.LocalizationId,
                                ChannelName = c.Name,
                                ServiceName = s.Name
                            });
                    result.Add((SearchEntityTypeEnum.Connection, connectionId.ServiceId, connectionId.ChannelId),
                        connectionName.ToDictionary(x => languageCache.GetByValue(x.LocalizationId),
                            x => $"{x.ServiceName} & {x.ChannelName}"));
                }
            }

            return result;
        }

        private IEnumerable<VmOrganizationBrokenLinkDetail> GetBrokenLinkDetails(List<VBrokenLink> entities,
            BrokenLinkNamesCache namesCache, Dictionary<Guid, Dictionary<string, string>> organizationNames)
        {
            foreach (var entity in entities)
            {
                var entityType = Enum.Parse<SearchEntityTypeEnum>(entity.EntityType);
                var subEntityType = Enum.Parse<SearchEntityTypeEnum>(entity.SubEntityType);
                namesCache.TryGetValue((entityType, entity.EntityId, entity.ConnectedChannelId), out var entityNames);
                organizationNames.TryGetValue(entity.OrganizationId, out var organizationName);
                yield return new VmOrganizationBrokenLinkDetail
                {
                    EntityId = entity.EntityId,
                    Name = entityNames,
                    EntityType = entityType,
                    SubEntityType = subEntityType,
                    OrganizationId = entity.OrganizationId,
                    ConnectedChannelId = entity.ConnectedChannelId,
                    OrganizationName = organizationName
                };
            }
        }

        private Dictionary<Guid, Dictionary<string, string>> GetOrganizationNames(IEnumerable<Guid> organizationIds)
        {
            var mainNameType = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
            return organizationCache.GetData()
                .Where(x => organizationIds.Contains(x.Key))
                .SelectMany(x =>
                    x.Value.Organization.OrganizationNames.Select(y => new
                        {UnificRootId = x.Key, y.LocalizationId, y.Name, y.TypeId}))
                .Where(x => x.TypeId == mainNameType)
                .GroupBy(x => x.UnificRootId)
                .ToDictionary(x => x.Key,
                    x => x.ToDictionary(y => languageCache.GetByValue(y.LocalizationId), y => y.Name));
        }

        private IQueryable<VBrokenLink> GetBrokenLinksInternal(IUnitOfWork unitOfWork, bool isException,
            IEnumerable<Guid> organizationTree, params Guid[] webPageIds)
        {
            var brokenLinkRepo = unitOfWork.CreateRepository<IVBrokenLinkRepository>();
            return brokenLinkRepo.All()
                .Where(x => organizationTree.Contains(x.OrganizationId)
                            && x.IsException == isException
                            && (webPageIds == null || !webPageIds.Any() || webPageIds.Contains(x.WebPageId)))
                .Distinct();
        }

        private void SaveChanges(Dictionary<string, bool> changedPages)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var webPageRepo = unitOfWork.CreateRepository<IWebPageRepository>();
                var webPages = webPageRepo.All().Where(w => changedPages.Keys.Contains(w.Url)).ToList();

                foreach (var webPage in webPages)
                {
                    webPage.IsBroken = changedPages[webPage.Url];
                    webPage.ValidationDate = DateTime.UtcNow;
                }

                unitOfWork.Save(SaveMode.AllowAnonymous);
            });
        }

        private List<VmBrokenLink> GetWebPages()
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var webPageRepo = unitOfWork.CreateRepository<IWebPageRepository>();
                var webPages = webPageRepo.All().Where(x => !x.IsException && x.Url != null && x.Url != "").OrderBy(x => x.Id);
                return translationEntity.TranslateAll<WebPage, VmBrokenLink>(webPages).ToList();
            });
        }

        private class LinkValidatorResult
        {
            public string Url { get; set; }

            public int? Status { get; set; }

            public bool Malformed { get; set; }

            public bool InternalError { get; set; }

            [JsonIgnore] public bool IsBroken => Malformed || InternalError || Status == null || Status > 399;
        }

        private class LinkValidatorResponse
        {
            public List<LinkValidatorResult> Results { get; set; }
        }

        private class BrokenLinkNamesCache : Dictionary<(SearchEntityTypeEnum, Guid, Guid?), Dictionary<string, string>>
        {
        }
    }
}
