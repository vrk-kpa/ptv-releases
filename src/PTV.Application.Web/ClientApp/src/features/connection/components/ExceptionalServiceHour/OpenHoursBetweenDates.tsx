import React from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { RhfCheckbox } from 'fields';
import { Text } from 'suomifi-ui-components';
import { ConnectionFormModel, cC, cHour } from 'types/forms/connectionFormTypes';
import { FormBlock } from 'features/connection/components/FormLayout';
import { toFieldId } from 'features/connection/utils/fieldid';

type OpenHoursBetweenDatesProps = {
  hourIndex: number;
  control: Control<ConnectionFormModel>;
  onChanged: () => void;
};

export function OpenHoursBetweenDates(props: OpenHoursBetweenDatesProps): React.ReactElement {
  const { t } = useTranslation();

  return (
    <FormBlock>
      <Text variant='bold' smallScreen={true}>
        {t('Ptv.ConnectionDetails.ExceptionalServiceHour.OpenOrClosed.Label')}
      </Text>

      <FormBlock marginTop='10px'>
        <RhfCheckbox
          control={props.control}
          id={toFieldId(`${cC.exceptionalOpeningHours}.${props.hourIndex}.${cHour.isClosed}`)}
          name={`${cC.exceptionalOpeningHours}.${props.hourIndex}.${cHour.isClosed}`}
          mode='edit'
          afterClick={props.onChanged}
        >
          {t('Ptv.Common.Closed')}
        </RhfCheckbox>
      </FormBlock>
    </FormBlock>
  );
}
