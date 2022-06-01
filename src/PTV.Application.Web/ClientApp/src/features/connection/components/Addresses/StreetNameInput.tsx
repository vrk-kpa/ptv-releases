import React, { useMemo, useState } from 'react';
import { FieldError, useController } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { ActionMeta } from 'react-select';
import Creatable from 'react-select/creatable';
import { Text } from 'suomifi-ui-components';
import { cAddress, cC } from 'types/forms/connectionFormTypes';
import { LanguageVersionType } from 'types/languageVersionTypes';
import ValidationMessage from 'components/ValidationMessage';
import { useFormMetaContext } from 'context/formMeta';
import { getAutocompleteQueryParams, useGetStreetNames } from 'hooks/queries/address/useGetStreetNames';
import { useDebounceValue } from 'hooks/useDebounceValue';
import { getTextByLangPriority } from 'utils/translations';
import { FormBlock } from 'features/connection/components/FormLayout';
import { AddressProps } from './Address';

type StreetNameSelectOption = {
  value: string;
  label: string;
};

export function StreetNameInput(props: AddressProps): React.ReactElement {
  const { t } = useTranslation();
  const meta = useFormMetaContext();
  const [searchText, setSearchText] = useState('');
  const debouncedSearchText = useDebounceValue(searchText, 300);

  const { data, isLoading, isError } = useGetStreetNames(getAutocompleteQueryParams(debouncedSearchText), {
    enabled: !!debouncedSearchText,
  });

  const { field: streetNameField, fieldState } = useController({
    name: `${cC.addresses}.${props.addressIndex}.${cAddress.streetName}`,
    control: props.control,
  });
  const fieldStateErrors = fieldState.error as LanguageVersionType<FieldError>;
  const validationError = fieldStateErrors?.[meta.selectedLanguageCode]?.message;

  const options = (): StreetNameSelectOption[] => {
    if (isLoading || isError || !data) return [];

    const result = data.items.map((street) => {
      const res: StreetNameSelectOption = {
        value: getTextByLangPriority(meta.selectedLanguageCode, street),
        label: getTextByLangPriority(meta.selectedLanguageCode, street),
      };
      return res;
    });

    return addSelectedValueToOptions(result);
  };

  const addSelectedValueToOptions = (options: StreetNameSelectOption[]): StreetNameSelectOption[] => {
    if (selectedValue !== null) {
      options.push(selectedValue as StreetNameSelectOption);
    }
    return options;
  };

  const handleOnChange = (value: StreetNameSelectOption | null, action: ActionMeta<StreetNameSelectOption>): void => {
    if (action.action === 'select-option' || action.action === 'create-option') {
      streetNameField.onChange({ [meta.selectedLanguageCode]: value?.value });
    } else if (action.action === 'clear') {
      streetNameField.onChange({});
    }
  };

  const selectedValue = useMemo((): StreetNameSelectOption | null => {
    if (!streetNameField.value) {
      return null;
    }
    const translation = getTextByLangPriority(meta.selectedLanguageCode, streetNameField.value);
    return {
      value: translation,
      label: translation,
    };
  }, [streetNameField.value, meta.selectedLanguageCode]);

  const formatCreateLabel = (value: string): React.ReactNode => {
    return <span>{`${t('Ptv.Common.Create')} "${value}"`}</span>;
  };

  return (
    <div>
      <FormBlock marginBottom='8px'>
        <Text smallScreen={true} variant='bold'>
          {t('Ptv.ConnectionDetails.Address.Street.Label')}
        </Text>
      </FormBlock>
      <Creatable<StreetNameSelectOption>
        isLoading={isLoading}
        onInputChange={(value: string) => setSearchText(value)}
        onChange={handleOnChange}
        options={options()}
        placeholder={t('Ptv.ConnectionDetails.Address.Street.Placeholder')}
        formatCreateLabel={formatCreateLabel}
        isClearable={true}
        value={selectedValue}
      />
      {validationError && <ValidationMessage message={t(validationError)} />}
    </div>
  );
}
