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

using Microsoft.Extensions.Configuration;
using PTV.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PTV.Framework.Extensions
{
    /// <summary>
    /// Class holder for app configuration loaded from appsettings.json file
    /// </summary>
    public class ApplicationConfiguration
    {
        private IConfiguration configuration;
        private readonly AWSSecrets awsSecrets;

        public ApplicationConfiguration(IConfiguration configuration)
        {
            this.configuration = configuration;
            awsSecrets = new AWSSecrets();
        }

        public IConfiguration RawConfiguration => configuration;

        public string GetAwsConnectionString(AwsDbConnectionStringEnum awsDbConnectionStringEnum)
        {
            switch (awsDbConnectionStringEnum)
            {
                case AwsDbConnectionStringEnum.ConnectionString:
                    return GetConnectionString();

                case AwsDbConnectionStringEnum.StsConnectionString:
                    return GetStsConnectionString();

                case AwsDbConnectionStringEnum.QuartzConnection:
                    return GetQuartzConnectionString();

                default:
                    throw new ArgumentOutOfRangeException(nameof(awsDbConnectionStringEnum), awsDbConnectionStringEnum, null);
            }
        }

        private string GetConnectionString()
        {
            var connectionString = configuration["Data:DefaultConnection:ConnectionString"];
            var connection = connectionString
                .Replace("%DB_NAME%", awsSecrets?.DbName)
                .Replace("%DB_USERNAME%", awsSecrets?.DbUserName)
                .Replace("%DB_PASSWORD%", awsSecrets?.DbPassword);
            return connection;
        }

        private string GetStsConnectionString()
        {
            var connectionString = configuration["Data:DefaultConnection:StsConnectionString"];
            var connection = connectionString
                .Replace("%DB_STS_NAME%", awsSecrets?.DbStsName)
                .Replace("%DB_STS_USERNAME%", awsSecrets?.DbStsUserName)
                .Replace("%DB_STS_PASSWORD%", awsSecrets?.DbStsPassword);
            return connection;
        }

        private string GetQuartzConnectionString()
        {
            var connectionString = configuration["ConnectionStrings:QuartzConnection"];
            var connection = connectionString
                .Replace("%DB_NAME%", awsSecrets?.DbName)
                .Replace("%DB_USERNAME%", awsSecrets?.DbUserName)
                .Replace("%DB_PASSWORD%", awsSecrets?.DbPassword);
            return connection;
        }

        public IConfigurationSection GetAwsConfiguration(string section)
        {
            if (!Enum.TryParse(section, true, out AwsConfigurationEnum awsSection))
            {
                return configuration.GetSection(section);
            }
            return GetAwsConfiguration(awsSection);
        }

        public IConfigurationSection GetAwsConfiguration(AwsConfigurationEnum awsConfigurationEnum)
        {
            switch (awsConfigurationEnum)
            {
                case AwsConfigurationEnum.MapService:
                    return GetMapConfiguration();

                case AwsConfigurationEnum.AccessibilityRegister:
                    return GetARConfiguration();

                case AwsConfigurationEnum.AccessibilityRegisterConfiguration:
                    return GetARSchedulerConfiguration();

                case AwsConfigurationEnum.QualityAgentService:
                    return GetQAConfiguration();

                case AwsConfigurationEnum.EmailNotifyConfiguration:
                    return GetENConfiguration();

                case AwsConfigurationEnum.LinkValidatorConfiguration:
                    return GetLVConfiguration();

                case AwsConfigurationEnum.RelayingTranslationOrderConfiguration:
                    return GetRelayingTranslationOrderConfuguration();

                case AwsConfigurationEnum.TransifexLocalizationConfiguration:
                    return GetTransifexLocalizationConfiguration();

                case AwsConfigurationEnum.TranslationOrderConfiguration:
                    return GetTranslationOrderConfiguration();

                case AwsConfigurationEnum.FintoConfiguration:
                    return GetFintoConfiguration();

                case AwsConfigurationEnum.KapaConfiguration:
                    return GetKapaConfiguration();

                case AwsConfigurationEnum.S3:
                    return GetS3Configuration();

                case AwsConfigurationEnum.LogConfiguration:
                    return GetLogConfiguration();

                case AwsConfigurationEnum.YPlatformConfiguration:
                    return GetYPlatformConfiguration();

                case AwsConfigurationEnum.PharmacyImportConfiguration:
                    return GetPharmaConfiguration();

                default:
                    throw new ArgumentOutOfRangeException(nameof(awsConfigurationEnum), awsConfigurationEnum, null);
            }
        }

        private IConfigurationSection GetPharmaConfiguration()
        {
            var result = configuration.GetSection("PharmacyImportConfiguration");
            result["ApiUserName"] = result["ApiUserName"].Replace("%PHARMA_USER_NAME%", awsSecrets?.PharmaUserName);
            result["ApiPassword"] = result["ApiPassword"].Replace("%PHARMA_PASSWORD%", awsSecrets?.PharmaPassword);
            result["OrganizationId"] = result["OrganizationId"].Replace("%PHARMA_ORGANIZATION%", awsSecrets?.PharmaOrganization);
            result["ServiceId"] = result["ServiceId"].Replace("%PHARMA_SERVICE%", awsSecrets?.PharmaService);
            return result;
        }

        private IConfigurationSection GetLogConfiguration()
        {
            var result = configuration.GetSection("Data:AWS:Log");
            result["AccessKey"] = result["AccessKey"].Replace("%LOG_ACCESS_KEY%", awsSecrets?.LogAccessKey);
            result["SecretKey"] = result["SecretKey"].Replace("%LOG_SECRET_KEY%", awsSecrets?.LogSecretKey);
            return result;
        }

        private IConfigurationSection GetS3Configuration()
        {
            var result = configuration.GetSection("Data:AWS:S3");
            result["AccessKey"] = result["AccessKey"].Replace("%S3_ACCESS_KEY%", awsSecrets?.S3AccessKey);
            result["SecretKey"] = result["SecretKey"].Replace("%S3_SECRET_KEY%", awsSecrets?.S3SecretKey);
            return result;
        }

        private IConfigurationSection GetMapConfiguration()
        {
            var result = configuration.GetSection("Data:MapService");
            result["UserName"] = result["UserName"].Replace("%MAP_USERNAME%", awsSecrets?.MapUserName);
            result["Password"] = result["Password"].Replace("%MAP_PASSWORD%", awsSecrets?.MapPassword);
            return result;
        }

        private IConfigurationSection GetARConfiguration()
        {
            var result = configuration.GetSection("Data:AccessibilityRegister");
            result["SystemId"] = result["SystemId"].Replace("%AR_SYSTEM_ID%", awsSecrets?.ArSystemId);
            result["ChecksumSecret"] = result["ChecksumSecret"].Replace("%AR_CHECKSUM_SECRET%", awsSecrets?.ArChecksumSecret);
            return result;
        }

        private IConfigurationSection GetARSchedulerConfiguration()
        {
            var result = configuration.GetSection("AccessibilityRegisterConfiguration");
            result["SystemId"] = result["SystemId"].Replace("%AR_SYSTEM_ID%", awsSecrets?.ArSystemId);
            return result;
        }

        private IConfigurationSection GetQAConfiguration()
        {
            var result = configuration.GetSection("Data:QualityAgentService");
            result["UserName"] = result["UserName"].Replace("%QA_USERNAME%", awsSecrets?.QaUserName);
            result["Password"] = result["Password"].Replace("%QA_PASSWORD%", awsSecrets?.QaPassword);
            return result;
        }

        private IConfigurationSection GetENConfiguration()
        {
            var result = configuration.GetSection("EmailNotifyConfiguration");
            result["UserName"] = result["UserName"].Replace("%EN_USERNAME%", awsSecrets?.EnUserName);
            result["Password"] = result["Password"].Replace("%EN_PASSWORD%", awsSecrets?.EnPassword);
            return result;
        }

        private IConfigurationSection GetLVConfiguration()
        {
            var result = configuration.GetSection("LinkValidatorConfiguration");
            result["ApiKey"] = result["ApiKey"].Replace("%FINTO_API_KEY%", awsSecrets?.FintoApiKey);
            return result;
        }

        private IConfigurationSection GetTranslationOrderConfiguration()
        {
            var result = configuration.GetSection("TranslationOrderConfiguration");
            result["UserName"] = result["UserName"].Replace("%TRANS_ORDER_USERNAME%", awsSecrets?.TransOrderUsername);
            result["Password"] = result["Password"].Replace("%TRANS_ORDER_PASSWORD%", awsSecrets?.TransOrderPassword);
            return result;
        }

        private IConfigurationSection GetRelayingTranslationOrderConfuguration()
        {
            var result = configuration.GetSection("RelayingTranslationOrderConfiguration");
            result["UserName"] = result["UserName"].Replace("%TRANS_ORDER_USERNAME%", awsSecrets?.TransOrderUsername);
            result["Password"] = result["Password"].Replace("%TRANS_ORDER_PASSWORD%", awsSecrets?.TransOrderPassword);
            return result;
        }

        private IConfigurationSection GetTransifexLocalizationConfiguration()
        {
            var result = configuration.GetSection("TransifexLocalizationConfiguration");
            result["authorization"] = result["authorization"].Replace("%TRANSIFEX_AUTHORIZATION%", awsSecrets?.TransifexAuthorization);
            return result;
        }

        private IConfigurationSection GetFintoConfiguration()
        {
            var result = configuration.GetSection("FintoConfiguration");
            result["ApiKey"] = result["ApiKey"].Replace("%FINTO_API_KEY%", awsSecrets?.FintoApiKey);
            return result;
        }

        private IConfigurationSection GetKapaConfiguration()
        {
            var result = configuration.GetSection("KapaConfiguration");
            result["ApiKey"] = result["ApiKey"].Replace("%KAPA_API_KEY%", awsSecrets?.KapaApiKey);
            return result;
        }

        private IConfigurationSection GetYPlatformConfiguration()
        {
            var result = configuration.GetSection("YPlatformConfiguration");
            result["Token"] = result["Token"].Replace("%Y_PLATFORM_TOKEN%", awsSecrets?.YPlatformToken);
            return result;
        }

        /// <summary>
        /// URL address on Kestrel is listening on
        /// </summary>
        public string BindingUrl { get; set; }

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
        /// String containing build number
        /// </summary>
        /// <returns></returns>
        public string GetReleaseVersion()
        {
            return configuration["ReleaseNumber"];
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
        /// Default Language
        /// </summary>
        /// <returns></returns>
        public string GetDefaultLanguage()
        {
            var defaultLanguage = configuration["ApplicationConfiguration:Language"];
            return defaultLanguage?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Default Language
        /// </summary>
        /// <returns></returns>
        public string GetDefaultCountryCode()
        {
            var countryCode = configuration["ApplicationConfiguration:CountryCode"];
            return countryCode?.Trim() ?? string.Empty;
        }

        public string GetQueryCollation()
        {
            var queryCollation = configuration["ApplicationConfiguration:QueryCollation"];
            return queryCollation?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// RedirectUrl
        /// </summary>
        /// <returns></returns>
        public string GetRedirectUrl()
        {
            return configuration["ApplicationConfiguration:RedirectUrl"];
        }

        /// <summary>
        /// PAHARedirectUrl
        /// </summary>
        /// <returns></returns>
        public string GetPAHARedirectUrl()
        {
            return configuration["ApplicationConfiguration:PAHARedirectUrl"];
        }

        /// <summary>
        /// Gets Menu links
        /// </summary>
        /// <returns></returns>
        public MenuLinksConfiguration GetMenuLinks()
        {
            var links = new MenuLinksConfiguration();
            configuration.GetSection("ApplicationConfiguration:MenuLinks").Bind(links);
            return links;
        }

       

        private bool? fakeAuthorizationConfiguration;

        /// <summary>
        /// True if fake authorization of token should be used
        /// </summary>
        public bool FakeAuthorization
        {
            get
            {
                if (fakeAuthorizationConfiguration == null)
                {
                    fakeAuthorizationConfiguration = configuration["ApplicationConfiguration:FakeAuthorization"]?.ToLower() == "true";
                }
                return fakeAuthorizationConfiguration ?? false;
            }
            set => fakeAuthorizationConfiguration = value;
        }

        private bool? usePAHAAuthenticationConfiguration;

        /// <summary>
        /// True if fake Paha authentication is used
        /// </summary>
        public bool UsePAHAAuthentication
        {
            get
            {
                if (usePAHAAuthenticationConfiguration == null)
                {
                    usePAHAAuthenticationConfiguration = configuration["ApplicationConfiguration:UsePAHAAuthentication"]?.ToLower() == "true";
                }
                return usePAHAAuthenticationConfiguration ?? false;
            }
            set => usePAHAAuthenticationConfiguration = value;
        }

        /// <summary>
        /// Token service url
        /// </summary>
        /// <returns></returns>
        public string GetTokenServiceUrl()
        {
            return configuration["ApplicationConfiguration:TokenServiceUrl"];
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
        public EnvironmentTypeEnum GetEnvironmentType()
        {
            EnvironmentTypeEnum eType;
            Enum.TryParse(configuration["EnvironmentType"], out eType);
            return eType;
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
        /// String as QuartzApi url address
        /// </summary>
        /// <returns></returns>
        public string GetQuartzApiUrl()
        {
            return configuration["Data:QuartzApi:Uri"];
        }

        /// <summary>
        /// String as QuartzApi url address
        /// </summary>
        /// <returns></returns>
        public IConfigurationSection GetQuartzApiProxySetting()
        {
            return configuration.GetSection("Data:QuartzApi:ProxyServerSettings");
        }

        /// <summary>
        /// Returns user name for map service
        /// </summary>
        /// <returns>user name</returns>
        public string GetMapUserName()
        {
            return GetAwsConfiguration(AwsConfigurationEnum.MapService)["UserName"];
        }

        /// <summary>
        /// Returns password for map service
        /// </summary>
        /// <returns>password</returns>
        public string GetMapPassword()
        {
            return GetAwsConfiguration(AwsConfigurationEnum.MapService)["Password"];
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
        /// Returns url of annotation service
        /// </summary>
        /// <returns>url</returns>
        public string GetAnnotationServiceUri()
        {
            return configuration["Data:AnnotationService:Uri"];
        }

        /// <summary>
        /// Returns configuration for sending emails via the PaHa notification gateway.
        /// </summary>
        /// <returns></returns>
        public virtual EmailNotificationConfiguration GetEmailConfiguration()
        {
            var emailConfiguration = new EmailNotificationConfiguration();
            GetAwsConfiguration(AwsConfigurationEnum.EmailNotifyConfiguration).Bind(emailConfiguration);
            return emailConfiguration;
        }

        /// <summary>
        /// Returns configuration for validating links via SEVI endpoint.
        /// </summary>
        /// <returns></returns>
        public virtual LinkValidatorConfiguration GetLinkValidatorConfiguration()
        {
            var linkValidatorConfiguration = new LinkValidatorConfiguration();
            GetAwsConfiguration(AwsConfigurationEnum.LinkValidatorConfiguration).Bind(linkValidatorConfiguration);
            return linkValidatorConfiguration;
        }

        public virtual PharmacyImportConfiguration GetPharmacyImportConfiguration()
        {
            var pharmacyImportConfiguration = new PharmacyImportConfiguration();
            GetAwsConfiguration(AwsConfigurationEnum.PharmacyImportConfiguration).Bind(pharmacyImportConfiguration);
            return pharmacyImportConfiguration;
        }

        /// <summary>
        /// Returns configuration for generating holiday dates.
        /// </summary>
        /// <returns></returns>
        public virtual HolidayConfiguration GetHolidayConfiguration()
        {
            var holidayConfiguration = new HolidayConfiguration();
            configuration.GetSection("HolidayConfiguration").Bind(holidayConfiguration);
            return holidayConfiguration;
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

        public int LogArchiveThreshold
        {
            get
            {
                if (logArchiveThreshold == null)
                {
                    if (int.TryParse(configuration["LogArchiveThreshold"], out var threshold))
                    {
                        logArchiveThreshold = threshold;
                    }
                }
                return logArchiveThreshold ?? 20;
            }
            set
            {
                logArchiveThreshold = value;
            }
        }

        private bool? logSavedEntitiesConfiguration;
        private int? logArchiveThreshold;

        /// <summary>
        /// How many minutes the open id nonce cookie should be valid (until it gets automatically removed by browser).
        /// </summary>
        /// <returns></returns>
        public int GetOpenIdNonceLifetimeMinutes()
        {
            // default lifetime is 60 minutes by framework
            int lifetime = 60;

            string cv = configuration["OpenIdNonceLifetime"];

            if (string.IsNullOrWhiteSpace(cv))
            {
                return lifetime;
            }

            int.TryParse(cv, out lifetime);

            return lifetime;
        }

        /// <summary>
        /// Max level of organisation hierarchy
        /// </summary>
        public int MaxOrganizationHierarchyLevel => int.TryParse(configuration["MaxOrganizationHierarchyLevel"], out var result) ? result : 7;
    }

    public class MenuLinksConfiguration
    {
        public string OwnProfile { get; set; }
        public string OwnOrganization { get; set; }
        public string Administration { get; set; }
        public string Statistics { get; set; }
        public string Support { get; set; }
        public string ServiceManagement { get; set; }
    }

    public class ServiceConnectionConfiguration
    {
        public string Uri { get; set; }
        public bool IgnoreCertificate { get; set; }
        public string Schema { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class AnnotationServiceConfiguration : ServiceConnectionConfiguration { }

    public class MapServiceConfiguration : ServiceConnectionConfiguration { }

    public class TestAccessUrlConfiguration : ServiceConnectionConfiguration { }

    public class QualityAgentConfiguration : ServiceConnectionConfiguration
    {
        public Dictionary<string, string> Profiles { get; set; }
        public List<string> Languages { get; set; }
    }
}