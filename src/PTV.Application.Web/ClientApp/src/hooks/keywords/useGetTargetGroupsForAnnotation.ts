import { useWatch } from 'react-hook-form';
import { Language } from 'types/enumTypes';
import { cService } from 'types/forms/serviceFormTypes';
import { TargetGroup } from 'types/targetGroupTypes';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import { isNonEmptyString } from 'utils/strings';
import { translateToLang } from 'utils/translations';
import { AnnotationQueryProps } from './types';

export function useGetTargetGroupsForAnnotation(props: AnnotationQueryProps): string[] {
  const { staticData } = useAppContextOrThrow();
  const targetGroups = useWatch({ control: props.control, name: `${cService.targetGroups}` });
  const gd = useWatch({ control: props.control, name: `${cService.generalDescription}` });
  const combinedTargetGroups = gd?.targetGroups ? [...targetGroups, ...gd.targetGroups] : targetGroups;
  return getTargetGroupNames(combinedTargetGroups, props.language, staticData.targetGroups);
}

function getTargetGroupNames(targetGroupIds: string[], language: Language, targetGroups: TargetGroup[]): string[] {
  return targetGroups
    .filter((x) => targetGroupIds.includes(x.code))
    .map((x) => translateToLang(language, x.names, null))
    .filter(isNonEmptyString);
}
