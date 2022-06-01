import { useWatch } from 'react-hook-form';
import { CitizenTargetGroup } from 'types/constants';
import { cService } from 'types/forms/serviceFormTypes';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import { isNonEmptyString } from 'utils/strings';
import { translateToLang } from 'utils/translations';
import { AnnotationQueryProps } from './types';

export function useGetLifeEventsForAnnotation(props: AnnotationQueryProps): string[] {
  const { staticData } = useAppContextOrThrow();
  const targetGroups = useWatch({ control: props.control, name: `${cService.targetGroups}` });
  const ids = useWatch({ control: props.control, name: `${cService.lifeEvents}` });

  if (!targetGroups.includes(CitizenTargetGroup)) return [];

  // Return only those life events which have translation
  return staticData.lifeEvents
    .filter((x) => ids.includes(x.id))
    .map((l) => translateToLang(props.language, l.names, null))
    .filter(isNonEmptyString);
}
