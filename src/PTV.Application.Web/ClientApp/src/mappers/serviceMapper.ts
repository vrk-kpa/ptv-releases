import { getKeys, valueOrDefault } from 'utils';
import { FormStateApiModel } from 'types/api/formStateApiModel';
import { ServiceApiLanguageModel, ServiceApiModel } from 'types/api/serviceApiModel';
import * as enumTypes from 'types/enumTypes';
import {
  ServiceFormValues,
  ServiceLanguageVersionValues,
  ServiceModel,
  ServiceModelLangaugeVersionsValuesType,
  ServiceProducer,
} from 'types/forms/serviceFormTypes';
import { LanguageVersionType } from 'types/languageVersionTypes';
import { unsetAreasWithoutParents } from 'mappers/areaInformationMapper';
import { toLocalDateTime } from 'utils/date';
import { createEmptyLanguageVersion } from 'utils/service';
import { getLVStatus } from 'utils/status';
import { toChargeApiModel, toChargeUiModel } from './chargeMapper';
import { toGDApiModel, toGDUiModel } from './gdMapper';
import { toLastTranslationsApiModel, toLastTranslationsUiModel } from './lastTranslationMapper';
import { toOtherLanguageVersionApiModel, toOtherLanguageVersionUiModel } from './otherVersionMapper';

function toUiLanguageVersion(input: ServiceApiLanguageModel, entityStatus: enumTypes.PublishingStatus): ServiceLanguageVersionValues {
  const { charge, status, purchaseProducerNames, otherProducerNames, ...sameFields } = input;

  return {
    ...sameFields,
    isEnabled: true,
    alternativeName: valueOrDefault(input.alternativeName, ''),
    charge: toChargeUiModel(charge),
    status: getLVStatus(status, entityStatus),
    purchaseProducers: purchaseProducerNames.map((x) => toServiceProducer(x)),
    otherProducers: otherProducerNames.map((x) => toServiceProducer(x)),
  };
}

function toServiceProducer(name: string): ServiceProducer {
  return {
    name: name,
  };
}

function toApiLanguageVersion(input: ServiceLanguageVersionValues, entityStatus: enumTypes.PublishingStatus): ServiceApiLanguageModel {
  const {
    hasAlternativeName,
    alternativeName,
    charge,
    modified,
    scheduledArchive,
    scheduledPublish,
    status,
    otherProducers,
    purchaseProducers,
    ...sameFields
  } = input;

  return {
    ...sameFields,
    hasAlternativeName: hasAlternativeName,
    alternativeName: hasAlternativeName ? alternativeName : null,
    charge: toChargeApiModel(charge),
    modified: toLocalDateTime(modified).toUTC().toISO(),
    scheduledArchive: !!scheduledArchive ? toLocalDateTime(scheduledArchive).toUTC().toISO() : null,
    scheduledPublish: !!scheduledPublish ? toLocalDateTime(scheduledPublish).toUTC().toISO() : null,
    status: getLVStatus(status, entityStatus),
    purchaseProducerNames: purchaseProducers.map((x) => x.name),
    otherProducerNames: otherProducers.map((x) => x.name),
  };
}

export function toServiceUiModel(input: ServiceApiModel): ServiceFormValues {
  const { languageVersions, otherModifiedVersion, otherPublishedVersion, generalDescription, status, lastTranslations, ...sameFields } =
    input;
  const uiLanguageVersions = toUiLanguageVersions(languageVersions, input.status);
  const convertedPublishedVersion = toOtherLanguageVersionUiModel(otherPublishedVersion);
  const convertedModifiedVersion = toOtherLanguageVersionUiModel(otherModifiedVersion);
  const convertedGD = !generalDescription ? null : toGDUiModel(generalDescription);
  const convertedLastTranslations = lastTranslations.map((x) => toLastTranslationsUiModel(x));

  return {
    ...sameFields,
    languageVersions: uiLanguageVersions,
    generalDescription: convertedGD,
    voucherType: input.voucherType,
    hasSelfProducers: input.selfProducers?.length > 0,
    otherModifiedVersion: convertedModifiedVersion,
    otherPublishedVersion: convertedPublishedVersion,
    lastTranslations: convertedLastTranslations,
    status,
  };
}

export function toServiceApiModel(input: ServiceFormValues): ServiceApiModel {
  const {
    languageVersions,
    otherPublishedVersion,
    otherModifiedVersion,
    generalDescription,
    status,
    lastTranslations,
    areaInformation,
    ...sameFields
  } = input;
  const convertedLVs = toApiLanguageVersions(input);
  const convertedPublishedVersion = toOtherLanguageVersionApiModel(otherPublishedVersion);
  const convertedModifiedVersion = toOtherLanguageVersionApiModel(otherModifiedVersion);
  const convertedGD = !generalDescription ? null : toGDApiModel(generalDescription);
  const convertedLastTranslations = lastTranslations.map((x) => toLastTranslationsApiModel(x));
  const cleanedAreaInformation = unsetAreasWithoutParents(areaInformation);

  return {
    ...sameFields,
    languageVersions: convertedLVs,
    generalDescription: convertedGD,
    connectedChannels: [],
    otherModifiedVersion: convertedModifiedVersion,
    otherPublishedVersion: convertedPublishedVersion,
    lastTranslations: convertedLastTranslations,
    areaInformation: cleanedAreaInformation,
    status,
  };
}

function toUiLanguageVersions(
  source: LanguageVersionType<ServiceApiLanguageModel>,
  servicePublishingStatus: enumTypes.PublishingStatus
): ServiceModelLangaugeVersionsValuesType {
  const result: ServiceModelLangaugeVersionsValuesType = {
    en: source.en ? toUiLanguageVersion(source.en, servicePublishingStatus) : createEmptyLanguageVersion('en'),
    fi: source.fi ? toUiLanguageVersion(source.fi, servicePublishingStatus) : createEmptyLanguageVersion('fi'),
    sv: source.sv ? toUiLanguageVersion(source.sv, servicePublishingStatus) : createEmptyLanguageVersion('sv'),
    se: source.se ? toUiLanguageVersion(source.se, servicePublishingStatus) : createEmptyLanguageVersion('se'),
    smn: source.smn ? toUiLanguageVersion(source.smn, servicePublishingStatus) : createEmptyLanguageVersion('smn'),
    sms: source.sms ? toUiLanguageVersion(source.sms, servicePublishingStatus) : createEmptyLanguageVersion('sms'),
  };
  return result;
}

function toApiLanguageVersions(source: ServiceFormValues): LanguageVersionType<ServiceApiLanguageModel> {
  const result: LanguageVersionType<ServiceApiLanguageModel> = {};
  const languages = getKeys(source.languageVersions);
  for (const lang of languages) {
    const langVersion = source.languageVersions[lang];
    if (langVersion.isEnabled) {
      result[lang] = toApiLanguageVersion(langVersion, source.status);
    }
  }

  return result;
}

export function toFormStateApiModel(input: ServiceModel): FormStateApiModel {
  return {
    entityType: 'service',
    entityId: input.id,
    formName: 'serviceForm',
    state: JSON.stringify(input),
    dataModelVersion: 'v.2021.1',
  };
}
