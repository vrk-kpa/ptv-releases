import _ from './env.mjs';
import { decode } from 'jsonwebtoken';
import fetch from 'node-fetch';

export const baseUrlIn = process.env.BASE_URL_IN;
export const baseUrlOut = process.env.BASE_URL_OUT;
export const organizationId = process.env.ORGANIZATION_ID;
export const accessToken = process.env.ACCESS_TOKEN;

export const showProgress = true;

var enumTypes = undefined;

if (!process.env.ORGANIZATION_ID || !process.env.BASE_URL_IN || !process.env.BASE_URL_OUT || !process.env.ACCESS_TOKEN) {
  console.error('Configure .env -file! Instructions in README.md');
  process.exit(1);
}

export const provisionTypeMapping = { // TODO: are these per environment? figure out how to get
  PurchaseServices: '0c4f9745-d316-4844-05ef-08d3c60ad657',
  ProcuredServices: '0c4f9745-d316-4844-05ef-08d3c60ad657',
  Other: '473ae05e-dd64-486d-05f0-08d3c60ad657',
  null: '473ae05e-dd64-486d-05f0-08d3c60ad657',
  SelfProduced: '9e3713c0-93d9-4fed-05ed-08d3c60ad657',
  SelfProducedServices: '9e3713c0-93d9-4fed-05ed-08d3c60ad657',
}

/**
 * Get languages, types, etc.
 */
export const getEnumTypes = async () => {
  if (!enumTypes) {
    const { activeOrganizationId } = decode(accessToken); // Extract organization id from jwt
    const url = `${baseUrlOut}/common/GetEnumTypes`;

    const requestOptions = {
      method: 'POST',
      body: JSON.stringify({userOrganization: activeOrganizationId}),
      redirect: 'follow',
      headers: {
        'Content-Type': 'application/json',
        'Accept-Encoding': 'gzip, deflate, br',
        authorization: `Bearer ${accessToken}`,
      },
    };

    if (showProgress) {
      console.log('🧠 Reading enumTypes configuration');
    }

    let data = await fetch(`${url}`, requestOptions)
      .then(response => {
        return response.text();
      })
      .catch(error => console.log('error', error));

      enumTypes = (JSON.parse(data)).data;
  }

  return enumTypes;
}

export const getRandomString = (length) => {
  return Array(length).fill().map(() => "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".charAt(Math.random() * 62)).join("")
}
