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
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework.Enums;
using Xunit;

namespace PTV.Framework.Tests.Paha
{
    public class PahaTokenProcessorTest
    {
        private const string exampleToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE1ODc0NjkwODgsInNjb3BlIjoibG9naW4iLCJjbGllbnRfaWQiOiJiMTI0M2RiZS04YzI3LTRlNDgtYmQyMy1jZGVjOWI2NmUwZDciLCJ1c2VybmFtZSI6InB0di50ZXN0aWtheXR0YWphQHBhaGEudGVzdCIsImZpcnN0TmFtZSI6IlRlaWphIiwibGFzdE5hbWUiOiJUZXN0Z3JhbiIsImVtYWlsIjoicHR2LnRlc3Rpa2F5dHRhamFAcGFoYS50ZXN0Iiwib3JnYW5pemF0aW9ucyI6W3siaWQiOiJkNDVjNThkNC00YThjLTQ2NzAtOGM1OS0wZTVmOWUyZGVkOTkiLCJuYW1lIjoiUFRWIE91bHUgVGVzdGlvcmdhbmlzYWF0aW8gIiwic3ViT3JnIjpmYWxzZX0seyJpZCI6Ijc3MzE5MmU0LTEyMWYtNDBhZC04ZjBlLTI5OWQxMzQ5NGJmNyIsIm5hbWUiOiJQVFYgS2VsYSBUZXN0aW9yZ2FuaXNhYXRpbyAiLCJzdWJPcmciOmZhbHNlfSx7ImlkIjoiYzQ5Yjc5NzctOWFkYi00NGY3LWIwNTgtOGRkMDIxM2RhYTUxIiwibmFtZSI6IlBhbHZlbHV0aWV0b3ZhcmFudG8gKFRlc3RhdXMpICIsInN1Yk9yZyI6ZmFsc2UsInJvbGVzIjpbIlBUVl9VU0VSIl19XSwicm9sZXMiOnsic2VtYSI6WyJhZG1pbiJdLCJwdHYiOlsidXNlciJdfSwiaWQiOiJiMTI0M2RiZS04YzI3LTRlNDgtYmQyMy1jZGVjOWI2NmUwZDciLCJhY3RpdmVPcmdhbml6YXRpb25JZCI6ImM0OWI3OTc3LTlhZGItNDRmNy1iMDU4LThkZDAyMTNkYWE1MSIsImFwaVVzZXJPcmdhbml6YXRpb24iOm51bGwsImlhdCI6MTU4NzM4MjY4N30.aHjK1Sv-rdHvIpro2FXhjwmzxO38lfd3zr8ZU3q4IfFaDXc5nYkkn6oC5ww0EITv9pfp272XLmcyAWrISPHxOhucdnR3LrP68K0UmjlraxbOzuI6HA3FWZRj0b1pVj7MOoroN09sLo2iRS9HllDvlxTgzJt62TJ_zfdmiLHB8Uo2roSYXFP0145KOhmRKlV7eFZZCESiPDJT4FNtOal3SFE1ZzyoBhkj977ZqTIjrl_KDuSOUkPzv993a-lkIZtlR-vDnUEjtKyTtxVkEK8juZWQgm-0d69egXsfNCPZxjCAVBMseaHpfOefeKIY--Ey9KN5MYn77_u8k9wnU98NCg";
        
        [Fact]
        public void ProcessToken()
        {
            var headerDictionary = new HeaderDictionary
            {
                { "Authorization", $"bearer {exampleToken}" }
            };
            
            
            var userAccessRightsCache = new UserAccessRightsCache();
            userAccessRightsCache.Init(new Dictionary<string, VmUserAccessRights>
            {
                {"PTV_USER", new VmUserAccessRights
                    {
                        AccessRights = AccessRightEnum.UiAppWrite,
                        GroupCode = "PTV_USER",
                        UserRole = UserRoleEnum.Pete
                    }
                }
            });
            
            var mockRequest = new Mock<HttpRequest>();
            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpAccessor = new Mock<IHttpContextAccessor>();
            var mockLogger = new Mock<ILogger<PahaTokenProcessor>>();
            var mockOrganizationTreeDataCache = new Mock<IOrganizationTreeDataCache>();

            mockRequest.Setup(x => x.Headers).Returns(headerDictionary);
            mockHttpContext.Setup(x => x.Request).Returns(mockRequest.Object);
            mockHttpAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);
            
            var pahaTokenProcessor = new PahaTokenProcessor(
                mockHttpAccessor.Object, 
                userAccessRightsCache, 
                mockLogger.Object, 
                mockOrganizationTreeDataCache.Object);

            pahaTokenProcessor.ProcessToken(exampleToken);
            
            Assert.False(pahaTokenProcessor.InvalidToken);
        }
    }
}
