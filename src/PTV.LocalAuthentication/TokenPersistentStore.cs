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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Framework;

namespace PTV.LocalAuthentication
{
    [RegisterService(typeof(ITokenStore), RegisterType.Singleton, "EnvironmentType=Prod")]
    internal class TokenPersistentStore : ITokenStore
    {
        private readonly IStsDataAccess stsDataAccess;

        public TokenPersistentStore(IStsDataAccess stsDataAccess)
        {
            this.stsDataAccess = stsDataAccess;
        }
        
        public string Add(TokenOutputData tokenData)
        {
            var created = DateTime.UtcNow;
            var hash = tokenData.AccessToken.GetSha256Hash();
            var id = hash.GetGuid();
            this.stsDataAccess.ExecuteWriter(unitOfWork =>
            {
                var tokenRep = unitOfWork.CreateRepository<IIssuedTokenRepository>();
                var oldOne = tokenRep.Get(hash);
                if (oldOne != null)
                {
                    oldOne.ValidTo = tokenData.ValidTo;
                    oldOne.Created = created;
                }
                else
                {
                    tokenRep.Add(new IssuedToken()
                    {
                        Created = created,
                        ValidTo = tokenData.ValidTo,
                        Hash = hash,
                        Id = id,
                        Token = tokenData.AccessToken
                    });
                }
                unitOfWork.Save(SaveMode.AllowAnonymous);
            });
            return hash;
        }

        public TokenOutputData Get(string hash)
        {
            return this.stsDataAccess.ExecuteReader(unitOfWork =>
            {
                var tokenRep = unitOfWork.CreateRepository<IIssuedTokenRepository>();
                var token = tokenRep.Get(hash);
                return token == null ? null : new TokenOutputData(token.Token, token.ValidTo);
            });
        }

        public void Remove(string hash)
        {
            this.stsDataAccess.ExecuteWriter(unitOfWork =>
            {
                unitOfWork.CreateRepository<IIssuedTokenRepository>().Remove(hash);
                unitOfWork.Save(SaveMode.AllowAnonymous);
            });
        }

        public void ClearExpired()
        {
            this.stsDataAccess.ExecuteWriter(unitOfWork =>
            {
                unitOfWork.CreateRepository<IIssuedTokenRepository>().RemoveExpired();
                unitOfWork.Save(SaveMode.AllowAnonymous);
            });
        }
    }
}