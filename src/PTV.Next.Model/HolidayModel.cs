using System;
using System.Collections.Generic;

namespace PTV.Next.Model
{
    public class HolidayModel
    {
        public DateTime Date { get; set; }
        public Guid Id { get; set; }
        public Dictionary<LanguageEnum, string> Names { get; set; }
    }
}