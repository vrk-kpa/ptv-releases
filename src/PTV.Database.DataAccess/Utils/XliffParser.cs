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
using System.Xml.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Utils
{
    [RegisterService(typeof(XliffParser), RegisterType.Singleton)]
    internal class XliffParser
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;
        private readonly ITextManager textManager;

        private int IdCounter;

        public XliffParser(ICacheManager cacheManager, ITextManager textManager)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
            this.textManager = textManager;
        }

        public string GenerateServiceXliff(ServiceVersioned sv, Guid sourceLocalizationId, Guid? targetLocalizationId)
        {
            IdCounter = 0;
            var sourceLanguageCode = languageCache.GetByValue(sourceLocalizationId);
            var targetLanguageCode = targetLocalizationId == null ? null : languageCache.GetByValue(targetLocalizationId.Value);

            var body = new XElement(Namespace.Xliff + XliffElement.Body);
            body.Add(null);
            body.Add(CreateServiceName(sv, sourceLocalizationId, targetLocalizationId));
            body.Add(CreateServiceDescription(sv, sourceLocalizationId, targetLocalizationId));
            body.Add(CreateServiceKeyword(sv, sourceLocalizationId, targetLocalizationId));
            body.Add(CreateServiceLaw(sv, sourceLocalizationId, targetLocalizationId));
            body.Add(CreateServiceProducer(sv, sourceLocalizationId, targetLocalizationId));
            body.Add(CreateServiceVoucher(sv, sourceLocalizationId, targetLocalizationId));
            body.Add(CreateServiceRequirement(sv, sourceLocalizationId, targetLocalizationId));

            var file = CreateFileEmelent(XliffService.FileOriginal, XliffService.FileDataType, sourceLanguageCode, targetLanguageCode);
            var header = CreateHeader(sv, sourceLocalizationId, targetLocalizationId);
            file.Add(header);
            file.Add(body);

            var root = CreateRootElement();
            root.Add(file);

            var doc = CreateDocument(root);
            //using (var writer = File.CreateText($"d:\\xliff\\service-{sv.Id}-{DateTime.Now:yyyyMMdd-hhmmss}.xliff")) { doc.Save(writer); }
            var result = doc.ToString();

            return result;
        }

        private static XDocument CreateDocument(XElement root)
        {
            return new XDocument(new XDeclaration(XmlElement.Verion, XmlElement.Utf, XmlElement.Standalone), root);
        }

        private static XElement CreateFileEmelent(string original, string dataType, string sourceLanguage, string targetLanguage = null)
        {
            var file = new XElement(Namespace.Xliff + XliffElement.File,
                new XAttribute(XliffAttribute.FileOriginal, original),
                new XAttribute(XliffAttribute.FileDataType, dataType),
                new XAttribute(XliffAttribute.FileSourceLanguage, sourceLanguage));
            if (targetLanguage != null) file.Add(new XAttribute(XliffAttribute.FileTargetLanguage, targetLanguage));
            return file;
        }

        private static XElement CreateHeader(ServiceVersioned sv, Guid sourceLocalizationId, Guid? targetLocalizationId)
        {
            return new XElement(Namespace.Xliff + XliffElement.Header,
                CreatePtvPropertiesElement(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(XliffAttribute.HeaderPropertiesId, sv.Id.ToString()),
                    new KeyValuePair<string, string>(XliffAttribute.HeaderPropertiesUnificRootId, sv.UnificRootId.ToString()),
                    new KeyValuePair<string, string>(XliffAttribute.HeaderPropertiesSourceLocalizationId, sourceLocalizationId.ToString()),
                    new KeyValuePair<string, string>(XliffAttribute.HeaderPropertiesTargetLocalizationId, targetLocalizationId?.ToString())
                }));
        }

        private static XElement CreatePtvPropertiesElement(IReadOnlyCollection<KeyValuePair<string, string>> properties)
        {
            if (properties == null) return null;
            var element = new XElement(Namespace.Ptv + XliffElement.PtvProperties);

            foreach (var prop in properties)
            {
                if (prop.Value != null) element.Add(new XAttribute(prop.Key, prop.Value));
            }
            return element;
        }

        private static XElement CreateRootElement()
        {
            return new XElement(Namespace.Xliff + XliffElement.Root,
                new XAttribute(XliffElement.XliffNamespace, Namespace.Xliff),
                new XAttribute(XNamespace.Xmlns + XliffElement.PtvNamespace, Namespace.Ptv),
                new XAttribute(XliffElement.Version, XliffElement.VersionValue));
        }

        #region service versioned

        private XElement CreateServiceName(ServiceVersioned sv, Guid sourceLocalizationId, Guid? targetLocalizationId = null)
        {
            var group = CreateServiceGroup(XliffService.ServiceNameGroupId, XliffService.ServiceNameGroupDataType);

            var name = CreateServiceName(sv, NameTypeEnum.Name, sourceLocalizationId, targetLocalizationId);
            if (name != null) group.Add(name);

            var alternateName = CreateServiceName(sv, NameTypeEnum.AlternateName, sourceLocalizationId, targetLocalizationId);
            if (name != null) group.Add(alternateName);

            return group;
        }

        private XElement CreateServiceDescription(ServiceVersioned sv, Guid sourceLocalizationId, Guid? targetLocalizationId = null)
        {
            var group = CreateServiceGroup(XliffService.ServiceDescriptionGroupId, XliffService.ServiceDescriptionGroupDataType);
            foreach (var descriptionType in Enum.GetValues(typeof(DescriptionTypeEnum)).Cast<DescriptionTypeEnum>())
            {
                var description = CreateServiceDescription(sv, descriptionType, sourceLocalizationId, targetLocalizationId);
                if (description != null) group.Add(description);
            }
            return group;
        }

        private XElement CreateServiceKeyword(ServiceVersioned sv, Guid sourceLocalizationId, Guid? targetLocalizationId = null)
        {
            var group = CreateServiceGroup(XliffService.ServiceKeywordGroupId, XliffService.ServiceKeywordGroupDataType);
            var sourceKeywords = sv.ServiceKeywords.Where(k => k.Keyword.LocalizationId == sourceLocalizationId).Select(k => k.Keyword.Name);
            var targetKeywords = targetLocalizationId.HasValue
                ? sv.ServiceKeywords.Where(k => k.Keyword.LocalizationId == targetLocalizationId.Value).Select(k => k.Keyword.Name).ToList()
                : null;

            group.Add(CreateTransUnitElement(null, string.Join("; ", sourceKeywords), targetKeywords.IsNullOrEmpty() ? null : string.Join("; ", targetKeywords), null));
            return group;
        }

        private XElement CreateServiceLaw(ServiceVersioned sv, Guid sourceLocalizationId, Guid? targetLocalizationId = null)
        {
            var group = CreateServiceGroup(XliffService.ServiceLawGroupId, XliffService.ServiceLawGroupDataType);
            var lawCounter = 1;
            foreach (var law in sv.ServiceLaws)
            {
                var lawName = law.Law.Names.SingleOrDefault(n => n.LocalizationId == sourceLocalizationId);
                if (lawName == null) continue;

                var groupId = $"Law {lawCounter++}: {lawName.Name}";
                var groupLaw = CreateServiceGroup(groupId, null);
                groupLaw.Add(CreatePtvPropertiesElement(new List<KeyValuePair<string, string>> {new KeyValuePair<string, string>(XliffAttribute.PropertyLawId, law.LawId.ToString())}));

                // name
                var lawNameTarget = targetLocalizationId.HasValue
                    ? law.Law.Names.SingleOrDefault(n => n.LocalizationId == targetLocalizationId.Value)
                    : null;
                groupLaw.Add(CreateTransUnitElement(null, lawName.Name, lawNameTarget?.Name, XliffService.ServiceLawName));

                // web page
                var lawWabPage = law.Law.WebPages.SingleOrDefault(w => w.WebPage.LocalizationId == sourceLocalizationId);
                if (lawWabPage != null)
                {
                    var lawWabPageTarget = targetLocalizationId.HasValue
                        ? law.Law.WebPages.SingleOrDefault(n => n.WebPage.LocalizationId == targetLocalizationId.Value)
                        : null;

                    groupLaw.Add(CreateTransUnitElement(null, lawWabPage.WebPage.Url, lawWabPageTarget?.WebPage.Url, XliffService.ServiceLawWebPage));
                }

                group.Add(groupLaw);
            }

            return group;
        }

        private XElement CreateServiceProducer(ServiceVersioned sv, Guid sourceLocalizationId, Guid? targetLocalizationId = null)
        {
            var group = CreateServiceGroup(XliffService.ServiceProducerGroupId, XliffService.ServiceProducerGroupDataType);
            foreach (var sp in sv.ServiceProducers)
            {
                var spai = sp.AdditionalInformations.SingleOrDefault(ai => ai.LocalizationId == sourceLocalizationId);
                if (spai == null) continue;

                var targetSpai = targetLocalizationId.HasValue
                    ? sp.AdditionalInformations.SingleOrDefault(ai => ai.LocalizationId == targetLocalizationId.Value)
                    : null;

                var properties = new List<KeyValuePair<string, string>>{new KeyValuePair<string, string>(XliffService.ServiceProducerId, sp.Id.ToString())};
                group.Add(CreateTransUnitElement(properties, spai.Text, targetSpai?.Text, XliffService.ServiceProducerAdditionalInfo));
            }

            return group;
        }

        private XElement CreateServiceVoucher(ServiceVersioned sv, Guid sourceLocalizationId, Guid? targetLocalizationId = null)
        {
            var group = CreateServiceGroup(XliffService.ServiceVoucherGroupId, XliffService.ServiceVoucherGroupDataType);
            var webPages = sv.ServiceWebPages.Where(w => w.WebPage != null);

            var webPagesForTranslation = webPages.Where(w => w.WebPage.LocalizationId == sourceLocalizationId).OrderBy(w => w.WebPage.OrderNumber);
            foreach (var voucher in webPagesForTranslation)
            {
                var targetVoucher = targetLocalizationId.HasValue && voucher.WebPage.OrderNumber.HasValue
                    ? sv.ServiceWebPages.FirstOrDefault(w => w.WebPage.LocalizationId == targetLocalizationId.Value && w.WebPage.OrderNumber == voucher.WebPage.OrderNumber)
                    : null;

                var groupVoucher = CreateServiceGroup(voucher.WebPageId.ToString(), null);
                var properties = new List<KeyValuePair<string, string>>{ new KeyValuePair<string, string>(XliffService.ServiceVoucherOrderNumber, voucher.WebPage.OrderNumber?.ToString())};
                if (targetVoucher != null) properties.Add(new KeyValuePair<string, string>(XliffService.ServiceVoucherTargetWebId, targetVoucher.WebPage.Id.ToString()));
                groupVoucher.Add(CreatePtvPropertiesElement(properties));
                
                if (!voucher.WebPage.Name.IsNullOrWhitespace()) groupVoucher.Add(CreateTransUnitElement(null, voucher.WebPage.Name, targetVoucher?.WebPage.Name, XliffService.ServiceVoucherName));
                if (!voucher.WebPage.Url.IsNullOrWhitespace()) groupVoucher.Add(CreateTransUnitElement(null, voucher.WebPage.Url, targetVoucher?.WebPage.Url, XliffService.ServiceVoucherWebPage));
                if (!voucher.WebPage.Description.IsNullOrWhitespace()) groupVoucher.Add(CreateTransUnitElement(null, voucher.WebPage.Description, targetVoucher?.WebPage.Description, XliffService.ServiceVoucherAdditionalInformation));
                group.Add(groupVoucher);
            }

            return group;
        }

        private XElement CreateServiceRequirement(ServiceVersioned sv, Guid sourceLocalizationId, Guid? targetLocalizationId = null)
        {
            var group = CreateServiceGroup(XliffService.ServiceRequiredGroupId, XliffService.ServiceRequiredGroupDataType);
            var cas = sv.ServiceRequirements.SingleOrDefault(r => r.LocalizationId == sourceLocalizationId);
            if (cas == null) return null;

            var targetCas = targetLocalizationId.HasValue
                ? sv.ServiceRequirements.SingleOrDefault(r => r.LocalizationId == targetLocalizationId.Value)
                : null;

            var properties = new List<KeyValuePair<string, string>> {new KeyValuePair<string, string>(XliffService.ServiceRequiredSourceId, cas.Id.ToString())};
            if (targetCas != null) properties.Add(new KeyValuePair<string, string>(XliffService.ServiceRequiredTargetId, targetCas.Id.ToString()));
            group.Add(CreateTransUnitElement(
                properties,
                textManager.ConvertToMarkDown(cas.Requirement),
                targetCas != null ? textManager.ConvertToMarkDown(targetCas.Requirement) : null,
                XliffService.ServiceRequiredGroupId));

            return group;
        }

        #region service name

            private XElement CreateServiceName(ServiceVersioned sv, NameTypeEnum nameType, Guid sourceLocalizationId, Guid? targetLocalizationId = null)
        {
            var sourceName = sv.ServiceNames.SingleOrDefault(n => n.TypeId == typesCache.Get<NameType>(nameType.ToString()) && n.LocalizationId == sourceLocalizationId);
            if (sourceName == null) return null;

            var targetName = targetLocalizationId.HasValue
                ? sv.ServiceNames.SingleOrDefault(n => n.TypeId == typesCache.Get<NameType>(nameType.ToString()) && n.LocalizationId == targetLocalizationId)
                : null;

            return CreateServiceName(sourceName.Name, targetName?.Name, nameType);
        }

        private XElement CreateServiceName(string source, string target, NameTypeEnum nameType)
        {
            var properties = new List<KeyValuePair<string, string>> {new KeyValuePair<string, string>(XliffAttribute.PropertyTypeId, typesCache.Get<NameType>(nameType.ToString()).ToString())};
            return CreateTransUnitElement(properties, source, target, nameType.ToString());
        }

        #endregion

        #region service description

        private XElement CreateServiceDescription(ServiceVersioned sv, DescriptionTypeEnum descriptionType, Guid sourceLocalizationId, Guid? targetLocalizationId = null)
        {
            var sourceDescription = sv.ServiceDescriptions.SingleOrDefault(n => n.TypeId == typesCache.Get<DescriptionType>(descriptionType.ToString()) && n.LocalizationId == sourceLocalizationId);
            if (sourceDescription == null) return null;

            var targetDescription = (targetLocalizationId.HasValue)
                ? sv.ServiceDescriptions.SingleOrDefault(n => n.TypeId == typesCache.Get<DescriptionType>(descriptionType.ToString()) && n.LocalizationId == targetLocalizationId)
                : null;

            return CreateServiceDescription(sourceDescription.Description, targetDescription?.Description, descriptionType);
        }

        private XElement CreateServiceDescription(string source, string target, DescriptionTypeEnum descriptionType)
        {
            var properties = new List<KeyValuePair<string, string>> {new KeyValuePair<string, string>(XliffAttribute.PropertyTypeId, typesCache.Get<DescriptionType>(descriptionType.ToString()).ToString())};

            source = ParseDescription(source, descriptionType);
            target = ParseDescription(target, descriptionType);
            return CreateTransUnitElement(properties, source, target, descriptionType.ToString());
        }

        private string ParseDescription(string description, DescriptionTypeEnum descriptionType)
        {
            switch (descriptionType)
            {
                case DescriptionTypeEnum.Description:
                case DescriptionTypeEnum.ServiceUserInstruction:
                    return textManager.ConvertToMarkDown(description);
                default:
                    return description;
            }
        }

        #endregion // service description

        #endregion // service versioned

        private XElement CreateTransUnitElement(List<KeyValuePair<string, string>> properties, string source, string target, string note)
        {
            var element = new XElement(Namespace.Xliff + XliffElement.TransUnit, new XAttribute(XliffAttribute.TransUnitId, ++IdCounter));
            element.Add(CreatePtvPropertiesElement(properties));
            element.Add(new XElement(Namespace.Xliff + XliffElement.TransUnitSource, source));
            if (target != null) element.Add(new XElement(Namespace.Xliff + XliffElement.TransUnitTarget, target));
            if (note != null) element.Add(new XElement(Namespace.Xliff + XliffElement.TransUnitNote, note));
            return element;
        }

        private static XElement CreateServiceGroup(string groupId, string groupDataType)
        {
            var group = new XElement(Namespace.Xliff + XliffElement.Group);
            if (groupId != null) group.Add(new XAttribute(XliffAttribute.GroupId, groupId));
            if (groupDataType != null) group.Add(new XAttribute(XliffAttribute.GroupDataType, groupDataType));
            return group;
        }
    }

    internal static class Namespace
    {
        internal static readonly XNamespace Xliff = "urn:oasis:names:tc:xliff:document:1.2";
        internal static readonly XNamespace Ptv = "ptv";
    }

    internal static class XliffElement
    {
        internal const string Body = "body";
        internal const string File = "file";
        internal const string Root = "xliff";
        internal const string Version = "version";
        internal const string VersionValue = "1.2";
        internal const string XliffNamespace = "xmlns";
        internal const string PtvNamespace = "ptv";
        internal const string Header = "header";
        internal const string PtvProperties = "properties";
        internal const string Group = "group";

        internal const string TransUnit = "trans-unit";
        internal const string TransUnitSource = "source";
        internal const string TransUnitTarget = "target";
        internal const string TransUnitNote = "note";
    }

    internal static class XliffAttribute
    {
        internal const string FileOriginal = "original";
        internal const string FileDataType = "datatype";
        internal const string FileSourceLanguage = "source-language";
        internal const string FileTargetLanguage = "target-language";

        internal const string HeaderPropertiesId = "Id";
        internal const string HeaderPropertiesUnificRootId = "UnificRootId";
        internal const string HeaderPropertiesSourceLocalizationId = "SourceLocalizationId";
        internal const string HeaderPropertiesTargetLocalizationId = "TargetLocalizationId";

        internal const string GroupId = "id";
        internal const string GroupDataType = "datatype";

        internal const string TransUnitId = "id";
        internal const string PropertyTypeId = "TypeId";
        internal const string PropertyLawId = "LawId";

    }

    internal static class XmlElement
    {
        internal const string Verion = "1.0";
        internal const string Utf = "utf-8";
        internal const string Standalone = "yes";
    }

    internal static class XliffService
    {
        internal const string FileOriginal = "service";
        internal const string FileDataType = "ServiceVersioned";

        internal const string ServiceNameGroupId = "ServiceName";
        internal const string ServiceNameGroupDataType = "ServiceName";

        internal const string ServiceDescriptionGroupId = "ServiceDescription";
        internal const string ServiceDescriptionGroupDataType = "ServiceDescription";

        internal const string ServiceKeywordGroupId = "ServiceKeyword";
        internal const string ServiceKeywordGroupDataType = "Keyword";

        internal const string ServiceLawGroupId = "ServiceLaw";
        internal const string ServiceLawGroupDataType = "Law";
        internal const string ServiceLawName = "Name";
        internal const string ServiceLawWebPage = "WebPage";

        internal const string ServiceProducerGroupId = "ServiceProducer";
        internal const string ServiceProducerGroupDataType = "ServiceProducerAdditionalInformation";
        internal const string ServiceProducerId = "ServiceProducerId";
        internal const string ServiceProducerAdditionalInfo = "ServiceProducerAdditionalInformation";

        internal const string ServiceVoucherGroupId = "ServiceVoucher";
        internal const string ServiceVoucherGroupDataType = "WebPage";
        internal const string ServiceVoucherName = "Name";
        internal const string ServiceVoucherWebPage = "WebPage";
        internal const string ServiceVoucherAdditionalInformation = "AdditionalInformation";
        internal const string ServiceVoucherTargetWebId = "targetWebId";
        internal const string ServiceVoucherOrderNumber = "orderNumber";

        internal const string ServiceRequiredGroupId = "ConditionsAndCriteria";
        internal const string ServiceRequiredGroupDataType = "ServiceRequirement";
        internal const string ServiceRequiredSourceId = "sourceId";
        internal const string ServiceRequiredTargetId = "targetId";


    }
}
