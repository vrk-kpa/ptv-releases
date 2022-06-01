import React from 'react';
import { Control, UseFormSetValue } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { ServiceModel, cLv } from 'types/forms/serviceFormTypes';
import { GeneralDescriptionModel } from 'types/generalDescriptionTypes';
import { useGdSpecificTranslationKey } from 'hooks/translations/useGdSpecificTranslationKey';
import * as fieldConfig from 'validation/fieldConfig';
import { RichTextDescription } from 'features/service/components/RichTextDescription';

type ProcessingTimeProps = {
  gd: GeneralDescriptionModel | null | undefined;
  qualitySelector: string;
  control: Control<ServiceModel>;
  setValue: UseFormSetValue<ServiceModel>;
};

export function ProcessingTime(props: ProcessingTimeProps): React.ReactElement {
  const { t } = useTranslation();

  const hintKey = useGdSpecificTranslationKey(
    props.control,
    'Ptv.Service.Form.Field.ProcessingTime.Hint',
    'Ptv.Service.Form.Field.ProcessingTime.GdSelected.Hint'
  );

  return (
    <RichTextDescription
      maxCharacters={fieldConfig.LargeFieldLength}
      labelText={t('Ptv.Service.Form.Field.ProcessingTime.Label')}
      hintText={t(hintKey)}
      placeHolder={t('Ptv.Service.Form.Field.ProcessingTime.Placeholder')}
      optionalText={t('Ptv.Common.Optional')}
      name={cLv.processingTime}
      gd={props.gd}
      qualitySelector={props.qualitySelector}
      control={props.control}
      setValue={props.setValue}
    />
  );
}
