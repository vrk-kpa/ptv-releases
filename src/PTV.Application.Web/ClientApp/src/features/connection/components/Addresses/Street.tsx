import React, { useMemo, useState } from 'react';
import { Control, UseFormSetValue, useController, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { ActionMeta } from 'react-select';
import Creatable from 'react-select/creatable';
import { FormatOptionLabelMeta } from 'react-select/dist/declarations/src/Select';
import { Text } from 'suomifi-ui-components';
import { StreetModel } from 'types/api/streetModel';
import { ConnectionFormModel, cAddress, cC } from 'types/forms/connectionFormTypes';
import { useFormMetaContext } from 'context/formMeta';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import { useGetStreets } from 'hooks/queries/address/useGetStreets';
import { getTextByLangPriority } from 'utils/translations';
import { FormBlock } from 'features/connection/components/FormLayout';

type SelectOption = {
  value: string;
  label: string;
  street?: StreetModel;
  postalCode: string | null;
};

export type StreetProps = {
  addressIndex: number;
  control: Control<ConnectionFormModel>;
  // Using any because using correct type crashes TS or causes type check to take 30 seconds
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  setValue: UseFormSetValue<any>;
};

export function Street(props: StreetProps): React.ReactElement {
  const { t } = useTranslation();
  const meta = useFormMetaContext();
  const appContext = useAppContextOrThrow();
  const [searchText, setSearchText] = useState('');

  const { field: streetNameField } = useController({
    name: `${cC.addresses}.${props.addressIndex}.${cAddress.streetName}`,
    control: props.control,
  });

  const { field } = useController({
    name: `${cC.addresses}.${props.addressIndex}.${cAddress.street}`,
    control: props.control,
  });

  const postalCode = useWatch({
    name: `${cC.addresses}.${props.addressIndex}.${cAddress.postalCode}`,
    control: props.control,
  });

  const query = useGetStreets(
    {
      language: meta.selectedLanguageCode,
      offset: 0,
      postalCode: '',
      searchText: searchText,
      onlyValid: true,
    },
    {
      enabled: !!searchText,
    }
  );

  function onChange(value: SelectOption | null, actionMeta: ActionMeta<SelectOption>) {
    if (actionMeta.action === 'clear') {
      field.onChange(null);
      streetNameField.onChange({ [meta.selectedLanguageCode]: '' });
      return;
    }

    if (actionMeta.action === 'select-option' && value?.street) {
      const street: StreetModel = {
        ...value.street,
      };

      field.onChange(street);
      streetNameField.onChange(value.street.names);

      // When user switches address the selected postal code might not be valid anymore so use
      // the postal code which has been associated with the address used has selected.
      if (postalCode) {
        const postalCodes = value.street.streetNumbers.map((x) => x.postalCode);
        if (!postalCodes.includes(postalCode)) {
          props.setValue(`${cC.addresses}.${props.addressIndex}.${cAddress.postalCode}`, value.postalCode);
        }
      }
    }
  }

  function onInputChange(newValue: string) {
    if (newValue.length > 2) {
      setSearchText(newValue);
    }
  }

  function onCreateOption(inputValue: string) {
    // Clear the street since user is writing street name that we don't know.
    // This way when address is saved server side checks that street does not exist
    // and it takes the streetName field and generates new street address from it.
    field.onChange(null);
    streetNameField.onChange({ [meta.selectedLanguageCode]: inputValue });
  }

  function createKey(postFix: string, id?: string) {
    return id ? `${id}_${postFix}` : `_${postFix}`;
  }

  const selectedValue = useMemo((): SelectOption | null => {
    const street = field.value;

    // If the user has typed an address which does not exist there is no street
    if (!street) {
      return {
        label: getTextByLangPriority(meta.selectedLanguageCode, streetNameField.value, ''),
        value: getTextByLangPriority(meta.selectedLanguageCode, streetNameField.value, 'no-street-name'),
        postalCode: null,
      };
    }

    return {
      label: '', // handled by formatOptionLabel
      value: createKey(street.municipalityCode, street.id),
      postalCode: null,
      street: {
        id: street.id,
        isValid: street.isValid,
        municipalityCode: street.municipalityCode,
        streetNumbers: [],
        names: streetNameField.value,
      },
    };
  }, [field, streetNameField.value, meta.selectedLanguageCode]);

  const options = useMemo((): SelectOption[] => {
    if (!query.data || query.isLoading || query.error) return [];

    const map = new Map<string, SelectOption>();
    for (const street of query.data.items) {
      for (const streetNumber of street.streetNumbers) {
        const key = createKey(streetNumber.postalCode, street.id);
        if (!map.has(key)) {
          map.set(key, {
            label: getTextByLangPriority(meta.selectedLanguageCode, street.names, ''),
            value: key,
            street: street,
            postalCode: streetNumber.postalCode,
          });
        }
      }
    }
    return Array.from(map.values());
  }, [query.data, query.isLoading, query.error, meta.selectedLanguageCode]);

  function formatTextFieldValue(option: SelectOption): React.ReactNode {
    if (!field.value) {
      return getTextByLangPriority(meta.selectedLanguageCode, streetNameField.value, '');
    }

    if (!option.street) {
      return <span>{option.label}</span>;
    }

    return getTextByLangPriority(meta.selectedLanguageCode, option.street.names, '');
  }

  function formatMenuValue(option: SelectOption): React.ReactNode {
    if (!option.street) {
      return <span>{t('Ptv.ConnectionDetails.Address.Street.Search.NotFound')}</span>;
    }

    const street = option.street;
    const streetName = getTextByLangPriority(meta.selectedLanguageCode, street.names, '');
    const postalCode = option.postalCode ?? '';
    const municipality = appContext.staticData.municipalities.find((x) => x.code === street.municipalityCode);
    const municipalityName = municipality ? getTextByLangPriority(meta.selectedLanguageCode, municipality?.names, '') : '';
    const str = `${streetName}, ${postalCode}, ${municipalityName}`;
    return str;
  }

  function formatOptionLabel(option: SelectOption, labelMeta: FormatOptionLabelMeta<SelectOption>): React.ReactNode {
    if (labelMeta.context === 'value') {
      return formatTextFieldValue(option);
    }

    return formatMenuValue(option);
  }

  return (
    <div>
      <FormBlock marginBottom='8px'>
        <Text smallScreen={true} variant='bold'>
          {t('Ptv.ConnectionDetails.Address.Street.Label')}
        </Text>
      </FormBlock>
      <Creatable<SelectOption>
        isLoading={query.isLoading}
        onInputChange={onInputChange}
        onChange={onChange}
        onCreateOption={onCreateOption}
        formatOptionLabel={formatOptionLabel}
        options={options}
        value={selectedValue}
        placeholder={t('Ptv.ConnectionDetails.Address.Street.Placeholder')}
        isClearable={true}
      />
    </div>
  );
}
