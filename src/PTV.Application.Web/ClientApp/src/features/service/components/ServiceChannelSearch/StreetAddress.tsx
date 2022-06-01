import React from 'react';
import { useTranslation } from 'react-i18next';
import { Paragraph } from 'suomifi-ui-components';
import { AddressModel } from 'types/api/connectionApiModel';
import { Country } from 'types/areaTypes';
import { Language } from 'types/enumTypes';
import { PostalCode } from 'types/postalCodeTypes';
import { getNameofCountry } from 'utils/countries';
import { getNameOfCity } from 'utils/postalcodes';
import { getTextByLangPriority } from 'utils/translations';

type StreetAddressProps = {
  address: AddressModel;
  lang: Language;
  postalCodes: PostalCode[];
  countries: Country[];
};

export function StreetAddress(props: StreetAddressProps): React.ReactElement {
  const { t } = useTranslation();
  const { address, lang, postalCodes, countries } = props;
  const country = getNameofCountry(address.countryCode, lang, countries);
  const city = getNameOfCity(address.postalCode, lang, postalCodes);

  return (
    <>
      <Paragraph>{t('Ptv.ConnectionDetails.AddressType.Street.Label')}</Paragraph>
      <Paragraph>{`${getTextByLangPriority(lang, address.streetName, address.streetName.fi ?? '')} ${address.streetNumber}, ${
        address.postalCode
      } ${city},  ${country}`}</Paragraph>
      <Paragraph>{address.languageVersions[lang]?.additionalInformation}</Paragraph>
    </>
  );
}
