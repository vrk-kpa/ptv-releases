import React from 'react';
import { Control, UseFormTrigger, useFieldArray } from 'react-hook-form';
import { Box } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { ConnectionFormModel, cC, cDaily, cHour, createTimeRangeModel } from 'types/forms/connectionFormTypes';
import { FormBlock } from 'features/connection/components/FormLayout';
import { AddTimeRange } from './AddTimeRange';
import { TimeRange } from './TimeRange';

const useStyles = makeStyles(() => ({
  list: {
    padding: '0px',
    margin: '0px',
    listStyle: 'none',
  },
}));

type TimeListProps = {
  hourIndex: number;
  dayIndex: number;
  control: Control<ConnectionFormModel>;
  trigger: UseFormTrigger<ConnectionFormModel>;
};

export function TimeList(props: TimeListProps): React.ReactElement {
  const classes = useStyles();
  const trigger = props.trigger;

  const { fields, remove, append } = useFieldArray({
    name: `${cC.standardOpeningHours}.${props.hourIndex}.${cHour.dailyOpeningTimes}.${props.dayIndex}.${cDaily.times}` as const,
    control: props.control,
  });

  function add() {
    append(createTimeRangeModel());
    trigger(undefined);
  }

  function removeTimeRange(index: number) {
    remove(index);
    trigger(undefined);
  }

  return (
    <div>
      <Box>
        <ul className={classes.list}>
          {fields.map((tr, index) => {
            return (
              <li key={tr.id}>
                <FormBlock display='inline-block' marginTop='10px'>
                  <TimeRange
                    trigger={props.trigger}
                    control={props.control}
                    hourIndex={props.hourIndex}
                    timeIndex={index}
                    dayIndex={props.dayIndex}
                    remove={() => removeTimeRange(index)}
                  />
                </FormBlock>
              </li>
            );
          })}
        </ul>
        <FormBlock marginTop='20px'>
          <AddTimeRange
            hourIndex={props.hourIndex}
            dayIndex={props.dayIndex}
            control={props.control}
            add={add}
            timeRangeCount={fields.length}
          />
        </FormBlock>
      </Box>
    </div>
  );
}
