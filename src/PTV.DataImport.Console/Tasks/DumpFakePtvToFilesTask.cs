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
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PTV.DataImport.Console.Services;

namespace PTV.DataImport.Console.Tasks
{
    public class DumpFakePtvToFilesTask
    {
        private readonly IServiceProvider srvProvider;
        private readonly ILogger logger;

        public DumpFakePtvToFilesTask(IServiceProvider serviceProvider)
        {
            srvProvider = serviceProvider;

            logger = srvProvider.GetService<ILoggerFactory>().CreateLogger<DumpFakePtvToFilesTask>();

            logger.LogDebug("DumpFakePtvToFilesTask .ctor");
        }

        public void ValidateJson()
        {
            var repo = srvProvider.GetService<ISourceRepository>();
            string methodName = null;

            try
            {
                methodName = "GetOrganizations()";
                // ReSharper disable once UnusedVariable
                var orgs = repo.GetOrganizations();

                methodName = "GetPhoneEntities()";
                // ReSharper disable once UnusedVariable
                var phones = repo.GetPhoneEntities();

                methodName = "GetServices()";
                // ReSharper disable once UnusedVariable
                var services = repo.GetServices();

                methodName = "GetTransactionForms()";
                // ReSharper disable once UnusedVariable
                var transactionForms = repo.GetTransactionForms();

                methodName = "GetOffices()";
                // ReSharper disable once UnusedVariable
                var offices = repo.GetOffices();

                methodName = "GetGeneralDescriptions()";
                // ReSharper disable once UnusedVariable
                var descs = repo.GetGeneralDescriptions();

                methodName = "GetElectronicTransactionServices()";
                // ReSharper disable once UnusedVariable
                var eServices = repo.GetElectronicTransactionServices();

                methodName = "GetElectronicInformationServices()";
                // ReSharper disable once UnusedVariable
                var eInfo = repo.GetElectronicInformationServices();
            }
            catch (JsonSerializationException ex)
            {
                throw new Exception($"ISourceRepository method name: {methodName}. {ex.Message}", ex);
            }
        }

        public void WriteToFiles()
        {
            var repo = srvProvider.GetService<ISourceRepository>();

            // organization entities
            var jsonList = repo.GetOrganizationsJson();
            logger.LogDebug("Fetched organizations from database and writing to file.");
            WriteToFile(FakePtvJsonFileNames.Organizations, jsonList);

            // phone (service) entities
            jsonList = repo.GetPhoneEntitiesJson();
            logger.LogDebug("Fetched phone services from database and writing to file.");
            WriteToFile(FakePtvJsonFileNames.PhoneChannels, jsonList);

            // service entities
            jsonList = repo.GetServicesJson();
            logger.LogDebug("Fetched services from database and writing to file.");
            WriteToFile(FakePtvJsonFileNames.Services, jsonList);

            // transaction service entities
            jsonList = repo.GetTransactionFormsJson();
            logger.LogDebug("Fetched transaction services from database and writing to file.");
            WriteToFile(FakePtvJsonFileNames.TransactionFormChannels, jsonList);

            // office entities / service location channels?
            jsonList = repo.GetOfficesJson();
            logger.LogDebug("Fetched service locations (office entities) from database and writing to file.");
            WriteToFile(FakePtvJsonFileNames.ServiceLocationsChannel, jsonList);

            // general descriptions
            jsonList = repo.GetGeneralDescriptionsJson();
            logger.LogDebug("Fetched general descriptions from database and writing to file.");
            WriteToFile(FakePtvJsonFileNames.GeneralDescriptions, jsonList);

            // electronic transaction services
            jsonList = repo.GetElectronicTransactionServicesJson();
            logger.LogDebug("Fetched electronic transaction services from database and writing to file.");
            WriteToFile(FakePtvJsonFileNames.ElectronicChannels, jsonList);

            // electronic information services
            jsonList = repo.GetElectronicInformationServicesJson();
            logger.LogDebug("Fetched electronic information services from database and writing to file.");
            WriteToFile(FakePtvJsonFileNames.WebpageChannels, jsonList);
        }

        /// <summary>
        /// Writes the JSON data to named file.
        /// </summary>
        /// <param name="filename">name of the file where the data is written</param>
        /// <param name="jsonData">JSON data object</param>
        /// <exception cref="System.ArgumentException"><i>filename</i> is null or an empty string</exception>
        /// <exception cref="System.ArgumentNullException"><i>jsonData</i> is a null reference</exception>
        private static void WriteToFile(string filename, object jsonData)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentException("The filename cannot be an empty string or a null reference.", nameof(filename));
            }

            if (jsonData == null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }

            // convert data to json
            var json = JsonConvert.SerializeObject(jsonData, Formatting.Indented);
            // overwrite the file always
            File.WriteAllText(FakePtvJsonFileNames.GetFilePath(filename), json);
        }
    }
}
