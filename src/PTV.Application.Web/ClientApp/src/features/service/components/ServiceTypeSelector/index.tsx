import React, { useMemo } from 'react';
import { Control, RegisterOptions, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { RadioOption, RhfRadioButtonGroup } from 'fields';
import { Language, Mode, ServiceType } from 'types/enumTypes';
import { ServiceModel, cLv, cService } from 'types/forms/serviceFormTypes';
import { Tooltip } from 'components/Tooltip';
import { getFieldId } from 'utils/fieldIds';

type ServiceTypeSelectorProps = {
  name: string;
  tabLanguage: Language;
  disabled?: boolean;
  mode: Mode;
  control: Control<ServiceModel>;
};

export function ServiceTypeSelector(props: ServiceTypeSelectorProps): React.ReactElement {
  const { t } = useTranslation();
  const id = getFieldId(props.name, props.tabLanguage, undefined);
  const generalDescription = useWatch({ control: props.control, name: `${cService.generalDescription}` });
  const tooltipText = !!generalDescription
    ? t('Ptv.Service.Form.Field.ServiceType.GdSelected.Tooltip')
    : t('Ptv.Service.Form.Field.ServiceType.Tooltip');

  const items = useMemo(() => {
    const rbs: RadioOption[] = [
      {
        value: 'Service',
        text: t('Ptv.Service.ServiceType.Service'),
      },
      {
        value: 'PermissionAndObligation',
        text: t('Ptv.Service.ServiceType.PermissionAndObligation'),
      },
      {
        value: 'ProfessionalQualifications',
        text: t('Ptv.Service.ServiceType.ProfessionalQualifications'),
      },
    ];

    return rbs;
  }, [t]);

  const rules: Omit<
    RegisterOptions<ServiceModel, `${cService.serviceType}`>,
    'valueAsNumber' | 'valueAsDate' | 'setValueAs' | 'disabled'
  > = {
    deps: [
      `${cService}.${props.tabLanguage}.${cLv.deadline}`,
      `${cService}.${props.tabLanguage}.${cLv.processingTime}`,
      `${cService}.${props.tabLanguage}.${cLv.periodOfValidity}`,
    ],
  };

  return (
    <RhfRadioButtonGroup<ServiceType>
      rules={rules}
      control={props.control}
      name={props.name}
      id={id}
      mode={props.mode}
      items={items}
      labelText={t('Ptv.Service.Form.Field.ServiceType.Label')}
      tooltipComponent={
        <Tooltip
          ariaInfoButtonLabelText={t('Ptv.Common.Tooltip.Button.Label', { label: t('Ptv.Service.Form.Field.ServiceType.Label') })}
          ariaCloseButtonLabelText={t('Ptv.Common.Tooltip.CloseButton.Label', { label: t('Ptv.Service.Form.Field.ServiceType.Label') })}
        >
          {tooltipText}
        </Tooltip>
      }
      disabled={props.disabled}
    />
  );
}
