﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PTV.Next.Model
{
    public class CountryModel
    {
        public string Code { get; set;}
        public Dictionary<LanguageEnum, string> Names {get; set;} = new Dictionary<LanguageEnum, string>();
    }
}
