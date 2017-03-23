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
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Framework;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Caches
{
    internal class TypeCache<T, T2> : ITypeCache where T : class, ITypeNamed<T2>, IType, IEntityIdentifier where T2 : IName
    {
        private readonly Dictionary<string, VmType> cacheData;
        private readonly Dictionary<Guid, VmType> reverseCache;


        public TypeCache(ICacheBuilder cacheBuilder)
        {
            var cacheDataEntities = cacheBuilder.BuildTypeCache<T, T2>();
            cacheData = cacheDataEntities.Values.Select(i => new VmType() { Id = i.Id, Code = i.Code, OrderNumber = i.OrderNumber, Names = i.Names.Select(j => new VmTypeName() { Name = j.Name, LocalizationId = j.LocalizationId}).ToList()}).ToDictionary(i => i.Code.ToLower(), i => i);
            reverseCache = cacheData.ToDictionary(i => i.Value.Id, i => i.Value);
        }

        public Guid Get(string key)
        {
            var guid = cacheData.TryGet(key.ToLower())?.Id;
            if (guid == null)
            {
                throw new ArgumentOutOfRangeException($"Guid for type '{typeof(T).Name}' with code '{key}' not found in cache!");
            }
            return guid.Value;
        }

        public string GetByValue(Guid typeId)
        {
            var code = reverseCache.TryGet(typeId)?.Code;
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentOutOfRangeException($"Type '{typeof(T).Name}' for guid '{typeId}' not found in cache!");
            }
            return code;
        }

        public bool Compare(Guid typeId, string filterBy)
        {
            return typeId == Get(filterBy);
        }

        public ICollection<VmType> GetCompleteStuct()
        {
            return cacheData.Values;
        }
    }

    public class VmType : VmBase
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public List<VmTypeName> Names { get; set; }
        public int? OrderNumber { get; set; }
    }

    public class VmTypeName : VmBase
    {
        public string Name { get; set; }
        public Guid LocalizationId { get; set; }
    }

    [RegisterService(typeof(ITypesCache), RegisterType.Singleton)]
    internal class TypesCache : ITypesCache
    {
        private readonly Dictionary<Type, ITypeCache> cacheData = new Dictionary<Type, ITypeCache>();
        private readonly Dictionary<string, Type> nameToTypeMap = new Dictionary<string, Type>();
        private readonly ICacheBuilder cacheBuilder;

        public TypesCache(ICacheBuilder cacheBuilder)
        {
            this.cacheBuilder = cacheBuilder;
            Refresh();
        }

        public ICollection<VmType> GetCacheData(string cacheName)
        {
            var cacheType = nameToTypeMap.TryGet(cacheName);
            if (cacheType == null) return null;
            var cache = cacheData.TryGet(cacheType);
            return cache?.GetCompleteStuct();
        }

        public ICollection<VmType> GetCacheData<T>() where T : IType
        {
            var cache = cacheData.TryGet(typeof(T));
            return cache?.GetCompleteStuct();
        }

        private void AddTypeCache<T, T2>(string cacheServiceStringName = null) where T : class, ITypeNamed<T2>, IEntityIdentifier where T2 : IName
        {
            cacheData.Add(typeof(T), new TypeCache<T, T2>(cacheBuilder));
            nameToTypeMap.Add(cacheServiceStringName ?? typeof(T).Name, typeof(T));
        }

        public bool Compare<T>(Guid? typeId, string filterBy) where T : IType
        {
            if (!typeId.IsAssigned()) return false;
            return GetTypeDataCache<T>().Compare(typeId.Value, filterBy);
        }

        private ITypeCache GetTypeDataCache<T>() where T : IType
        {
            var typeData = cacheData.TryGet(typeof(T));
            if (typeData == null)
            {
                throw new Exception($"Requested type {typeof(T).Name} is not added to cache.");
            }
            return typeData;
        }

        public Guid Get<T>(string key) where T : IType
        {
            return Get<T>(key, null);
        }

        public Guid Get<T>(string key, string defaultKey) where T : IType
        {
            Guid guid = GetTypeDataCache<T>().Get(key);
            if (!string.IsNullOrEmpty(defaultKey) && !guid.IsAssigned())
            {
                return GetTypeDataCache<T>().Get(defaultKey);
            }
            return guid;
        }

        public string GetByValue<T>(Guid id) where T : IType
        {
            return GetTypeDataCache<T>().GetByValue(id);
        }

        IType IEntityCache<string, IType>.Get(string key)
        {
            throw new NotSupportedException("Use ITypesCache.Get<T> method instead of this");
        }

        string IEntityCache<string, IType>.GetByValue(IType value)
        {
            throw new NotSupportedException();
        }

        public void Refresh()
        {
            cacheData.Clear();
            nameToTypeMap.Clear();
            AddTypeCache<PublishingStatusType, PublishingStatusTypeName>("PublishingStatuses");
            AddTypeCache<NameType, NameTypeName>();
            AddTypeCache<DescriptionType, DescriptionTypeName>();
            AddTypeCache<ServiceCoverageType, ServiceCoverageTypeName>("ServiceCoverageTypes");
            AddTypeCache<ProvisionType, ProvisionTypeName>();
            AddTypeCache<ServiceType, ServiceTypeName>("ServiceTypes");
            AddTypeCache<ServiceChannelType, ServiceChannelTypeName>("ChannelTypes");
            AddTypeCache<ServiceChargeType, ServiceChargeTypeName>("ChargeTypes");
            AddTypeCache<PhoneNumberType, PhoneNumberTypeName>("PhoneNumberTypes");
            AddTypeCache<ServiceHourType, ServiceHourTypeName>();
            AddTypeCache<WebPageType, WebPageTypeName>("WebPageTypes");
            AddTypeCache<AddressType, AddressTypeName>();
            AddTypeCache<RoleType, RoleTypeName>();
            AddTypeCache<AttachmentType, AttachmentTypeName>();
            AddTypeCache<PrintableFormChannelUrlType, PrintableFormChannelUrlTypeName>();
            AddTypeCache<OrganizationType, OrganizationTypeName>();
            AddTypeCache<CoordinateType, CoordinateTypeName>();
            AddTypeCache<Language, LanguageName>("Languages");
        }
    }
}
