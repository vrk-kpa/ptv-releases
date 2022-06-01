import { NestedValue } from 'react-hook-form';
import { ConnectableChannel } from 'types/api/serviceChannelModel';
import { AreaInformationModel } from 'types/areaTypes';
import { ClassificationItem, OntologyTerm } from 'types/classificationItemsTypes';
import * as enumTypes from 'types/enumTypes';
import { GeneralDescriptionModel } from 'types/generalDescriptionTypes';
import { LinkModel } from 'types/link';
import { TranslationAvailabilityType } from 'types/miscellaneousTypes';
import { OrganizationModel } from 'types/organizationTypes';
import { ChargeModel } from './chargeType';
import { OtherVersionType } from './otherVersionType';
import { LastTranslationType } from './translationTypes';

export type ServiceVoucherLink = {
  id?: string;
  name: string;
  url: string;
  additionalInformation: string;
  orderNumber: number;
};

export enum cServiceVoucherLink {
  name = 'name',
  url = 'url',
  additionalInformation = 'additionalInformation',
}

export type ServiceVoucherModel = {
  info: string;
  links: ServiceVoucherLink[];
  // RHF does not support validating array min/max length and individual items
  // so this tag is used for the former. See https://github.com/react-hook-form/react-hook-form/discussions/7815
  linksErrorTag: string | undefined;
};

export enum cVoucher {
  info = 'info',
  links = 'links',
  linksErrorTag = 'linksErrorTag',
}

// react hook form does not support plain arrays like string[]
export type ServiceProducer = {
  name: string;
};

export enum cSp {
  name = 'name',
}

export type ServiceLanguageVersionValues = {
  // To simplify data binding, the UI will always have all the language versions
  // available to it and this flag is to distinguish between actual language version and placeholder
  isEnabled: boolean;
  language: enumTypes.Language;
  status: enumTypes.PublishingStatus;
  name: string;
  hasAlternativeName: boolean;
  alternativeName: string;
  summary: string;
  description: string | null | undefined;
  userInstructions: string | null | undefined;
  conditions: string | null | undefined;
  deadline: string | null | undefined;
  processingTime: string | null | undefined;
  periodOfValidity: string | null | undefined;
  laws: LinkModel[];
  charge: ChargeModel;
  voucher: ServiceVoucherModel;
  keywords: string[];
  purchaseProducers: ServiceProducer[];
  otherProducers: ServiceProducer[];
  modified: string;
  modifiedBy: string;
  scheduledPublish: string | null | undefined;
  scheduledArchive: string | null | undefined;
  translationAvailability: TranslationAvailabilityType | null | undefined;
};

export type ServiceModelLanguageVersion = ServiceLanguageVersionValues & {
  keywords: NestedValue<string[]>;
};

export enum cLv {
  isEnabled = 'isEnabled',
  name = 'name',
  hasAlternativeName = 'hasAlternativeName',
  alternativeName = 'alternativeName',
  summary = 'summary',
  description = 'description',
  userInstructions = 'userInstructions',
  conditions = 'conditions',
  deadline = 'deadline',
  processingTime = 'processingTime',
  periodOfValidity = 'periodOfValidity',
  laws = 'laws',
  charge = 'charge',
  voucher = 'voucher',
  keywords = 'keywords',
  purchaseProducers = 'purchaseProducers',
  otherProducers = 'otherProducers',
  backgroundDescription = 'backgroundDescription',
}

export type ServiceModelLangaugeVersionsValuesType = {
  [language in enumTypes.Language]: ServiceLanguageVersionValues;
};

export type ServiceModelLanguageVersionsType = {
  [language in enumTypes.Language]: ServiceModelLanguageVersion;
};

export type ServiceFormValues = {
  id: string | null | undefined;
  status: enumTypes.PublishingStatus;
  targetGroups: string[];
  serviceClasses: string[];
  ontologyTerms: OntologyTerm[];
  lifeEvents: string[];
  industrialClasses: string[];
  fundingType: enumTypes.FundingType;
  serviceType: enumTypes.ServiceType;
  chargeType: enumTypes.ChargeType | null;
  languageVersions: ServiceModelLangaugeVersionsValuesType;
  voucherType: enumTypes.VoucherType;
  areaInformation: AreaInformationModel;
  responsibleOrganization: OrganizationModel | null | undefined;
  otherResponsibleOrganizations: OrganizationModel[];
  unificRootId: string | null | undefined;
  generalDescription: GeneralDescriptionModel | null | undefined;
  hasSelfProducers: boolean;
  selfProducers: OrganizationModel[];
  purchaseProducers: OrganizationModel[];
  otherProducers: OrganizationModel[];
  connectedChannels: ConnectableChannel[];
  version: string;
  otherPublishedVersion: OtherVersionType | null | undefined;
  otherModifiedVersion: OtherVersionType | null | undefined;
  lastTranslations: LastTranslationType[];
  languages: string[];
};

export type ServiceModel = ServiceFormValues & {
  serviceClasses: NestedValue<string[]>;
  ontologyTerms: NestedValue<OntologyTerm[]>;
  languageVersions: ServiceModelLanguageVersionsType;
  purchaseProducers: NestedValue<OrganizationModel[]>;
  otherProducers: NestedValue<OrganizationModel[]>;
};

export enum cService {
  id = 'id',
  targetGroups = 'targetGroups',
  fundingType = 'fundingType',
  serviceType = 'serviceType',
  chargeType = 'chargeType',
  languageVersions = 'languageVersions',
  voucherType = 'voucherType',
  languages = 'languages',
  areaInformation = 'areaInformation',
  serviceClasses = 'serviceClasses',
  ontologyTerms = 'ontologyTerms',
  lifeEvents = 'lifeEvents',
  industrialClasses = 'industrialClasses',
  responsibleOrganization = 'responsibleOrganization',
  otherResponsibleOrganizations = 'otherResponsibleOrganizations',
  generalDescription = 'generalDescription',
  hasSelfProducers = 'hasSelfProducers',
  selfProducers = 'selfProducers',
  purchaseProducers = 'purchaseProducers',
  otherProducers = 'otherProducers',
  version = 'version',
  status = 'status',
  unificRootId = 'unificRootId',
  connectedChannels = 'connectedChannels',
  otherModifiedVersion = 'otherModifiedVersion',
  otherPublishedVersion = 'otherPublishedVersion',
  lastTranslations = 'lastTranslations',
}

export type ServiceFormContext = {
  serviceClasses: ClassificationItem[];
  uiLanguage: enumTypes.Language;
};
