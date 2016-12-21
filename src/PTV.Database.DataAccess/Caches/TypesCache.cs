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
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Framework;

namespace PTV.Database.DataAccess.Caches
{
    internal class TypeCache<T> : ITypeCache where T : TypeBase
    {
        private readonly Dictionary<string, Guid> cacheData;

        public TypeCache(ICacheBuilder cacheBuilder)
        {
            cacheData = cacheBuilder.BuildCache<T, string, Guid>(i => i.Code, i => i.Id);
        }


        public Guid Get(string key)
        {
            return cacheData.TryGet(key);
        }

        public string GetByValue(Guid typeId)
        {
            return cacheData.FirstOrDefault(x => x.Value == typeId).Key;
        }

        public bool Compare(Guid? typeId, string filterBy)
        {
            return typeId == Get(filterBy);
        }
    }

    [RegisterService(typeof(ITypesCache), RegisterType.Singleton)]
    internal class TypesCache : ITypesCache
    {
        private readonly Dictionary<Type, ITypeCache> cacheData = new Dictionary<Type, ITypeCache>();

        public TypesCache(ICacheBuilder cacheBuilder)
        {
            AddTypeCache<PublishingStatusType>(cacheBuilder);
            AddTypeCache<NameType>(cacheBuilder);
            AddTypeCache<DescriptionType>(cacheBuilder);
            AddTypeCache<ServiceCoverageType>(cacheBuilder);
            AddTypeCache<ProvisionType>(cacheBuilder);
            AddTypeCache<ServiceType>(cacheBuilder);
            AddTypeCache<ServiceChannelType>(cacheBuilder);
            AddTypeCache<ServiceChargeType>(cacheBuilder);
            AddTypeCache<PhoneNumberType>(cacheBuilder);
            AddTypeCache<ServiceHourType>(cacheBuilder);
            AddTypeCache<WebPageType>(cacheBuilder);
            AddTypeCache<AddressType>(cacheBuilder);
            AddTypeCache<RoleType>(cacheBuilder);
            AddTypeCache<AttachmentType>(cacheBuilder);
            AddTypeCache<PrintableFormChannelUrlType>(cacheBuilder);
            AddTypeCache<OrganizationType>(cacheBuilder);
        }

        private void AddTypeCache<T>(ICacheBuilder cacheBuilder) where T : TypeBase
        {
            cacheData.Add(typeof(T), new TypeCache<T>(cacheBuilder));
        }

//        private void AddTypeCache(Type type, ICacheBuilder cacheBuilder)
//        {
//            cacheBuilder
//            cacheData.Add(type, cacheBuilder.BuildCache<T, string, Guid>(i => i.Code, i => i.Id));
//        }

        public bool Compare<T>(Guid? typeId, string filterBy) where T : TypeBase
        {
            return GetTypeDataCache<T>().Compare(typeId, filterBy);
        }

        private ITypeCache GetTypeDataCache<T>() where T : TypeBase
        {
            var typeData = cacheData.TryGet(typeof(T));
            if (typeData == null)
            {
                throw new Exception($"Requested type {typeof(T).Name} is not added to cache.");
            }
            return typeData;
        }

        public Guid Get<T>(string key) where T : TypeBase
        {
            return Get<T>(key, null);
        }

        public Guid Get<T>(string key, string defaultKey) where T : TypeBase
        {
            Guid guid = GetTypeDataCache<T>().Get(key);
            if (!string.IsNullOrEmpty(defaultKey) && !guid.IsAssigned())
            {
                return GetTypeDataCache<T>().Get(defaultKey);
            }
            return guid;
        }

        public string GetByValue<T>(Guid id) where T : TypeBase
        {
            return GetTypeDataCache<T>().GetByValue(id);
        }

        Guid IEntityCache<string, Guid>.Get(string key)
        {
            throw new NotSupportedException("Use ITypesCache.Get<T> method instead of this");
        }

        string IEntityCache<string, Guid>.GetByValue(Guid value)
        {
            throw new NotSupportedException();
        }
    }
}
