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

namespace PTV.TaskScheduler.Enums
{
    /// <summary>
    /// Job types
    /// </summary>
    public enum JobTypeEnum
    {
        /// <summary>
        /// Postal codes job
        /// </summary>
        PostalCodes,

        /// <summary>
        /// Municipality codes job
        /// </summary>
        MunicipalityCodes,

        /// <summary>
        /// Province codes job
        /// </summary>
        ProvinceCodes,

        /// <summary>
        /// Business region codes job
        /// </summary>
        BusinessRegionCodes,

        /// <summary>
        /// Hospital region codes job
        /// </summary>
       HospitalRegionCodes,

       /// <summary>
       /// Finto data - LifeEvents
       /// </summary>
       FintoDataLifeEvents,

       /// <summary>
       /// Finto data - ServiceClasses
       /// </summary>
       FintoDataServiceClasses,

       /// <summary>
       /// Finto data - ServiceClasses
       /// </summary>
       FintoDataIndustrialClasses,

       /// <summary>
       /// Finto data - TargetGroups
       /// </summary>
       FintoDataTargetGroups,

       /// <summary>
       /// Finto data - OntologyTerms
       /// </summary>
       FintoDataOntologyTerms,

       /// <summary>
       /// Digital authorization codes job
       /// </summary>
       DigitalAuthorizationCodes,

       /// <summary>
       /// Language codes job
       /// </summary>
       LanguageCodes,

        /// <summary>
        /// Send new translation orders.
        /// </summary>
        TranslationOrderSendNew,

        /// <summary>
        /// Processing data by state.
        /// </summary>
        TranslationOrderProcessingDataByState,

        /// <summary>
        /// Send again translation order.
        /// </summary>
        TranslationOrderSendAgain,

        /// <summary>
        /// Archive entities by expiration date
        /// </summary>
        ArchiveEntitiesByExpirationDate,

        /// <summary>
        /// Restore clean database
        /// </summary>
        RestoreCleanDatabase,

        /// <summary>
        /// Maintenance test user
        /// </summary>
        MaintenanceTestUsers,
        
        /// <summary>
        /// SOTE job
        /// </summary>
        Sote,
        
        /// <summary>
        /// MassTool job
        /// </summary>
        MassTool,
        
        /// <summary>
        /// Clear notification older than one month
        /// </summary>
        ClearNotifications,
        
        /// <summary>
        /// Update GeoServer views
        /// </summary>
        GeoServer,
        
        /// <summary>
        /// Download geo-data from CLS server
        /// </summary>
        StreetData

    }
}