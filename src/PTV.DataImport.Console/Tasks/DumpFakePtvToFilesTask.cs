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
using PTV.DataImport.ConsoleApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Extensions.Logging;

namespace PTV.DataImport.ConsoleApp.Tasks
{
    public class DumpFakePtvToFilesTask
    {
        private IServiceProvider _srvProvider;
        private ILogger _logger;

        public DumpFakePtvToFilesTask(IServiceProvider serviceProvider)
        {
            _srvProvider = serviceProvider;

            _logger = _srvProvider.GetService<ILoggerFactory>().CreateLogger<DumpFakePtvToFilesTask>();

            _logger.LogDebug("DumpFakePtvToFilesTask .ctor");
        }

        public void ValidateJson()
        {
            ISourceRepository repo = _srvProvider.GetService<ISourceRepository>();
            string methodName = null;

            try
            {
                methodName = "GetOrganizations()";
                var orgs = repo.GetOrganizations();

                methodName = "GetPhoneEntities()";
                var phones = repo.GetPhoneEntities();

                methodName = "GetServices()";
                var services = repo.GetServices();

                methodName = "GetTransactionForms()";
                var transactionforms = repo.GetTransactionForms();

                methodName = "GetOffices()";
                var offices = repo.GetOffices();

                methodName = "GetGeneralDescriptions()";
                var descs = repo.GetGeneralDescriptions();

                methodName = "GetElectronicTransactionServices()";
                var eservices = repo.GetElectronicTransactionServices();

                methodName = "GetElectronicInformationServices()";
                var einfo = repo.GetElectronicInformationServices();
            }
            catch (Newtonsoft.Json.JsonSerializationException ex)
            {
                throw new Exception($"ISourceRepository method name: {methodName}. {ex.Message}", ex);
            }
        }

        public void WriteToFiles()
        {
            ISourceRepository repo = _srvProvider.GetService<ISourceRepository>();

            // organization entities
            var jsonlist = repo.GetOrganizationsJson();
            _logger.LogDebug("Fetched organizations from database and writing to file.");
            WriteToFile(FakePtvJsonFileNames.Organizations, jsonlist);

            // phone (service) entities
            jsonlist = repo.GetPhoneEntitiesJson();
            _logger.LogDebug("Fetched phone services from database and writing to file.");
            WriteToFile(FakePtvJsonFileNames.PhoneChannels, jsonlist);

            // service entities
            jsonlist = repo.GetServicesJson();
            _logger.LogDebug("Fetched services from database and writing to file.");
            WriteToFile(FakePtvJsonFileNames.Services, jsonlist);

            // transaction service entities
            jsonlist = repo.GetTransactionFormsJson();
            _logger.LogDebug("Fetched transaction services from database and writing to file.");
            WriteToFile(FakePtvJsonFileNames.TransactionFormChannels, jsonlist);

            // office entities / service location channels?
            jsonlist = repo.GetOfficesJson();
            _logger.LogDebug("Fetched service locations (office entities) from database and writing to file.");
            WriteToFile(FakePtvJsonFileNames.ServiceLocationsChannel, jsonlist);

            // general descriptions
            jsonlist = repo.GetGeneralDescriptionsJson();
            _logger.LogDebug("Fetched general descriptions from database and writing to file.");
            WriteToFile(FakePtvJsonFileNames.GeneralDescriptions, jsonlist);

            // electronic transaction services
            jsonlist = repo.GetElectronicTransactionServicesJson();
            _logger.LogDebug("Fetched electronic transaction services from database and writing to file.");
            WriteToFile(FakePtvJsonFileNames.ElectronicChannels, jsonlist);

            // electronic information services
            jsonlist = repo.GetElectronicInformationServicesJson();
            _logger.LogDebug("Fetched electronic information services from database and writing to file.");
            WriteToFile(FakePtvJsonFileNames.WebpageChannels, jsonlist);
        }

        /// <summary>
        /// Writes the JSON data to named file.
        /// </summary>
        /// <param name="filename">name of the file where the data is writen</param>
        /// <param name="jsondata">JSON data object</param>
        /// <exception cref="System.ArgumentException"><i>filename</i> is null or an empty string</exception>
        /// <exception cref="System.ArgumentNullException"><i>jsondata</i> is a null reference</exception>
        private static void WriteToFile(string filename, object jsondata)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentException("The filename cannot be an empty string or a null reference.", nameof(filename));
            }

            if (jsondata == null)
            {
                throw new ArgumentNullException(nameof(jsondata));
            }

            // convert data to json
            var json = JsonConvert.SerializeObject(jsondata, Formatting.Indented);
            // overwrite the file always
            File.WriteAllText(FakePtvJsonFileNames.GetFilePath(filename), json);
        }
    }
}
