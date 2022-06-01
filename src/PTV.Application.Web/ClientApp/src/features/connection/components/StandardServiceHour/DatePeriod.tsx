import React from 'react';
import { Control, UseFormTrigger, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Grid } from '@mui/material';
import { ConnectionFormModel, cC, cHour } from 'types/forms/connectionFormTypes';
import { FormBlock } from 'features/connection/components/FormLayout';
import { TempDateField } from 'features/connection/components/TempDateField';
import { getStdHourFieldName, toFieldId } from 'features/connection/utils/fieldid';

type DatePeriodProps = {
  hourIndex: number;
  control: Control<ConnectionFormModel>;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  trigger: UseFormTrigger<any>;
};

export function DatePeriod(props: DatePeriodProps): React.ReactElement | null {
  const { t } = useTranslation();

  const fromFieldName = getStdHourFieldName(props.hourIndex, cHour.openingHoursFrom);
  const toFieldName = getStdHourFieldName(props.hourIndex, cHour.openingHoursTo);

  const validityPeriod = useWatch({
    name: `${cC.standardOpeningHours}.${props.hourIndex}.${cHour.validityPeriod}` as const,
    control: props.control,
  });

  if (validityPeriod === 'UntilFurtherNotice') {
    return null;
  }

  function onChanged() {
    // Need to trigger validation, otherwise form has errors but they are not show to the user
    props.trigger([fromFieldName, toFieldName]);
  }

  return (
    <FormBlock marginTop='20px' marginLeft='20px'>
      <Grid container>
        <Grid item>
          <FormBlock marginRight='15px'>
            <TempDateField
              name={fromFieldName}
              id={toFieldId(fromFieldName)}
              control={props.control}
              label={t('Ptv.Common.StartDay')}
              onChanged={onChanged}
            />
          </FormBlock>
        </Grid>
        <Grid item>
          <TempDateField
            name={toFieldName}
            id={toFieldId(toFieldName)}
            control={props.control}
            label={t('Ptv.Common.EndDay')}
            onChanged={onChanged}
          />
        </Grid>
      </Grid>
    </FormBlock>
  );
}
