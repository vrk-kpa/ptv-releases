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
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PTV.Framework
{
    [RegisterService(typeof(EnvironmentHelper),RegisterType.Singleton)]
    public class EnvironmentHelper
    {
        private ExecutingEnvironment environmentType;

        public ExecutingEnvironment GetExecutingEnvironment()
        {
            if (environmentType == ExecutingEnvironment.Unknown)
            {
                var executingAssembly = Assembly.GetEntryAssembly().GetName().Name;
                switch (executingAssembly)
                {
                    case "PTV.Application.Web": environmentType = ExecutingEnvironment.Web; break;
                    case "PTV.Application.Api": environmentType = ExecutingEnvironment.UiApi; break;
                    case "PTV.Application.OpenApi": environmentType = ExecutingEnvironment.OpenApi; break;
                    default: environmentType = ExecutingEnvironment.Unknown; break;
                }
            }
            return environmentType;
        }
    }

    public enum ExecutingEnvironment
    {
        Unknown,
        Web,
        UiApi,
        OpenApi
    }
}
