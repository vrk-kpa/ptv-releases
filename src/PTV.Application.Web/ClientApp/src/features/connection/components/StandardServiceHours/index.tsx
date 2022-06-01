import React from 'react';
import { Control, UseFormTrigger, useFieldArray } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Button, Paragraph } from 'suomifi-ui-components';
import { ConnectionFormModel, cC, createStandardServiceHour } from 'types/forms/connectionFormTypes';
import { VisualHeading } from 'components/VisualHeading';
import { useFormMetaContext } from 'context/formMeta';
import { FormBlock } from 'features/connection/components/FormLayout';
import { RemovableItemBlock } from 'features/connection/components/RemovableItemBlock';
import { StandardServiceHour } from 'features/connection/components/StandardServiceHour';
import { toFieldId } from 'features/connection/utils/fieldid';

const useStyles = makeStyles(() => ({
  serviceHoursContent: {
    '&:first-child': {
      marginTop: 0,
    },
  },
}));

type StandardServiceHoursProps = {
  control: Control<ConnectionFormModel>;
  trigger: UseFormTrigger<ConnectionFormModel>;
};

export function StandardServiceHours(props: StandardServiceHoursProps): React.ReactElement {
  const meta = useFormMetaContext();
  const { t } = useTranslation();

  const classes = useStyles();

  const { fields, append, remove } = useFieldArray({
    name: `${cC.standardOpeningHours}`,
    control: props.control,
  });

  function addServiceHour() {
    const index = fields.length;
    append(createStandardServiceHour(meta.availableLanguages));
    props.trigger(`${cC.standardOpeningHours}.${index}`);
  }

  function removeServiceHour(index: number) {
    remove(index);
  }

  return (
    <div>
      <VisualHeading className={classes.serviceHoursContent} id='standard-service-hours-heading' variant='h4'>
        {t('Ptv.ConnectionDetails.StandardServiceHours.Title')}
      </VisualHeading>
      <FormBlock marginTop='20px' marginBottom='20px'>
        <Paragraph>{t('Ptv.ConnectionDetails.StandardServiceHours.Description')}</Paragraph>
      </FormBlock>
      {fields.map((item, index) => {
        return (
          <div key={item.id}>
            <RemovableItemBlock onRemove={() => removeServiceHour(index)}>
              <StandardServiceHour trigger={props.trigger} control={props.control} hourIndex={index} />
            </RemovableItemBlock>
          </div>
        );
      })}

      <Button
        id={toFieldId('add-standard-service-hour')}
        aria-describedby='standard-service-hours-heading'
        onClick={addServiceHour}
        variant='secondary'
      >
        {t('Ptv.ConnectionDetails.StandardServiceHours.AddNew')}
      </Button>
    </div>
  );
}
