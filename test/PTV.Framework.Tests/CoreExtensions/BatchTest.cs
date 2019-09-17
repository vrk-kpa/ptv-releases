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
using System.Drawing;
using System.Linq;
using Xunit;

namespace PTV.Framework.Tests.CoreExtensions
{
    public class BatchTest
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void EnumerableZeroOrNegativeBatchThrows(int size)
        {
            var enumerable = GetEnumerable(100);

            Assert.Throws<Exception>(() => enumerable.Batch(size).ToList());
        }
        
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void QueryableZeroOrNegativeBatchThrows(int size)
        {
            var queryable = GetQueryable(100);

            Assert.Throws<Exception>(() => queryable.Batch(size).ToList());
        }

        [Fact]
        public void EnumerableCorrectBatchSizes()
        {
            var enumerable = GetEnumerable(100);

            var batches = enumerable.Batch(30).ToList();
            var firstBatch = batches.First().ToList();
            var lastBatch = batches.Last().ToList();
            
            Assert.Equal(4, batches.Count());
            
            Assert.Equal(30, firstBatch.Count());
            Assert.Equal(0, firstBatch.First().X);
            Assert.Equal(29, firstBatch.Last().X);
            
            Assert.Equal(10, lastBatch.Count());
            Assert.Equal(90, lastBatch.First().X);
            Assert.Equal(99, lastBatch.Last().X);
        }

        [Fact]
        public void QueryableCorrectBatchSizes()
        {
            var queryable = GetQueryable(100);

            var batches = queryable.Batch(30).ToList();
            var firstBatch = batches.First().ToList();
            var lastBatch = batches.Last().ToList();
            
            Assert.Equal(4, batches.Count());
            
            Assert.Equal(30, firstBatch.Count());
            Assert.Equal(0, firstBatch.First().X);
            Assert.Equal(29, firstBatch.Last().X);
            
            Assert.Equal(10, lastBatch.Count());
            Assert.Equal(90, lastBatch.First().X);
            Assert.Equal(99, lastBatch.Last().X);
        }

        private IEnumerable<Point> GetEnumerable(int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new Point(i, i);
            }
        }

        private IQueryable<Point> GetQueryable(int count)
        {
            return GetEnumerable(count).AsQueryable();
        }
    }
}
