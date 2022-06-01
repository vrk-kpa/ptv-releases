using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace PTV.Application.Web
{
    class ProtocolValidator : OpenIdConnectProtocolValidator
    {
        protected override void ValidateState(OpenIdConnectProtocolValidationContext validationContext)
        {
        }
        protected override void ValidateAtHash(OpenIdConnectProtocolValidationContext validationContext)
        {

        }

        public override void ValidateTokenResponse(OpenIdConnectProtocolValidationContext validationContext)
        {

        }

        protected override void ValidateCHash(OpenIdConnectProtocolValidationContext validationContext)
        {

        }

        protected override void ValidateIdToken(OpenIdConnectProtocolValidationContext validationContext)
        {

        }

        public override void ValidateAuthenticationResponse(OpenIdConnectProtocolValidationContext validationContext)
        {

        }
    }
}