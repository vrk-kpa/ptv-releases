import { cloneDeep } from 'lodash';
import { getKeys } from 'utils';
import { ServiceApiLanguageModel } from 'types/api/serviceApiModel';
import { Language, language } from 'types/enumTypes';
import { ServiceFormValues, ServiceLanguageVersionValues, ServiceModelLangaugeVersionsValuesType } from 'types/forms/serviceFormTypes';
import { LanguageVersionType, LanguageVersionWithName } from 'types/languageVersionTypes';
import { OrganizationModel } from 'types/organizationTypes';
import { CreateEmptyAreaInformation } from 'utils/areaInformation';
import { utcNowISO } from './date';
import { LanguagePriorities } from './languages';

type LanguageVersions = LanguageVersionType<ServiceApiLanguageModel>;
type Selector<T> = (lv: ServiceApiLanguageModel) => T | null | undefined;

export function getServiceValue<T>(lvs: LanguageVersions, wantedLanguage: Language, selector: Selector<T>, fallBackValue: T): T {
  const languages = [wantedLanguage].concat(LanguagePriorities);
  for (const lang of languages) {
    const lv = lvs[lang];
    if (lv) {
      const value = selector(lv);
      if (value) {
        return value;
      }
    }
  }

  return fallBackValue;
}

type ServiceModelSelector<T> = (lv: ServiceLanguageVersionValues) => T | null | undefined;

export function getServiceModelValue<T>(
  lvs: ServiceModelLangaugeVersionsValuesType,
  wantedLanguage: Language,
  selector: ServiceModelSelector<T>,
  fallBackValue: T
): T {
  const languages = [wantedLanguage].concat(LanguagePriorities);
  for (const lang of languages) {
    const lv = lvs[lang];
    if (lv) {
      const value = selector(lv);
      if (value) {
        return value;
      }
    }
  }

  return fallBackValue;
}

export function getEnabledLanguages(languageVersions: ServiceModelLangaugeVersionsValuesType): Language[] {
  return getKeys(languageVersions).filter((l) => languageVersions[l].isEnabled);
}

export function getEnabledLanguagesWithName(languageVersions: ServiceModelLangaugeVersionsValuesType): LanguageVersionWithName[] {
  return getKeys(languageVersions)
    .filter((l) => languageVersions[l].isEnabled)
    .map((l) => ({ name: languageVersions[l].name, language: l }));
}

export function getEnabledLanguagesByPriority(languageVersions: ServiceModelLangaugeVersionsValuesType): Language[] {
  const enabledLanguages = getEnabledLanguages(languageVersions);
  return LanguagePriorities.filter((x) => enabledLanguages.includes(x));
}

export function createEmptyServiceUiModel(responsibleOrganization: OrganizationModel | null | undefined): ServiceFormValues {
  const languageVersions: ServiceModelLangaugeVersionsValuesType = {
    en: createEmptyLanguageVersion('en'),
    fi: createEmptyLanguageVersion('fi'),
    sv: createEmptyLanguageVersion('sv'),
    se: createEmptyLanguageVersion('se'),
    smn: createEmptyLanguageVersion('smn'),
    sms: createEmptyLanguageVersion('sms'),
  };

  return {
    status: 'Draft',
    fundingType: 'PubliclyFunded',
    serviceType: 'Service',
    targetGroups: [],
    serviceClasses: [],
    ontologyTerms: [],
    lifeEvents: [],
    industrialClasses: [],
    id: null,
    chargeType: null,
    voucherType: 'NotUsed',
    areaInformation: CreateEmptyAreaInformation(),
    languageVersions: languageVersions,
    responsibleOrganization: responsibleOrganization,
    otherResponsibleOrganizations: [],
    unificRootId: null,
    generalDescription: null,
    hasSelfProducers: Boolean(responsibleOrganization),
    selfProducers: responsibleOrganization ? [responsibleOrganization] : [],
    purchaseProducers: [],
    otherProducers: [],
    connectedChannels: [],
    version: '0.1',
    otherModifiedVersion: null,
    otherPublishedVersion: null,
    lastTranslations: [],
    languages: [],
  };
}

export function createEmptyLanguageVersion(language: Language): ServiceLanguageVersionValues {
  return {
    isEnabled: false,
    language: language,
    status: 'Draft',
    name: ``,
    hasAlternativeName: false,
    alternativeName: '',
    summary: ``,
    description: null,
    userInstructions: null,
    conditions: null,
    deadline: null,
    processingTime: null,
    periodOfValidity: null,
    laws: [],
    charge: {
      info: '',
    },
    voucher: {
      info: '',
      links: [],
      linksErrorTag: undefined,
    },
    keywords: [],
    purchaseProducers: [],
    otherProducers: [],
    modified: utcNowISO(),
    modifiedBy: '',
    scheduledArchive: null,
    scheduledPublish: null,
    translationAvailability: null,
  };
}

export function copyService(
  source: ServiceFormValues,
  responsibleOrganization: OrganizationModel | null | undefined,
  wantedLanguages: Language[]
): ServiceFormValues {
  const copy = cloneDeep(source);
  copy.id = null;
  copy.status = 'Draft';
  copy.responsibleOrganization = responsibleOrganization;
  copy.unificRootId = null;
  copy.generalDescription = null;
  copy.version = '0.1';
  copy.otherPublishedVersion = null;
  copy.otherModifiedVersion = null;
  copy.lastTranslations = [];
  copy.hasSelfProducers = Boolean(responsibleOrganization);
  copy.selfProducers = responsibleOrganization ? [responsibleOrganization] : [];

  for (const lang of language) {
    const lv = copy.languageVersions[lang];
    if (wantedLanguages.includes(lv.language)) {
      copy.languageVersions[lang] = copyLanguageVersion(lv);
    } else {
      copy.languageVersions[lang] = createEmptyLanguageVersion(lang);
    }
  }

  return copy;
}

function copyLanguageVersion(source: ServiceLanguageVersionValues): ServiceLanguageVersionValues {
  const copy = cloneDeep(source);
  copy.status = 'Draft';
  copy.modified = utcNowISO();
  copy.modifiedBy = '';
  copy.scheduledPublish = null;
  copy.scheduledArchive = null;
  copy.translationAvailability = null;
  return copy;
}
