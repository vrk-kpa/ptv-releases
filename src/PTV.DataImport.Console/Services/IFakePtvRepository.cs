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
using PTV.DataImport.ConsoleApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTV.DataImport.ConsoleApp.Services
{
    public interface IFakePtvRepository
    {
        /// <summary>
        /// Get all organizations.
        /// </summary>
        List<SourceOrganizationEntity> GetOrganizations();

        /// <summary>
        /// Get all organization ids.
        /// </summary>
        List<int> GetOrganizationIds();

        /// <summary>
        /// Get organization with id.
        /// </summary>
        /// <param name="organizationId">organization id</param>
        /// <returns>found organization or null if no organization is found</returns>
        SourceOrganizationEntity GetOrganization(int organizationId);

        /// <summary>
        /// Get all phone entities.
        /// </summary>
        List<SourcePhoneEntity> GetPhoneEntities();

        /// <summary>
        /// Get all services.
        /// </summary>
        List<SourceServiceEntity> GetServices();

        /// <summary>
        /// Get all transaction forms.
        /// </summary>
        List<SourceTransactionFormEntity> GetTransactionForms();

        /// <summary>
        /// Get all offices.
        /// </summary>
        List<SourceOfficeEntity> GetOffices();

        /// <summary>
        /// Get all general descriptions.
        /// </summary>
        List<SourceGeneralDescription> GetGeneralDescriptions();

        /// <summary>
        /// Get all electronic transaction services.
        /// </summary>
        List<SourceElectronicTransactionService> GetElectronicTransactionServices();

        /// <summary>
        /// Get all electronic information services.
        /// </summary>
        List<SourceElectronicInformationService> GetElectronicInformationServices();
    }
}
