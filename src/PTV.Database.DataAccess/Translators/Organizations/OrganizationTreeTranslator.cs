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
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.ServiceDataHolders;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Organizations
{
    [RegisterService(typeof(ITranslator<OrganizationVersioned, VmTreeItem>), RegisterType.Transient)]
    internal class OrganizationTreeTranslator : Translator<OrganizationVersioned, VmTreeItem>
    {
        private ILanguageCache languageCache;

        public OrganizationTreeTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ILanguageCache languageCache) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = languageCache;
        }

        public override VmTreeItem TranslateEntityToVm(OrganizationVersioned entity)
        {
            var model = CreateEntityViewModelDefinition<VmTreeItem>(entity)
                .AddPartial(i => i, o => o as VmListItem)
                //                .AddLocalizable(i => i.OrganizationNames.Where(name => name.TypeId == i.DisplayNameTypeId).Cast<IText>(), o => o.Name)
                //.AddSimple(i => i.UnificRootId, o => o.Id)
                .AddSimple(i => i.UnificRoot.Children.Count == 0, o => o.IsLeaf)
                .GetFinal();
//            if (string.IsNullOrEmpty(model.Name))
//            {
//                model.Name =
//                    entity.OrganizationNames.Where(name => name.TypeId == entity.DisplayNameTypeId)
//                        .OrderBy(x => x.Localization.Order)
//                        .FirstOrDefault()?.Name;
//            }
            return model;
        }

        public override OrganizationVersioned TranslateVmToEntity(VmTreeItem vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<OrganizationVersioned, VmListItem>), RegisterType.Transient)]
    internal class OrganizationSimpleListTranslator : Translator<OrganizationVersioned, VmListItem>
    {
        private readonly ILanguageOrderCache languageOrderCache;
        private ITypesCache typesCache;

        public OrganizationSimpleListTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.languageOrderCache = cacheManager.LanguageOrderCache;
            this.typesCache = cacheManager.TypesCache;

        }

        public override VmListItem TranslateEntityToVm(OrganizationVersioned entity)
        {
            var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());

            var model = CreateEntityViewModelDefinition(entity)
                .AddLocalizable(i => i.OrganizationNames.Where(name => name.TypeId == i.OrganizationDisplayNameTypes.FirstOrDefault(type => type.LocalizationId == name.LocalizationId)?.DisplayNameTypeId).Cast<IText>(), o => o.Name)
                .AddSimple(i => i.UnificRootId, o => o.Id)
                .AddNavigation(i => new OrganizationVersioned
                    {
                        Id = i.UnificRootId,
                        OrganizationNames = i.OrganizationNames
                            .Where(name => entity.LanguageAvailabilities.Any(x => x.LanguageId == name.LocalizationId && x.StatusId == psPublished)
                                           && name.TypeId == i.OrganizationDisplayNameTypes.FirstOrDefault(type => type.LocalizationId == name.LocalizationId)?.DisplayNameTypeId).ToList()
                    } as INameReferences,
                    o => o.Translation)
                .GetFinal();
            if (string.IsNullOrEmpty(model.Name))
            {
                model.Name =
                    entity.OrganizationNames.Where(name => name.TypeId == entity.OrganizationDisplayNameTypes.FirstOrDefault(type => type.LocalizationId == name.LocalizationId)?.DisplayNameTypeId)
                        .OrderBy(x => languageOrderCache.Get(x.LocalizationId))
                        .FirstOrDefault()?.Name;
            }
            return model;
        }

        public override OrganizationVersioned TranslateVmToEntity(VmListItem vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<OrganizationTreeItem, VmTreeItem>), RegisterType.Transient)]
    internal class OrganizationTreeItemTranslator : Translator<OrganizationTreeItem, VmTreeItem>
    {
        private readonly ILanguageOrderCache languageOrderCache;

        public OrganizationTreeItemTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ILanguageOrderCache languageOrderCache) : base(resolveManager, translationPrimitives)
        {
            this.languageOrderCache = languageOrderCache;
        }

        public override VmTreeItem TranslateEntityToVm(OrganizationTreeItem entity)
        {
            var model = CreateEntityViewModelDefinition<VmTreeItem>(entity)
//                .AddLocalizable(i => i.Organization.OrganizationNames.Where(name => name.TypeId == i.Organization.DisplayNameTypeId).Cast<IText>(), o => o.Name)
                .AddPartial(i => i.Organization, o => o as VmListItem)
                .AddSimple(i => i.Organization.UnificRootId, o => o.Id)
                .AddSimple(i => i.Children == null || i.Children.Count == 0, o => o.IsLeaf)
                .GetFinal();
            if (string.IsNullOrEmpty(model.Name))
            {
                model.Name =
                    entity.Organization.OrganizationNames.Where(name => name.TypeId == entity.Organization.OrganizationDisplayNameTypes.FirstOrDefault(type => type.LocalizationId == name.LocalizationId)?.DisplayNameTypeId)
                        .OrderBy(x => languageOrderCache.Get(x.LocalizationId))
                        .FirstOrDefault()?.Name;
            }
            return model;
        }

        public override OrganizationTreeItem TranslateVmToEntity(VmTreeItem vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<OrganizationVersioned, VmExpandedVmTreeItem>), RegisterType.Transient)]
    internal class OrganizationTreeExpandedTranslator : Translator<OrganizationVersioned, VmExpandedVmTreeItem>
    {
        private ILanguageCache languageCache;

        public OrganizationTreeExpandedTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ILanguageCache languageCache) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = languageCache;
        }

        public override VmExpandedVmTreeItem TranslateEntityToVm(OrganizationVersioned entity)
        {
            if (entity != null && entity.UnificRoot == null)
            {
                throw new Exception("OrganizationVersioned.UnificRoot is NULL in OrganizationTreeExpandedTranslator");
            }
            var model = CreateEntityViewModelDefinition<VmExpandedVmTreeItem>(entity)
                .AddPartial(i => i, o => o as VmListItem)
                .AddSimple(i => i.UnificRoot.Children.Any(), o => o.AreChildrenLoaded)
                .AddCollection(i => i.UnificRoot.Children, o => o.Children)
                .AddSimple(i => i.UnificRoot.Children.Count == 0, o => o.IsLeaf)
                .GetFinal();
            return model;
        }

        public override OrganizationVersioned TranslateVmToEntity(VmExpandedVmTreeItem vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<OrganizationTreeItem, VmExpandedVmTreeItem>), RegisterType.Transient)]
    internal class OrganizationTreeItemExpandedTranslator : Translator<OrganizationTreeItem, VmExpandedVmTreeItem>
    {
        private ILanguageCache languageCache;

        public OrganizationTreeItemExpandedTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ILanguageCache languageCache) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = languageCache;
        }

        public override VmExpandedVmTreeItem TranslateEntityToVm(OrganizationTreeItem entity)
        {
            var model = CreateEntityViewModelDefinition<VmExpandedVmTreeItem>(entity)
                .AddLocalizable(i => i.Organization.OrganizationNames.Where(name => name.TypeId == i.Organization.OrganizationDisplayNameTypes.FirstOrDefault(type => type.LocalizationId == name.LocalizationId)?.DisplayNameTypeId).Cast<IText>(), o => o.Name)
                .AddSimple(i => i.Organization.UnificRootId, o => o.Id)
                .AddSimple(i => i.Children != null && i.Children.Any(), o => o.AreChildrenLoaded)
                .AddCollection(i => i.Children, o => o.Children)
                .AddSimple(i => i.Children == null || i.Children.Count == 0, o => o.IsLeaf)
                .GetFinal();

            if (string.IsNullOrEmpty(model.Name))
            {
                model.Name =
                    entity.Organization.OrganizationNames.Where(name => name.TypeId == entity.Organization.OrganizationDisplayNameTypes.FirstOrDefault(type => type.LocalizationId == name.LocalizationId)?.DisplayNameTypeId)
                        .OrderBy(x => x.Localization.OrderNumber)
                        .FirstOrDefault()?.Name;
            }
            return model;
        }

        public override OrganizationTreeItem TranslateVmToEntity(VmExpandedVmTreeItem vModel)
        {
            throw new NotImplementedException();
        }
    }
}
