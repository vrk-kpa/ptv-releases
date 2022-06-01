import React from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Paragraph, Text } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { FaxNumberLvModel } from 'types/forms/connectionFormTypes';
import { LanguageVersionType } from 'types/languageVersionTypes';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import { getDialCodeOrDefault } from 'utils/phoneNumbers';

const useStyles = makeStyles(() => ({
  block: {
    marginTop: '20px',
  },
}));

type FaxNumbersViewProps = {
  faxNumbers: LanguageVersionType<FaxNumberLvModel[]>;
  lang: Language;
};

export function FaxNumbersView(props: FaxNumbersViewProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();
  const appContext = useAppContextOrThrow();
  const { faxNumbers, lang } = props;

  return (
    <div className={classes.block}>
      <Text variant='bold'>{t('Ptv.ConnectionDetails.FaxNumbers.Title')}</Text>
      {faxNumbers[lang]?.map((number, _index) => {
        const dialCode = getDialCodeOrDefault(number.dialCodeId, appContext.staticData.dialCodes);
        const text = dialCode ? `(${dialCode.code}) ${number.number}` : number.number;
        return (
          <div key={number.id}>
            <Paragraph>{text}</Paragraph>
          </div>
        );
      })}
    </div>
  );
}
