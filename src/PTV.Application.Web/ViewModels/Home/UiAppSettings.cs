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
using PTV.Domain.Model.Models.Security;
using PTV.Framework;
using PTV.Framework.Enums;

namespace PTV.Application.Web.ViewModels.Home
{
    public class UiAppSettings
    {
        public bool IsWebpackDisabled { get; set; }
        public string CustomApiUrl { get; set; }
        public EnvironmentTypeEnum EnvironmentType { get; set; }
        public string EnvironmentTypeText => EnvironmentType.ToString().ToLower();
        public bool IsBugReportAvailable => false; // (EnvironmentType & (EnvironmentTypeEnum.Dev | EnvironmentTypeEnum.Qa | EnvironmentTypeEnum.Test)) > 0;
        public string StsUrl { get; set; }
        public string AccessToken { get; set; }
        public string IdentityToken { get; set; }
        public string VersionPrefix { get; set; }
        public string Version { get; set; }
        public VmUserInfo UserInfo { get; set; }
        public MapDNSes MapDNSNames { get; set; }
    }
}
