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
using System.Collections.Generic;
using System.Linq;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.Extensions;

namespace PTV.Domain.Logic.Address
{
    [RegisterService(typeof(AddressLogic),RegisterType.Singleton)]
    public class AddressLogic
    {
        private MapServiceProvider mapServiceProvider;

        public AddressLogic(MapServiceProvider mapServiceProvider)
        {
            this.mapServiceProvider = mapServiceProvider;
        }

        public void PrefilterModel(IAddresses vm)
        {
            if (vm != null)
            {
                vm.VisitingAddresses = vm.VisitingAddresses.Where(IsFilled).Select(h => SetMissingValues(h, AddressTypeEnum.Visiting)).ToList();
                vm.PostalAddresses = vm.PostalAddresses.Where(IsFilled).Select(h => SetMissingValues(h, AddressTypeEnum.Postal)).ToList();
            }
        }

        public bool IsFilled(VmAddressSimple address)
        {
            return !string.IsNullOrEmpty(address.Street + address.PoBox);
        }


        public VmAddressSimple SetMissingValues(VmAddressSimple hour, AddressTypeEnum type)
        {
            hour.AddressType = type;
            return hour;
        }
    }
}
