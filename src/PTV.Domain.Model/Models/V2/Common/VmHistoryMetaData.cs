using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PTV.Domain.Model.Models.V2.Common
{
    /// <summary>
    /// VmHistoryMetaData
    /// </summary>
    public class VmHistoryMetaData
    {
        /// <summary>
        /// 
        /// </summary>
        public VmHistoryMetaData()
        {
            LanguagesMetaData = new List<VmHistoryMetaDataLanguage>();
        }
        /// <summary>
        /// 
        /// </summary>
        public Guid EntityStatusId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public HistoryAction HistoryAction { get; set; }
        /// <summary>
        /// 
        /// </summary>        
        public List<VmHistoryMetaDataLanguage> LanguagesMetaData { get; set; }        
    }
    /// <summary>
    /// VmHistoryMetaDataLanguage
    /// </summary>
    public class VmHistoryMetaDataLanguage
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid EntityStatusId { get; set; }
        /// <summary>
        /// 
        /// </summary>        
        public Guid LanguageId { get; set; }        
    }
    /// <summary>
    /// Entity history action
    /// </summary>
    public enum HistoryAction
    {
        /// <summary>
        /// save action
        /// </summary>
        Save,
        /// <summary>
        /// delete action
        /// </summary>
        Delete,
        /// <summary>
        /// pulblish action
        /// </summary>
        Publish,
        /// <summary>
        /// restore deleted action
        /// </summary>
        Restore,
        /// <summary>
        /// witdraw published action
        /// </summary>
        Withdraw,
        /// <summary>
        /// Translation order send
        /// </summary>
        TranslationOrdered,
        /// <summary>
        /// Translation order received
        /// </summary>
        TranslationReceived,
        /// <summary>
        /// copy action
        /// </summary>
        Copy
    }
}
