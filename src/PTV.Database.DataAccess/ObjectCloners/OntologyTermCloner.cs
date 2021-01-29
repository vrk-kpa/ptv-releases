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
using System.Collections.Generic;
using System.Linq;
using PTV.Database.Model.Models;

namespace PTV.Database.DataAccess.ObjectCloners
{
    internal class OntologyTermCloner
    {
        public OntologyTermParentCloner ChildCloner { get; set; }
        
        public OntologyTermParentCloner ParentCloner { get; set; }
        
        public OntologyTermNameCloner NameCloner { get; set; }
        
        public OntologyTermExactMatchCloner ExactMatchCloner { get; set; }
        
        public IEnumerable<OntologyTerm> CloneCollection(IEnumerable<OntologyTerm> originalCollection)
        {
            if (originalCollection == null)
            {
                yield break;
            }

            foreach (var relation in originalCollection)
            {
                yield return Clone(relation);
            }
        }
        
        public OntologyTerm Clone(OntologyTerm original)
        {
            if (original == null)
            {
                return null;
            }

            return new OntologyTerm
            {
                Children = ChildCloner?.CloneCollection(original.Children).ToHashSet() ??
                           new HashSet<OntologyTermParent>(),
                Code = original.Code,
                Created = original.Created,
                Id = original.Id,
                Label = original.Label,
                Modified = original.Modified,
                Names = NameCloner?.CloneCollection(original.Names).ToHashSet() ?? new HashSet<OntologyTermName>(),
                Parents = ParentCloner?.CloneCollection(original.Parents).ToHashSet() ??
                          new HashSet<OntologyTermParent>(),
                Uri = original.Uri,
                CreatedBy = original.CreatedBy,
                IsValid = original.IsValid,
                ModifiedBy = original.ModifiedBy,
                OntologyType = original.OntologyType,
                OrderNumber = original.OrderNumber,
                ParentUri = original.ParentUri,
                LastOperationIdentifier = original.LastOperationIdentifier,
                LastOperationType = original.LastOperationType,
                LastOperationTimeStamp = original.LastOperationTimeStamp,
                OntologyTermExactMatches =
                    ExactMatchCloner?.CloneCollection(original.OntologyTermExactMatches).ToHashSet() ??
                    new HashSet<OntologyTermExactMatch>()
            };
        }
    }
}
