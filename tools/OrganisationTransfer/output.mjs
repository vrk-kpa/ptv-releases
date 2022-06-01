import fetch from 'node-fetch';
import TokenBucketRateLimiter from './TokenBucketRateLimiter.mjs';
import {
  baseUrlOut,
  accessToken,
  showProgress,
  organizationId,
  getEnumTypes,
  getRandomString,
  provisionTypeMapping,
} from './configuration.mjs';

const tokenBucket = new TokenBucketRateLimiter({
  maxRequests: 1, // concurrent requests
  maxRequestWindowMS: 5000, // within time window
});

const postHeaders = {
  'Content-Type': 'application/json',
  'Accept-Encoding': 'gzip, deflate, br',
  authorization: `Bearer ${accessToken}`,
};

export const saveOrganization = async (organization) => {
  const url = `${baseUrlOut}/organization/SaveOrganization`;

  const requestOptions = {
    method: 'POST',
    body: JSON.stringify(organization),
    headers: postHeaders,
  };

  let data = await tokenBucket.acquireToken(async () => {
    if (showProgress) {
      console.log(`🏠 Saving organization`);
    }

    return await fetch(`${url}`, requestOptions)
      .then(response => {
        return response.text();
      })
      .catch(error => console.log('error', error));
  });

  const savedOrganization = JSON.parse(data).data;

  if (showProgress) {
    console.log(`❗ New organization id ${savedOrganization?.id}`);
  }

  return savedOrganization;
}

export const publishEntity = async (id, languagesAvailabilities, type) => {
  const url = `${baseUrlOut}/${type}/publishEntity`;

  const requestOptions = {
    method: 'POST',
    body: JSON.stringify({
      id: id,
      languagesAvailabilities: languagesAvailabilities,
    }),
    headers: postHeaders,
  };

  let data = await tokenBucket.acquireToken(async () => {
    if (showProgress) {
      console.log(`📫 publishing ${type}`);
    }

    return await fetch(`${url}`, requestOptions)
      .then(response => {
        return response.text();
      })
      .catch(error => console.log('error', error));
  });
}

export const saveService = async (service) => {
  const url = `${baseUrlOut}/service/SaveService`;

  const requestOptions = {
    method: 'POST',
    body: JSON.stringify(service),
    headers: postHeaders,
  };

  let data = await tokenBucket.acquireToken(async () => {
    if (showProgress) {
      console.log(`🔧 Saving service ${service.name['fi']}`);
    }

    return await fetch(`${url}`, requestOptions)
      .then(response => {
        return response.text();
      })
      .catch(error => console.log('error', error));
  });

  let result = {};
  try {
    result = JSON.parse(data);

    if (result.messages.errors && result.messages.errors.length > 0) {
      console.error(result.messages.errors)
    }

    if (showProgress) {
      console.log(`❗ New service id ${result?.data?.id}`);
    }
  } catch (e) {
    console.error('☹️ Unable to save service');
    console.error(e);
    console.error(data);
  }

  return result.data;
}

export const saveServiceChannel = async (serviceChannel) => {
  const { serviceChannelType } = serviceChannel;
  const url = `${baseUrlOut}/channel/${getSaveUrl(serviceChannelType)}`;

  const requestOptions = {
    method: 'POST',
    body: JSON.stringify(serviceChannel),
    headers: postHeaders,
  };

  let data = await tokenBucket.acquireToken(async () => {
    if (showProgress) {
      console.log(`🌊 Saving service channel`);
    }

    return await fetch(`${url}`, requestOptions)
      .then(response => {
        return response.text();
      })
      .catch(error => console.log('error', error));
  });

  let result = '';
  try {
    result = JSON.parse(data);
  } catch (e) {
    console.error(e);
  }
  if (!result.data) {
    console.error('☹️ Unable to save service channel');
    console.error(data);
    return {}
  }

  if (showProgress) {
    console.log(`❗ New channel id ${result.data.id}`);
  }

  return result.data;
}

