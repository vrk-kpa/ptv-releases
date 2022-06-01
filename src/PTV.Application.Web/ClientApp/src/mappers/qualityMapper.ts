import { RawDraftContentState } from 'draft-js';
import { v4 as uuidv4 } from 'uuid';
import { DescriptionType, DomainEnumType, Language, VoucherType } from 'types/enumTypes';
import { ConnectionFormModel } from 'types/forms/connectionFormTypes';
import { ServiceLanguageVersionValues, ServiceModel, ServiceVoucherModel } from 'types/forms/serviceFormTypes';
import { AdditionalInformationQualityItem, QualityItem, QualityRequest } from 'types/qualityAgentRequests';
import { itemToArray } from 'utils/arrays';
import { getPlainText, parseRawDescription } from 'utils/draftjs';

const getQualityItem = (value: string | null | undefined, language: Language, type?: string, key?: string): QualityItem | null => {
  if (!value) {
    return null;
  }

  return {
    value,
    language,
    type,
    _key: key,
  };
};

const getRequirements = (version: ServiceLanguageVersionValues): QualityItem[] =>
  !version.conditions ? [] : itemToArray(getQualityItem(version.conditions, version.language));

const getNames = (version: ServiceLanguageVersionValues): QualityItem[] =>
  !version.name ? [] : itemToArray(getQualityItem(version.name, version.language, 'Name', `serviceNames.Name.${version.language}`));

const getVouchers = (version: ServiceLanguageVersionValues): QualityItem[] =>
  version.voucher?.links
    ?.filter((v) => !!v.additionalInformation)
    .map((voucher, index) => ({
      language: version.language,
      _key: `serviceVouchers.${version.language}.${index}`,
      value: voucher.name,
      additionalInformation: voucher.additionalInformation,
    }));

const getAdditionalInformationItem = (value: string, language: Language, key: string): AdditionalInformationQualityItem | null => {
  if (!value) return null;

  return {
    _key: key,
    additionalInformation: [{ value: value, language: language }],
  };
};

const getDescriptionItem = (
  value: RawDraftContentState | null | undefined,
  language: Language,
  type: DescriptionType,
  prefix = 'serviceDescriptions'
): QualityItem | null => {
  if (!value) {
    return null;
  }

  return getQualityItem(getPlainText(value), language, type, `${prefix}.${type}.${language}`);
};

const getDescriptions = (version: ServiceLanguageVersionValues): QualityItem[] => {
  const language = version.language;

  const deadline: RawDraftContentState = version.deadline != null ? JSON.parse(version.deadline) : null;
  const description: RawDraftContentState = version.description != null ? JSON.parse(version.description) : null;
  const periodOfValidity: RawDraftContentState = version.periodOfValidity != null ? JSON.parse(version.periodOfValidity) : null;
  const processingTime: RawDraftContentState = version.processingTime != null ? JSON.parse(version.processingTime) : null;
  const userInstructions: RawDraftContentState = version.userInstructions != null ? JSON.parse(version.userInstructions) : null;
  const chargeInfo: RawDraftContentState = version.charge?.info != null ? JSON.parse(version.charge?.info) : null;

  return [
    getDescriptionItem(deadline, language, 'DeadLine'),
    getDescriptionItem(description, language, 'Description'),
    getDescriptionItem(periodOfValidity, language, 'ValidityTime'),
    getDescriptionItem(processingTime, language, 'ProcessingTime'),
    getQualityItem(version.summary, language, 'Summary', `serviceDescriptions.Summary.${language}`),
    getDescriptionItem(userInstructions, language, 'UserInstruction'),
    getDescriptionItem(chargeInfo, language, 'ChargeTypeAdditionalInfo'),
  ].filter((x) => !!x) as QualityItem[];
};

const getVoucherItem = (voucherType: VoucherType, voucher: ServiceVoucherModel, language: Language): QualityItem[] => {
  if (voucherType === 'Url') {
    return voucher?.links
      ?.filter((v) => !!v.additionalInformation)
      .map((voucher, index) => ({
        language: language,
        _key: `serviceVouchers.${language}.${index}`,
        value: voucher.name,
        additionalInformation: voucher.additionalInformation,
      }));
  }

  if (voucherType === 'NoUrl') {
    return [
      {
        language: language,
        _key: `serviceVouchers.${language}`,
        value: '',
        additionalInformation: voucher.info,
      },
    ];
  }

  return [];
};

const getFromDraftString = (
  value: string | null | undefined,
  language: Language,
  type: DescriptionType,
  prefix = 'connectionDescriptions'
) => {
  if (!value) {
    return null;
  }

  const rawContent = parseRawDescription(value);
  const stringContent = getPlainText(rawContent);
  return getQualityItem(stringContent, language, type, `${prefix}.${type}.${language}`);
};

export const getRequest = (formType: DomainEnumType, model: ServiceModel | ConnectionFormModel, language: Language): QualityRequest => {
  switch (formType) {
    case 'Services':
      return getServiceRequest(model as ServiceModel, language);
    case 'Relations':
      return getConnectionRequest(model as ConnectionFormModel, language);
    default:
      throw Error('Unknown form type!');
  }
};

const getServiceRequest = (model: ServiceModel, language: Language): QualityRequest => {
  // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
  const version = model.languageVersions[language]!;

  return {
    Language: language,
    Profile: 'VRKp',
    Input: {
      // Used for unsaved services without a real ID
      alternativeId: !model.id ? uuidv4() : null,
      GeneralServiceDescription: {
        descriptions: [], // TODO
        names: itemToArray(model.generalDescription?.languageVersions[language]?.name),
        publishingStatus: model.generalDescription?.status ?? 'Published',
        type: 'Service',
      },
      id: model.id,
      requirements: getRequirements(version),
      serviceDescriptions: getDescriptions(version),
      serviceNames: getNames(version),
      serviceVouchers: getVouchers(version),
      status: version.status,
      type: 'Service',
    },
  };
};

const getConnectionRequest = (model: ConnectionFormModel, language: Language): QualityRequest => {
  // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
  const version = model.languageVersions[language]!;
  const result = [
    getFromDraftString(version.description, language, 'Description', 'connectionDescriptions'),
    getFromDraftString(version.charge.info, language, 'ChargeTypeAdditionalInfo', 'connectionDescriptions'),
  ].filter((x) => !!x) as QualityItem[];

  return {
    Language: language,
    Profile: 'VRKp',
    Input: {
      alternativeId: uuidv4(),
      id: undefined,
      status: 'Draft',
      serviceChannels: [
        {
          description: [result],
        },
      ],
    },
  };
};

export { getQualityItem, getDescriptionItem, getVoucherItem, getAdditionalInformationItem };
