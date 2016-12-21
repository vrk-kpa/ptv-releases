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
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.ServiceManager;

namespace PTV.Domain.Model.Models
{
    public class VmHours : VmEntityBase
    {
        [JsonProperty(PropertyName = "alterTitle")]
        public string AdditionalInformation { get; set; }

        public long? ValidFrom { get; set; }
        public long? ValidTo { get; set; }
        public bool IsDateRange { get; set; }

        [JsonIgnore]
        public ServiceHoursTypeEnum ServiceHoursType { get; set; }
    }

    public class VmNormalHours : VmHours
    {
        [JsonProperty(PropertyName = "dailyOpeningHours")]
        public List<VmDailyHours> DailyHours { get; set; }

        public bool Nonstop { get; set; }
    }

    public class VmSpecialHours : VmHours, IDailyHours
    {
        public WeekDayEnum DayFrom { get; set; }
        public WeekDayEnum? DayTo { get; set; }
        public long? TimeFrom { get; set; }
        public long? TimeTo { get; set; }
        [JsonIgnore]
        public Guid? OwnerReferenceId { get { return Id;} set {} }
    }

    public class VmExceptionalHours : VmHours, IDailyHours
    {
        [JsonIgnore]
        WeekDayEnum IDailyHours.DayFrom
        {
            get { return WeekDayEnum.Monday; }
            set {  }
        }

        [JsonIgnore]
        WeekDayEnum? IDailyHours.DayTo
        {
            get { return WeekDayEnum.Sunday; }
            set { }
        }

        public long? TimeFrom { get; set; }
        public long? TimeTo { get; set; }
        public bool Closed { get; set; }
        [JsonIgnore]
        Guid? IVmOwnerReference.OwnerReferenceId
        {
            get { return Id; }
            set { }
        }
    }

    public enum DailyHoursExtraTypes { NotDefined, Hidden, Vissible }

    public interface IDailyHours: IVmOwnerReference
    {
        WeekDayEnum DayFrom { get; set; }
        WeekDayEnum? DayTo { get; set; }
        long? TimeFrom { get; set; }
        long? TimeTo { get; set; }
    }

    public class VmDailyHours : IDailyHours
    {
        public string Id => $"{OwnerReferenceId}_{(int)DayFrom}";
        public bool Day { get; set; }
        public WeekDayEnum DayFrom { get; set; }
        [JsonIgnore]
        public WeekDayEnum? DayTo { get { return null; } set {} }
        public long? TimeFrom { get; set; }
        public long? TimeTo { get; set; }
        public long? TimeFromExtra { get; set; }
        public long? TimeToExtra { get; set; }
        public DailyHoursExtraTypes? Extra { get; set; }

        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }
    }

    public class VmExtraHours : IDailyHours
    {
        public VmDailyHours Hours { get; set; }

        Guid? IVmOwnerReference.OwnerReferenceId
        {
            get { return Hours.OwnerReferenceId; }
            set {  }
        }

        WeekDayEnum IDailyHours.DayFrom
        {
            get { return Hours.DayFrom; }
            set { }
        }

        WeekDayEnum? IDailyHours.DayTo
        {
            get { return Hours.DayTo; }
            set { }
        }

        long? IDailyHours.TimeFrom
        {
            get { return Hours.TimeFromExtra ?? 0; }
            set { }
        }

        long? IDailyHours.TimeTo
        {
            get { return Hours.TimeToExtra ?? 0; }
            set { }
        }
    }
}