export const connectServiceToServiceChannel = async (serviceId, serviceChannelUnificRootIds) => {
  if (!serviceId || serviceChannelUnificRootIds.length < 1) {
    console.error('Service id missing or 0 channels!');
    console.error(serviceId);
    console.error(serviceChannelUnificRootIds);
    return;
  }
  const url = `${baseUrlOut}/service/SaveRelations`;

  const requestBody = {
    id: serviceId,
    unificRootId: serviceId,
    SelectedConnections: serviceChannelUnificRootIds.map(c => {
      return {
        unificRootId: c,
      }
    }),
  };

  const requestOptions = {
    method: 'POST',
    body: JSON.stringify(requestBody),
    headers: postHeaders,
  };

  let data = await tokenBucket.acquireToken(async () => {
    if (showProgress) {
      console.log(`🔗 Connecting service ${serviceId} to ${serviceChannelUnificRootIds.length} channel(s): ${JSON.stringify(serviceChannelUnificRootIds)}`);
    }

    return await fetch(`${url}`, requestOptions)
      .then(response => {
        return response.text();
      })
      .catch(error => console.log('error', error));
  });

  return data;
}

export const getOntologyId = async (keyword, language) => {
  const url = `${baseUrlOut}/ontology/search`;

  const requestOptions = {
    method: 'GET',
    headers: postHeaders,
  };

  let data = await tokenBucket.acquireToken(async () => {
    return await fetch(`${url}?expression=${keyword}&language=${language}`, requestOptions)
      .then(response => {
        return response.text();
      })
      .catch(error => console.log('error', error));
  });

  let result = null;
  try {
    if (data) {
      const parsed = JSON.parse(data);
      if (parsed.length > 0) {
        result = parsed[0].id;
      }
    }
  } catch (e) {
    console.error(e);
    console.log(data);
  }

  return result;
}

export const convertOrganization = async (organization) => {
  organization = replaceMunicipalities(organization);
  organization = anonymizeOrganizationEmails(organization); // check: does this even work?

  const {
    Municipalities,
    DialCodes,
    PhoneNumberTypes,
    Languages,
    PublishingStatuses
  } = await getEnumTypes();

  const languagesAvailabilities = organization.organizationNames.map(name => name.language).map(la => {
    return {
      canBePublished: true,
      code: la,
      isNew: false,
      languageId: Languages.find(({ code }) => code == la).id,
      lastFailedPublishDate: null,
      modified: (new Date).getTime(),
      modifiedBy: "anonymous@email.com",
      statusId: PublishingStatuses.find(({ code }) => code == 'Published').id,
      validFrom: null,
      validTo: null,
      validatedFields: null,
    }
  })

  const output = {
    id: null,
    oid: null,
    publishingStatus: PublishingStatuses.find(({ code }) => code == 'Published').id,
    languagesAvailabilities: languagesAvailabilities,
    isMainOrganization: !organization.parentOrganizationId,
    parentId: organization.parentOrganizationId ? organization.parentOrganizationId : null,
    organizationType: await getOrganizationType(organization.organizationType),
    municipality: Municipalities.find(({ code }) => code == organization.municipality)?.id,
    name: organization.organizationNames.reduce((acc, item) => {
      acc[item.language] = `${item.value}`;
      return acc;
    }, {}),
    shortDescription: organization.organizationDescriptions.reduce((acc, item) => {
      if (item.type === 'Summary') {
        acc[item.language] = item.value;
      }
      return acc;
    }, {}),
    description: organization.organizationDescriptions.reduce((acc, item) => {
      if (item.type === 'Description') {
        acc[item.language] = getRichText(item.value);
      }
      return acc;
    }, {}),
    emails: organization.emails ? organization.emails : {},
    phoneNumbers: organization.phoneNumbers ? organization.phoneNumbers.reduce(async (acc, pn) => {
      if (!acc[pn.language]) {
        acc[pn.language] = [];
      }
      acc[pn.language].push({
        phoneNumber: pn.number,
        wholePhoneNumber: pn.number,
        additionalInformation: pn.additionalInformation ? pn.additionalInformation : null,
        chargeDescription: pn.chargeDescription ? pn.chargeDescription : null,
        isLocalNumber: !!pn.isFinnishServiceNumber,
        chargeType: await getServiceChargeTypeId(pn.serviceChargeType),
        type: PhoneNumberTypes.find(({ code }) => code == 'Phone').id,
        isLocalNumberParsed: false,
        dialCode: DialCodes.find(({ code }) => code == pn.prefixNumber).id
      })
      return acc;
    }, {}) : {},
    webPages: organization.webPages ? organization.webPages.reduce((acc, wp) => {
      if (!acc[wp.language]) {
        acc[wp.language] = [];
      }
      acc[wp.language].push({
        urlAddress: wp.url,
        name: wp.value,
      })
      return acc;
    }, {}) : {},
    postalAddresses: organization.addresses.reduce((acc, item) => addressReducer(acc, item, 'Postal', Municipalities), []),
    visitingAddresses: [], //organization.addresses.reduce((acc, item) => addressReducer(acc, item, 'Visiting', Municipalities), []),
    electronicInvoicingAddresses: organization.electronicInvoicingAddresses ? organization.electronicInvoicingAddresses : [],
    action: "saveAndPublish"
  }

  return output;
}

