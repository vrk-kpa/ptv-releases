import React from 'react';
import { useTranslation } from 'react-i18next';
import { Paragraph } from 'suomifi-ui-components';
import { AddressModel } from 'types/api/connectionApiModel';
import { Language } from 'types/enumTypes';
import { AddressLvModel } from 'types/forms/connectionFormTypes';
import { PostalCode } from 'types/postalCodeTypes';
import { getLanguageVersionValue } from 'utils/languageVersions';
import { getNameOfCity } from 'utils/postalcodes';

type PoBoxAddressProps = {
  address: AddressModel;
  lang: Language;
  postalCodes: PostalCode[];
};

export function PoBoxAddress(props: PoBoxAddressProps): React.ReactElement {
  const { t } = useTranslation();
  const { address, lang, postalCodes } = props;

  const poBox = getLanguageVersionValue<AddressLvModel, string>(address.languageVersions, lang, (x) => x.poBox, '');
  const info = getLanguageVersionValue<AddressLvModel, string>(address.languageVersions, lang, (x) => x.additionalInformation, '');
  const city = getNameOfCity(address.postalCode, lang, postalCodes);

  return (
    <>
      <Paragraph>{t('Ptv.ConnectionDetails.AddressType.PostOfficeBox.Label')}</Paragraph>
      <Paragraph>{`${poBox} ${address.postalCode} ${city},  ${info}`}</Paragraph>
    </>
  );
}
