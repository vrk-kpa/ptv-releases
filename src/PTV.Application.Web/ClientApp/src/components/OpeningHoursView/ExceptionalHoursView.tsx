import React from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Paragraph, Text } from 'suomifi-ui-components';
import { ExceptionalServiceHour } from 'types/api/connectionApiModel';
import { Language } from 'types/enumTypes';
import { toDate, toTimeNoTimeZone } from 'utils/date';

const useStyles = makeStyles(() => ({
  exceptionalHoursWrapper: {
    marginTop: '20px',
  },
}));

function ExceptionalHoursView(exceptionalHours: ExceptionalServiceHour[], lang: Language): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();

  return (
    <div className={classes.exceptionalHoursWrapper}>
      {exceptionalHours.length > 0 && <Text variant='bold'>{t('Ptv.ConnectionDetails.ExceptionalServiceHours.Title')}</Text>}
      {exceptionalHours.map((exceptional, _index) => {
        return (
          <div key={exceptional.id}>
            {exceptional.isClosed && <Paragraph>{t('Ptv.ConnectionDetails.HolidayServiceHour.Closed.Label')}</Paragraph>}
            {!exceptional.isClosed && <Paragraph>{t('Ptv.ConnectionDetails.HolidayServiceHour.Open.Label')}</Paragraph>}
            {exceptional.openingHoursFrom && !exceptional.openingHoursTo && (
              <Paragraph>{`${toDate(exceptional.openingHoursFrom)}`}</Paragraph>
            )}
            {exceptional.openingHoursFrom && exceptional.openingHoursTo && (
              <Paragraph>{`${toDate(exceptional.openingHoursFrom)} - ${toDate(exceptional.openingHoursTo)}`}</Paragraph>
            )}
            {exceptional.timeFrom && exceptional.timeTo && exceptional.timeFrom !== exceptional.timeTo && (
              <Paragraph>{`${toTimeNoTimeZone(exceptional.timeFrom)} - ${toTimeNoTimeZone(exceptional.timeTo)}`}</Paragraph>
            )}
            {exceptional.languageVersions[lang]?.additionalInformation !== null && (
              <Paragraph>{exceptional.languageVersions[lang]?.additionalInformation}</Paragraph>
            )}
          </div>
        );
      })}
    </div>
  );
}

export { ExceptionalHoursView };
