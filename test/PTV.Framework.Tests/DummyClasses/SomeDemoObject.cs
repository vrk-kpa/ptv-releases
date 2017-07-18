using System;
using System.Collections.Generic;
using System.Text;

namespace PTV.Framework.Tests.DummyClasses
{
    class SomeDemoObject
    {
        public int Integer { get; set; }

        public string String { get; set; }

        /// <summary>
        /// Mainly for list item validators. Type string defines the ItemType for example AlternateName
        /// </summary>
        public string Type { get; set; }
    }
}