export const convertService = async (service, organization) => {

  const {
    AreaInformationTypes,
    ServiceClasses,
    TopLifeEvents,
    TopTargetGroups,
    IndustrialClasses,
    Languages,
    Municipalities,
    Provinces,
    HospitalRegions,
    BusinessRegions,
    PublishingStatuses,
    ChargeTypes,
    FundingTypes,
    ServiceTypes,
    VoucherTypes,
  } = await getEnumTypes();

  const keywords = service.keywords.reduce((acc, item) => {
    if (!acc[item.language]) {
      acc[item.language] = [];
    }
    acc[item.language].push(item.value);
    return acc;
  }, {})

  const namePostfix = `-${getRandomString(6)}`;

  let languageVersions = {};

  let shortDescriptions = {};
  let longDescriptions = {};
  let userInstructions = {};
  let deadline = {};

  service.languages.forEach(l => {
    const alternativeName = `${service.serviceNames.find(({ language, type }) => language == l && type == 'AlternativeName')}`;
    if (!service.serviceNames.find(({ language }) => language == l)) {
      console.log(`🤷 Missing data: service ${service.id} is available in "${l}" but does not have name in that language.`);
      return;
    }

    shortDescriptions[l] = service.serviceDescriptions.find(d => d.language === l && d.type === 'Summary')?.value;
    longDescriptions[l] = getRichText(service.serviceDescriptions.find(d => d.language === l && d.type === 'Description')?.value);
    userInstructions[l] = getRichText(service.serviceDescriptions.find(d => d.language === l && d.type === '"UserInstruction"')?.value);
    deadline[l] = service.serviceDescriptions.find(({ language, type }) => language == l && type == 'DeadLine')?.value;

    languageVersions[l] = {
      isEnabled: true,
      language: l,
      name: `${service.serviceNames.find(({ language }) => language == l).value}${namePostfix}`,
      summary: shortDescriptions[l],
      description: longDescriptions[l],
      userInstructions: userInstructions[l],
      conditions: getRichText(service.serviceDescriptions.find(({ language, type }) => language == l && type == 'Conditions')?.value),
      deadline: deadline[l],
      processingTime: service.serviceDescriptions.find(({ language, type }) => language == l && type == 'ProcessingTime')?.value,
      periodOfValidity: service.serviceDescriptions.find(({ language, type }) => language == l && type == 'ValidityTime')?.value,
      //laws: [], //!?!?
      keywords: keywords[l],
      modifiedBy: 'anonymous',
      translationAvailability: null,
      hasAlternativeName: !!alternativeName,
      alternativeName: alternativeName,
      "charge": {
        "hasLink": false,
        "link": {
          "url": "",
          "name": ""
        },
        "info": null
      },
      modified: new Date().toISOString(),
      scheduledArchive: null,
      scheduledPublish: null,
      status: service.publishingStatus,
      "purchaseProducerNames": [],
      "otherProducerNames": [],
    };
  });

  const serviceProducers = service.organizations.filter(o => {
    return o.roleType == 'Producer';
  }).map(o => {
    return {
      organization: o.organization?.id,
      roleType: o.roleType,
      provisionType: provisionTypeMapping[o.provisionType],
      additionalInformation: '',
    };
  }).filter(o => o.organization);
  const selfProducers = service.organizations.filter(o => {
    return o.roleType == 'Producer' && o.provisionType == 'SelfProducedServices'; // I'm not 100% sure of this
  });
  const procurers = service.organizations.filter(o => {
    return o.roleType == 'Producer' && o.provisionType == 'ProcuredServices';
  });

  const areaTypes = [];
  const areaMunicipalities = service.areas.reduce((acc, area) => {
    let areaCodes = [];
    if (area.type == 'Municipality') {
      if (area.code) {
        areaCodes.push(area.code);
      } else if (area.municipalities) {
        areaCodes.push(...(area.municipalities.map(m => m.code)));
      }
      for (let areaCode of areaCodes) {
        const m = Municipalities.find(({ code }) => code == areaCode);
        if (m) {
          acc.push(m.id);
        } else {
          if (showProgress) {
            console.log(`☹️ Municipality ${areaCode} does not exist`)
          }
        }
      }
    }
    return acc;
  }, []);
  const areaProvinces = service.areas.reduce((acc, area) => {
    if (area.type == 'Province') {
      const p = Provinces.find(({ code }) => code == area.code);
      if (p) {
        acc.push(p.id);
      } else {
        if (showProgress) {
          console.log(`☹️ Province ${area.code} does not exist`)
        }
      }
    }
    return acc;
  }, []);
  const areaHospitalRegions = service.areas.reduce((acc, area) => {
    if (area.type == 'HospitalRegion') {
      const hr = HospitalRegions.find(({ code }) => code == area.code);
      if (hr) {
        acc.push(hd.id);
      } else {
        if (showProgress) {
          console.log(`☹️ Hospital region ${area.code} does not exist`)
        }
      }
    }
    return acc;
  }, []);
  const areaBusinessRegions = service.areas.reduce((acc, area) => {
    if (area.type == 'BusinessRegion') {
      const br = BusinessRegions.find(({ code }) => code == area.code);
      if (br) {
        acc.push(br.id);
      } else {
        if (showProgress) {
          console.log(`☹️ Business region ${area.code} does not exist`)
        }
      }
    }
    return acc;
  }, []);
  if (areaMunicipalities.length > 0) {
    areaTypes.push('Municipality');
  }
  if (areaProvinces.length > 0) {
    areaTypes.push('Province');
  }
  if (areaHospitalRegions.length > 0) {
    areaTypes.push('HospitalRegion');
  }
  if (areaBusinessRegions.length > 0) {
    areaTypes.push('BusinessRegion');
  }
  let areaInformationType = AreaInformationTypes.find(({ code }) => code == service.areaType);
  if (!areaInformationType) {
    areaInformationType = AreaInformationTypes.find(({ code }) => code == 'AreaType');
  }

  const alternativeName = service.serviceNames.find(({ type }) => type == 'AlternativeName');

  let serviceType = 'Service';
  switch (service.type) {
    case 'PermitOrObligation':
      serviceType = 'PermissionAndObligation';
      break;
    case 'ProfessionalQualification':
      serviceType = 'ProfessionalQualifications';
      break;
  }

  const output = {
    name: service.serviceNames.reduce((acc, item) => {
      acc[item.language] = `${item.value}${namePostfix}`;
      return acc;
    }, {}),
    organization: organization.id,
    fundingType: FundingTypes.find(({ code }) => code == service.fundingType)?.id,
    serviceType: ServiceTypes.find(({ code }) => code == serviceType)?.id,
    targetGroups: service.targetGroups ? service.targetGroups.map(tg => getTargetGroupId(tg.code, TopTargetGroups)).filter(x => x) : [],
    serviceClasses: service.serviceClasses ? service.serviceClasses.map(({ code }) => {
      return ServiceClasses.find(sc => sc.code == code).id;
    }) : [],
    ontologyTerms: (await Promise.all(service.ontologyTerms ? service.ontologyTerms.map(async ot => {
      const ontologyTerms = ot.name.filter(n => n.value != null).reduce((acc, n) => { acc[n.language] = n.value; return acc }, {});
      const ontology = ot.name.find(n => n.value != null);
      const ontologyId = await getOntologyId(ontology.value, ontology.language);

      return {
        id: ontologyId,
        code: ot.code ? ot.code : '',
        names: ontologyTerms,
      };
    }) : [])).filter(ot => ot.id),
    lifeEvents: service.lifeEvents ? service.lifeEvents.map(({ code }) => {
      return TopLifeEvents.find(le => le.code == code)?.id;
    }).filter(c => typeof c == 'string') : [],
    industrialClasses: [], //service.industrialClasses ? service.industrialClasses : [], // array of uids.
    chargeType: service.serviceChargeType ? getServiceChargeTypeId(service.serviceChargeType) : null,
    //voucherType: VoucherTypes.find(({ code }) => code == (service.serviceVouchersInUse ? 'NoUrl' : 'NotUsed'))?.id, // note: this is not exactly correct
    serviceVouchers: {},
    areaInformation: {
      areaInformationType: areaInformationType.id,
      areaTypes: areaTypes,
      municipalities: areaMunicipalities,
      provinces: areaProvinces,
      businessRegions: areaBusinessRegions,
      hospitalRegions: areaHospitalRegions,
    },
    responsibleOrganization: service.organizations.filter(item => item.roleType == 'Responsible').map(o => o.organization)[0], // TODO: use organization here
    otherResponsibleOrganizations: service.organizations.filter(item => item.roleType == 'OtherResponsible').map(o => o.organization),
    languages: service.languages?.map(languageCode => { return Languages.find(({ code }) => languageCode == code).id }),
    languagesAvailabilities: service.languages?.map(languageCode => {
      const lang = Languages.find(({ code }) => languageCode == code);
      return {
        canBePublished: true,
        code: languageCode,
        isNew: false,
        languageId: lang.id,
        lastFailedPublishDate: null,
        modified: (new Date).getTime(),
        modifiedBy: "anonymous@email.com",
        statusId: PublishingStatuses.find(({ code }) => code == 'Published').id,
        validFrom: null,
        validTo: null,
        validatedFields: null,
      }
    }),
    languageVersions: languageVersions,
    generalDescriptionServiceTypeId: {}, // !?
    generalDescriptionName: {}, // !?
    generalDescription: null, //service.generalDescriptionId,
    hasSelfProducers: selfProducers.length > 0,
    selfProducers: selfProducers,
    purchaseProducers: [], // ??
    otherProducers: service.otherProducers ? service.otherProducers : [], // ??
    connectedChannels: [], // Does nothing it seems
    laws: service.legislation,
    shortDescription: shortDescriptions,
    description: longDescriptions,
    serviceProducers: serviceProducers,
    responsibleOrganizations: service.organizations.filter(item => item.roleType == 'Responsible').map(o => o.organization?.id),
    overrideTargetGroups: [], // !? (could be something from service.targetGroups)
    missingLanguages: [], // guid[]
    keywords: service.keyword ? service.keyword.reduce((acc, kw) => {
      if (kw.language && kw.value) {
        acc[kw.language] = kw.value;
      }
      return acc;
    }, {}) : {},
    deadlineInformation: deadline,
    conditionOfServiceUsage: {},
    alternateName: alternativeName ? { [alternativeName.language]: alternativeName.value } : {},
  };

  return output;
}

