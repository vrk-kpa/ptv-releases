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
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PTV.DataMapper.ConsoleApp.Models
{
    public abstract class SourceBase<T> where T : class
    {
        private StringBuilder errorMsg;

        public SourceBase()
        {
            errorMsg = new StringBuilder();
        }


        public string ErrorMsg
        {
            get { return errorMsg.ToString(); }
            protected set
            {
                errorMsg.Append(value);
            }
        }

        public abstract T ConvertToVm(string orgId, string code, int id = 0);

        protected string GetStringWithMaxLength(string str, int length)
        {
            // For testing purposes comment out
            //if (str.Length > length) ErrorMsg = $"Value length is greater than { length }";
            return str.Length > length ? str.Substring(0, length) : str;
        }
    }
}
