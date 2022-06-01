import React from 'react';
import { Control, UseFormTrigger } from 'react-hook-form';
import { makeStyles } from '@mui/styles';
import { weekDayType } from 'types/enumTypes';
import { ConnectionFormModel, cC } from 'types/forms/connectionFormTypes';
import { FormBlock } from 'features/connection/components/FormLayout';
import { Day } from './Day';

const useStyles = makeStyles(() => ({
  list: {
    padding: '0px',
    margin: '0px',
    listStyle: 'none',
  },
}));

type WeekdaysProps = {
  hourIndex: number;
  control: Control<ConnectionFormModel>;
  trigger: UseFormTrigger<ConnectionFormModel>;
};

export function Weekdays(props: WeekdaysProps): React.ReactElement {
  const classes = useStyles();

  return (
    <div>
      <ul className={classes.list}>
        {weekDayType.map((day, index) => {
          const id = `${cC.standardOpeningHours}.${props.hourIndex}.${day}`;
          return (
            <li id={id} key={id}>
              <FormBlock marginTop='16px'>
                <Day day={day} control={props.control} trigger={props.trigger} hourIndex={props.hourIndex} dayIndex={index} />
              </FormBlock>
            </li>
          );
        })}
      </ul>
    </div>
  );
}
