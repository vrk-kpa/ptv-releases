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
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Interfaces.Caches.Finto;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.Localization;
using PTV.Domain.Model.Models.Localization;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IMessagesService), RegisterType.Transient)]
    internal class MessagesService : IMessagesService
    {
        private IContextManager contextManager;
        private ITranslationViewModel translationToEntity;
        private ILocalizationMessagesCache cache;
        
        public MessagesService(IContextManager contextManager, ITranslationViewModel translationToEntity, ILocalizationMessagesCache cache)
        {
            this.contextManager = contextManager;
            this.translationToEntity = translationToEntity;
            this.cache = cache;
        }

        /// <summary>
        /// Gets ui localized text for application
        /// </summary>
        /// <returns></returns>
        public IVmListItemsData<IVmLanguageMessages> GetMessages()
        {
            return new VmListItemsData<IVmLanguageMessages>(cache.GetData().Values);
        }        
        



        public void SaveMessages(IEnumerable<IVmLanguageMessages> messages)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                translationToEntity.TranslateAll<IVmLanguageMessages, Localization>(messages, unitOfWork);
                unitOfWork.Save(SaveMode.AllowAnonymous);
            });
        }
    }
}