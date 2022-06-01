import React, { FunctionComponent } from 'react';
import { Control, UseFormSetValue } from 'react-hook-form';
import { ServiceModel } from 'types/forms/serviceFormTypes';
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
import { ServiceChargeDescriptionItem } from './ServiceChargeDescriptionItem';

interface ServiceChargeDescriptionInterface {
  name: string;
  gd: GeneralDescriptionModel | null | undefined;
  control: Control<ServiceModel>;
  setValue: UseFormSetValue<ServiceModel>;
}

export const ServiceChargeDescription: FunctionComponent<ServiceChargeDescriptionInterface> = (props) => {
  const meta = useFormMetaContext();
  const name = useGetFieldName();
  const language = useGetSelectedLanguage();
  const compareName = useGetCompareFieldName();
  const id = useGetFieldId();
  const compareId = useGetCompareFieldId();

  function renderRight(): React.ReactElement | null {
    if (!meta.compareLanguageCode) {
      return null;
    }

    return (
      <ServiceChargeDescriptionItem
        name={compareName(props.name, meta.compareLanguageCode)}
        id={compareId(props.name, meta.compareLanguageCode)}
        gd={props.gd}
        language={meta.compareLanguageCode}
        control={props.control}
        setValue={props.setValue}
      />
    );
  }

  return (
    <ComparisonView
      left={
        <ServiceChargeDescriptionItem
          name={name(props.name)}
          id={id(props.name)}
          gd={props.gd}
          language={language}
          control={props.control}
          setValue={props.setValue}
        />
      }
      right={renderRight()}
    />
  );
};
