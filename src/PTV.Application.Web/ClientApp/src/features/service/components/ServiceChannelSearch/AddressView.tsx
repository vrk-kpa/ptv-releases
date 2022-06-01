import React, { useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Text } from 'suomifi-ui-components';
import { AddressModel } from 'types/api/connectionApiModel';
import { Language } from 'types/enumTypes';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import { ForeignAddress } from './ForeignAddress';
import { PoBoxAddress } from './PoBoxAddress';
import { StreetAddress } from './StreetAddress';

const useStyles = makeStyles((theme) => ({
  block: {
    marginTop: '20px',
  },
}));

type AddressViewProps = {
  addresses: AddressModel[];
  lang: Language;
};

function AddressView(props: AddressViewProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();
  const { addresses, lang } = props;
  const { staticData } = useAppContextOrThrow();

  const allPostalCodes = useMemo(() => staticData.municipalities.map((x) => x.postalCodes).flat(), [staticData.municipalities]);

  return (
    <div className={classes.block}>
      <Text variant='bold'>{t('Ptv.ConnectionDetails.Addresses.Title')}</Text>
      {addresses.map((address, _index) => {
        return (
          <div key={`${address.street?.id}-${address.streetName}-${address.postalCode}`}>
            {address.type === 'Street' && (
              <StreetAddress address={address} lang={lang} postalCodes={allPostalCodes} countries={staticData.countries} />
            )}
            {address.type === 'PostOfficeBox' && <PoBoxAddress address={address} lang={lang} postalCodes={allPostalCodes} />}
            {address.type === 'Foreign' && <ForeignAddress address={address} lang={lang} countries={staticData.countries} />}
          </div>
        );
      })}
    </div>
  );
}

export { AddressView };
