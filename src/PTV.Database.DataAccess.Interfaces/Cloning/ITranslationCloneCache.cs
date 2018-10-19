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
using PTV.Database.DataAccess.Interfaces.Translators;

namespace PTV.Database.DataAccess.Interfaces.Cloning
{
    public interface ITranslationCloneCache
    {
        void AddToCachedSet<T>(T entityOriginal, T entityCloned) where T : class;

        List<IClonedTraceIdentification<T>> GetFromCachedSet<T>() where T : class;

        IList<object> GetEntitiesCascade(IList<object> rootEntites);

        void MarkAsProcessedByTranslator<T>(T clonedEntity) where T : class;

        void MarkAsMustBeKept<T>(T clonedEntity) where T : class;

        IList<T> GetTouchedEntities<T>(IList<T> rootEntites, IList<object> exceptions) where T : class;

        void MarkAllAsMustBeKept<T>(Func<T, bool> callCondition) where T : class;

        void MarkAllAsMustBeKept<T>(List<T> entities) where T : class;

        void MarkAsDeletable<T>(T clonedEntity) where T : class;

        void MarkAllAsDeletable<T>(Func<T, bool> callCondition) where T : class;

        List<T> MarkAllAsDeletable<T>(List<T> entities, bool ifNotProcessedOnly = false) where T : class;
    }
}
