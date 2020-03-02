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
namespace PTV.Domain.Model.Enums
{
    /// <summary>
    /// Defines Suomi.fi content types.
    /// </summary>
    public enum FeedbackContentType
    {
        /// <summary>
        /// Service
        /// </summary>
        Service,
        /// <summary>
        /// Permission or obligation
        /// </summary>
        PermitOrObligation,
        /// <summary>
        /// Professional qualification
        /// </summary>
        ProfessionalQualification,
        /// <summary>
        /// E-channel
        /// </summary>
        EChannel,
        /// <summary>
        /// Web page
        /// </summary>
        WebPage,
        /// <summary>
        /// Printable form
        /// </summary>
        PrintableForm,
        /// <summary>
        /// Phone channel
        /// </summary>
        Phone,
        /// <summary>
        /// Service location
        /// </summary>
        ServiceLocation,
        /// <summary>
        /// Organization
        /// </summary>
        Organization
    }
}
