import { useWatch } from 'react-hook-form';
import { useIsFetching } from 'react-query';
import { AnnotationToolRequest } from 'types/annotationToolTypes';
import { cLv, cService } from 'types/forms/serviceFormTypes';
import { getPlainText, parseRawDescription } from 'utils/draftjs';
import { AnnotationQueryProps } from './types';
import { useGetIndustrialClassesForAnnotation } from './useGetIndustrialClassesForAnnotation';
import { useGetLifeEventsForAnnotation } from './useGetLifeEventsForAnnotation';
import { useGetServiceClassesForAnnotation } from './useGetServiceClassesForAnnotation';
import { useGetTargetGroupsForAnnotation } from './useGetTargetGroupsForAnnotation';

type AnnotationQueryResult = {
  isEnabled: boolean;
  requestData: AnnotationToolRequest | null;
};

export function useAnnotationToolDependency(props: AnnotationQueryProps): AnnotationQueryResult {
  const id = useWatch({ control: props.control, name: `${cService.id}` });
  const name = useWatch({ control: props.control, name: `${cService.languageVersions}.${props.language}.${cLv.name}` });
  const shortDescription = useWatch({ control: props.control, name: `${cService.languageVersions}.${props.language}.${cLv.summary}` });
  const description = useWatch({ control: props.control, name: `${cService.languageVersions}.${props.language}.${cLv.description}` });
  const serviceClasses = useGetServiceClassesForAnnotation(props);
  const targetGroups = useGetTargetGroupsForAnnotation(props);
  const lifeEvents = useGetLifeEventsForAnnotation(props);
  const industrialClasses = useGetIndustrialClassesForAnnotation(props);
  const isFetchingAnnotations = useIsFetching(['ontology', 'annotations']);

  const request: AnnotationToolRequest = {
    id: id ?? null,
    name: name,
    shortDescription: shortDescription,
    description: getPlainText(parseRawDescription(description)),
    targetGroups: targetGroups,
    serviceClasses: serviceClasses,
    lifeEvents: lifeEvents,
    industrialClasses: industrialClasses,
    languageCode: props.language,
  };

  return {
    isEnabled: getIsEnabled(request, isFetchingAnnotations),
    requestData: request,
  };
}

function getIsEnabled(request: AnnotationToolRequest, isFetching: number): boolean {
  if (isFetching > 0) return false;
  if (request.targetGroups.length < 1) return false;
  if (request.serviceClasses.length < 1) return false;
  if (!request.name) return false;
  if (!request.shortDescription) return false;
  if (!request.description) return false;
  return true;
}
