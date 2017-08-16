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
using PTV.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Interfaces.Translators
{
    public interface ITranslationEntity
    {
        TTranslateTo Translate<TTranslateFrom, TTranslateTo>(TTranslateFrom entity) where TTranslateFrom : class where TTranslateTo : class;

        TTranslateTo TranslateFirst<TTranslateFrom, TTranslateTo>(IQueryable<TTranslateFrom> query) where TTranslateFrom : class where TTranslateTo : class;

        IReadOnlyList<TTranslateTo> TranslateAll<TTranslateFrom, TTranslateTo>(IQueryable<TTranslateFrom> query) where TTranslateFrom : class where TTranslateTo : class;

        IReadOnlyList<TTranslateTo> TranslateAll<TTranslateFrom, TTranslateTo>(IEnumerable<TTranslateFrom> enumerable) where TTranslateFrom : class where TTranslateTo : class;

        IReadOnlyList<TTranslateTo> TranslateAll<TTranslateFrom, TTranslateTo>(ICollection<TTranslateFrom> collection) where TTranslateFrom : class where TTranslateTo : class;

        void SetLanguage(LanguageCode language);
        void SetLanguage(Guid? language);
    }
}
