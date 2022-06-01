import React, { useMemo } from 'react';
import { Control, UseFormTrigger, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { RhfRadioButtonGroup } from 'fields';
import { ConnectionFormModel, ValidityPeriod, cC, cHour } from 'types/forms/connectionFormTypes';
import { FormBlock } from 'features/connection/components/FormLayout';
import { toFieldId } from 'features/connection/utils/fieldid';
import { OpenBetweenDates } from './OpenBetweenDates';
import { OpenUntilFurtherNotice } from './OpenUntilFurtherNotice';

type OpeningTypeProps = {
  hourIndex: number;
  control: Control<ConnectionFormModel>;
  trigger: UseFormTrigger<ConnectionFormModel>;
};

export function OpeningType(props: OpeningTypeProps): React.ReactElement {
  const { t } = useTranslation();

  const validityPeriod = useWatch({
    name: `${cC.specialOpeningHours}.${props.hourIndex}.${cHour.validityPeriod}`,
    control: props.control,
  });

  const items = useMemo(() => {
    return [
      {
        value: 'UntilFurtherNotice',
        text: t('Ptv.ConnectionDetails.SpecialServiceHour.Validity.Option.UntilFurtherNotice'),
      },
      {
        value: 'BetweenDates',
        text: t('Ptv.ConnectionDetails.SpecialServiceHour.Validity.Option.BetweenDates'),
      },
    ];
  }, [t]);

  const untilFurtherNoticedChecked = validityPeriod === 'UntilFurtherNotice';
  const betweenDatesChecked = validityPeriod === 'BetweenDates';

  function triggerValidation() {
    // Currently there is no better way to trigger validation for whole item
    props.trigger([
      `${cC.specialOpeningHours}.${props.hourIndex}.${cHour.openingHoursFrom}`,
      `${cC.specialOpeningHours}.${props.hourIndex}.${cHour.openingHoursTo}`,
      `${cC.specialOpeningHours}.${props.hourIndex}.${cHour.dayFrom}`,
      `${cC.specialOpeningHours}.${props.hourIndex}.${cHour.dayTo}`,
      `${cC.specialOpeningHours}.${props.hourIndex}.${cHour.timeFrom}`,
      `${cC.specialOpeningHours}.${props.hourIndex}.${cHour.timeTo}`,
    ]);
  }

  return (
    <div>
      <RhfRadioButtonGroup<ValidityPeriod>
        control={props.control}
        name={`${cC.specialOpeningHours}.${props.hourIndex}.${cHour.validityPeriod}`}
        id={toFieldId(`${cC.specialOpeningHours}.${props.hourIndex}.${cHour.validityPeriod}`)}
        mode='edit'
        items={items}
        labelText={t('Ptv.ConnectionDetails.SpecialServiceHour.Validity.Label')}
        onChanged={triggerValidation}
      />

      {untilFurtherNoticedChecked && (
        <FormBlock marginBottom='15px' marginTop='20px'>
          <OpenUntilFurtherNotice control={props.control} hourIndex={props.hourIndex} validate={triggerValidation} />
        </FormBlock>
      )}
      {betweenDatesChecked && (
        <FormBlock marginBottom='15px' marginTop='20px'>
          <OpenBetweenDates control={props.control} hourIndex={props.hourIndex} validate={triggerValidation} />
        </FormBlock>
      )}
    </div>
  );
}
