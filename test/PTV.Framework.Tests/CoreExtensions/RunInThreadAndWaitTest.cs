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
using Xunit;

namespace PTV.Framework.Tests.CoreExtensions
{
    public class RunInThreadAndWaitTest
    {
        [Fact]
        public void TestRunInThreadAndWait()
        {
            Action act = () => Console.WriteLine("Output from test method: TestRunInThreadAndWait");

            Framework.CoreExtensions.RunInThreadAndWait(act);
        }

        [Fact]
        public void TestRunInThreadAndWaitWithNullAction()
        {
            // this will crash the whole app domain currently because there is NullReferenceException in the call
            Framework.CoreExtensions.RunInThreadAndWait(null);

            // NOTE! Decision should the RunInThreadAndWait handle the possible exception in the thread and throw own exception
            // or let it go to appdomain unhandled exception and tear down the appdomain
        }
    }
}
