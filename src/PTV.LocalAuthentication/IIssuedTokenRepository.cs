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
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Framework;

namespace PTV.LocalAuthentication
{
    public interface IIssuedTokenRepository
    {
        IssuedToken Add(IssuedToken toAdd);
        bool Exists(IssuedToken token);
        bool Exists(string hash);
        void Remove(IssuedToken token);
        void Remove(string hash);
        IssuedToken Get(string hash);
        void RemoveExpired();
    }

    [RegisterService(typeof(IIssuedTokenRepository), RegisterType.Scope)]
    public class IssuedTokenRepository : IIssuedTokenRepository
    {
        private readonly StsDbContext stsContext;

        public IssuedTokenRepository(StsDbContext context)
        {
            this.stsContext = context;
        }


        public IssuedToken Add(IssuedToken toAdd)
        {
            this.stsContext.IssuedTokens.Add(toAdd);
            return toAdd;
        }

        public bool Exists(IssuedToken token)
        {
            return Exists(token.Hash);
        }

        public bool Exists(string hash)
        {
            return this.stsContext.IssuedTokens.Any(i => i.Hash == hash);
        }

        public void Remove(IssuedToken token)
        {
            this.stsContext.Remove(token);
        }

        public void Remove(string hash)
        {
            var toDelete = this.stsContext.IssuedTokens.FirstOrDefault(i => i.Hash == hash);
            if (toDelete != null) this.stsContext.IssuedTokens.Remove(toDelete);
        }

        public IssuedToken Get(string hash)
        {
            var currentTime = DateTime.UtcNow;
            var dbToken = this.stsContext.IssuedTokens.FirstOrDefault(i => i.Hash == hash);
            if (dbToken != null)
            {
                if (dbToken.ValidTo > currentTime)
                {
                    return dbToken;
                }
                else
                {
                    this.stsContext.IssuedTokens.Remove(dbToken);
                }
            }
            return null;
        }

        public void RemoveExpired()
        {
            var currentTime = DateTime.UtcNow;
            var toDelete = this.stsContext.IssuedTokens.Where(i => i.ValidTo <= currentTime);
            this.stsContext.IssuedTokens.RemoveRange(toDelete);
        }
    }
}
