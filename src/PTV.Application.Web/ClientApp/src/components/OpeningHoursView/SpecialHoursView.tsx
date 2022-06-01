import React from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Paragraph, Text } from 'suomifi-ui-components';
import { SpecialServiceHour } from 'types/api/connectionApiModel';
import { Language } from 'types/enumTypes';
import { toDate, toTimeNoTimeZone } from 'utils/date';
import { getKeyForWeekday } from 'utils/translations';

const useStyles = makeStyles(() => ({
  specialHoursWrapper: {
    marginTop: '20px',
  },
}));

function SpecialHoursView(specialHours: SpecialServiceHour[], lang: Language): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();

  return (
    <div className={classes.specialHoursWrapper}>
      {specialHours.length > 0 && <Text variant='bold'>{t('Ptv.ConnectionDetails.SpecialServiceHours.Title')}</Text>}
      {specialHours.map((special, _index) => {
        return (
          <div key={special.id}>
            {special.languageVersions[lang]?.additionalInformation && (
              <Paragraph>{special.languageVersions[lang]?.additionalInformation}</Paragraph>
            )}
            {special.dayFrom && special.dayTo && (
              <Paragraph>{`${t(getKeyForWeekday(special.dayFrom))} - ${
                special.dayTo != null ? t(getKeyForWeekday(special.dayTo)) : ''
              }`}</Paragraph>
            )}
            {special.timeFrom && special.timeTo && (
              <Paragraph>{`${toTimeNoTimeZone(special.timeFrom)} - ${toTimeNoTimeZone(special.timeTo)}`}</Paragraph>
            )}
            {special.openingHoursFrom && special.openingHoursTo && (
              <Paragraph>{`${toDate(special.openingHoursFrom)} - ${toDate(special.openingHoursTo)}`}</Paragraph>
            )}
          </div>
        );
      })}
    </div>
  );
}

export { SpecialHoursView };
