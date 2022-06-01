using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces.Security;
using PTV.Framework;
using PTV.Next.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Next.Organization
{
    public interface IOrganizationMapper
    {
        List<OrganizationModel> Map(List<Guid> ids);
        OrganizationModel Map(Guid id);
        UserOrganizationsAndRolesModel Map(IReadOnlyList<IUserOrganizationRoles> userOrganizationRoles);
        OrganizationModel Map(VmListItemWithStatus item);
        List<OrganizationModel> Map(List<VmListItemWithStatus> source);
    }

    [RegisterService(typeof(IOrganizationMapper), RegisterType.Transient)]
    internal class OrganizationMapper : IOrganizationMapper
    {
        private ITypesCache typesCache;
        private readonly Dictionary<Guid, VmListItemWithStatus> organizations;

        public OrganizationMapper(ITypesCache typesCache,
            IOrganizationTreeDataCache organizationCache)
        {
            this.typesCache = typesCache;
            organizations = organizationCache.GetFlatDataVm();
        }

        public List<OrganizationModel> Map(List<Guid> ids)
        {
            return ids.Select(Map).Where(x => x != null).ToList();
        }

        public OrganizationModel Map(Guid id)
        {
            if (organizations.TryGetValue(id, out var org))
            {
                return Map(org);
            }

            return null;
        }

        public List<OrganizationModel> Map(List<VmListItemWithStatus> source)
        {
            var models = new List<OrganizationModel>();

            source.ForEach(item =>
            {
                models.Add(Map(item));
            });

            return models;
        }

        public OrganizationModel Map(VmListItemWithStatus item)
        {
            var org = new OrganizationModel
            {
                Id = item.Id,
                Name = item.Name,
                AlternateName = item.AlternateName,
                Code = item.Code,
                PublishingStatus = Common.ToEnum<PublishingStatus>(typesCache.GetByValue<PublishingStatusType>(item.PublishingStatusId)),
                Texts = item.Translation.Texts,
                VersionedId = item.VersionedId
            };

            if (item.TypeId.HasValue)
            {
                var typeText = typesCache.GetByValue<OrganizationType>(item.TypeId ?? Guid.Empty);
                org.Type = typeText.Parse<OrganizationTypeEnum>();
            }

            return org;
        }

        public UserOrganizationsAndRolesModel Map(IReadOnlyList<IUserOrganizationRoles> userOrganizationRoles)
        {
            var orgIds = userOrganizationRoles.Select(x => x.OrganizationId).ToList();
            var userOrganizations = orgIds.Where(organizations.ContainsKey)
                .Select(x => Map(organizations[x]))
                .ToList();

            var roles = new List<OrganizationRoleModel>();
            foreach (var userRole in userOrganizationRoles)
            {
                roles.Add(new OrganizationRoleModel
                {
                    IsMain = userRole.IsMain,
                    OrganizationId = userRole.OrganizationId,
                    Role = userRole.Role
                });
            }

            return new UserOrganizationsAndRolesModel
            {
                OrganizationRoles = roles,
                UserOrganizations = userOrganizations
            };
        }
    }
}
