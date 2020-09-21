﻿/**
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

using PTV.Database.DataAccess.Interfaces;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.EntityCloners
{
    [RegisterService(typeof(IEntityCloner<StatutoryServiceGeneralDescriptionVersioned>), RegisterType.Transient)]
    internal class GeneralDescriptionCloner : EntityCloner<StatutoryServiceGeneralDescriptionVersioned>
    {
        public GeneralDescriptionCloner(IResolveManager resolveManager, IEntityNavigationsMap entityNavigationsMap) : base(resolveManager, entityNavigationsMap)
        {
        }

        public override void CloningDefinition()
        {
            AddClone(i => i.Names);
            AddClone(i => i.Descriptions);
            AddClone(i => i.IndustrialClasses);
            AddClone(i => i.Languages);
            AddClone(i => i.LanguageAvailabilities);
            AddClone(i => i.StatutoryServiceLaws);
            AddClone(i => i.LifeEvents);
            AddClone(i => i.OntologyTerms);
            AddClone(i => i.StatutoryServiceRequirements);
            AddClone(i => i.ServiceClasses);
            AddClone(i => i.TargetGroups);
        }
    }
}
