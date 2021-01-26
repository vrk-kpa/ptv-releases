/**
 * The MIT License
 * Copyright (c) 2020 Finnish Digital Agency (DVV)
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
using PTV.Domain.Model.Models.Interfaces;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces.Localization;
using PTV.Framework.Interfaces;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// Extended VmListItem with status
    /// </summary>
    public class VmListItemWithStatus : VmListItem
    {
        /// <summary>
        /// Publishing status of item
        /// </summary>
        [JsonProperty("publishingStatus")]
        public Guid PublishingStatusId { get; set; }
        /// <summary>
        /// Gets or sets the version Id.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid VersionedId { get; set; }
        /// <summary>
        /// Type ID of the entity.
        /// </summary>
        public Guid? TypeId { get; set; }
    }

    /// <summary>
    /// Type has property if is available for current user
    /// </summary>
    public interface IVmRestrictableType : IVmBase
    {
        /// <summary>
        ///
        /// </summary>
        EVmRestrictionFilterType Access { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        string Code { get; set; }

        /// <summary>
        /// Gets or sets the order number.
        /// </summary>
        /// <value>
        /// The order number.
        /// </value>
        int? OrderNumber { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class VmEnumType : VmListItem, IVmRestrictableType
    {
        /// <summary>
        ///
        /// </summary>
        public EVmRestrictionFilterType Access { get; set; } = EVmRestrictionFilterType.Allowed;

        /// <summary>
        ///
        /// </summary>
        public bool Allowed => Access == EVmRestrictionFilterType.Allowed;
    }



    /// <summary>
        /// View model of list item
        /// </summary>
        /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmListItem" />
        /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmOwnerReference" />
        /// <seealso cref="PTV.Domain.Model.Models.Interfaces.Localization.IVmTranslatedItem" />
        public class VmListItem : IVmListItem, IVmOwnerReference, IVmTranslatedItem, IVmOrderable
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public Dictionary<string, string> Description { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the alternate name.
        /// </summary>
        /// <value>
        /// The alternate name.
        /// </value>
        public string AlternateName { get; set; }
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; set; }
        /// <summary>
        /// Gets or sets the order number.
        /// </summary>
        /// <value>
        /// The order number.
        /// </value>
        [JsonIgnore]
        public int? OrderNumber { get; set; }
        /// <summary>
        /// Gets or sets the translation.
        /// </summary>
        /// <value>
        /// The translation.
        /// </value>
        [JsonConverter(typeof(VmTranslationJsonConverter))]
		public IVmTranslationItem Translation { get; set; }
        /// <summary>
        /// Id of owner entity
        /// </summary>
        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the default translation.
        /// </summary>
        /// <value>
        /// The default translation.
        /// </value>
        string IVmTranslatedItem.DefaultTranslation
        {
            get => Name;
            set => Name = value;
        }
    }

    /// <summary>
    /// Model for Regex Parsers
    /// </summary>
    public class VmParser : IVmBase
    {
        /// <summary>
        /// Gets or sets the Pattern.
        /// </summary>
        /// <value>
        /// The pattern.
        /// </value>
        public string Pattern { get; set; }
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; set; }
    }

    /// <summary>
    /// View model translation converter
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    public class VmTranslationJsonConverter : JsonConverter
    {
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var p = JsonConvert.SerializeObject(value);
            writer.WriteRawValue(p);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return null;
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
