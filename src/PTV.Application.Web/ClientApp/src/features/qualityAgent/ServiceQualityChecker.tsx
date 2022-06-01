import React, { useContext, useEffect, useState } from 'react';
import { Control, useWatch } from 'react-hook-form';
import { RawDraftContentState } from 'draft-js';
import { v4 as uuidv4 } from 'uuid';
import { DomainEnumType, Language } from 'types/enumTypes';
import { ServiceModel, ServiceProducer, cLv, cService } from 'types/forms/serviceFormTypes';
import { AdditionalInformationQualityItem, QualityItem, QualityRequest } from 'types/qualityAgentRequests';
import { DispatchQualityAgentContext, qualityChecked } from 'context/qualityAgent';
import { qualityCheckLoading } from 'context/qualityAgent/actions';
import { useCheckQuality } from 'hooks/queries/useCheckQuality';
import { useDebounceObject } from 'hooks/useDebounceObject';
import { getAdditionalInformationItem, getDescriptionItem, getQualityItem, getVoucherItem } from 'mappers/qualityMapper';
import { itemToArray } from 'utils/arrays';

type ServiceQualityCheckerProps = {
  language: Language;
  enabled: boolean;
  formType: DomainEnumType;
  control: Control<ServiceModel>;
};

export default function ServiceQualityChecker(props: ServiceQualityCheckerProps): React.ReactElement {
  const { language, control } = props;
  const dispatch = useContext(DispatchQualityAgentContext);
  const [alternativeId] = useState<string>(uuidv4());

  const modelId = useWatch({ control: control, name: `${cService.id}` });
  const name = useWatch({ control: control, name: `${cService.languageVersions}.${language}.${cLv.name}` });
  const status = useWatch({ control: control, name: `${cService.status}` });
  const versionConditions = useWatch({ control: control, name: `${cService.languageVersions}.${language}.${cLv.conditions}` });
  const deadline = useWatch({ control: control, name: `${cService.languageVersions}.${language}.${cLv.deadline}` });
  const periodOfValidity = useWatch({
    control: control,
    name: `${cService.languageVersions}.${language}.${cLv.periodOfValidity}`,
  });
  const processingTime = useWatch({ control: control, name: `${cService.languageVersions}.${language}.${cLv.processingTime}` });
  const userInstructions = useWatch({
    control: control,
    name: `${cService.languageVersions}.${language}.${cLv.userInstructions}`,
  });
  const chargeInfo = useWatch({ control: control, name: `${cService.languageVersions}.${language}.${cLv.charge}` })?.info;
  const summary = useWatch({ control: control, name: `${cService.languageVersions}.${language}.${cLv.summary}` });
  const voucher = useWatch({ control: control, name: `${cService.languageVersions}.${language}.${cLv.voucher}` });
  const description = useWatch({ control: control, name: `${cService.languageVersions}.${language}.${cLv.description}` });

  const voucherType = useWatch({ control: control, name: `${cService.voucherType}` });

  const purchaseProducers = useWatch({ control: control, name: `${cService.languageVersions}.${language}.${cLv.purchaseProducers}` });
  const otherProducers = useWatch({ control: control, name: `${cService.languageVersions}.${language}.${cLv.otherProducers}` });

  const generalDescriptionLanguageVersion = useWatch({ control: control, name: `${cService.generalDescription}` })?.languageVersions[
    `${language}`
  ];
  const generalDescriptionSummary = generalDescriptionLanguageVersion?.summary;
  const generalDescriptionUserInstructions = generalDescriptionLanguageVersion?.userInstructions;
  const generalDescriptionProcessingTime = generalDescriptionLanguageVersion?.processingTime;
  const generalDescriptionPeriodOfValidity = generalDescriptionLanguageVersion?.periodOfValidity;
  const generalDescriptionBackgroundDescription = generalDescriptionLanguageVersion?.backgroundDescription;
  const generalDescriptionDeadline = generalDescriptionLanguageVersion?.deadline;
  const generalDescriptionDescription = generalDescriptionLanguageVersion?.description;

  const request: QualityRequest = {
    Language: language,
    Profile: 'VRKp',
    Input: {
      // Used for unsaved services without a real ID
      alternativeId: !modelId ? alternativeId : null,
      GeneralServiceDescription: {
        descriptions: [
          getQualityItem(generalDescriptionSummary, language, 'Summary'),
          getDescriptionItem(
            generalDescriptionUserInstructions ? (JSON.parse(generalDescriptionUserInstructions) as RawDraftContentState) : null,
            language,
            'UserInstruction'
          ),
          getDescriptionItem(
            generalDescriptionUserInstructions ? (JSON.parse(generalDescriptionUserInstructions) as RawDraftContentState) : null,
            language,
            'UserInstruction'
          ),
          getQualityItem(generalDescriptionProcessingTime, language, 'ProcessingTime'),
          getQualityItem(generalDescriptionPeriodOfValidity, language, 'ValidityTime'),
          getDescriptionItem(
            generalDescriptionBackgroundDescription ? (JSON.parse(generalDescriptionBackgroundDescription) as RawDraftContentState) : null,
            language,
            'BackgroundDescription'
          ),
          getQualityItem(generalDescriptionDeadline, language, 'DeadLine'),
          getDescriptionItem(
            generalDescriptionDescription ? (JSON.parse(generalDescriptionDescription) as RawDraftContentState) : null,
            language,
            'Description'
          ),
        ].filter((x) => !!x) as QualityItem[],
        names: itemToArray(name),
        publishingStatus: status ?? 'Published',
        type: 'Service',
      },
      id: modelId,
      requirements: versionConditions
        ? itemToArray(
            getDescriptionItem(versionConditions ? (JSON.parse(versionConditions) as RawDraftContentState) : null, language, 'Conditions')
          )
        : [],
      organizations:
        purchaseProducers.length > 0 || otherProducers.length > 0
          ? ([
              ...purchaseProducers.map((producer: ServiceProducer, index: number) =>
                getAdditionalInformationItem(producer.name, language, `organization.${language}.purchaseProducers.${index}`)
              ),
              ...otherProducers.map((producer: ServiceProducer, index: number) =>
                getAdditionalInformationItem(producer.name, language, `organization.${language}.otherProducers.${index}`)
              ),
            ] as AdditionalInformationQualityItem[])
          : [],
      serviceDescriptions: [
        getDescriptionItem(deadline ? (JSON.parse(deadline) as RawDraftContentState) : null, language, 'DeadLine'),
        getDescriptionItem(description ? (JSON.parse(description) as RawDraftContentState) : null, language, 'Description'),
        getDescriptionItem(periodOfValidity ? (JSON.parse(periodOfValidity) as RawDraftContentState) : null, language, 'ValidityTime'),
        getDescriptionItem(processingTime ? (JSON.parse(processingTime) as RawDraftContentState) : null, language, 'ProcessingTime'),
        getQualityItem(summary, language, 'Summary', `serviceDescriptions.Summary.${language}`),
        getDescriptionItem(userInstructions ? (JSON.parse(userInstructions) as RawDraftContentState) : null, language, 'UserInstruction'),
        getDescriptionItem(chargeInfo ? (JSON.parse(chargeInfo) as RawDraftContentState) : null, language, 'ChargeTypeAdditionalInfo'),
      ].filter((x) => !!x) as QualityItem[],
      serviceNames: name ? itemToArray(getQualityItem(name, language, 'Name', `serviceNames.Name.${language}`)) : [],
      serviceVouchers: getVoucherItem(voucherType, voucher, language),
      status: status,
      type: 'Service',
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
