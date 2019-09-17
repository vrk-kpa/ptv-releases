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
    // enum values based on Finto item translated to english
    // public service producers (TT1): http://finto.fi/ptvl/fi/page/TT1
    // Alueellinen yhteistoimintaorganisaatio (RegionalOrganization)
    // Kunta (Municipality)
    // Valtio (State)
    // private service producers (TT2): http://finto.fi/ptvl/fi/page/TT2
    // Järjestöt (Organization)
    // Yritykset (Company)

    /// <summary>
    /// organization types
    /// </summary>
    public enum OrganizationTypeEnum
    {
        /// <summary>
        /// The state
        /// </summary>
        State,
        /// <summary>
        /// The municipality
        /// </summary>
        Municipality,
        /// <summary>
        /// The regional organization
        /// </summary>
        RegionalOrganization,
        /// <summary>
        /// The organization
        /// </summary>
        Organization,
        /// <summary>
        /// The company
        /// </summary>
        Company,
        /// <summary>
        /// Public service producers
        /// </summary>
        TT1,
        /// <summary>
        /// Private service producers
        /// </summary>
        TT2
        
/* SOTE organization types has been disabled (SFIPTV-1177)
        /// <summary>
        /// Social and health service - private
        /// </summary> 
        SotePrivate,
        /// <summary>
        /// Social and health service - public
        /// </summary>
        SotePublic,	
		/// <summary>
        /// Region organization
        /// </summary>
        Region
*/
    }
}
