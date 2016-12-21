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
using System.Collections.Generic;
using System.Linq;
using Moq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Framework;

namespace PTV.Database.DataAccess.Tests.Translators
{
    public abstract class TestBase
    {
        internal TestRegisterServiceInfo CreateRegisterServiceInfo<T>(T instance)
        {
            return new TestRegisterServiceInfo { Instance = instance, RegisterAs = typeof(T)};
        }

        internal void RegisterDbSet<T,T2>(List<T> entityData, Mock<T2> unitOfWork) where T : class where T2 : class, IUnitOfWork
        {
            var dbset = new TestDbSet<T>(entityData);
            unitOfWork.Setup(uw => uw.GetSet<T>()).Returns(dbset);
        }

        internal List<T> CreateCodeData<T>(Type enumType) where T : ICode, new()
        {
            return Enum.GetNames(enumType).Select(type => new T { Code = type }).ToList();
        }

        internal Dictionary<Guid, string> CreateCodeGuidList(Type enumType)
        {
            return Enum.GetNames(enumType).ToDictionary(type => type.ToString().GetGuid(), type => type.ToString());
        }

        internal Mock<ILanguageCache> SetupLanguageCacheMock(Mock<ILanguageCache> languageCacheMock = null)
        {
            languageCacheMock = languageCacheMock ?? new Mock<ILanguageCache>();
            var languages = CreateCodeGuidList(typeof (LanguageCode));
            languageCacheMock.Setup(i => i.Filter(It.IsAny<Guid>(), It.IsAny<LanguageCode>())).Returns(true);
            languageCacheMock.Setup(i => i.Filter(It.IsAny<List<ILocalizable>>(), It.IsAny<LanguageCode>())).Returns((List<ILocalizable> i) => i.FirstOrDefault());
            languageCacheMock.Setup(i => i.Get(It.IsAny<string>())).Returns((string id) => id.GetGuid());
            languageCacheMock.Setup(i => i.GetByValue(It.IsAny<Guid>())).Returns((Guid id) => languages.TryGet(id));
            return languageCacheMock;
        }
        internal Mock<ITypesCache> SetupTypesCacheMock<T>(Mock<ITypesCache> typesCacheMock = null, Type typesEnum = null) where T : TypeBase
        {
            typesCacheMock = typesCacheMock ?? new Mock<ITypesCache>();

            var types = new Dictionary<Guid, string>();
            if (typesEnum != null)
            {
                types = CreateCodeGuidList(typesEnum);
            }
            typesCacheMock.Setup(i => i.Get<T>(It.IsAny<string>())).Returns((string id) => id.GetGuid());
            typesCacheMock.Setup(i => i.GetByValue<T>(It.IsAny<Guid>())).Returns((Guid id) => (typesEnum != null) ? types.TryGet(id) : id.ToString());
            typesCacheMock.Setup(i => i.Compare<T>(It.IsAny<Guid>(), It.IsAny<string>())).Returns((Guid id, string code) => id == code.GetGuid());
            return typesCacheMock;
        }

        internal Mock<ICacheManager> SetupCacheManager(ILanguageCache languageCache = null, ITypesCache typesCache = null, Mock<ICacheManager> cacheManagerMock = null)
        {
            languageCache = languageCache ?? SetupLanguageCacheMock().Object;
            cacheManagerMock = new Mock<ICacheManager>();
            cacheManagerMock.Setup(i => i.LanguageCache).Returns(languageCache);
            cacheManagerMock.Setup(i => i.TypesCache).Returns(typesCache);
            return cacheManagerMock;
        }
    }
}
