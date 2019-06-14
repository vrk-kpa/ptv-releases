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
using FluentAssertions;
using Xunit;

namespace PTV.Framework.Tests.CoreExtensions
{
    public class InclusiveToListTest
    {
        [Fact]
        public void ReadOnlyListInclusiveToList()
        {
            List<string> theList = new List<string>()
            {
                "ptv", "vrk", "kapa"
            };

            int originalListCount = theList.Count;

            // create a readonly wrapper
            var roList = theList.AsReadOnly();

            var list = roList.InclusiveToList();

            list.Should().NotBeNull();
            list.Count.Should().Be(originalListCount);

            int count = list.Count;

            // and we should be able to add new items to the returned list
            list.Add("kehä I");
            // the count should be increased by one
            list.Count.Should().Be(count + 1);
            // the old list shouldn't change
            theList.Count.Should().Be(originalListCount);
        }

        [Fact]
        public void WritableListInclusiveToList()
        {
            List<string> theList = new List<string>()
            {
                "ptv", "vrk", "kapa"
            };
            
            // because the passed in list is not readonlycollection the returned list should be the same instance
            var list = theList.InclusiveToList();

            list.Should().NotBeNull();
            list.Count.Should().Be(theList.Count);
            // object references should be the same
            list.Should().BeSameAs(theList);

            int count = list.Count;

            // and we should be able to add new items to the returned list
            list.Add("kehä I");
            // the count should be increased by one
            list.Count.Should().Be(count + 1);
            // the old list count should also increase
            theList.Count.Should().Be(list.Count);
        }
        
        [Fact]
        public void NullListInclusiveToList()
        {
            IReadOnlyList<string> nullList = null;

            List<string> list = nullList.InclusiveToList();

            list.Should().BeNull();
        }

        [Fact]
        public void EmptyReadOnlyListInclusiveToList()
        {
            List<string> theList = new List<string>(){};

            var roList = theList.AsReadOnly();

            var list = ((IReadOnlyList<string>)roList).InclusiveToList();
            int count = list.Count;
            list.Add("kehä I");

            list.Count.Should().Be(count + 1);
        }
    }
}
