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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.Import;
using PTV.Database.DataAccess.Caches;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace PTV.Database.DataAccess.Translators.Authorization
{

    [RegisterService(typeof(ITranslator<AuthorizationEntryPoint, VmAuthEntryPoint>), RegisterType.Scope)]
    internal class AuthorizationTranslator : Translator<AuthorizationEntryPoint, VmAuthEntryPoint>
    {
        public AuthorizationTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmAuthEntryPoint TranslateEntityToVm(AuthorizationEntryPoint entity)
        {
            throw new NotSupportedException();
        }

        public override AuthorizationEntryPoint TranslateVmToEntity(VmAuthEntryPoint vModel)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(vModel.Token);
            var email = token.Claims.FirstOrDefault(c => c.Type.ToLowerInvariant() == "username" || c.Type.ToLowerInvariant() == "email")?.Value;
            var dateTime = DateTime.UtcNow;
            return CreateViewModelEntityDefinition<AuthorizationEntryPoint>(vModel)
                .UseDataContextCreate(input => true, output => output.Id, input => Guid.NewGuid())
                .AddNavigation(i => email, o => o.CreatedBy)
                .AddNavigation(i => email, o => o.ModifiedBy)
                .AddSimple(i => dateTime, o => o.Modified)
                .AddSimple(i => dateTime, o => o.Created)
                .AddNavigation(i => i.Token, o => o.Token).GetFinal();
        }
    }
}
