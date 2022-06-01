import React, { FunctionComponent } from 'react';
import { Control, UseFormTrigger } from 'react-hook-form';
import { ServiceModel, cLv } from 'types/forms/serviceFormTypes';
import { GeneralDescriptionModel } from 'types/generalDescriptionTypes';
import { ComparisonView } from 'components/ComparisonView';
import {
  useFormMetaContext,
  useGetCompareFieldId,
  useGetCompareFieldName,
  useGetFieldId,
  useGetFieldName,
  useGetSelectedLanguage,
} from 'context/formMeta';
import { ServiceLaws } from './ServiceLaws';

type ServiceLawsCompareProps = {
  gd: GeneralDescriptionModel | null | undefined;
  control: Control<ServiceModel>;
  trigger: UseFormTrigger<ServiceModel>;
};

export const ServiceLawsCompare: FunctionComponent<ServiceLawsCompareProps> = (props: ServiceLawsCompareProps) => {
  const meta = useFormMetaContext();
  const name = useGetFieldName();
  const compareName = useGetCompareFieldName();
  const id = useGetFieldId();
  const compareId = useGetCompareFieldId();
  const language = useGetSelectedLanguage();

  function renderRight(): React.ReactElement | null {
    if (!meta.compareLanguageCode) {
      return null;
    }

    return (
      <ServiceLaws
        name={compareName(cLv.laws, meta.compareLanguageCode)}
        id={compareId(cLv.laws, meta.compareLanguageCode)}
        language={meta.compareLanguageCode}
        mode={meta.mode}
        gd={props.gd}
        control={props.control}
        trigger={props.trigger}
      />
    );
  }

  return (
    <ComparisonView
      left={
        <ServiceLaws
          name={name(cLv.laws)}
          id={id(cLv.laws)}
          language={language}
          mode={meta.mode}
          gd={props.gd}
          control={props.control}
          trigger={props.trigger}
        />
      }
      right={renderRight()}
    />
  );
};
