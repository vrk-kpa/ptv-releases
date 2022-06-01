# Organisation transfer

Reads organisation, its services and its service channels from one environment and saves them to another environment via API.

Set base url, organisation and API authorization token in the `.env` -file.
Also copy `.npmrc` from PTV.Application.Web

Options which need to be configured in the environment file.

* BASE_URL_IN = source where to get the organization data
* BASE_URL_OUT = target where to put the organizaton data
* ORGANIZATION_ID = id of the source organization
* ACCESS_TOKEN = access token to target system API

To execute do `npm install` and then `npm run transfer`

Warning: This is NOT a backup tool, the conversion between the input and output is most certainly incomplete somehow.

