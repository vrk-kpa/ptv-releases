import process from 'process';

import { getEnumTypes, organizationId, showProgress } from './configuration.mjs';
import {
  getOrganization,
  getServicesForOrganization,
  getServiceChannels,
} from './input.mjs'
import {
  convertServiceChannel,
  convertService,
  convertOrganization,
  saveServiceChannel,
  saveService,
  saveOrganization,
  connectServiceToServiceChannel,
  getOntologyId,
  publishEntity,
} from './output.mjs'

// Get data

let organization = await getOrganization(organizationId);

const services = await getServicesForOrganization(organization);
const serviceChannelPromises = services.map(s => getServiceChannels(s));
let serviceChannels = await Promise.all(serviceChannelPromises);
serviceChannels = serviceChannels.flatMap(a => a);

if (showProgress) {
  console.log(`total number of service channels ${serviceChannels.length}`);
}

// Save data

organization = await convertOrganization(organization);
const savedOrganization = await saveOrganization(organization);
await publishEntity(savedOrganization.id, organization.languagesAvailabilities, 'organization');

let serviceIds = [];
const savedServices = await Promise.all(services.map(async (service) => {
  const convertedService = await convertService(service, savedOrganization);
  const saved = await saveService(convertedService);
  if (service.id) {
    serviceIds.push({ oldId: service.id, newId: saved.id, oldChannelIds: service?.serviceChannels?.map(sc => sc.serviceChannel.id) });
  }
  return saved;
}));

let serviceChannelIds = [];
const savedServiceChannels = await Promise.all(serviceChannels.map(async (serviceChannel) => {
  const convertedServiceChannel = await convertServiceChannel(serviceChannel, savedOrganization);
  const saved = await saveServiceChannel(convertedServiceChannel);
  serviceChannelIds.push({ oldId: serviceChannel.id, unificRootId: saved.unificRootId });
  await publishEntity(saved.id, saved.languagesAvailabilities, 'channel');
  return saved;
}));

for (let s of services) {
  const serviceData = serviceIds.find(si => si.oldId == s.id);
  const { newId, oldChannelIds } = serviceData;

  if (!s.serviceChannels || !oldChannelIds) {
    if (showProgress) {
      console.log(`Service ${s.id} has no service channels`);
    }
    continue;
  }

  const newChannelIds = oldChannelIds.map(i => {
    return serviceChannelIds.find(sci => sci.oldId == i).unificRootId;
  });

  if (newChannelIds.length > 0) {
    const response = await connectServiceToServiceChannel(newId, newChannelIds);
    await publishEntity(serviceData.id, serviceData.languagesAvailabilities, 'service');
  } else {
    if (showProgress) {
      console.log(`Service ${s.id}'s service channels have somehow got lost`);
    }
  }
}

console.log(`mem usage ${(process.memoryUsage().heapUsed / 2 ** 20).toFixed(2)} MB`);

console.log('Done.');

