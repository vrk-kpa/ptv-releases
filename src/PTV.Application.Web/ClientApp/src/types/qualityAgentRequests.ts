import { Language, MainEntityType, PublishingStatus } from './enumTypes';

export const qualityProfile = [
  'VRKp', // service
  'VRKak', // channel
  'VRKo', // organization
] as const;
export type QualityProfile = typeof qualityProfile[number];

export type QualityItem = {
  language: Language;
  type?: string | null | undefined;
  value: string;
  _key?: string | null | undefined;
  additionalInformation?: string | QualityItem | null | undefined;
};

export type AdditionalInformationQualityItem = {
  _key: string;
  additionalInformation?: QualityItem[];
};

type QualityGDRequest = {
  descriptions: QualityItem[];
  names: string[];
  publishingStatus: PublishingStatus;
  type: MainEntityType;
};

type QualityServiceRequest = {
  alternativeId?: string | null | undefined;
  GeneralServiceDescription: QualityGDRequest;
  id: string | null | undefined;
  requirements: QualityItem[];
  serviceDescriptions: QualityItem[];
  serviceNames: QualityItem[];
  serviceVouchers: QualityItem[];
  status: PublishingStatus;
  type: MainEntityType;
  organizations: AdditionalInformationQualityItem[];
};

type QualityConnectionDescription = {
  description: QualityItem[][];
};

type QualityConnectionRequest = {
  serviceChannels: QualityConnectionDescription[];
};

// TODO: placeholder
type QualityChannelRequest = {
  alternativeId?: string | null | undefined;
  id: string | null | undefined;
  type: MainEntityType;
};

// TODO: placeholder
type QualityOrganizationRequest = {
  alternativeId?: string | null | undefined;
  id: string | null | undefined;
  type: MainEntityType;
};

export type QualityRequest = {
  Language: Language;
  Profile: QualityProfile;
  Input: QualityServiceRequest | QualityChannelRequest | QualityOrganizationRequest | QualityConnectionRequest;
};
