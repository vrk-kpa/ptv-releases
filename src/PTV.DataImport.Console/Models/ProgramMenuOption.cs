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

namespace PTV.DataImport.ConsoleApp.Models
{
    public enum ProgramMenuOption
    {
        Exit = 0,
        /// <summary>
        /// Creates database, seeds system data and imports data.
        /// </summary>
        InitialPtvCreate,
        /// <summary>
        /// Creates database if it doesn't exist and applies migrations.
        /// </summary>
        CreateOrMigrateDatabase,
        /// <summary>
        /// Seeds system data to existing database.
        /// </summary>
        SeedSystemData,
        /// <summary>
        /// Creates an organization and a business for each municipality
        /// </summary>
        CreateMunicipalityOrganizations,
        /// <summary>
        /// Imports general descriptions to database after converting excel to json
        /// </summary>
        //ImportGeneralDescriptions,
        /// <summary>
        /// Import the fake PTV to PTV database.
        /// </summary>
        //ImportFakePtv,
        /// <summary>
        /// Dump fake PTV database to json files on local disk.
        /// </summary>
        DumpFakePtv,
        /// <summary>
        /// Validate Fake PTV JSON against model classes.
        /// </summary>
        ValidateFakePtvJson,
        /// <summary>
        /// Generates postal codes json
        /// </summary>
        CreatePostalCodesJson,
        /// <summary>
        /// Imports organization data
        /// </summary>
        ImportOrganizations,
        /// <summary>
        /// Update coordinates for existing address
        /// </summary>
        UpdateCoordinatesForAddress,
        /// <summary>
        /// Imports finto data
        /// </summary>
        ImportOrUpdateFinto,
        /// <summary>
        /// Downloads finto json from service views
        /// </summary>
        DownloadFinto,
        /// <summary>
        /// Creates or updates general description from JSON
        /// </summary>
        UpdateGeneralDescription,
        /// <summary>
        /// Check langauges of texts of main entities and fill in Language Availabilities, check publishing statuses and add proper Versioning info
        /// </summary>
        UpdateLanguageAvailabilitiesAndVersions,
        /// <summary>
        /// Check description field which are about 2500 characters and text type then convert to JSON
        /// </summary>
        UpdateTextDescription,

        /// <summary>
        /// Update general description - description to new background General description
        /// </summary>
        UpdateServiceDescriptionByGeneralDescription,

		/// <summary>
        ///
        /// </summary>
        FindCyclicOrganizations,

        /// <summary>
        ///
        /// </summary>
        DeleteOldData,

        /// <summary>
        /// Download and import digital authorizations
        /// </summary>
        DownloadAndImportDigitalAuthorizations,

        /// <summary>
        /// The fix finto merge
        /// </summary>
        FixFintoMerge,
        /// <summary>
        /// Download postal codes from code service and write to PostalCode.json file.
        /// </summary>
        DownloadPostalCodesFromCodeServiceTask
    }
}
