import React from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Paragraph, Text } from 'suomifi-ui-components';
import { StandardServiceHour } from 'types/api/connectionApiModel';
import { Language } from 'types/enumTypes';
import { toDate, toTimeNoTimeZone } from 'utils/date';
import { getKeyForWeekday } from 'utils/translations';

const useStyles = makeStyles(() => ({
  standardHoursWrapper: {
    marginTop: '20px',
  },
}));

function StandardHoursView(standardHours: StandardServiceHour[], lang: Language): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();

  return (
    <div className={classes.standardHoursWrapper}>
      {standardHours.length > 0 && <Text variant='bold'>{t('Ptv.ConnectionDetails.StandardServiceHour.MainTitle')}</Text>}
      {standardHours.map((hours, _index) => {
        return (
          <div key={hours.id}>
            {hours.languageVersions[lang]?.additionalInformation && (
              <Paragraph>{hours.languageVersions[lang]?.additionalInformation}</Paragraph>
            )}
            {hours.isNonStop && <Paragraph>{t('Ptv.StandardServiceHour.OpeningType.Always')}</Paragraph>}
            {hours.isReservation && <Paragraph>{t('Ptv.StandardServiceHour.OpeningType.OpenByReservation')}</Paragraph>}
            {hours.isPeriod && (
              <Paragraph>{t('Ptv.ConnectionDetails.ExceptionalServiceHour.Validity.Option.BetweenDates.Label')}</Paragraph>
            )}
            {hours.openingHoursFrom && hours.openingHoursTo && (
              <Paragraph>{`${toDate(hours.openingHoursFrom)} - ${toDate(hours.openingHoursTo)}`}</Paragraph>
            )}
            {Object.values(hours.dailyOpeningTimes).map((value, key) => {
              return (
                <Paragraph key={key}>{`${t(getKeyForWeekday(value.day))}: ${value.times
                  .map((x) => `${toTimeNoTimeZone(x.from)} - ${toTimeNoTimeZone(x.to)}`)
                  .join(', ')}`}</Paragraph>
              );
            })}
          </div>
        );
      })}
    </div>
  );
}

export { StandardHoursView };
