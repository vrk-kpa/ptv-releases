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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTV.Framework.Interfaces
{
    public interface ITextManager
    {
        /// <summary>
        /// Convert Json to Pure text.
        /// </summary>
        /// <param name="json"></param>
        /// <returns>String</returns>
        string ConvertToPureText(string json);

        /// <summary>
        /// Convert Json to Markdown.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="showHeadingElement"></param>
        /// <returns>String</returns>
        string ConvertToMarkDown(string json, bool showHeadingElement = false);

        /// <summary>
        /// Convert MarkDown text to Json by TextBlock object.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>String</returns>
        string ConvertMarkdownToJson(string text);

        /// <summary>
        /// Convert text only with loine breaks(\n) to Json
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        string ConvertTextWithLineBreaksToJson(string text);
    }
}
