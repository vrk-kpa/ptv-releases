import React from 'react';
import { useTranslation } from 'react-i18next';
import { Paragraph } from 'suomifi-ui-components';
import { AddressModel } from 'types/api/connectionApiModel';
import { Country } from 'types/areaTypes';
import { Language } from 'types/enumTypes';
import { AddressLvModel } from 'types/forms/connectionFormTypes';
import { getNameofCountry } from 'utils/countries';
import { getLanguageVersionValue } from 'utils/languageVersions';

type ForeignAddressProps = {
  address: AddressModel;
  lang: Language;
  countries: Country[];
};

export function ForeignAddress(props: ForeignAddressProps): React.ReactElement {
  const { t } = useTranslation();
  const { address, lang, countries } = props;

  const foreignAddress = getLanguageVersionValue<AddressLvModel, string>(address.languageVersions, lang, (x) => x.foreignAddress, '');
  const country = getNameofCountry(address.countryCode, lang, countries);

  return (
    <>
      <Paragraph>{t('Ptv.ConnectionDetails.AddressType.Foreign.Label')}</Paragraph>
      <Paragraph>{`${foreignAddress}, ${country}`}</Paragraph>
    </>
  );
}
