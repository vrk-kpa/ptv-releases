import React from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Paragraph, Text } from 'suomifi-ui-components';
import { HolidayServiceHourModel } from 'types/forms/connectionFormTypes';
import { toTimeNoTimeZone } from 'utils/date';

const useStyles = makeStyles(() => ({
  holidayHoursWrapper: {
    marginTop: '20px',
  },
}));

function HolidayHoursView(holidayHours: HolidayServiceHourModel[]): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();

  return (
    <div className={classes.holidayHoursWrapper}>
      {holidayHours.length > 0 && <Text variant='bold'>{t('Ptv.ConnectionDetails.HolidayServiceHours.Title')}</Text>}
      {holidayHours.map((holiday, _index) => {
        return (
          <div key={holiday.id}>
            {holiday.isClosed && (
              <Paragraph>{`${t(`Ptv.Holiday.${holiday.code}`)}: ${t('Ptv.ConnectionDetails.HolidayServiceHour.Closed.Label')}`}</Paragraph>
            )}
            {!holiday.isClosed && holiday.from && holiday.to && (
              <Paragraph>{`${t(`Ptv.Holiday.${holiday.code}`)}: ${toTimeNoTimeZone(holiday.from)} - ${toTimeNoTimeZone(
                holiday.to
              )}`}</Paragraph>
            )}
          </div>
        );
      })}
    </div>
  );
}

export { HolidayHoursView };
