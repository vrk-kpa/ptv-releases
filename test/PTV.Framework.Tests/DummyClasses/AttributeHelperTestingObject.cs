using System;
using System.Collections.Generic;
using System.Text;

namespace PTV.Framework.Tests.DummyClasses
{
    class AttributeHelperTestingObject
    {
        public int Integer { get; set; }

        public int? NullableInteger { get; set; }
        public string String { get; set; }

        public DateTime DateTime { get; set; }

        public SomeDemoObject SomeDemo { get; set; }
    }
}
