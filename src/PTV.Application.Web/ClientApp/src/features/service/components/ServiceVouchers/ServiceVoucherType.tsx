import React, { FunctionComponent, useMemo } from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Box from '@mui/material/Box';
import { RadioOption, RhfRadioButtonGroup } from 'fields';
import { Language, Mode, VoucherType } from 'types/enumTypes';
import { ServiceModel, cLv, cService, cVoucher } from 'types/forms/serviceFormTypes';
import { getFieldId } from 'utils/fieldIds';

interface ServiceVoucherTypeInterface {
  name: string;
  tabLanguage: Language;
  mode: Mode;
  control: Control<ServiceModel>;
  enabledLanguages: Language[];
}

export const ServiceVoucherType: FunctionComponent<ServiceVoucherTypeInterface> = (
  props: ServiceVoucherTypeInterface
): React.ReactElement => {
  const { t } = useTranslation();
  const fieldName = `${cService.voucherType}`;
  const fieldId = getFieldId(props.name, props.tabLanguage, undefined);

  const items = useMemo((): RadioOption[] => {
    const toValue = (value: VoucherType): string => value;
    return [
      {
        value: toValue('NotUsed'),
        text: t('Ptv.Service.Form.Field.ServiceVoucherType.NotUsed'),
      },
      {
        value: toValue('NoUrl'),
        text: t('Ptv.Service.Form.Field.ServiceVoucherType.NoUrl'),
      },
      {
        value: toValue('Url'),
        text: t('Ptv.Service.Form.Field.ServiceVoucherType.Url'),
      },
    ];
  }, [t]);

  const deps = getDeps(props.enabledLanguages);

  return (
    <Box>
      <RhfRadioButtonGroup
        rules={{ deps: deps }}
        control={props.control}
        id={fieldId}
        name={fieldName}
        mode={props.mode}
        labelText={t('Ptv.Service.Form.Field.ServiceVoucherType.Label')}
        items={items}
      />
    </Box>
  );
};

function getDeps(languages: Language[]): string[] {
  return languages.flatMap((lang) => [
    `${cService.languageVersions}.${lang}.${cLv.voucher}.${cVoucher.links}`,
    `${cService.languageVersions}.${lang}.${cLv.voucher}.${cVoucher.linksErrorTag}`,
  ]);
}
