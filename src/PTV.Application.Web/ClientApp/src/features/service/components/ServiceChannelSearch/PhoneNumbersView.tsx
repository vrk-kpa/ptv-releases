import React from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Paragraph, Text } from 'suomifi-ui-components';
import { PhoneNumberLv } from 'types/api/connectionApiModel';
import { Language } from 'types/enumTypes';
import { LanguageVersionType } from 'types/languageVersionTypes';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import { getDialCodeOrDefault } from 'utils/phoneNumbers';

const useStyles = makeStyles(() => ({
  block: {
    marginTop: '20px',
  },
}));

type PhoneNumbersViewProps = {
  phoneNumbers: LanguageVersionType<PhoneNumberLv[]>;
  lang: Language;
};

function PhoneNumbersView(props: PhoneNumbersViewProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();
  const appContext = useAppContextOrThrow();
  const { phoneNumbers, lang } = props;

  return (
    <div className={classes.block}>
      <Text variant='bold'>{t('Ptv.ConnectionDetails.PhoneNumber.Number.Label')}</Text>
      {phoneNumbers[lang]?.map((number, _index) => {
        const dialCode = getDialCodeOrDefault(number.dialCodeId, appContext.staticData.dialCodes);
        const text = dialCode ? `(${dialCode.code}) ${number.number}` : number.number;
        return (
          <div key={number.id}>
            <Paragraph>{text}</Paragraph>
            {number.additionalInformation && <Paragraph>{number.additionalInformation}</Paragraph>}
            {number.chargeType === 'Free' && <Paragraph>{t('Ptv.Service.ChargeType.Free')}</Paragraph>}
            {number.chargeType === 'Charged' && <Paragraph>{t('Ptv.Service.ChargeType.Charged')}</Paragraph>}
            {number.chargeType === 'Other' && <Paragraph>{t('Ptv.Service.ChargeType.Other')}</Paragraph>}
            {number.chargeDescription && <Paragraph>{number.chargeDescription}</Paragraph>}
          </div>
        );
      })}
    </div>
  );
}

export { PhoneNumbersView };
