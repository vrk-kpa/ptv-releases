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
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.MapServer.Common;
using PTV.MapServer.ExceptionHandler;
using PTV.MapServer.Interfaces;
using PTV.MapServer.Models;

namespace PTV.MapServer.WfsHandlers
{
    [RegisterService(typeof(WfsGetCapabilitiesHandler), RegisterType.Transient)]
    internal class WfsGetCapabilitiesHandler : WfsOperationBase
    {
        private const string TemplatesPath = ".\\WfsTemplates";
        private const string WFS_CapabilitiesTemplate_Prefix = "WFS_GetCapabilitiesTemplate";
        private const string Wfs_Template_Extension = ".xml";


        public WfsGetCapabilitiesHandler(MapServerConfiguration configuration, IResolveManager resolveManager)
            : base(configuration, resolveManager)
        {}

        public override XDocument Handle(HttpGetRequestQuery requestParameters)
        {
            var parameters = new WfsGetCapabilitiesParameters();
            return Handle(parameters);
        }

        public XDocument Handle(WfsGetCapabilitiesParameters requestParameters)
        {


            WfsSupportedVersionEnum version;

            // handle 'AcceptVersions' request parameter
            if (requestParameters.AcceptVersions != null)
            {
                version = GetAcceptableVersion(requestParameters.AcceptVersions);
            }

            // handle 'Version' request parameter
            // according to specification, if both params are set (version and acceptVersion)
            // version parameter should be omitted
            else if (!string.IsNullOrEmpty(requestParameters.Version) && requestParameters.AcceptVersions == null)
            {
                version = GetSupportedVersionForRequestedVersion(requestParameters.Version);
            }

            // if no version parameter is set, use highets supported version
            else
            {
                version = WfsSupportedVersionEnum._1_1_0;
            }

            // load XML document
            var path = Path.Combine(Path.Combine(TemplatesPath, WFS_CapabilitiesTemplate_Prefix + version + Wfs_Template_Extension));
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"XML template for version {ParseEnumVersionToVersionFormat(version)} was not found.");
            }

            var xmlTemplate = path;
            var doc = XDocument.Load(xmlTemplate);

            // handle 'section' request parameter
            if (requestParameters.Sections != null)
            {
                HandleResultSections(requestParameters.Sections, doc);
            }

            return doc;
        }

        private WfsSupportedVersionEnum GetAcceptableVersion(string acceptVersions)
        {
            var versions = acceptVersions.Split(',').Select(v => v.Trim()).ToList();
            return GetAcceptableVersion(versions);

        }

        private WfsSupportedVersionEnum GetAcceptableVersion(IEnumerable<string> acceptVersions)
        {
            foreach (var v in acceptVersions)
            {
                RequestValidation.ValidateVersion(v, "AcceptVersion");

                var enumStringVersion = ParseStringVersionToEnumFormat(v);
                WfsSupportedVersionEnum enumVersion;
                if (Enum.TryParse(enumStringVersion, true, out enumVersion))
                {
                    return enumVersion;
                }
            }

            throw new OwsException(OwsExceptionCodeEnum.VersionNegotiationFailed,
                "acceptVersion",
                "No supported version has been found.");
        }

        private static WfsSupportedVersionEnum GetSupportedVersionForRequestedVersion(string requestedVersionStr)
        {
            RequestValidation.ValidateVersion(requestedVersionStr, "version");

            // since PTV supports just 1.1.0 version for now,
            // no version handling is needed
            // the only supported version is used
            return WfsSupportedVersionEnum._1_1_0;
        }

        private static void HandleResultSections(string requestedSections, XDocument doc)
        {
            var sectionList = requestedSections.Split(',').Select(s => s.Trim()).ToList();
            HandleResultSections(sectionList, doc);
        }

        private static void HandleResultSections(ICollection<string> sectionList, XDocument doc)
        {
            if (sectionList.Contains("All", StringComparer.OrdinalIgnoreCase)) return;

            var sectionsToRemove = GetSectionsToRemove(sectionList);
            foreach (var section in sectionsToRemove)
            {
                var element = doc.Root.Elements().FirstOrDefault(e => e.Name.LocalName == section.ToString());
                element?.Remove();
            }
        }

        private static IEnumerable<GetCapabilitiesSectionEnum> GetSectionsToRemove(IEnumerable<string> requestedSectionList)
        {
            var requestedSections = ParseRequestedSections(requestedSectionList);
            var suppportedSections = Enum.GetValues(typeof(GetCapabilitiesSectionEnum)).Cast<GetCapabilitiesSectionEnum>().ToList();
            return suppportedSections.Except(requestedSections);
        }

        private static IEnumerable<GetCapabilitiesSectionEnum> ParseRequestedSections(IEnumerable<string> requestedSections)
        {
            var result = new List<GetCapabilitiesSectionEnum>();
            foreach (var s in requestedSections)
            {
                GetCapabilitiesSectionEnum section;
                if (Enum.TryParse(s, true, out section))
                {
                    result.Add(section);
                }
            }

            return result;
        }

    }
}
