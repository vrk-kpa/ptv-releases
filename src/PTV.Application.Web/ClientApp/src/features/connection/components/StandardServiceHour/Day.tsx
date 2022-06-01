import React from 'react';
import { Control, UseFormTrigger, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { RhfCheckbox } from 'fields';
import { Weekday } from 'types/enumTypes';
import { ConnectionFormModel, cC, cDaily, cHour } from 'types/forms/connectionFormTypes';
import { getKeyForWeekday } from 'utils/translations';
import { FormBlock } from 'features/connection/components/FormLayout';
import { getWeekdayFieldName, toFieldId } from 'features/connection/utils/fieldid';
import { TimeList } from './TimeList';

const useStyles = makeStyles(() => ({
  timeList: {
    marginLeft: '20px',
  },
}));

type DayProps = {
  hourIndex: number;
  dayIndex: number;
  day: Weekday;
  control: Control<ConnectionFormModel>;
  trigger: UseFormTrigger<ConnectionFormModel>;
};

export function Day(props: DayProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();

  const activeFieldName = getWeekdayFieldName(props.hourIndex, props.dayIndex, cDaily.active);

  const active = useWatch({
    name: `${cC.standardOpeningHours}.${props.hourIndex}.${cHour.dailyOpeningTimes}.${props.dayIndex}.${cDaily.active}` as const,
    control: props.control,
  });

  function triggerValidation() {
    props.trigger(undefined);
  }

  return (
    <div>
      <FormBlock marginTop='0px' display='inline-block'>
        <RhfCheckbox
          afterClick={triggerValidation}
          control={props.control}
          id={toFieldId(activeFieldName)}
          name={activeFieldName}
          mode='edit'
        >
          {t(getKeyForWeekday(props.day)).toLowerCase()}
        </RhfCheckbox>
      </FormBlock>
      {active && (
        <div className={classes.timeList}>
          <TimeList control={props.control} trigger={props.trigger} hourIndex={props.hourIndex} dayIndex={props.dayIndex} />
        </div>
      )}
    </div>
  );
}
