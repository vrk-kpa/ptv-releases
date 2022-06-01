import React, { useMemo, useState } from 'react';
import { Control, useController, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Select, { ActionMeta } from 'react-select';
import _ from 'lodash';
import { Text } from 'suomifi-ui-components';
import { ConnectionFormModel, cAddress, cC } from 'types/forms/connectionFormTypes';
import { PostalCode } from 'types/postalCodeTypes';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import { FormBlock } from 'features/connection/components/FormLayout';
import { PostalCodeSelectOption, searchPostalCodes, toPostalCodeOption } from './utils';

export type PostalCodeSelectorProps = {
  addressIndex: number;
  control: Control<ConnectionFormModel>;
};

export function PostalCodeSelector(props: PostalCodeSelectorProps): React.ReactElement {
  const { t } = useTranslation();
  const appContext = useAppContextOrThrow();
  const { municipalities } = appContext.staticData;
  const [userInput, setUserInput] = useState('');

  const street = useWatch({
    name: `${cC.addresses}.${props.addressIndex}.${cAddress.street}`,
    control: props.control,
  });

  const addressType = useWatch({
    name: `${cC.addresses}.${props.addressIndex}.${cAddress.type}`,
    control: props.control,
  });

  const { field } = useController({
    name: `${cC.addresses}.${props.addressIndex}.${cAddress.postalCode}`,
    control: props.control,
  });

  function onInputChange(newValue: string) {
    if (newValue.length < 2) {
      return;
    }
    setUserInput(newValue);
  }

  function onChange(value: PostalCodeSelectOption | null, actionMeta: ActionMeta<PostalCodeSelectOption>) {
    if (actionMeta.action === 'clear') {
      field.onChange(null);
      return;
    }

    if (actionMeta.action === 'select-option' && value?.value) {
      field.onChange(value.value);
    }
  }

  const options = useMemo((): PostalCodeSelectOption[] => {
    function toOptions(postalCodes: PostalCode[]): PostalCodeSelectOption[] {
      return postalCodes.map((x): PostalCodeSelectOption => toPostalCodeOption(x));
    }

    if (addressType === 'PostOfficeBox') {
      return searchPostalCodes(municipalities, userInput, field.value);
    }

    // If User has selected street, show only postal codes valid for that street
    if (street) {
      const streetPostalCodes = street.streetNumbers.map((x) => x.postalCode);
      const municipalityCode = street.municipalityCode;
      const municipality = municipalities.find((x) => x.code === municipalityCode);
      if (municipality) {
        const postalCodes = municipality.postalCodes.filter((x) => streetPostalCodes.includes(x.code));
        return _.sortBy(toOptions(postalCodes), (x) => x.sortValue);
      }

      return [];
    }

    return searchPostalCodes(municipalities, userInput, field.value);
  }, [municipalities, userInput, field.value, street, addressType]);

  const selectedValue = useMemo((): PostalCodeSelectOption | null => {
    if (!field.value) return null;

    return {
      label: field.value,
      value: field.value,
      sortValue: parseInt(field.value, 10),
    };
  }, [field.value]);

  return (
    <div>
      <FormBlock marginBottom='8px'>
        <Text smallScreen={true} variant='bold'>
          {t('Ptv.ConnectionDetails.Address.PostalCode.Label')}
        </Text>
      </FormBlock>
      <Select<PostalCodeSelectOption>
        isLoading={false}
        onChange={onChange}
        onInputChange={onInputChange}
        options={options}
        value={selectedValue}
        placeholder={t('Ptv.ConnectionDetails.Address.PostalCode.Placeholder')}
        isClearable={true}
      />
    </div>
  );
}
