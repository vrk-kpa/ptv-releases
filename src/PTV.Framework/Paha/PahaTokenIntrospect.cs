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
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PTV.Framework.Enums;

namespace PTV.Framework.Paha
{
    public class PahaTokenIntrospect
    {
        public PahaTokenIntrospect(IPahaToken pahaToken)
        {
            var oldToken = pahaToken as PahaToken1;

            Active = pahaToken.Active;
            FirstName = pahaToken.FirstName;
            LastName = pahaToken.LastName;
            Hetu = string.Empty;
            Client_id = oldToken?.Client_id.ToString() ?? string.Empty;
            Exp = pahaToken.Exp;
            Jti = pahaToken.Jti;
            Email = pahaToken.Email;
            UserName = oldToken?.UserName ?? pahaToken.Email;
            Id = pahaToken.Id;
            ApiUserOrganization = oldToken?.ApiUserOrganization ?? pahaToken.ActiveOrganizationId;
            ActiveOrganizationId = pahaToken.ActiveOrganizationId;
            AllOrganizations = oldToken?.Organizations.ToDictionary(x => x.Id) ??  new Dictionary<Guid, PahaOrganizationDto>()
            {
                {
                    pahaToken.ActiveOrganizationId, 
                    new PahaOrganizationDto(new PahaOrganizationInternalDto
                    {
                        Id = pahaToken.ActiveOrganizationId,
                        Name =  pahaToken.ActiveOrganizationName
                    })
                }
            };
            GlobalPtvRole = pahaToken.PtvRole;
        }

        public UserAccessRightsGroupEnum GlobalPtvRole { get; set; }

        public Dictionary<Guid, PahaOrganizationDto> AllOrganizations { get; set; }

        public Guid ActiveOrganizationId { get; set; }

        public Guid ApiUserOrganization { get; set; }

        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public Guid? Jti { get; set; }

        public string Client_id { get; set; }

        public string Hetu { get; set; }

        public long Exp { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public bool Active { get; set; }

        public string Organizations => JsonConvert.SerializeObject(AllOrganizations.Values.Select(i => new PahaOrganizationInternalDto(i)).ToList());
        public string Roles => JsonConvert.SerializeObject(new PahaGlobalRolesDto {Vrk_Ptv = new List<string> {GlobalPtvRole.ToString()}});
    }
}
