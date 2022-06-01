import React, { useCallback, useMemo } from 'react';
import { Control, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { RhfRadioButtonGroup } from 'fields';
import { ConnectionFormModel, cC, cHour } from 'types/forms/connectionFormTypes';
import { FormBlock } from 'features/connection/components/FormLayout';
import { getExceptionalHourFieldName, toFieldId } from 'features/connection/utils/fieldid';
import { TimesSelect } from './TimesSelect';

type OpenHoursDayProps = {
  hourIndex: number;
  control: Control<ConnectionFormModel>;
  onChanged: () => void;
};

const Open = 'open';
const Closed = 'closed';

export function OpenHoursDay(props: OpenHoursDayProps): React.ReactElement {
  const { t } = useTranslation();

  const isClosed = useWatch({
    name: `${cC.exceptionalOpeningHours}.${props.hourIndex}.${cHour.isClosed}`,
    control: props.control,
  });

  const items = useMemo(() => {
    return [
      {
        value: Closed,
        text: t('Ptv.Common.Closed'),
      },
      {
        value: Open,
        text: t('Ptv.Common.Open'),
      },
    ];
  }, [t]);

  const toFieldValue = useCallback((str: string): boolean => {
    return str === Closed ? true : false;
  }, []);

  const toRadioButtonValue = useCallback((value: boolean): string => {
    return value === true ? Closed : Open;
  }, []);

  const fieldName = getExceptionalHourFieldName(props.hourIndex, cHour.isClosed);

  return (
    <div>
      <RhfRadioButtonGroup<boolean>
        control={props.control}
        id={toFieldId(fieldName)}
        name={fieldName}
        mode='edit'
        labelText={t('Ptv.ConnectionDetails.ExceptionalServiceHour.OpenOrClosed.Label')}
        items={items}
        toFieldValue={toFieldValue}
        toRadioButtonValue={toRadioButtonValue}
        onChanged={props.onChanged}
      />

      {!isClosed && (
        <FormBlock>
          <TimesSelect control={props.control} hourIndex={props.hourIndex} onChanged={props.onChanged} />
        </FormBlock>
      )}
    </div>
  );
}
