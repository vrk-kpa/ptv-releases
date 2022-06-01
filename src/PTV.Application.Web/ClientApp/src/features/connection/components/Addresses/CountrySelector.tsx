import React, { useMemo } from 'react';
import { Control, useController } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Select from 'react-select';
import _ from 'lodash';
import { Text } from 'suomifi-ui-components';
import { Country } from 'types/areaTypes';
import { ConnectionFormModel, cAddress, cC } from 'types/forms/connectionFormTypes';
import ValidationMessage from 'components/ValidationMessage';
import { useFormMetaContext } from 'context/formMeta';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import { getTextByLangPriority } from 'utils/translations';
import { FormBlock } from 'features/connection/components/FormLayout';

type SelectOption = {
  value: string;
  label: string;
  country: Country;
};

export type CountrySelectorProps = {
  addressIndex: number;
  control: Control<ConnectionFormModel>;
};

export function CountrySelector(props: CountrySelectorProps): React.ReactElement {
  const { t } = useTranslation();
  const appContext = useAppContextOrThrow();
  const meta = useFormMetaContext();

  const { field, fieldState } = useController({
    name: `${cC.addresses}.${props.addressIndex}.${cAddress.countryCode}`,
    control: props.control,
  });
  const validationError = fieldState.error?.message;

  function onChange(value: SelectOption | null) {
    field.onChange(value?.country.code);
  }

  const options = useMemo((): SelectOption[] => {
    const result: SelectOption[] = [];
    for (const country of appContext.staticData.countries) {
      result.push({
        label: getTextByLangPriority(meta.selectedLanguageCode, country.names, ''),
        value: country.code,
        country: country,
      });
    }

    return _.sortBy(result, (x) => x.label);
  }, [appContext.staticData.countries, meta.selectedLanguageCode]);

  const selectedValue = useMemo((): SelectOption | null => {
    if (!field.value) return null;
    const country = appContext.staticData.countries.find((x) => x.code === field.value);
    if (!country) return null;
    return {
      label: getTextByLangPriority(meta.selectedLanguageCode, country.names, ''),
      value: field.value,
      country: country,
    };
  }, [field.value, appContext.staticData.countries, meta.selectedLanguageCode]);

  return (
    <div>
      <FormBlock marginBottom='8px'>
        <Text smallScreen={true} variant='bold'>
          {t('Ptv.ConnectionDetails.Address.Country.Label')}
        </Text>
      </FormBlock>
      <Select<SelectOption>
        isLoading={false}
        onChange={onChange}
        value={selectedValue}
        options={options}
        placeholder={t('Ptv.ConnectionDetails.Address.Country.Placeholder')}
        isClearable={true}
      />
      {validationError && <ValidationMessage message={t(validationError)} />}
    </div>
  );
}
