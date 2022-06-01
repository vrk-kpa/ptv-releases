import { useWatch } from 'react-hook-form';
import { ClassificationItem } from 'types/classificationItemsTypes';
import { Language } from 'types/enumTypes';
import { cService } from 'types/forms/serviceFormTypes';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import { isNonEmptyString } from 'utils/strings';
import { translateToLang } from 'utils/translations';
import { AnnotationQueryProps } from './types';

export function useGetServiceClassesForAnnotation(props: AnnotationQueryProps): string[] {
  const { staticData } = useAppContextOrThrow();
  const serviceClasses = useWatch({ control: props.control, name: `${cService.serviceClasses}` });
  const gd = useWatch({ control: props.control, name: `${cService.generalDescription}` });
  const combinedServiceClasses = gd?.serviceClasses ? [...serviceClasses, ...gd.serviceClasses] : serviceClasses;
  return getServiceClassNames(combinedServiceClasses, props.language, staticData.serviceClasses);
}

function getServiceClassNames(serviceClassIds: string[], language: Language, serviceClasses: ClassificationItem[]): string[] {
  return serviceClasses
    .filter((x) => serviceClassIds.includes(x.id))
    .map((x) => translateToLang(language, x.names, null))
    .filter(isNonEmptyString);
}
