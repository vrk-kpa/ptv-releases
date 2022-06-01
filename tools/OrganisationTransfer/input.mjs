import fetch from 'node-fetch';
import TokenBucketRateLimiter from './TokenBucketRateLimiter.mjs';
import { baseUrlIn, showProgress } from './configuration.mjs';

const tokenBucket = new TokenBucketRateLimiter({
  maxRequests: 10, // concurrent requests
  maxRequestWindowMS: 1000, // within time window
});

export const getOrganization = async (id) => {
  const url = `${baseUrlIn}/Organization/`;

  const requestOptions = {
    method: 'GET',
    redirect: 'follow'
  };

  if (showProgress) {
    console.log(`🏠 Getting organization ${id}`);
  }

  let data = await fetch(`${url}${id}`, requestOptions)
    .then(response => {
      return response.text();
    })
    //.then(result => console.log(result))
    .catch(error => console.log('error', error));

  //console.log(data)
  return JSON.parse(data);
}

export const getServicesForOrganization = async (organization) => {
  const url = `${baseUrlIn}/Service/`;

  var requestOptions = {
    method: 'GET',
    redirect: 'follow'
  };

  let serviceIds = organization.services.map(s => {
    return s.service.id
  });
  serviceIds = [...new Set(serviceIds)]; // Remove duplicate ids

  const promises = serviceIds.map(id => {
    return tokenBucket.acquireToken(async () => {
      return fetch(`${url}${id}`, requestOptions)
        .then(response => {
          if (showProgress) {
            console.log(`🔧 Getting service ${id}`);
          }

          return response.text();
        })
        .catch(error => console.log('error', error));
    });
  });

  if (showProgress) {
    console.log(`🤔 Getting ${promises.length} services for organization ${organization.organizationNames.find(n => n.language === 'fi').value}`);
  }

  return (await Promise.all(promises)).map(service => {
    return JSON.parse(service);
  });
}

export const getServiceChannels = async (service) => {
  const url = `${baseUrlIn}/ServiceChannel/`;

  let serviceIChannelds = service.serviceChannels ? service.serviceChannels.map(s => s.serviceChannel.id) : [];

  const requestOptions = {
    method: 'GET',
    redirect: 'follow'
  };

  const promises = serviceIChannelds.map(id => {
    return tokenBucket.acquireToken(async () => {
      return fetch(`${url}${id}`, requestOptions)
        .then(response => {
          if (showProgress) {
            console.log(`🌊 Getting channel ${id}`);
          }
          return response.text();
        })
        .catch(error => console.log('error', error));
    });
  });

  const serviceChannels = await Promise.all(promises);

  return serviceChannels.map(serviceChannel => {
    let output = {}
    try {
      output = JSON.parse(serviceChannel);
    } catch (e) {
      console.error(`☹️ unable to parse service channel`);
      console.error(e);
      console.error(serviceChannel);
    }
    return output;
  });
}
