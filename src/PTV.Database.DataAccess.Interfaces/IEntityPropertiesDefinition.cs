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

namespace PTV.Database.DataAccess.Interfaces
{
//   public interface IEntityPropertiesDefinition
//        {
//            List<ForeignKeyNavigationPair> NavigationsMap { get; }
//            List<PrimaryKeyDefinition> PrimaryKeysDefinition { get; }
//            List<CollectionNavigationDefinition> CollectionNavigationMap { get; }
//        }

    /// <summary>
    /// Holder class for relations between navigation and foreign key
    /// </summary>
    public class ForeignKeyNavigationPair
    {
        public List<NameTypePairDefinition> ForeignKeys { get; set; }
        public NameTypePairDefinition Navigation { get; set; }
    }

    public class CollectionNavigationDefinition
    {
        public NameTypePairDefinition Navigation { get; set; }
        public NameTypePairDefinition DeclaringEntity { get; set; }
        public NameTypePairDefinition PrincipalEntity { get; set; }
        public List<ForeignKeyPairDefinition> ForeignKeys { get; set; }
    }

    public class ForeignKeyPairDefinition
    {
        public NameTypePairDefinition DeclaringEntityColumn { get; set; }
        public NameTypePairDefinition PrincipalEntityColumn { get; set; }
    }

    public class EntityPropertiesDefinition //: IEntityPropertiesDefinition
    {
        public Type EntityType { get; set; }
        public List<ForeignKeyNavigationPair> NavigationsMap { get; set; }
        public List<PrimaryKeyDefinition> PrimaryKeysDefinition { get; set; }
        public List<CollectionNavigationDefinition> CollectionNavigationMap { get; set; }
    }

    public class PrimaryKeyDefinition : NameTypePairDefinition
    {
        public bool IsPrimaryAndForeign;
    }

    public class NameTypePairDefinition
    {
        public string Name;
        public Type Type;
    }
}