export const convertServiceChannel = async (serviceChannel, organization) => { // from input model to output model
  const {
    Languages,
    DialCodes,
    PublishingStatuses,
    AccessibilityClassificationLevelTypes,
    PhoneNumberTypes,
    Municipalities,
  } = await getEnumTypes();

  // LOCATION!?
  const output = {
    publishingStatus: PublishingStatuses.find(({ code }) => serviceChannel.publishingStatus == code).id,
    serviceChannelType: serviceChannel.serviceChannelType,
    languagesAvailabilities: serviceChannel.languages.map(la => {
      const lang = Languages.find(({ code }) => code == la);
      return {
        languageId: lang.id,
        code: lang.code,
      };
    }),
    templateId: null,
    templateOrganizationId: null,
    name: serviceChannel.languages.reduce((acc, l) => {
      let name = serviceChannel.serviceChannelNames.find(n => n.language == l);
      if (!name) {
        name = serviceChannel.serviceChannelNames.find(n => n.language == 'fi');
      }
      acc[l] = `${name.value}-${getRandomString(6)}`;
      return acc;
    }, {}),
    organization: organization.id ? organization.id : organizationId,
    shortDescription: serviceChannel.languages.reduce((acc, l) => {
      let shortDescription = serviceChannel.serviceChannelDescriptions.find(d => d.language === l && d.type === 'Summary');
      if (!shortDescription) {
        shortDescription = serviceChannel.serviceChannelDescriptions.find(d => d.type === 'Summary');
      }
      acc[l] = shortDescription.value;
      return acc;
    }, {}),
    description: serviceChannel.languages.reduce((acc, l) => {
      let longDescription = serviceChannel.serviceChannelDescriptions.find(d => d.language === l && d.type === 'Description');
      if (!longDescription) {
        longDescription = serviceChannel.serviceChannelDescriptions.find(d => d.type === 'Description');
      }
      acc[l] = getRichText(longDescription.value);
      return acc;
    }, {}),
    urlAddress: serviceChannel.urlAddress ? serviceChannel.urlAddress : null,
    onlineAuthentication: serviceChannel.onlineAuthentication ? serviceChannel.onlineAuthentication : null,
    languages: serviceChannel.languages.map(la => {
      return Languages.find(({ code }) => code == la).id;
    }),
    phoneNumbers: serviceChannel.phoneNumbers ? await serviceChannel.phoneNumbers.reduce(async (acc, pn) => {
      if (!acc[pn.language]) {
        acc[pn.language] = [];
      }
      acc[pn.language].push({
        phoneNumber: pn.number,
        wholePhoneNumber: pn.number,
        additionalInformation: pn.additionalInformation ? pn.additionalInformation : null,
        chargeDescription: pn.chargeDescription ? pn.chargeDescription : null,
        isLocalNumber: !!pn.isFinnishServiceNumber,
        chargeType: await getServiceChargeTypeId(pn.serviceChargeType),
        Type: PhoneNumberTypes.find(({ code }) => code == 'Phone').id,
        isLocalNumberParsed: false,
        dialCode: DialCodes.find(({ code }) => code == pn.prefixNumber).id
      });
      return acc;
    }, {}) : {},
    urlAddress: serviceChannel.webPages ? serviceChannel.webPages.reduce((acc, item) => {
      acc[item.language] = item.url;
      return acc;
    }, {}) : [],
    supportEmails: serviceChannel.supportEmails ? serviceChannel.supportEmails : {},
    accessibilityClassifications: serviceChannel.accessibilityClassification ? serviceChannel.accessibilityClassification.reduce((acc, ac) => {
      const aclt = AccessibilityClassificationLevelTypes.find(({ code }) => code == ac.accessibilityClassificationLevel);
      acc[ac.language] = {
        accessibilityClassificationLevelType: aclt.id,
        wcagLevelType: ac.wcagLevel ? ac.wcagLevel : null,
        name: ac.accessibilityStatementWebPageName,
        urlAddress: ac.accessibilityStatementWebPage,
        id: null,
      };
      return acc;
    }, {}) : {},
    visitingAddresses: serviceChannel.addresses?.reduce((acc, item) => addressReducer(acc, item, 'Location', Municipalities), []),
    serviceCollections: [],
  };

  return output;
}

