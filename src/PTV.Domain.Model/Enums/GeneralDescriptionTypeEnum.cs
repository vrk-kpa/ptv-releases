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

namespace PTV.Domain.Model.Enums
{
    /// <summary>
    /// general descritpion type enum
    /// </summary>
    public enum GeneralDescriptionTypeEnum
    {
        /// <summary>
        /// Municipality services
        /// </summary>
        Municipality,
        /// <summary>
        /// Business Subregion services
        /// </summary>
        BusinessSubregion,
        /// <summary>
        /// Aland Island services
        /// </summary>
        AlandIsland
/* SOTE has been disabled (SFIPTV-1177)        
        /// <summary>
        /// Services prescribed by the Freedom of Choice Act 
        /// </summary>
        PrescribedByFreedomOfChoiceAct,
        /// <summary>
        /// Other permission granted Social and Health Services
        /// </summary>
        OtherPermissionGrantedSote
*/        
    }
}