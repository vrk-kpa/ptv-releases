import { UseFormSetValue, UseFormTrigger } from 'react-hook-form';
import { OntologyTerm } from 'types/classificationItemsTypes';
import { ChargeType, GeneralDescriptionType, Language, ServiceType } from 'types/enumTypes';
import { ServiceModel, cLv, cService } from 'types/forms/serviceFormTypes';
import { GeneralDescriptionModel } from 'types/generalDescriptionTypes';
import { GdSearchQuery, GdSearchResult } from 'hooks/queries/useSearchGeneralDescriptions';

const ValidChargeTypes: (null | ChargeType)[] = ['Free', 'Charged'];

export type SearchParameterState = {
  enabled: boolean;
  searchText: string;
  serviceType: ServiceType | undefined;
  areaType: GeneralDescriptionType | undefined;
  result: GdSearchResult | null | undefined;
  pageNumber: number;
};

type BaseGdParams = {
  setValue: UseFormSetValue<ServiceModel>;
  trigger: UseFormTrigger<ServiceModel>;
  enabledLanguages: Language[];
};

type CopyGdValuesToService = BaseGdParams & {
  gd: GeneralDescriptionModel;
  useGdName: boolean;
  serviceLifeEvents: string[];
  serviceType: ServiceType;
  serviceClassIds: string[];
  ontologyTerms: OntologyTerm[];
};

export function getSearchParameters(parameters: SearchParameterState, sortLanguage: Language): GdSearchQuery {
  return {
    name: parameters.searchText,
    sortLanguage: sortLanguage,
    pageNumber: parameters.pageNumber,
    generalDescriptionType: parameters.areaType,
    serviceType: parameters.serviceType,
  };
}

export function getNumberOfPages(searchResult: GdSearchResult | undefined | null): number {
  if (!searchResult) {
    return 0;
  }

  const perPage = searchResult.maxPageCount;
  const total = searchResult.count;
  if (perPage === 0 || total === 0) {
    return 0;
  }

  return Math.ceil(total / perPage);
}

export function getCurrentPageNumber(searchResult: GdSearchResult | undefined | null): number {
  if (searchResult?.pageNumber) {
    return searchResult.pageNumber;
  }

  return 1;
}

export function selectGeneralDescription(params: CopyGdValuesToService): void {
  params.setValue(`${cService.generalDescription}`, params.gd);
  params.setValue(`${cService.serviceType}`, params.gd.serviceType);
  setChargeInfo(params);
  setServiceClasses(params);
  setOntologyTerms(params);
  setServiceNames(params);
  copyLifeEvents(params);
  triggerValidationOfRelatedFields(params);
}

export function unselectGeneralDescription(params: BaseGdParams): void {
  params.setValue(`${cService.generalDescription}`, null);
  triggerValidationOfRelatedFields(params);
}

function setServiceClasses(params: CopyGdValuesToService): void {
  // The GD might contain same service classes user has already selected
  // for service. Remove duplicates from the service
  const ids = params.serviceClassIds.filter((id) => !params.gd.serviceClasses.includes(id));
  params.setValue(`${cService.serviceClasses}`, ids, { shouldValidate: true });
}

function setOntologyTerms(params: CopyGdValuesToService): void {
  // The GD might contain same ontology terms user has already selected
  // for service. Remove duplicates from the service
  const ontologyTerms = params.ontologyTerms.filter((term) => !params.gd.ontologyTerms.some((x) => x.id === term.id));
  params.setValue(`${cService.ontologyTerms}`, ontologyTerms, { shouldValidate: true });
}

function setChargeInfo(params: CopyGdValuesToService): void {
  if (ValidChargeTypes.includes(params.gd.chargeType)) {
    params.setValue(`${cService.chargeType}`, params.gd.chargeType);
  }
}

function setServiceNames(params: CopyGdValuesToService): void {
  if (!params.useGdName) return;

  for (const lang of params.enabledLanguages) {
    const gdLv = params.gd.languageVersions[lang];
    if (gdLv?.name) {
      params.setValue(`${cService.languageVersions}.${lang}.${cLv.name}`, gdLv.name);
      params.trigger(`${cService.languageVersions}.${lang}.${cLv.name}`);
    }
  }
}

function copyLifeEvents(params: CopyGdValuesToService): void {
  if (params.gd.lifeEvents.length === 0) return;
  const all = params.gd.lifeEvents.concat(params.serviceLifeEvents);
  const unique = [...new Set(all)];
  params.setValue(`${cService.lifeEvents}`, unique);
}

function triggerValidationOfRelatedFields(params: BaseGdParams): void {
  params.trigger(`${cService.ontologyTerms}`);
}
