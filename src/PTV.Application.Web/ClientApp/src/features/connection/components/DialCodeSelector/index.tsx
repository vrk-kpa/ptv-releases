import React, { useCallback, useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import Select from 'react-select';
import { Text } from 'suomifi-ui-components';
import { DialCode } from 'types/enumItemType';
import { Language } from 'types/enumTypes';
import { useFormMetaContext } from 'context/formMeta';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import { getTextByLangPriority } from 'utils/translations';
import { FormBlock } from 'features/connection/components/FormLayout';

type SelectOption = {
  value: string;
  countryName: string;
  label: string;
  dialCode: DialCode;
};

export type DialCodeSelectorProps = {
  id: string;
  onChange: (dialCode: DialCode | null) => void;
  selectedDialCodeId: string | null | undefined;
};

export function DialCodeSelector(props: DialCodeSelectorProps): React.ReactElement {
  const { staticData } = useAppContextOrThrow();
  const dialCodes = staticData.dialCodes;
  const countries = staticData.countries;
  const meta = useFormMetaContext();
  const { t } = useTranslation();

  function onChange(value: SelectOption | null) {
    props.onChange(value?.dialCode || null);
  }

  const getCountryName = useCallback(
    (lang: Language, countryCode: string) => {
      const country = countries.find((x) => x.code === countryCode);
      if (!country) return '';
      return getTextByLangPriority(lang, country.names, '');
    },
    [countries]
  );

  const options = useMemo((): SelectOption[] => {
    const results: SelectOption[] = [];
    for (const dialCode of dialCodes) {
      const countryName = getCountryName(meta.selectedLanguageCode, dialCode.countryCode);
      results.push({
        label: countryName ? `(${dialCode.code}) ${countryName}` : `(${dialCode.code})`,
        value: dialCode.id,
        countryName: countryName,
        dialCode: dialCode,
      });
    }

    return results.sort((x, y) => x.countryName.localeCompare(y.countryName));
  }, [meta.selectedLanguageCode, dialCodes, getCountryName]);

  const selectedValue = useMemo((): SelectOption | null => {
    if (!props.selectedDialCodeId) return null;

    const option = options.find((x) => x.value === props.selectedDialCodeId);
    if (!option) return null;

    const countryName = getCountryName(meta.selectedLanguageCode, option.dialCode.countryCode);
    return {
      label: countryName ? `(${option.dialCode.code}) ${countryName}` : `(${option.dialCode.code})`,
      value: option.dialCode.id,
      countryName: countryName,
      dialCode: option.dialCode,
    };
  }, [getCountryName, meta.selectedLanguageCode, options, props.selectedDialCodeId]);

  return (
    <div>
      <FormBlock marginBottom='8px'>
        <Text smallScreen={true} variant='bold'>
          {t('Ptv.Common.DialCodeSelector.Label')}
        </Text>
      </FormBlock>
      <Select<SelectOption>
        id={props.id}
        isLoading={false}
        onChange={onChange}
        value={selectedValue}
        options={options}
        placeholder={t('Ptv.Common.WriteOrChoose')}
        isClearable={true}
      />
    </div>
  );
}
