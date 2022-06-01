import { useWatch } from 'react-hook-form';
import { BusinessTargetGroup } from 'types/constants';
import { cService } from 'types/forms/serviceFormTypes';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import { isNonEmptyString } from 'utils/strings';
import { translateToLang } from 'utils/translations';
import { AnnotationQueryProps } from './types';

export function useGetIndustrialClassesForAnnotation(props: AnnotationQueryProps): string[] {
  const { staticData } = useAppContextOrThrow();
  const targetGroups = useWatch({ control: props.control, name: `${cService.targetGroups}` });
  const ids = useWatch({ control: props.control, name: `${cService.industrialClasses}` });

  if (!targetGroups.includes(BusinessTargetGroup)) return [];

  // Return only those industrial classes which have translation
  return staticData.industrialClasses
    .filter((x) => ids.includes(x.id))
    .map((l) => translateToLang(props.language, l.names, null))
    .filter(isNonEmptyString);
}
