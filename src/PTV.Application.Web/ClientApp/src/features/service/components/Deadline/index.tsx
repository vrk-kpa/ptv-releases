import React from 'react';
import { Control, UseFormSetValue } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { ServiceModel, cLv } from 'types/forms/serviceFormTypes';
import { GeneralDescriptionModel } from 'types/generalDescriptionTypes';
import { useGdSpecificTranslationKey } from 'hooks/translations/useGdSpecificTranslationKey';
import * as fieldConfig from 'validation/fieldConfig';
import { RichTextDescription } from 'features/service/components/RichTextDescription';

type DeadlineProps = {
  gd: GeneralDescriptionModel | null | undefined;
  qualitySelector: string;
  control: Control<ServiceModel>;
  setValue: UseFormSetValue<ServiceModel>;
};

export function Deadline(props: DeadlineProps): React.ReactElement {
  const { t } = useTranslation();

  const hintKey = useGdSpecificTranslationKey(
    props.control,
    'Ptv.Service.Form.Field.Deadline.Hint',
    'Ptv.Service.Form.Field.Deadline.GdSelected.Hint'
  );

  return (
    <RichTextDescription
      maxCharacters={fieldConfig.LargeFieldLength}
      labelText={t('Ptv.Service.Form.Field.Deadline.Label')}
      hintText={t(hintKey)}
      placeHolder={t('Ptv.Service.Form.Field.Deadline.Placeholder')}
      optionalText={t('Ptv.Common.Optional')}
      name={cLv.deadline}
      gd={props.gd}
      qualitySelector={props.qualitySelector}
      control={props.control}
      setValue={props.setValue}
    />
  );
}
