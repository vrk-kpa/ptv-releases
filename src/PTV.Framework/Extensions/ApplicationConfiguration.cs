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
using Microsoft.Extensions.Configuration;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PTV.Framework.Extensions
{
    /// <summary>
    /// Class holder for app configuration loaded from appsettings.json file
    /// </summary>
    public class ApplicationConfiguration
    {
        private IConfigurationRoot configuration;

        public ApplicationConfiguration(IConfigurationRoot configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// URL address of STS server
        /// </summary>
        public string StsAddress => configuration["STS"];

        /// <summary>
        /// True/False if Webpack usage is disabled
        /// </summary>
        /// <returns></returns>
        public bool IsWebpackDisabled()
        {
            return configuration["DisableWebpack"] == "true";
        }

        /// <summary>
        /// Returns <see cref="Version"/> of web application
        /// </summary>
        /// <returns></returns>
        public Version GetVersion()
        {
            return Assembly.GetEntryAssembly().GetName().Version;
        }

        /// <summary>
        /// String containing build number
        /// </summary>
        /// <returns></returns>
        public string GetBuildNumber()
        {
            return configuration["BuildNumber"];
        }

        /// <summary>
        /// String containing prefix for version number
        /// </summary>
        /// <returns></returns>
        public string GetVersionPrefix()
        {
            return configuration["VersionPrefix"];
        }

        /// <summary>
        /// URL address of Api applicetion (UI-API)
        /// </summary>
        /// <returns></returns>
        public string GetApiUrl()
        {
            var url = configuration["ApiUrl"] ?? string.Empty;
            if (url.Length > 0 && !url.EndsWith("/"))
            {
                url += "/";
            }
            return url;
        }

        /// <summary>
        /// InternetConnectionAvaliable property is set
        /// </summary>
        /// <returns></returns>
        public bool IsInternetAvailable()
        {
            return configuration["InternetConnectionAvaliable"] == "true";
        }


        /// <summary>
        /// Get string containing version and build number of app
        /// </summary>
        /// <returns></returns>
        public string GetVersionWithBuildNumber()
        {
            Version version = GetVersion();
            string buildNumber = GetBuildNumber();
            if (!string.IsNullOrEmpty(buildNumber))
            {
                return $"{version.Major}.{version.Minor}.{version.Build}.{buildNumber}";
            }
            return version.ToString();
        }

        /// <summary>
        /// String as Enviroment Type
        /// </summary>
        /// <returns></returns>
        public string GetEnvironmentType()
        {
            return configuration["EnvironmentType"];
        }

        /// <summary>
        /// String as STS url address
        /// </summary>
        /// <returns></returns>
        public string GetStsUrl()
        {
            return configuration["STS"];
        }

        /// <summary>
        /// Returns user name for map service
        /// </summary>
        /// <returns>user name</returns>
        public string GetMapUserName()
        {
            return configuration["Data:MapService:UserName"];
        }

        /// <summary>
        /// Returns password for map service
        /// </summary>
        /// <returns>password</returns>
        public string GetMapPassword()
        {
            return configuration["Data:MapService:Password"];
        }

        /// <summary>
        /// Returns url of map service
        /// </summary>
        /// <returns>url</returns>
        public string GetMapServiceUri()
        {
            return configuration["Data:MapService:Uri"];
        }

        /// <summary>
        /// True or False if Entities types should be logged when saved
        /// </summary>
        public bool LogSavedEntities
        {
            get
            {
                if (logSavedEntitiesConfiguration == null)
                {
                    logSavedEntitiesConfiguration = configuration["LogSavedEntities"]?.ToLower() == "true";
                }
                return logSavedEntitiesConfiguration ?? false;
            }
            set
            {
                logSavedEntitiesConfiguration = value;
            }
        }

        private bool? logSavedEntitiesConfiguration;
    }
}
