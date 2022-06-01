import { cService } from './forms/serviceFormTypes';

export const channelType = ['ServiceLocation', 'PrintableForm', 'WebPage', 'EChannel', 'Phone'] as const;
export type ChannelType = typeof channelType[number];

export const formType = ['Service', 'GeneralDescription', 'Organization', 'ServiceCollection', ...channelType] as const;
export type FormType = typeof formType[number];

export const mainEntityType = ['Service', 'Channel', 'ServiceCollection', 'Organization', 'GeneralDescription'] as const;
export type MainEntityType = typeof mainEntityType[number];

export const chargetype = ['Free', 'Charged', 'Other'] as const;
export type ChargeType = typeof chargetype[number];

export type ConnectionType = 'NotCommon' | 'CommonForAll' | 'CommonFor';

export const fundingtype = ['MarketFunded', 'PubliclyFunded'] as const;
export type FundingType = typeof fundingtype[number];

export const language = ['fi', 'sv', 'en', 'se', 'smn', 'sms'] as const;
export type Language = typeof language[number];

export const translatableLanguage = ['fi', 'sv', 'en'] as const;
export type TranslatableLanguage = typeof translatableLanguage[number];

export type PublishingStatus = 'Draft' | 'Modified' | 'Published' | 'OldPublished' | 'Deleted' | 'Removed';

export const allowedLanguageVersionStatusesToPublish: PublishingStatus[] = ['Draft', 'Modified', 'Published'];

export const servicetype = ['ProfessionalQualifications', 'Service', 'PermissionAndObligation'] as const;
export type ServiceType = typeof servicetype[number];

export type AppEnvironment = 'Test' | 'Dev' | 'Qa' | 'Trn' | 'Prod';

export const vouchertype = ['NotUsed', 'NoUrl', 'Url'] as const;
export type VoucherType = typeof vouchertype[number];

export enum componentMode {
  DISPLAY = 'display',
  SELECT = 'select',
  SUMMARY = 'summary',
}

export type MessageType = 'info' | 'error' | 'generalDescription';

export const areaInformationType = ['WholeCountry', 'WholeCountryExceptAlandIslands', 'AreaType'] as const;
export type AreaInformationType = typeof areaInformationType[number];

export const areaType = ['Municipality', 'Province', 'BusinessRegions', 'HospitalRegions'] as const;
export type AreaType = typeof areaType[number];

export const organizationType = ['State', 'Municipality', 'RegionalOrganization', 'Organization', 'Company', 'TT1', 'TT2'] as const;
export type OrganizationType = typeof organizationType[number];

export const roleType = ['Eeva', 'Pete', 'Shirley'] as const;
export type UserRole = typeof roleType[number];

export const generalDescriptionType = ['Municipality', 'BusinessSubregion', 'Church'] as const;
export type GeneralDescriptionType = typeof generalDescriptionType[number];

export const classificationItemTypes = [
  cService.serviceClasses,
  cService.lifeEvents,
  cService.industrialClasses,
  cService.ontologyTerms,
] as const;
export type ClassificationItemTypes = typeof classificationItemTypes[number];

export const domainEnumType = [
  'Organizations',
  'OrganizationStructure',
  'Services',
  'Channels',
  'ServiceCollections',
  'Relations',
  'GeneralDescriptions',
  'CurrentIssues',
  'PreviousIssues',
  'None',
] as const;

export type DomainEnumType = typeof domainEnumType[number];

export enum YesNoType {
  YES = 'Yes',
  NO = 'No',
}

export type ValidationErrorType = 'mandatoryField' | 'publishedOrganizationLanguageMandatoryField' | 'publishedMandatoryField';

export const permissionGroup = [
  'organizations',
  'organizationStructure',
  'previousIssues',
  'currentIssues',
  'generalDescriptions',
  'relations',
  'channels',
  'serviceCollections',
  'services',
  'organizationsMassTool',
  'generalDescriptionsMassTool',
  'servicesMassTool',
  'serviceCollectionsMassTool',
  'channelsMassTool',
  'adminSection',
] as const;
export type PermissionGroup = typeof permissionGroup[number];

export enum PermissionEnum {
  None = 0,
  Create = 1,
  Read = 2,
  Update = 4,
  Delete = 8,
  Publish = 16,
  All = 255,
}

export const phoneType = ['Phone', 'Sms', 'Fax'] as const;
export type PhoneType = typeof phoneType[number];

export const phoneNumberDialCodeType = ['Normal', 'NationalWithoutDialCode'] as const;
export type PhoneNumberDialCodeType = typeof phoneNumberDialCodeType[number];

export const addressType = ['Street', 'PostOfficeBox', 'Foreign', 'NoAddress', 'Other'] as const;
export type AddressType = typeof addressType[number];

export const addressCharacterType = ['Visiting', 'Postal', 'Delivery'] as const;
export type AddressCharacterType = typeof addressCharacterType[number];

export const coordinateType = ['Main', 'User', 'AccessibilityRegister'] as const;
export type CoordinateType = typeof coordinateType[number];

export const modeType = ['view', 'edit'] as const;
export type Mode = typeof modeType[number];

export const serviceHourType = ['Standard', 'Exception', 'Special'] as const;
export type ServiceHourType = typeof serviceHourType[number];

// Do not change the ordering, most likely there is code that assumes that Monday is the first day of week
export const weekDayType = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'] as const;
export type Weekday = typeof weekDayType[number];

export const holidayType = [
  'NewYearEve',
  'NewYearDay',
  'Epiphany',
  'MaundyThursday',
  'GoodFriday',
  'EasterSunday',
  'EasterMonday',
  'MayDayEve',
  'MayDay',
  'AscensionDay',
  'WhitSunday',
  'MidsummerEve',
  'MidsummerDay',
  'AllSaintsDay',
  'IndependenceDay',
  'ChristmasEve',
  'ChristmasDay',
  'SecondDayOfChristmas',
] as const;
export type HolidayEnum = typeof holidayType[number];

export const translationStateType = [
  'ReadyToSend',
  'SendError',
  'Sent',
  'InProgress',
  'Arrived',
  'EstimateCompleted',
  'FileError',
  'DeliveredFileError',
  'FailForInvestigation',
  'ArrivedIsConfirm',
  'RequestForRepetition',
  'RequestForCancel',
  'Canceled',
] as const;

export type TranslationStateType = typeof translationStateType[number];

export const inTranslationStateType = [
  'ReadyToSend',
  'Sent',
  'InProgress',
  'FileError',
  'SendError',
  'DeliveredFileError',
  'FailForInvestigation',
  'RequestForRepetition',
  'RequestForCancel',
];

export type InTranslationStateType = typeof inTranslationStateType[number];

export const subEntityType = [
  'Unknown',
  'Organization',
  'SericeCollection',
  ...channelType,
  ...servicetype,
  ...generalDescriptionType,
] as const;
export type SubEntityType = typeof subEntityType[number];

export const descriptionType = [
  'Description',
  'Summary',
  'UserInstruction',
  'DeadLine',
  'ProcessingTime',
  'ValidityTime',
  'ChargeTypeAdditionalInfo',
  'BackgroundDescription',
  'Conditions',
] as const;
export type DescriptionType = typeof descriptionType[number];

export type InputStatus = 'default' | 'error' | 'warning' | 'success';