export const anonymizeOrganizationEmails = (organization) => {
  organization.emails = organization.emails.forEach(element => {
    element.value = "anonymous@email.com";
    return element;
  });
  return organization;
}

const addressReducer = (acc, item, group, Municipalities) => {
  if (item.type === group) {
    const { streetAddress, postOfficeBoxAddress } = item;
    let address;
    if (streetAddress) {
      address = streetAddress;
    }
    if (postOfficeBoxAddress) {
      address = postOfficeBoxAddress;
    }
    acc.push({
      additionalInformation: address.additionalInformation?.reduce((acc, item) => {
        acc[item.language] = item.value;
        return acc;
      }, {}),
      streetNumber: address.streetNumber ? address.streetNumber : null,
      streetName: address.street?.reduce((acc, item) => {
        acc[item.language] = item.value;
        return acc;
      }, {}),
      streetNumberRange: null, // we have nothing, this would require guid
      postalCode: address.postalCode ? address.postalCode : null,
      coordinates: [{
        isMain: true,
        latitude: item.latitude,
        longitude: item.longitude,
        coordinateState: item.coordinateState,
        id: null // needs guid?
      }],
      municipality: Municipalities.find(({ code }) => code == address.municipality)?.id,
      postOffice: address.postOffice ? address.postOffice : null, // we have name, this would require guid
      streetType: item.subType,
      invalidAddress: true,
      postalCodeOutsideChange: null,
    });
  }
  return acc;
}

