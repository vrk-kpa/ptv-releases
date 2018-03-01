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
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.Model.Models;
using PTV.Framework;
using System.Security.Cryptography;
using PTV.Domain.Model.Models;

namespace PTV.Database.DataAccess.Services.V2
{
    [RegisterService(typeof(IAccessibilityRegisterService), RegisterType.Transient)]
    internal class AccessibilityRegisterService : IAccessibilityRegisterService
    {
        private readonly AccessibilityRegisterSettings accessibilityRegisterSettings;
        private readonly IContextManager contextManager;
        private readonly IUserIdentification userIdentification;

        public AccessibilityRegisterService(
            IOptions<AccessibilityRegisterSettings> accessibilityRegisterSettings,
            IContextManager contextManager,
            IUserIdentification userIdentification) 
        {
            this.accessibilityRegisterSettings = accessibilityRegisterSettings.Value;
            this.contextManager = contextManager;
            this.userIdentification = userIdentification;
        }

        public string GenerateUrl(VmAccessibilityRegisterUrl accessibilityRegisterUrl)
        {
            var checksumSecret = accessibilityRegisterSettings.ChecksumSecret;
            var systemId = accessibilityRegisterSettings.SystemId;
            var servicePointId = accessibilityRegisterUrl.ServiceChannelId;
            var user = userIdentification.UserName;
            var validUntil = DateTime.Today.AddDays(accessibilityRegisterSettings.ValidUntilDays).ToString("s");

            var streetAddress = accessibilityRegisterUrl.StreetAddress;
            var postOffice = accessibilityRegisterUrl.PostOffice;
            var serviceName = accessibilityRegisterUrl.ServiceName;
            var northing = accessibilityRegisterUrl.Northing;
            var easting = accessibilityRegisterUrl.Esting;
            
            var values = checksumSecret + systemId + servicePointId + user + validUntil + streetAddress + postOffice + serviceName + northing + easting;
            var hash = Sha256(values);

            return string.Format(accessibilityRegisterSettings.BaseUrl,
                systemId,
                servicePointId,
                WebUtility.UrlEncode(user),
                WebUtility.UrlEncode(validUntil),
                WebUtility.UrlEncode(serviceName),
                WebUtility.UrlEncode(streetAddress),
                WebUtility.UrlEncode(postOffice),
                northing,
                easting,
                hash.ToUpper());
        }

        public void SaveAccessibilityRegister(Guid serviceChannelId)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var ar = new AccessibilityRegister
                {
                    Id = Guid.NewGuid(),
                    ServiceChannelId = serviceChannelId,
                };

                var arRepository = unitOfWork.CreateRepository<IAccessibilityRegisterRepository>();
                arRepository.Add(ar);
                unitOfWork.Save(parentEntity: ar);
            });
        }

        private static string Sha256(string toHash)
        {
            using (var algorithm = SHA256.Create())
            {
                var hash = string.Empty;
                var crypto = algorithm.ComputeHash(Encoding.ASCII.GetBytes(toHash));
                return crypto.Aggregate(hash, (current, theByte) => current + theByte.ToString("x2"));
            }
        }
    }
}
