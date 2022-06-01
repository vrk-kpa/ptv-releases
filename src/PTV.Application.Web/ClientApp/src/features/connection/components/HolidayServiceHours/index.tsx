import React from 'react';
import { Control, UseFormTrigger, useFieldArray } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Paragraph } from 'suomifi-ui-components';
import { HolidayEnum } from 'types/enumTypes';
import { ConnectionFormModel, HolidayServiceHourModel, cC, createHolidayServiceHour } from 'types/forms/connectionFormTypes';
import { VisualHeading } from 'components/VisualHeading';
import { FormBlock } from 'features/connection/components/FormLayout';
import { HolidayServiceHour } from 'features/connection/components/HolidayServiceHour';
import { RemovableItemBlock } from 'features/connection/components/RemovableItemBlock';
import { HolidaySelect } from './HolidaySelect';

const useStyles = makeStyles(() => ({
  root: {
    '& p.noTopMargin': {
      marginTop: 0,
    },
  },
}));

type HolidayServiceHoursProps = {
  control: Control<ConnectionFormModel>;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  trigger: UseFormTrigger<any>;
};

export function HolidayServiceHours(props: HolidayServiceHoursProps): React.ReactElement {
  const { t } = useTranslation();

  const classes = useStyles();

  const { fields, replace, remove } = useFieldArray({
    name: `${cC.holidayOpeningHours}`,
    control: props.control,
  });

  const selected = fields.map((x) => x.code);

  function addHoliday(holiday: HolidayEnum) {
    // Note: If you use append() it creates duplicate holiday and it happens only in dev build.
    // If you put console.log into this method you can see it is called only once but for some
    // reason the fields array contains two items: one with RHF generated id and one without id.
    // This does not hapen with other service hours (standard, exceptional etc.)
    // Even stranger:
    // Put simple Button inside HolidaySelect and call this method from the onClick handler -> works
    const existing: HolidayServiceHourModel[] = fields;
    const length = fields.length;
    replace(existing.concat([createHolidayServiceHour(holiday)]));
    props.trigger(`${cC.holidayOpeningHours}.${length}`);
  }

  function removeHoliday(holiday: HolidayEnum) {
    const index = fields.findIndex((x) => x.code === holiday);
    if (index !== -1) {
      // Note: if you use remove() the UI crashes when validation is executed. For some reason
      // the validation receives holiday hours where there exists an item but that item is invalid
      // This does not hapen with other service hours (standard, exceptional etc.)
      const existing: HolidayServiceHourModel[] = fields;
      const keep = existing.filter((x) => x.code !== holiday);
      replace(keep);
    }
  }

  function removeServiceHour(index: number) {
    remove(index);
  }

  return (
    <div className={classes.root}>
      <VisualHeading className='noTopMargin' variant='h4'>
        {t('Ptv.ConnectionDetails.HolidayServiceHours.Title')}
      </VisualHeading>
      <FormBlock marginTop='20px' marginBottom='20px'>
        <Paragraph>{t('Ptv.ConnectionDetails.HolidayServiceHours.Description')}</Paragraph>
      </FormBlock>
      <FormBlock marginBottom='20px'>
        <HolidaySelect control={props.control} add={addHoliday} remove={removeHoliday} selectedHolidays={selected} />
      </FormBlock>

      {fields.map((item, index) => {
        return (
          <div key={item.id}>
            <RemovableItemBlock onRemove={() => removeServiceHour(index)}>
              <HolidayServiceHour control={props.control} hourIndex={index} trigger={props.trigger} holiday={item.code} />
            </RemovableItemBlock>
          </div>
        );
      })}
    </div>
  );
}
