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
using Microsoft.AspNetCore.Http;
using System;
using PTV.Domain.Logic;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Application.Framework
{
    [RegisterService(typeof(IUserIdentification), RegisterType.Scope)]
    public class UserIdentification : IUserIdentification, IThreadUserInterface
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IPahaTokenAccessor pahaTokenAccessor;

        public string UserName => pahaTokenAccessor.UserName;
        public Guid UserOrganization => pahaTokenAccessor.ActiveOrganizationId;
        public int UserRole => (int) pahaTokenAccessor.UserRole;

        public UserIdentification(IHttpContextAccessor contextAccessor, IPahaTokenAccessor pahaTokenAccessor)
        {
            this.contextAccessor = contextAccessor;
            this.pahaTokenAccessor = pahaTokenAccessor;
        }

        void IThreadUserInterface.CopyBearerToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return;
            }
            if (this.contextAccessor.HttpContext == null)
            {
                this.contextAccessor.HttpContext = new DefaultHttpContext();
            }

            this.contextAccessor.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";
            this.pahaTokenAccessor.ProcessToken(token);
        }

        void IThreadUserInterface.CopyBearerToken(IHttpContextAccessor ctxAccessor)
        {
            if (ctxAccessor.HttpContext == null)
            {
                return;
            }
            var token = ctxAccessor.GetBearerToken();
            if (string.IsNullOrEmpty(token))
            {
                return;
            }
            if (this.contextAccessor.HttpContext == null)
            {
                this.contextAccessor.HttpContext = new DefaultHttpContext();
            }

            this.contextAccessor.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";
            this.pahaTokenAccessor.ProcessToken(token);
        }

        void IThreadUserInterface.SetBearerToken(string token)
        {
            if (this.contextAccessor.HttpContext == null)
            {
                this.contextAccessor.HttpContext = new DefaultHttpContext();
            }
            this.contextAccessor.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";
            this.pahaTokenAccessor.ProcessToken(token);
        }
    }
}
