﻿/**
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
using PTV.Database.DataAccess.Interfaces.Translators;

namespace PTV.Database.DataAccess.Interfaces.Cloning
{
    public class ClonedTraceIdentification<T> : IClonedTraceIdentification<T>
    {
        public T OriginalEntity { get; set; }
        public T ClonedEntity { get; set; }

        public bool ProcessedByTranslator { get; set; }
        public bool MustBeKept { get; set; }

        object IClonedTraceIdentification.OriginalEntity
        {
            get { return OriginalEntity; }
            set { OriginalEntity = (T)value; }
        }

        object IClonedTraceIdentification.ClonedEntity
        {
            get { return ClonedEntity; }
            set { ClonedEntity = (T)value; }
        }
    }
}
