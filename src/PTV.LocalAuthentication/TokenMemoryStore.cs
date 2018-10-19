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
using System.Collections.Concurrent;
using System.Linq;
using PTV.Framework;

namespace PTV.LocalAuthentication
{
    [RegisterService(typeof(ITokenStore), RegisterType.Singleton, "EnvironmentType!Prod")]
    internal class TokenMemoryStore : ITokenStore
    {
        private class TokenStoreItem
        {
            public TokenStoreItem(TokenOutputData token, DateTime validTo, DateTime addedAt)
            {
                this.Token = token;
                this.ValidTo = validTo;
                this.AddedAt = addedAt;
            }
            public TokenOutputData Token { get; }
            public DateTime ValidTo { get; }
            public DateTime AddedAt { get; }
        }

        private static ConcurrentDictionary<string, TokenStoreItem> internalStore { get; } = new ConcurrentDictionary<string, TokenStoreItem>();
        
        public string Add(TokenOutputData tokenData)
        {
            var hash = tokenData.AccessToken.GetSha256Hash();
            internalStore[hash] = new TokenStoreItem(tokenData, tokenData.ValidTo, DateTime.UtcNow);
            return hash;
        }

        public TokenOutputData Get(string hash)
        {
            var storedToken =  internalStore.TryGetOrDefault(hash, null);
            if (storedToken == null)
            {
                return null;
            }

            if (storedToken.ValidTo < DateTime.UtcNow)
            {
                Remove(hash);
                return null;

            }
            return storedToken.Token;
        }

        public void Remove(string hash)
        {
            internalStore.TryRemove(hash, out TokenStoreItem removed);
        }

        public void ClearExpired()
        {
            var timeNow = DateTime.UtcNow;
            var expired =  internalStore.Where(i => i.Value.ValidTo < timeNow).Select(i => i.Key).ToList();
            expired.ForEach(Remove);
        }
    }
}