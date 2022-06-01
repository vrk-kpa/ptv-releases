import React from 'react';
import { Control, UseFormTrigger, useFieldArray } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Button, Paragraph } from 'suomifi-ui-components';
import { ConnectionFormModel, cC, createSpecialServiceHour } from 'types/forms/connectionFormTypes';
import { VisualHeading } from 'components/VisualHeading';
import { useFormMetaContext } from 'context/formMeta';
import { FormBlock } from 'features/connection/components/FormLayout';
import { RemovableItemBlock } from 'features/connection/components/RemovableItemBlock';
import { SpecialServiceHour } from 'features/connection/components/SpecialServiceHour';
import { toFieldId } from 'features/connection/utils/fieldid';

type SpecialServiceHoursProps = {
  control: Control<ConnectionFormModel>;
  trigger: UseFormTrigger<ConnectionFormModel>;
};

export function SpecialServiceHours(props: SpecialServiceHoursProps): React.ReactElement {
  const meta = useFormMetaContext();
  const { t } = useTranslation();

  const { fields, append, remove } = useFieldArray({
    name: `${cC.specialOpeningHours}`,
    control: props.control,
  });

  function addServiceHour() {
    append(createSpecialServiceHour(meta.availableLanguages));
  }

  function removeServiceHour(index: number) {
    remove(index);
  }

  return (
    <div>
      <VisualHeading id='special-service-hours-heading' variant='h4'>
        {t('Ptv.ConnectionDetails.SpecialServiceHours.Title')}
      </VisualHeading>
      <FormBlock marginTop='20px' marginBottom='20px'>
        <Paragraph>{t('Ptv.ConnectionDetails.SpecialServiceHours.Description')}</Paragraph>
      </FormBlock>
      {fields.map((item, index) => {
        return (
          <div key={item.id}>
            <RemovableItemBlock onRemove={() => removeServiceHour(index)}>
              <SpecialServiceHour control={props.control} hourIndex={index} trigger={props.trigger} />
            </RemovableItemBlock>
          </div>
        );
      })}

      <Button
        aria-describedby='special-service-hours-heading'
        id={toFieldId('add-special-service-hour')}
        onClick={addServiceHour}
        variant='secondary'
      >
        {t('Ptv.ConnectionDetails.SpecialServiceHours.AddNew')}
      </Button>
    </div>
  );
}