/**
 * Find all 'municipality' keys from organization object and replace the complex array with just the municipality code
 */
const replaceMunicipalities = (node) => {
  if (!node) {
    return node;
  }
  if (typeof node == 'array') {
    node = node.map(subnode => replaceMunicipalities(subnode));
  }
  if (typeof node == 'object') {
    for (const [key, value] of Object.entries(node)) {
      if (key == 'municipality') {
        node[key] = `${value?.code}`;
      } else {
        replaceMunicipalities(value);
      }
    }
  }
  return node;
}

const getTargetGroupId = (targetGroupCode, TopTargetGroups) => {
  let targetGroupId;
  TopTargetGroups.forEach(
    tg => {
      if (tg.code == targetGroupCode) {
        targetGroupId = tg.id;
      }
      if (!targetGroupId && typeof tg.children == 'array') {
        targetGroupId = getTargetGroupId(tg.chilren, TopTargetGroups)
      }
    }
  )
  return targetGroupId;
}

const getRichText = (text) => {
  return JSON.stringify({
    entityMap: {},
    blocks: [
      {
        key: getRandomString(5),
        text: `${text}`,
        type: "unstyled",
        depth: 0,
        inlineStyleRanges: [],
        entityRanges: [],
        data: {}
      }
    ]
  });
}

