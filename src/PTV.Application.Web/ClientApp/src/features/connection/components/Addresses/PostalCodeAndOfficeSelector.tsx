import React, { useCallback, useContext, useMemo, useState } from 'react';
import { useController } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Select, { ActionMeta } from 'react-select';
import { Text } from 'suomifi-ui-components';
import { cAddress, cC } from 'types/forms/connectionFormTypes';
import { PostalCode } from 'types/postalCodeTypes';
import ValidationMessage from 'components/ValidationMessage';
import { AppContext } from 'context/AppContextProvider';
import { useFormMetaContext } from 'context/formMeta';
import { translateToLang } from 'utils/translations';
import { AddressProps } from './Address';

type PostalCodeAndOfficeSelectOption = {
  value: string;
  label: string;
};

type PostalCodeAndOfficeSelectorProps = {
  selectedValue?: PostalCodeAndOfficeSelectOption;
};

function PostalCodeAndOfficeSelector(props: PostalCodeAndOfficeSelectorProps & AddressProps): React.ReactElement {
  const { t } = useTranslation();
  const meta = useFormMetaContext();
  const [searchText, setSearchText] = useState('');
  const { staticData } = useContext(AppContext);

  const { field: postalCodeField, fieldState } = useController({
    name: `${cC.addresses}.${props.addressIndex}.${cAddress.postalCode}`,
    control: props.control,
  });
  const validationError = fieldState.error?.message;

  const allPostalCodes = useMemo(() => staticData.municipalities.map((x) => x.postalCodes).flat(), [staticData.municipalities]).sort(
    (a, b) => a.code.localeCompare(b.code)
  );

  const handleOnChange = (value: PostalCodeAndOfficeSelectOption | null, action: ActionMeta<PostalCodeAndOfficeSelectOption>): void => {
    if (action.action === 'select-option') {
      postalCodeField.onChange(value ? value.value : '');
    } else if (action.action === 'clear') {
      postalCodeField.onChange('');
    }
  };

  const getSelectOptions = (): PostalCodeAndOfficeSelectOption[] => {
    if (searchText.length < 2) return [];
    const matchingPostalCodes = allPostalCodes.filter((x) => {
      return getPostalCodeLabel(x)?.toLocaleLowerCase().includes(searchText.toLocaleLowerCase());
    });
    return matchingPostalCodes.map((x): PostalCodeAndOfficeSelectOption => {
      return { value: x.code, label: getPostalCodeLabel(x) };
    });
  };

  const getPostalCodeLabel = useCallback(
    (value: PostalCode): string => {
      const name = value && value.names ? translateToLang(meta.selectedLanguageCode, value.names, null) : null;
      return name ? `${value.code}, ${name}` : value.code;
    },
    [meta.selectedLanguageCode]
  );

  const selectedValue = useMemo((): PostalCodeAndOfficeSelectOption | null => {
    if (!postalCodeField.value) {
      return null;
    }
    const matchingPostalCode = allPostalCodes.find((x) => x.code === postalCodeField.value);
    const postalCodeLabel = matchingPostalCode ? getPostalCodeLabel(matchingPostalCode) : null;
    return {
      value: postalCodeField.value,
      label: postalCodeLabel ? postalCodeLabel : postalCodeField.value,
    };
  }, [postalCodeField.value, allPostalCodes, getPostalCodeLabel]);

  return (
    <div>
      <Text smallScreen={true} variant='bold'>
        {t('Ptv.ConnectionDetails.Address.PostalCodeAndOffice.Label')}
      </Text>
      <Select<PostalCodeAndOfficeSelectOption>
        onInputChange={(value: string) => {
          setSearchText(value);
        }}
        onChange={handleOnChange}
        options={getSelectOptions()}
        placeholder={t('Ptv.Common.WriteOrChoose')}
        isClearable={true}
        value={selectedValue}
      />
      {validationError && <ValidationMessage message={t(validationError)} />}
    </div>
  );
}

export { PostalCodeAndOfficeSelector };
