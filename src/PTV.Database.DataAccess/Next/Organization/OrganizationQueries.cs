using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Next;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.Model.Models;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Next.Organization
{
    [RegisterService(typeof(IOrganizationQueries), RegisterType.Transient)]
    internal class OrganizationQueries : IOrganizationQueries
    {
        private readonly IOrganizationTreeDataCache organizationCache;
        private readonly IUserOrganizationService userOrganizationService;
        private readonly IPahaTokenAccessor pahaTokenAccessor;
        private ITypesCache typesCache;
        private readonly IOrganizationMapper mapper;

        public OrganizationQueries(IOrganizationTreeDataCache organizationCache, 
            ITypesCache typesCache,
            IUserOrganizationService userOrganizationService,
            IPahaTokenAccessor pahaTokenAccessor,
            IOrganizationMapper mapper)
        {
            this.organizationCache = organizationCache;
            this.userOrganizationService = userOrganizationService;
            this.pahaTokenAccessor = pahaTokenAccessor;
            this.typesCache = typesCache;
            this.mapper = mapper;
        }
        
        public List<OrganizationModel> Get(List<Guid> ids)
        {
            return this.mapper.Map(ids);
        }

        public UserOrganizationsAndRolesModel GetAllUserOrganizationsAndRoles()
        {
            var userOrganizationRoles = userOrganizationService.GetAllUserOrganizationRoles();
            return mapper.Map(userOrganizationRoles);
        }

        public List<OrganizationModel> Search(OrganizationSearchModel parameters)
        {
            var allOrganizations = organizationCache.GetFlatDataVm();
            var organizations = new List<VmListItemWithStatus>();

            if (parameters.SearchAll || pahaTokenAccessor.UserRole == UserRoleEnum.Eeva)
            {
                organizations = allOrganizations.Values.ToList();
            }
            else
            {
                // User wants to search only her own organizations
                var ids = userOrganizationService.GetAllUserOrganizationIds();
                organizations = ids.Where(allOrganizations.ContainsKey).Select(x => allOrganizations[x]).ToList();
            }

            var allowedStatuses = parameters.SearchOnlyDraftAndPublished
                ? new List<PublishingStatus> {PublishingStatus.Draft, PublishingStatus.Published, PublishingStatus.Modified}
                : new List<PublishingStatus>();

            var searchText = parameters.SearchValue.ToLower();
            var result = organizations.Where(x => IsMatch(x, searchText, allowedStatuses)).ToList();
            return mapper.Map(result);
        }

        private bool IsMatch(VmListItemWithStatus item, string searchText, List<PublishingStatus> allowedStatuses)
        {
            var status = Common.ToEnum<PublishingStatus>(typesCache.GetByValue<PublishingStatusType>(item.PublishingStatusId));
            if (allowedStatuses.Any() && !allowedStatuses.Contains(status))
            {
                return false;
            }
            
            return item.Name.ToLower().Contains(searchText) || 
                item.Translation.Texts.Any(x => !string.IsNullOrEmpty(x.Value) && x.Value.ToLower().Contains(searchText));
        }
    }
}
