import { getKeys } from 'utils';
import { GdApiLanguageModel, GdApiModel } from 'types/api/gdApiModel';
import { PublishingStatus } from 'types/enumTypes';
import { GdLangaugeVersionsType, GeneralDescriptionLanguageModel, GeneralDescriptionModel } from 'types/generalDescriptionTypes';
import { LanguageVersionType } from 'types/languageVersionTypes';
import { createEmptyLanguageVersion } from 'utils/gd';
import { getLVStatus } from 'utils/status';
import { toChargeApiModel, toChargeUiModel } from './chargeMapper';

function toApiLanguageVersion(input: GeneralDescriptionLanguageModel, entityStatus: PublishingStatus): GdApiLanguageModel {
  const { charge, status, ...sameFields } = input;

  return {
    ...sameFields,
    charge: toChargeApiModel(charge),
    status: getLVStatus(status, entityStatus),
  };
}

function toUiLanguageVersion(input: GdApiLanguageModel, entityStatus: PublishingStatus): GeneralDescriptionLanguageModel {
  const { charge, status, ...sameFields } = input;

  return {
    ...sameFields,
    isEnabled: true,
    charge: toChargeUiModel(charge),
    status: getLVStatus(status, entityStatus),
  };
}

function toUiLanguageVersions(source: LanguageVersionType<GdApiLanguageModel>, publishingStatus: PublishingStatus): GdLangaugeVersionsType {
  const result: GdLangaugeVersionsType = {
    en: source.en ? toUiLanguageVersion(source.en, publishingStatus) : createEmptyLanguageVersion('en'),
    fi: source.fi ? toUiLanguageVersion(source.fi, publishingStatus) : createEmptyLanguageVersion('fi'),
    sv: source.sv ? toUiLanguageVersion(source.sv, publishingStatus) : createEmptyLanguageVersion('sv'),
    se: source.se ? toUiLanguageVersion(source.se, publishingStatus) : createEmptyLanguageVersion('se'),
    smn: source.smn ? toUiLanguageVersion(source.smn, publishingStatus) : createEmptyLanguageVersion('smn'),
    sms: source.sms ? toUiLanguageVersion(source.sms, publishingStatus) : createEmptyLanguageVersion('sms'),
  };
  return result;
}

function toApiLanguageVersions(source: GeneralDescriptionModel): LanguageVersionType<GdApiLanguageModel> {
  const result: LanguageVersionType<GdApiLanguageModel> = {};
  const languages = getKeys(source.languageVersions);
  for (const lang of languages) {
    const langVersion = source.languageVersions[lang];
    if (langVersion.isEnabled) {
      result[lang] = toApiLanguageVersion(langVersion, source.status);
    }
  }

  return result;
}

export function toGDUiModel(input: GdApiModel): GeneralDescriptionModel {
  const { languageVersions, status, ontologyTerms, ...sameFields } = input;
  const uiLanguageVersions = toUiLanguageVersions(languageVersions, input.status);

  return {
    ...sameFields,
    languageVersions: uiLanguageVersions,
    status,
    ontologyTerms: ontologyTerms,
  };
}

export function toGDApiModel(input: GeneralDescriptionModel): GdApiModel {
  const { status, ontologyTerms, ...sameFields } = input;
  const convertedLVs = toApiLanguageVersions(input);

  return {
    ...sameFields,
    languageVersions: convertedLVs,
    status,
    ontologyTerms: ontologyTerms,
  };
}
