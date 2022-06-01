import { ClassificationItem } from 'types/classificationItemsTypes';
import { cService } from 'types/forms/serviceFormTypes';
import { StaticData } from 'types/settingTypes';

export function getClassificationItem(
  id: string,
  data: StaticData,
  classification: cService.serviceClasses | cService.ontologyTerms | cService.lifeEvents | cService.industrialClasses
): ClassificationItem | undefined {
  if (classification === cService.serviceClasses) return data.serviceClasses.find((x) => x.id === id);
  if (classification === cService.ontologyTerms) return data.ontologyTerms.find((x) => x.id === id);
  if (classification === cService.lifeEvents) return data.lifeEvents.find((x) => x.id === id);
  if (classification === cService.industrialClasses) return data.industrialClasses.find((x) => x.id === id);
  throw Error(`Unsupported classification: ${classification}`);
}

export function getClassificationItems(
  ids: string[],
  data: StaticData,
  classification: cService.serviceClasses | cService.ontologyTerms | cService.lifeEvents | cService.industrialClasses
): ClassificationItem[] {
  return ids.map((x) => getClassificationItem(x, data, classification)).filter((item): item is ClassificationItem => !!item);
}
