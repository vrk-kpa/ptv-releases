import React from 'react';
import { Control, UseFormSetValue } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { ServiceModel, cLv } from 'types/forms/serviceFormTypes';
import { GeneralDescriptionModel } from 'types/generalDescriptionTypes';
import { Tooltip } from 'components/Tooltip';
import { useGdSpecificTranslationKey } from 'hooks/translations/useGdSpecificTranslationKey';
import * as fieldConfig from 'validation/fieldConfig';
import { RichTextDescription } from 'features/service/components/RichTextDescription';

type ConditinsAndCriteriaProps = {
  gd: GeneralDescriptionModel | null | undefined;
  qualitySelector: string;
  control: Control<ServiceModel>;
  setValue: UseFormSetValue<ServiceModel>;
};

export function ConditionsAndCriteria(props: ConditinsAndCriteriaProps): React.ReactElement {
  const { t } = useTranslation();

  const hintKey = useGdSpecificTranslationKey(
    props.control,
    'Ptv.Service.Form.Field.Conditions.Hint',
    'Ptv.Service.Form.Field.Conditions.GdSelected.Hint'
  );

  const tooltipKey = useGdSpecificTranslationKey(
    props.control,
    'Ptv.Service.Form.Field.Conditions.Tooltip',
    'Ptv.Service.Form.Field.Conditions.GdSelected.Tooltip'
  );

  return (
    <RichTextDescription
      maxCharacters={fieldConfig.ExtraLargeFieldLength}
      labelText={t('Ptv.Service.Form.Field.Conditions.Label')}
      hintText={t(hintKey)}
      placeHolder={t('Ptv.Service.Form.Field.Conditions.Placeholder')}
      optionalText={t('Ptv.Common.Optional')}
      name={cLv.conditions}
      gd={props.gd}
      qualitySelector={props.qualitySelector}
      control={props.control}
      setValue={props.setValue}
      tooltipComponent={
        <Tooltip
          ariaInfoButtonLabelText={t('Ptv.Common.Tooltip.Button.Label', { label: t('Ptv.Service.Form.Field.Conditions.Label') })}
          ariaCloseButtonLabelText={t('Ptv.Common.Tooltip.CloseButton.Label', { label: t('Ptv.Service.Form.Field.Conditions.Label') })}
        >
          {t(tooltipKey)}
        </Tooltip>
      }
    />
  );
}
