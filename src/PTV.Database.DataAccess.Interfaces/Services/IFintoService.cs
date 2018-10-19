﻿/**
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
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Extensions;
using System.Collections.Generic;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    public interface IFintoService
    {
        VmTreeItem GetFintoTree(IVmNode model);
        IVmListItemsData<IVmListItem> GetFilteredTree(IVmGetFilteredTree vmGetFilteredTree);
        /// <summary>
        /// Gets parent hierarchy for ontology term
        /// </summary>
        /// <param name="model">IVmGetFilteredTree model</param>
        /// <returns></returns>
        IVmListItemsData<IVmListItem> GetAnnotationHierarchy(IVmGetFilteredTree model);
        IVmListItemsData<VmListItem> Search(IVmGetFilteredTree vmGetFilteredTree);

        IVmOpenApiFintoItemVersionBase GetServiceClassByUri(string uri);
        IVmOpenApiFintoItemVersionBase GetOntologyTermByUri(string uri);
        IVmOpenApiFintoItemVersionBase GetTargetGroupByUri(string uri);
        IVmOpenApiFintoItemVersionBase GetLifeEventByUri(string uri);
        IVmOpenApiFintoItemVersionBase GetIndustrialClassByUri(string uri);
        /// <summary>
        /// Returns all annotations for service from https://api.profium.com/annotate/v1
        /// </summary>
        /// <param name="serviceInfo"></param>
        /// <returns>List of ontology terms</returns>
        VmAnnotations GetAnnotations(ServiceInfo serviceInfo);
        (List<string>, List<string>) CheckServiceClasses(List<string> uriList);
        List<string> CheckOntologyTerms(List<string> uriList);
        List<string> CheckTargetGroups(List<string> uriList);
        List<string> CheckLifeEvents(List<string> uriList);
        List<string> CheckIndustrialClasses(List<string> uriList);
    }
}
