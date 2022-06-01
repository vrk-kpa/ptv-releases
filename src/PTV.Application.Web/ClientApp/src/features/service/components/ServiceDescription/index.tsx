import React from 'react';
import { Control, UseFormSetValue } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { ServiceModel, cLv } from 'types/forms/serviceFormTypes';
import { GeneralDescriptionModel } from 'types/generalDescriptionTypes';
import { Tooltip } from 'components/Tooltip';
import { useGdSpecificTranslationKey } from 'hooks/translations/useGdSpecificTranslationKey';
import * as fieldConfig from 'validation/fieldConfig';
import { RichTextDescription } from 'features/service/components/RichTextDescription';

type ServiceDescriptionProps = {
  gd: GeneralDescriptionModel | null | undefined;
  qualitySelector: string;
  control: Control<ServiceModel>;
  setValue: UseFormSetValue<ServiceModel>;
};

export function ServiceDescription(props: ServiceDescriptionProps): React.ReactElement {
  const { t } = useTranslation();

  const hintKey = useGdSpecificTranslationKey(
    props.control,
    'Ptv.Service.Form.Field.Description.Hint',
    'Ptv.Service.Form.Field.Description.GdSelected.Hint'
  );
  const tooltipKey = useGdSpecificTranslationKey(
    props.control,
    'Ptv.Service.Form.Field.Description.Tooltip',
    'Ptv.Service.Form.Field.Description.GdSelected.Tooltip'
  );

  return (
    <RichTextDescription
      maxCharacters={fieldConfig.ExtraLargeFieldLength}
      labelText={t('Ptv.Service.Form.Field.Description.Label')}
      hintText={t(hintKey)}
      placeHolder={t('Ptv.Service.Form.Field.Description.Placeholder')}
      name={cLv.description}
      gd={props.gd}
      qualitySelector={props.qualitySelector}
      control={props.control}
      setValue={props.setValue}
      tooltipComponent={
        <Tooltip
          ariaInfoButtonLabelText={t('Ptv.Common.Tooltip.Button.Label', { label: t('Ptv.Service.Form.Field.Description.Label') })}
          ariaCloseButtonLabelText={t('Ptv.Common.Tooltip.CloseButton.Label', { label: t('Ptv.Service.Form.Field.Description.Label') })}
        >
          {t(tooltipKey)}
        </Tooltip>
      }
    />
  );
}
