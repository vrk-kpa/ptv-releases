import React, { useCallback, useMemo } from 'react';
import { Control, UseFormTrigger, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Grid } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { RhfRadioButtonGroup } from 'fields';
import { HolidayEnum } from 'types/enumTypes';
import { ConnectionFormModel, cC, cHour } from 'types/forms/connectionFormTypes';
import { VisualHeading } from 'components/VisualHeading';
import { getKeyForHoliday } from 'utils/translations';
import { FormBlock } from 'features/connection/components/FormLayout';
import { toFieldId } from 'features/connection/utils/fieldid';
import { TimesSelect } from './TimesSelect';

const useStyles = makeStyles(() => ({
  root: {
    '& p.noTopMargin': {
      marginTop: 0,
    },
  },
}));

type HolidayServiceHourProps = {
  hourIndex: number;
  holiday: HolidayEnum;
  control: Control<ConnectionFormModel>;
  trigger: UseFormTrigger<ConnectionFormModel>;
};

const Closed = 'closed';
const Open = 'open';

export function HolidayServiceHour(props: HolidayServiceHourProps): React.ReactElement {
  const { t } = useTranslation();

  const classes = useStyles();

  const isClosed = useWatch({
    name: `${cC.holidayOpeningHours}.${props.hourIndex}.${cHour.isClosed}`,
    control: props.control,
  });

  const toFieldValue = useCallback((str: string): boolean => {
    return str === Closed ? true : false;
  }, []);

  const toRadioButtonValue = useCallback((value: boolean): string => {
    return value === true ? Closed : Open;
  }, []);

  const items = useMemo(() => {
    return [
      {
        value: Closed,
        text: t('Ptv.ConnectionDetails.HolidayServiceHour.Closed.Label'),
      },
      {
        value: Open,
        text: t('Ptv.ConnectionDetails.HolidayServiceHour.Open.Label'),
      },
    ];
  }, [t]);

  function triggerValidation() {
    props.trigger([
      `${cC.holidayOpeningHours}.${props.hourIndex}.${cHour.from}`,
      `${cC.holidayOpeningHours}.${props.hourIndex}.${cHour.to}`,
    ]);
  }

  return (
    <Grid container direction='column' className={classes.root}>
      <Grid item>
        <VisualHeading className='noTopMargin' variant='h4'>
          {t(getKeyForHoliday(props.holiday))}
        </VisualHeading>
      </Grid>

      <Grid item>
        <FormBlock marginTop='20px'>
          <RhfRadioButtonGroup<boolean>
            control={props.control}
            name={`${cC.holidayOpeningHours}.${props.hourIndex}.${cHour.isClosed}`}
            id={toFieldId(`${cC.holidayOpeningHours}.${props.hourIndex}.${cHour.isClosed}`)}
            mode='edit'
            items={items}
            labelText={t('Ptv.ConnectionDetails.HolidayServiceHour.Opening.Label')}
            toFieldValue={toFieldValue}
            toRadioButtonValue={toRadioButtonValue}
            onChanged={triggerValidation}
          />
        </FormBlock>
      </Grid>

      {!isClosed && (
        <Grid item>
          <FormBlock marginTop='15px'>
            <TimesSelect control={props.control} hourIndex={props.hourIndex} validate={triggerValidation} />
          </FormBlock>
        </Grid>
      )}
    </Grid>
  );
}
