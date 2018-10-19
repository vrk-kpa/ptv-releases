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
using PTV.Database.DataAccess.ApplicationDbContext;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.EntityCloners
{
    [RegisterService(typeof(IEntityCloner<Address>), RegisterType.Transient)]
    internal class AddressCloner : EntityCloner<Address>
    {
        public AddressCloner(IResolveManager resolveManager, IEntityNavigationsMap entityNavigationsMap) : base(resolveManager, entityNavigationsMap)
        {
        }

        public override void CloningDefinition()
        {
            AddClone(i => i.Coordinates);
            AddClone(i => i.AddressAdditionalInformations);
            AddClone(i => i.AddressStreets);
            AddClone(i => i.AddressPostOfficeBoxes);
            AddClone(i => i.AddressForeigns);
            AddClone(i => i.AddressOthers);
            AddClone(i => i.Receivers);
        }
    }

    [RegisterService(typeof(IEntityCloner<AddressStreet>), RegisterType.Transient)]
    internal class AddressStreetCloner : EntityCloner<AddressStreet>
    {
        public AddressStreetCloner(IResolveManager resolveManager, IEntityNavigationsMap entityNavigationsMap) : base(resolveManager, entityNavigationsMap)
        {
        }

        public override void CloningDefinition()
        {
            AddClone(i => i.StreetNames);
        }
    }

    [RegisterService(typeof(IEntityCloner<AddressPostOfficeBox>), RegisterType.Transient)]
    internal class AddressPostOfficeBoxCloner : EntityCloner<AddressPostOfficeBox>
    {
        public AddressPostOfficeBoxCloner(IResolveManager resolveManager, IEntityNavigationsMap entityNavigationsMap) : base(resolveManager, entityNavigationsMap)
        {
        }

        public override void CloningDefinition()
        {
            AddClone(i => i.PostOfficeBoxNames);
        }
    }

    [RegisterService(typeof(IEntityCloner<AddressForeign>), RegisterType.Transient)]
    internal class AddressForeignCloner : EntityCloner<AddressForeign>
    {
        public AddressForeignCloner(IResolveManager resolveManager, IEntityNavigationsMap entityNavigationsMap) : base(resolveManager, entityNavigationsMap)
        {
        }

        public override void CloningDefinition()
        {
            AddClone(i => i.ForeignTextNames);
        }
    }
    
    [RegisterService(typeof(IEntityCloner<AddressOther>), RegisterType.Transient)]
    internal class AddressOtherCloner : EntityCloner<AddressOther>
    {
        public AddressOtherCloner(IResolveManager resolveManager, IEntityNavigationsMap entityNavigationsMap) : base(resolveManager, entityNavigationsMap)
        {
        }

        public override void CloningDefinition()
        {
        }
    }
}