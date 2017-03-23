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
using PTV.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of hours
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmEntityBase" />
    public class VmHours : VmEntityBase
    {
        /// <summary>
        /// Gets or sets the additional information.
        /// </summary>
        /// <value>
        /// The additional information.
        /// </value>
        [JsonProperty(PropertyName = "alterTitle")]
        public string AdditionalInformation { get; set; }

        /// <summary>
        /// Gets or sets the valid from.
        /// </summary>
        /// <value>
        /// The valid from.
        /// </value>
        public long? ValidFrom { get; set; }
        /// <summary>
        /// Gets or sets the valid to.
        /// </summary>
        /// <value>
        /// The valid to.
        /// </value>
        public long? ValidTo { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is date range.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is date range; otherwise, <c>false</c>.
        /// </value>
        public bool IsDateRange { get; set; }

        /// <summary>
        /// Gets or sets the type of the service hours.
        /// </summary>
        /// <value>
        /// The type of the service hours.
        /// </value>
        [JsonIgnore]
        public ServiceHoursTypeEnum ServiceHoursType { get; set; }
    }

    /// <summary>
    /// View model of normal hours
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmHours" />
    public class VmNormalHours : VmHours
    {
        /// <summary>
        /// Gets or sets the daily hours.
        /// </summary>
        /// <value>
        /// The daily hours.
        /// </value>
        [JsonProperty(PropertyName = "dailyOpeningHours")]
        public List<VmDailyHours> DailyHours { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="VmNormalHours"/> is nonstop.
        /// </summary>
        /// <value>
        ///   <c>true</c> if nonstop; otherwise, <c>false</c>.
        /// </value>
        public bool Nonstop { get; set; }
    }

    /// <summary>
    /// View model of special hours
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmHours" />
    /// <seealso cref="PTV.Domain.Model.Models.IDailyHours" />
    public class VmSpecialHours : VmHours, IDailyHours
    {
        /// <summary>
        /// Gets or sets the day from.
        /// </summary>
        /// <value>
        /// The day from.
        /// </value>
        public WeekDayEnum DayFrom { get; set; }
        /// <summary>
        /// Gets or sets the day to.
        /// </summary>
        /// <value>
        /// The day to.
        /// </value>
        public WeekDayEnum? DayTo { get; set; }
        /// <summary>
        /// Gets or sets the time from.
        /// </summary>
        /// <value>
        /// The time from.
        /// </value>
        public long? TimeFrom { get; set; }
        /// <summary>
        /// Gets or sets the time to.
        /// </summary>
        /// <value>
        /// The time to.
        /// </value>
        public long? TimeTo { get; set; }
        /// <summary>
        /// Id of owner entity
        /// </summary>
        [JsonIgnore]
        public Guid? OwnerReferenceId { get { return Id;} set {} }
    }

    /// <summary>
    /// View model of exceptional hours
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmHours" />
    /// <seealso cref="PTV.Domain.Model.Models.IDailyHours" />
    public class VmExceptionalHours : VmHours, IDailyHours
    {
        /// <summary>
        /// Gets or sets the day from.
        /// </summary>
        /// <value>
        /// The day from.
        /// </value>
        [JsonIgnore]
        WeekDayEnum IDailyHours.DayFrom
        {
            get { return WeekDayEnum.Monday; }
            set {  }
        }

        /// <summary>
        /// Gets or sets the day to.
        /// </summary>
        /// <value>
        /// The day to.
        /// </value>
        [JsonIgnore]
        WeekDayEnum? IDailyHours.DayTo
        {
            get { return WeekDayEnum.Sunday; }
            set { }
        }

        /// <summary>
        /// Gets or sets the time from.
        /// </summary>
        /// <value>
        /// The time from.
        /// </value>
        public long? TimeFrom { get; set; }
        /// <summary>
        /// Gets or sets the time to.
        /// </summary>
        /// <value>
        /// The time to.
        /// </value>
        public long? TimeTo { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="VmExceptionalHours"/> is closed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if closed; otherwise, <c>false</c>.
        /// </value>
        public bool Closed { get; set; }
        /// <summary>
        /// Id of owner entity
        /// </summary>
        [JsonIgnore]
        Guid? IVmOwnerReference.OwnerReferenceId
        {
            get { return Id; }
            set { }
        }
    }

    /// <summary>
    /// daily hours extra types
    /// </summary>
    public enum DailyHoursExtraTypes
    {
        /// <summary>
        /// The not defined
        /// </summary>
        NotDefined,
        /// <summary>
        /// The hidden
        /// </summary>
        Hidden,
        /// <summary>
        /// The vissible
        /// </summary>
        Vissible
    }

    /// <summary>
    /// Interface of daily hours
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmOwnerReference" />
    public interface IDailyHours: IVmOwnerReference
    {
        /// <summary>
        /// Gets or sets the day from.
        /// </summary>
        /// <value>
        /// The day from.
        /// </value>
        WeekDayEnum DayFrom { get; set; }
        /// <summary>
        /// Gets or sets the day to.
        /// </summary>
        /// <value>
        /// The day to.
        /// </value>
        WeekDayEnum? DayTo { get; set; }
        /// <summary>
        /// Gets or sets the time from.
        /// </summary>
        /// <value>
        /// The time from.
        /// </value>
        long? TimeFrom { get; set; }
        /// <summary>
        /// Gets or sets the time to.
        /// </summary>
        /// <value>
        /// The time to.
        /// </value>
        long? TimeTo { get; set; }
    }

    /// <summary>
    /// View model of daily hours
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.IDailyHours" />
    public class VmDailyHours : IDailyHours
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id => $"{OwnerReferenceId}_{(int)DayFrom}";
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="VmDailyHours"/> is day.
        /// </summary>
        /// <value>
        ///   <c>true</c> if day; otherwise, <c>false</c>.
        /// </value>
        public bool Day { get; set; }
        /// <summary>
        /// Gets or sets the day from.
        /// </summary>
        /// <value>
        /// The day from.
        /// </value>
        public WeekDayEnum DayFrom { get; set; }
        /// <summary>
        /// Gets or sets the day to.
        /// </summary>
        /// <value>
        /// The day to.
        /// </value>
        [JsonIgnore]
        public WeekDayEnum? DayTo { get { return null; } set {} }
        /// <summary>
        /// Gets or sets the time from.
        /// </summary>
        /// <value>
        /// The time from.
        /// </value>
        public long? TimeFrom { get; set; }
        /// <summary>
        /// Gets or sets the time to.
        /// </summary>
        /// <value>
        /// The time to.
        /// </value>
        public long? TimeTo { get; set; }
        /// <summary>
        /// Gets or sets the time from extra.
        /// </summary>
        /// <value>
        /// The time from extra.
        /// </value>
        public long? TimeFromExtra { get; set; }
        /// <summary>
        /// Gets or sets the time to extra.
        /// </summary>
        /// <value>
        /// The time to extra.
        /// </value>
        public long? TimeToExtra { get; set; }
        /// <summary>
        /// Gets or sets the extra.
        /// </summary>
        /// <value>
        /// The extra.
        /// </value>
        public DailyHoursExtraTypes? Extra { get; set; }

        /// <summary>
        /// Id of owner entity
        /// </summary>
        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }
    }

    /// <summary>
    /// View model of extra hours
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.IDailyHours" />
    public class VmExtraHours : IDailyHours
    {
        /// <summary>
        /// Gets or sets the hours.
        /// </summary>
        /// <value>
        /// The hours.
        /// </value>
        public VmDailyHours Hours { get; set; }

        /// <summary>
        /// Id of owner entity
        /// </summary>
        Guid? IVmOwnerReference.OwnerReferenceId
        {
            get { return Hours.OwnerReferenceId; }
            set {  }
        }

        /// <summary>
        /// Gets or sets the day from.
        /// </summary>
        /// <value>
        /// The day from.
        /// </value>
        WeekDayEnum IDailyHours.DayFrom
        {
            get { return Hours.DayFrom; }
            set { }
        }

        /// <summary>
        /// Gets or sets the day to.
        /// </summary>
        /// <value>
        /// The day to.
        /// </value>
        WeekDayEnum? IDailyHours.DayTo
        {
            get { return Hours.DayTo; }
            set { }
        }

        /// <summary>
        /// Gets or sets the time from.
        /// </summary>
        /// <value>
        /// The time from.
        /// </value>
        long? IDailyHours.TimeFrom
        {
            get { return Hours.TimeFromExtra ?? 0; }
            set { }
        }

        /// <summary>
        /// Gets or sets the time to.
        /// </summary>
        /// <value>
        /// The time to.
        /// </value>
        long? IDailyHours.TimeTo
        {
            get { return Hours.TimeToExtra ?? 0; }
            set { }
        }
    }
}
