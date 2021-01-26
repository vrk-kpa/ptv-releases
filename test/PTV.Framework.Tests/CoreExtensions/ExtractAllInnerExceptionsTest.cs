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
using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace PTV.Framework.Tests.CoreExtensions
{
    public class ExtractAllInnerExceptionsTest
    {
        [Fact]
        public void TestExtractAllInnerExceptionsWithNull()
        {
            string message = Framework.CoreExtensions.ExtractAllInnerExceptions(null);
            message.Should().BeEmpty();
        }

        [Fact]
        public void TestExtractAllInnerExceptions()
        {
            // the implementation only concatenates exception type and message (no stacktrace)
            // format is: excp.GetType().Name + " : " + excp.Message + "\n" and this same is repeated as long there are innerexceptions

            ArgumentNullException nex = new ArgumentNullException("nex", "Parameter cannot be null.");
            ArgumentException argEx = new ArgumentException("Invalid argument supplied.", nex);
            Exception ex = new Exception("An error occured when calling typography service.", argEx);

            string extractedMessage = Framework.CoreExtensions.ExtractAllInnerExceptions(ex);
            extractedMessage.Should().NotBeNullOrWhiteSpace();

            // create the expected message parts separated with the \n
            // remove \r\n so that we can split with \n easily
            string partOne = $"{ex.GetType().Name} : {ex.Message}";//.Replace(Environment.NewLine, string.Empty);
            string partTwo = $"{argEx.GetType().Name} : {argEx.Message}";//.Replace(Environment.NewLine, string.Empty);
            string partThree = $"{nex.GetType().Name} : {nex.Message}";//.Replace(Environment.NewLine, string.Empty);

            // count all parts separated by new line character (ArgumentNullException ads another line in message)
            var parts = new string[] {partOne, partTwo, partThree, string.Empty};
            var totalCount = parts.SelectMany(x => x.Split(Environment.NewLine)).Count();

            // don't remove empty entries
            var splittedMsgs = extractedMessage.Split(Environment.NewLine, StringSplitOptions.None);
            extractedMessage.Should().Be(string.Join(Environment.NewLine, parts));
            Assert.Equal(parts.Length, totalCount);
            splittedMsgs.Length.Should().Be(totalCount, $"Count should be {totalCount} because there are 3 exceptions and after last exception there will be '\\n' which will produce the extra string in the array.");

            splittedMsgs[0].Should().Be(partOne);
            splittedMsgs[1].Should().Be(partTwo);
            $"{splittedMsgs[2]}{splittedMsgs[3]}".Should().Be(partThree);
        }
    }
}
