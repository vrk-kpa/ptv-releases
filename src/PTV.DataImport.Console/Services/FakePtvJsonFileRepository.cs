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
using System.Linq;
using System.Threading.Tasks;
using PTV.DataImport.ConsoleApp.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;

namespace PTV.DataImport.ConsoleApp.Services
{
    public class FakePtvJsonFileRepository : IFakePtvRepository
    {
        private ILogger<IFakePtvRepository> _logger;

        public FakePtvJsonFileRepository(ILogger<IFakePtvRepository> logger)
        {
            _logger = logger;
        }

        public List<SourceElectronicInformationService> GetElectronicInformationServices()
        {
            return FakePtvJsonFileRepository.ReadJsonFile<SourceElectronicInformationService>(FakePtvJsonFileNames.GetFilePath(FakePtvJsonFileNames.WebpageChannels));
        }

        public List<SourceElectronicTransactionService> GetElectronicTransactionServices()
        {
            return FakePtvJsonFileRepository.ReadJsonFile<SourceElectronicTransactionService>(FakePtvJsonFileNames.GetFilePath(FakePtvJsonFileNames.ElectronicChannels));
        }

        public List<SourceGeneralDescription> GetGeneralDescriptions()
        {
            return FakePtvJsonFileRepository.ReadJsonFile<SourceGeneralDescription>(FakePtvJsonFileNames.GetFilePath(FakePtvJsonFileNames.GeneralDescriptions));
        }

        public List<SourceOfficeEntity> GetOffices()
        {
            return FakePtvJsonFileRepository.ReadJsonFile<SourceOfficeEntity>(FakePtvJsonFileNames.GetFilePath(FakePtvJsonFileNames.ServiceLocationsChannel));
        }

        public SourceOrganizationEntity GetOrganization(int organizationId)
        {
            return GetOrganizations().Find(o => o.Id == organizationId);
        }

        public List<int> GetOrganizationIds()
        {
            return GetOrganizations().Select(o => o.Id).ToList();
        }

        public List<SourceOrganizationEntity> GetOrganizations()
        {
            return FakePtvJsonFileRepository.ReadJsonFile<SourceOrganizationEntity>(FakePtvJsonFileNames.GetFilePath(FakePtvJsonFileNames.Organizations));
        }

        public List<SourcePhoneEntity> GetPhoneEntities()
        {
            return FakePtvJsonFileRepository.ReadJsonFile<SourcePhoneEntity>(FakePtvJsonFileNames.GetFilePath(FakePtvJsonFileNames.PhoneChannels));
        }

        public List<SourceServiceEntity> GetServices()
        {
            return FakePtvJsonFileRepository.ReadJsonFile<SourceServiceEntity>(FakePtvJsonFileNames.GetFilePath(FakePtvJsonFileNames.Services));
        }

        public List<SourceTransactionFormEntity> GetTransactionForms()
        {
            return FakePtvJsonFileRepository.ReadJsonFile<SourceTransactionFormEntity>(FakePtvJsonFileNames.GetFilePath(FakePtvJsonFileNames.TransactionFormChannels));
        }

        private static List<T> ReadJsonFile<T>(string jsonFileName)
        {
            return JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(jsonFileName));
        }
    }
}
