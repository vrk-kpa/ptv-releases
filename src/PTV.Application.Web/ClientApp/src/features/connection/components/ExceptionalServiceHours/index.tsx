import React from 'react';
import { Control, UseFormTrigger, useFieldArray } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Button, Paragraph } from 'suomifi-ui-components';
import { ConnectionFormModel, cC, cHour, createExceptionalServiceHour } from 'types/forms/connectionFormTypes';
import { VisualHeading } from 'components/VisualHeading';
import { useFormMetaContext } from 'context/formMeta';
import { ExceptionalServiceHour } from 'features/connection/components/ExceptionalServiceHour';
import { FormBlock } from 'features/connection/components/FormLayout';
import { RemovableItemBlock } from 'features/connection/components/RemovableItemBlock';
import { toFieldId } from 'features/connection/utils/fieldid';

const useStyles = makeStyles(() => ({
  root: {
    '& p.noTopMargin': {
      marginTop: 0,
    },
  },
}));

type ExceptionalServiceHoursProps = {
  control: Control<ConnectionFormModel>;
  trigger: UseFormTrigger<ConnectionFormModel>;
};

export function ExceptionalServiceHours(props: ExceptionalServiceHoursProps): React.ReactElement {
  const meta = useFormMetaContext();
  const { t } = useTranslation();

  const classes = useStyles();

  const { fields, append, remove } = useFieldArray({
    name: `${cC.exceptionalOpeningHours}`,
    control: props.control,
  });

  function addServiceHour() {
    const index = fields.length;
    append(createExceptionalServiceHour(meta.availableLanguages));
    props.trigger(`${cC.exceptionalOpeningHours}.${index}.${cHour.validityPeriod}`);
  }

  function removeServiceHour(index: number) {
    remove(index);
  }

  return (
    <div className={classes.root}>
      <VisualHeading className='noTopMargin' id='exceptional-service-hours-heading' variant='h4'>
        {t('Ptv.ConnectionDetails.ExceptionalServiceHours.Title')}
      </VisualHeading>
      <FormBlock marginTop='20px' marginBottom='20px'>
        <Paragraph>{t('Ptv.ConnectionDetails.ExceptionalServiceHours.Description')}</Paragraph>
      </FormBlock>
      {fields.map((item, index) => {
        return (
          <div key={item.id}>
            <RemovableItemBlock onRemove={() => removeServiceHour(index)}>
              <ExceptionalServiceHour control={props.control} hourIndex={index} trigger={props.trigger} />
            </RemovableItemBlock>
          </div>
        );
      })}

      <Button
        aria-describedby='exceptional-service-hours-heading'
        id={toFieldId('add-exceptional-service-hour')}
        onClick={addServiceHour}
        variant='secondary'
      >
        {t('Ptv.ConnectionDetails.ExceptionalServiceHours.AddNew')}
      </Button>
    </div>
  );
}
