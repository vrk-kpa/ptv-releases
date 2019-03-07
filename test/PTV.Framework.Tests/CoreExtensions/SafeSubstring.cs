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
using Xunit;

namespace PTV.Framework.Tests.CoreExtensions
{
    public class SafeSubstring
    {
        private const string ExampleInput = "ABCDEF";
        private const int ExampleLength = 6;
        
        [Fact]
        public void NullInputReturnsNull()
        {
            string input = null;
            
            var result = input.SafeSubstring(0, 1);
            
            Assert.Null(result);
        }

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(0, -1)]
        public void IncorrectRangeReturnsNull(int start, int length)
        {
            var result = ExampleInput.SafeSubstring(start, length);
            
            Assert.Null(result);
        }

        [Fact]
        public void LateStartReturnsNull()
        {
            var start = ExampleLength + 1;
            var length = ExampleLength + 2;

            var result = ExampleInput.SafeSubstring(start, length);
            
            Assert.Null(result);
        }

        [Theory]
        [InlineData(0, 0, "")] // Start at the beginning, take no characters
        [InlineData(0, 1, "A")] // Start at the beginning, take 1 character
        [InlineData(0, ExampleLength, ExampleInput)] // Start at the beginning, take all characters
        [InlineData(0, ExampleLength + 1,
            ExampleInput)] // Start at the beginning, try to take more characters than there are
        [InlineData(ExampleLength - 1, 1, "F")] // Start at the end, take one character
        [InlineData(ExampleLength - 1, int.MaxValue,
            "F")] // Start at the end, try to take as many characters as possible
        public void ValidInputReturnsCorrectResult(int start, int length, string expectedResult)
        {
            var actualResult = ExampleInput.SafeSubstring(start, length);
            
            Assert.Equal(expectedResult, actualResult);
        }
    }
}
