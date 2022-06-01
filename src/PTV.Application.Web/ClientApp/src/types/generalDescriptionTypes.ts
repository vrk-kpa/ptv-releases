import { GdServiceChannelModel } from './api/serviceChannelModel';
import { OntologyTerm } from './classificationItemsTypes';
import { ChargeType, GeneralDescriptionType, Language, PublishingStatus, ServiceType } from './enumTypes';
import { ChargeModel } from './forms/chargeType';
import { LinkModel } from './link';

export type GeneralDescriptionLanguageModel = {
  isEnabled: boolean;
  language: Language;
  status: PublishingStatus;
  name: string;
  alternativeName: string;
  description: string | null | undefined;
  backgroundDescription: string | null | undefined;
  generalDescriptionTypeAdditionalInformation: string | null | undefined;
  summary: string;
  userInstructions: string | null | undefined;
  deadline: string | null | undefined;
  processingTime: string | null | undefined;
  periodOfValidity: string | null | undefined;
  conditions: string | null | undefined;
  charge: ChargeModel;
  keywords: string[];
  laws: LinkModel[];
};

export enum cGdLv {
  language = 'language',
  status = 'status',
  name = 'name',
  alternativeName = 'alternativeName',
  description = 'description',
  backgroundDescription = 'backgroundDescription',
  generalDescriptionTypeAdditionalInformation = 'generalDescriptionTypeAdditionalInformation',
  summary = 'summary',
  userInstructions = 'userInstructions',
  deadline = 'deadline',
  processingTime = 'processingTime',
  periodOfValidity = 'periodOfValidity',
  conditions = 'conditions',
  charge = 'charge',
  keywords = 'keywords',
  laws = 'laws',
}

export type GdLangaugeVersionsType = {
  [language in Language]: GeneralDescriptionLanguageModel;
};

export type GeneralDescriptionModel = {
  channels: GdServiceChannelModel[];
  chargeType: ChargeType | null;
  generalDescriptionType: GeneralDescriptionType;
  id: string;
  industrialClasses: string[];
  languageVersions: GdLangaugeVersionsType;
  lifeEvents: string[];
  ontologyTerms: OntologyTerm[];
  status: PublishingStatus;
  serviceClasses: string[];
  serviceType: ServiceType;
  targetGroups: string[];
  unificRootId: string;
};

export enum cGeneralDescription {
  action = 'action',
  alternativeId = 'alternativeId',
  backgroundDescription = 'backgroundDescription',
  chargeType = 'chargeType',
  conditionOfServiceUsage = 'conditionOfServiceUsage',
  connections = 'connections',
  deadLineInformation = 'deadLineInformation',
  description = 'description',
  expireOn = 'expireOn',
  generalDescriptionTypeAdditionalInformation = 'generalDescriptionTypeAdditionalInformation',
  generalDescriptionType = 'generalDescriptionType',
  id = 'id',
  industrialClasses = 'industrialClasses',
  isExpireWarningVisible = 'isExpireWarningVisible',
  isNotUpdatedWarningVisible = 'isNotUpdatedWarningVisible',
  keywords = 'keywords',
  languagesAvailabilities = 'languagesAvailabilities',
  laws = 'laws',
  lifeEvents = 'lifeEvents',
  missingLanguages = 'missingLanguages',
  name = 'name',
  oid = 'oid',
  ontologyTerms = 'ontologyTerms',
  previousInfo = 'previousInfo',
  processingTimeInformation = 'processingTimeInformation',
  publishAction = 'publishAction',
  publishingStatus = 'publishingStatus',
  serviceClasses = 'serviceClasses',
  serviceType = 'serviceType',
  shortDescription = 'shortDescription',
  targetGroups = 'targetGroups',
  translationAvailability = 'translationAvailability',
  userInstruction = 'userInstruction',
  unificRootId = 'unificRootId',
  userName = 'userName',
  validityTimeInformation = 'validityTimeInformation',
  version = 'version',
  languageVersions = 'languageVersions',
}