const getSaveUrl = (serviceChannelType) => {
  switch (serviceChannelType) {
    case 'EChannel':
      return 'SaveElectronicChannel';
    case 'WebPage':
      return 'SaveWebPageChannel';
    case 'PrintableForm':
      return 'SavePrintableFormChannel';
    case 'Phone':
      return 'SavePhoneChannel';
    case 'ServiceLocation':
      return 'SaveServiceLocationChannel';
  }
}

const getServiceChargeTypeId = async (serviceChargeType) => {
  const { ChargeTypes } = await getEnumTypes();

  switch (serviceChargeType) {
    case "Charged":
    case "Chargeable":
      return ChargeTypes.find(({ code }) => code == 'Charged').id;
    case "FreeOfCharge":
    case "Free":
      return ChargeTypes.find(({ code }) => code == 'Free').id;
    case "Other":
      return ChargeTypes.find(({ code }) => code == 'Other').id;
  }
}

const getOrganizationType = async (organizationTypeAsString) => {
  const {
    OrganizationTypes,
  } = await getEnumTypes();

  let id = '';
  OrganizationTypes.forEach(
    o => {
      o.children.forEach(
        c => {
          if (organizationTypeAsString == c.code) {
            id = c.id;
          }
        }
      )
    }
  )

  return id;
}
