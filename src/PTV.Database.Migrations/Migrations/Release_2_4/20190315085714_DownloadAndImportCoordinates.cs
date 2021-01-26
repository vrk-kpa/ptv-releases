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
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Extensions;

namespace PTV.Database.Migrations.Migrations.Release_2_4
{
    public partial class DownloadAndImportCoordinates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            this.AddMigrationAction(serviceProvider =>
            {
                var service = serviceProvider.GetService<IPostalCodeCoordinatesService>();
                var contextManager = serviceProvider.GetService<IContextManager>();
                var postalCodeCoordinatesSettings = new PostalCodeCoordinatesSettings();
                var applicationConfiguration = serviceProvider.GetService<ApplicationConfiguration>();
                applicationConfiguration.RawConfiguration.GetSection("PostalCodeCoordinatesSettings").Bind(postalCodeCoordinatesSettings);

                if (postalCodeCoordinatesSettings.DownloadOneByOne)
                {
                    DownloadOneByOne(postalCodeCoordinatesSettings, service, contextManager);
                }
                else
                {
                    DownloadByBatches(postalCodeCoordinatesSettings, service, contextManager);
                }
            });
        }

        private void DownloadOneByOne(PostalCodeCoordinatesSettings postalCodeCoordinatesSettings, IPostalCodeCoordinatesService service, IContextManager contextManager)
        {
            var postalCodes = contextManager.ExecuteReader(unitOfWork => unitOfWork
                .CreateRepository<IPostalCodeRepository>()
                .All()
                .Select(postalCode => new VmPostalCode
                {
                    Id = postalCode.Id,
                    Code = postalCode.Code
                }).ToList());

            var batchCounter = 1;
            var totalBatches = postalCodes.Count / postalCodeCoordinatesSettings.BatchSize;

            postalCodes.Batch(postalCodeCoordinatesSettings.BatchSize).ForEach(batch =>
            {
                Console.WriteLine($"Processing batch {batchCounter} / {totalBatches}.");

                var batchCoordinates = Asyncs.HandleAsyncInSync(() => PtvHttpClient.UseAsync(async client =>
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Xml));

                    var response = await service.DownloadCoordinates(client, batch,
                        postalCodeCoordinatesSettings, new StringBuilder());
                    return response;
                }));
                contextManager.ExecuteWriter(unitOfWork => service.UpdateCoordinates(unitOfWork, batchCoordinates));

                batchCounter++;
            });
        }

        private void DownloadByBatches(PostalCodeCoordinatesSettings postalCodeCoordinatesSettings,
            IPostalCodeCoordinatesService service, IContextManager contextManager)
        {
            var batchStart = 0;
            var lastBatchSize = postalCodeCoordinatesSettings.BatchSize;

            while (lastBatchSize == postalCodeCoordinatesSettings.BatchSize)
            {
                Console.WriteLine($"Processing batch from {batchStart}");

                var batchCoordinates = Asyncs.HandleAsyncInSync(() => PtvHttpClient.UseAsync(async client =>
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Xml));

                    var response = await service.DownloadBatch(client, batchStart,
                        postalCodeCoordinatesSettings.BatchSize, postalCodeCoordinatesSettings, new StringBuilder());

                    return response;
                }));
                contextManager.ExecuteWriter(unitOfWork => service.UpdateCoordinates(unitOfWork, batchCoordinates));

                lastBatchSize = batchCoordinates.Count;
                batchStart += lastBatchSize;

                Console.WriteLine($"Downloaded {lastBatchSize} coordinates.");
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
