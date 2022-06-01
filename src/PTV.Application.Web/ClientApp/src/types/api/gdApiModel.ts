import { OntologyTerm } from 'types/classificationItemsTypes';
import * as enumTypes from 'types/enumTypes';
import { LanguageVersionType } from 'types/languageVersionTypes';
import { LinkModel } from 'types/link';
import { ChargeApiModel } from './chargeApiModel';
import { GdServiceChannelModel } from './serviceChannelModel';

export type GdApiLanguageModel = {
  language: enumTypes.Language;
  status: enumTypes.PublishingStatus;
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
  charge: ChargeApiModel;
  keywords: string[];
  laws: LinkModel[];
};

export type GdApiModel = {
  id: string;
  unificRootId: string;
  generalDescriptionType: enumTypes.GeneralDescriptionType;
  chargeType: enumTypes.ChargeType | null;
  serviceType: enumTypes.ServiceType;
  targetGroups: string[];
  industrialClasses: string[];
  serviceClasses: string[];
  lifeEvents: string[];
  ontologyTerms: OntologyTerm[];
  channels: GdServiceChannelModel[];
  languageVersions: LanguageVersionType<GdApiLanguageModel>;
  status: enumTypes.PublishingStatus;
};
