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
using System.Collections.Specialized;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// ids of tasks
    /// </summary>
    public enum TasksIdsEnum
    {
        /// <summary>
        /// outdated draft services
        /// </summary>
        OutdatedDraftServices,
        /// <summary>
        /// outdated published services
        /// </summary>
        OutdatedPublishedServices,
        /// <summary>
        /// outdated draft channels
        /// </summary>
        OutdatedDraftChannels,
        /// <summary>
        /// outdated published channels
        /// </summary>
        OutdatedPublishedChannels,
        /// <summary>
        /// services without channels
        /// </summary>
        ServicesWithoutChannels,
        /// <summary>
        /// channels without services
        /// </summary>
        ChannelsWithoutServices,
        /// <summary>
        /// Translation arrived
        /// </summary>
        TranslationArrived,
        /// <summary>
        /// Organizations without mandatory language translations
        /// </summary>
        MissingLanguageOrganizations
    }
    /// <summary>
    /// View model for tasks numner
    /// </summary>
    public class VmTasksBase
    {
        /// <summary>
        /// Task Id
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter), true)]
        public TasksIdsEnum Id { get; set; }
        /// <summary>
        /// Number of entities
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// Send to client only positive number 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeCount()
        {
            return Count >= 0;
        }
    }

    /// <summary>
    /// View model for tasks number
    /// </summary>
    public class VmTasks : VmTasksBase, IVmSearchBase
    {
        /// <summary>
        /// C tor
        /// </summary>
        public VmTasks()
        {
            
        }
        
        /// <summary>
        /// c tor
        /// </summary>
        /// <param name="baseModel"></param>
        public VmTasks(IVmSearchBase baseModel)
        {
            Count = baseModel.Count;
            MaxPageCount = baseModel.MaxPageCount;
            PageNumber = baseModel.PageNumber;
            MoreAvailable = baseModel.MoreAvailable;
        }
        
        /// <summary>
        /// Entitties of tasks
        /// </summary>
        public IEnumerable<IVmBase> Entities { get; set; }
       
        /// <summary>
        /// indicates, whehter pospone button should be visible
        /// </summary>
        public bool IsPostponeButtonVisible { get; set; }
        

        /// <inheritdoc />
        public int MaxPageCount { get; set; }

        /// <inheritdoc />
        public int PageNumber { get; set; }

        /// <inheritdoc />
        public bool MoreAvailable { get; set; }        
    }
}
