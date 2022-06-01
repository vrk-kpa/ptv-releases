import * as enumTypes from 'types/enumTypes';
import { LanguageVersionType } from 'types/languageVersionTypes';
import { OrganizationModel } from 'types/organizationTypes';

export type ServiceChannel = {
  id: string;
  unificRootId: string;
  channelType: enumTypes.ChannelType;
  organization: OrganizationModel | null | undefined;
  connectionType: enumTypes.ConnectionType;
  modifiedBy: string;
  modified: string;
  onlineAuthenticationRequired: boolean;
  electronicSigningRequired: boolean;
  numberOfSignaturesRequired: number;
  businessRegions: string[];
  hospitalRegions: string[];
  provinces: string[];
  municipalities: string[];
  languages: string[];
  languageVersions: LanguageVersionType<ServiceChannelLanguageVersion>;
  connections: ServiceChannelConnection[];
};

export type ServiceChannelLanguageVersion = {
  status: enumTypes.PublishingStatus;
  language: enumTypes.Language;
  name: string;
  alternativeName: string;
  description: string;
  shortDescription: string;
  website: string;
  formIdentifier: string;
  scheduledPublish: string | null | undefined;
};

export type ConnectableChannel = {
  id: string;
  unificRootId: string;
  channelType: enumTypes.ChannelType;
  isASTIConnection: boolean;
  organization: OrganizationModel | null | undefined;
  connectionType: enumTypes.ConnectionType;
  modifiedBy: string;
  modified: string;
  languageVersions: LanguageVersionType<ConnectableChannelLanguageVersion>;
};

export type ConnectableChannelLanguageVersion = {
  status: enumTypes.PublishingStatus;
  language: enumTypes.Language;
  name: string;
  modifiedBy: string;
  modified: string;
};

export type GdServiceChannelModel = {
  id: string;
  unificRootId: string;
  channelType: enumTypes.ChannelType;
  organization: OrganizationModel | null | undefined;
  connectionType: enumTypes.ConnectionType;
  modifiedBy: string;
  modified: string;
  languageVersions: LanguageVersionType<GdServiceChannelLvModel>;
};

export type GdServiceChannelLvModel = {
  status: enumTypes.PublishingStatus;
  language: enumTypes.Language;
  name: string;
  modifiedBy: string;
  modified: string;
};

export type ServiceChannelConnection = {
  serviceId: string;
  serviceUnificRootId: string;
  serviceOrganization: OrganizationModel;
  languageVersions: LanguageVersionType<ServiceChannelConnectionLanguageVersion>;
};

export type ServiceChannelConnectionLanguageVersion = {
  language: enumTypes.Language;
  serviceName: string;
};
