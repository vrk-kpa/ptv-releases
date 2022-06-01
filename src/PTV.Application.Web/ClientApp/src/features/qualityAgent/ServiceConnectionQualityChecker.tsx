import React, { useContext, useEffect } from 'react';
import { Control, useWatch } from 'react-hook-form';
import { RawDraftContentState } from 'draft-js';
import { DomainEnumType, Language } from 'types/enumTypes';
import { ConnectionFormModel, cC } from 'types/forms/connectionFormTypes';
import { cLv } from 'types/forms/serviceFormTypes';
import { QualityItem, QualityRequest } from 'types/qualityAgentRequests';
import { DispatchQualityAgentContext, qualityChecked } from 'context/qualityAgent';
import { qualityCheckLoading } from 'context/qualityAgent/actions';
import { useCheckQuality } from 'hooks/queries/useCheckQuality';
import { useDebounceObject } from 'hooks/useDebounceObject';
import { getDescriptionItem } from 'mappers/qualityMapper';

type ServiceConnectionQualityCheckerProps = {
  language: Language;
  enabled: boolean;
  formType: DomainEnumType;
  control: Control<ConnectionFormModel>;
};

export default function ServiceConnectionQualityChecker(props: ServiceConnectionQualityCheckerProps): React.ReactElement {
  const dispatch = useContext(DispatchQualityAgentContext);

  const description = useWatch({ control: props.control, name: `${cC.languageVersions}.${props.language}.${cLv.description}` });
  const chargeTypeAdditionalInfo = useWatch({
    control: props.control,
    name: `${cC.languageVersions}.${props.language}.${cLv.charge}`,
  }).info;

  const connectionData: QualityItem[][] = [
    [
      getDescriptionItem(
        description ? (JSON.parse(description) as RawDraftContentState) : null,
        props.language,
        'Description',
        'connectionDescriptions'
      ),
      getDescriptionItem(
        chargeTypeAdditionalInfo ? (JSON.parse(chargeTypeAdditionalInfo) as RawDraftContentState) : null,
        props.language,
        'ChargeTypeAdditionalInfo',
        'connectionDescriptions'
      ),
    ].filter((x) => !!x) as QualityItem[],
  ];

  const request: QualityRequest = {
    Language: props.language,
    Profile: 'VRKp',
    Input: {
      serviceChannels: [
        {
          description: connectionData,
        },
      ],
    },
  };

  const debouncedRequest = useDebounceObject(request, 500);

  const query = useCheckQuality(debouncedRequest, {
    enabled: props.enabled,
    refetchOnMount: true, // force refetch when language tab is changed
    onSuccess: (response) => qualityChecked(dispatch, response?.data ?? []),
  });

  const isLoading = query.isLoading;

  useEffect(() => {
    qualityCheckLoading(dispatch, isLoading);
  }, [dispatch, isLoading]);

  return <></>;
}
