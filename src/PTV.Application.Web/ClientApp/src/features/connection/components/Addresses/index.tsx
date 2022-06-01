import React from 'react';
import { Control, UseFormTrigger, useFieldArray } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Button, Paragraph } from 'suomifi-ui-components';
import { ConnectionFormModel, cC, createAddress } from 'types/forms/connectionFormTypes';
import { VisualHeading } from 'components/VisualHeading';
import { useFormMetaContext } from 'context/formMeta';
import { FormBlock } from 'features/connection/components/FormLayout';
import { RemovableItemBlock } from 'features/connection/components/RemovableItemBlock';
import { toFieldId } from 'features/connection/utils/fieldid';
import { Address } from './Address';

type AddressesProps = {
  control: Control<ConnectionFormModel>;
  trigger: UseFormTrigger<ConnectionFormModel>;
};

export function Addresses(props: AddressesProps): React.ReactElement {
  const { t } = useTranslation();
  const meta = useFormMetaContext();

  const { fields, remove, append } = useFieldArray({
    name: `${cC.addresses}`,
    control: props.control,
  });

  function addAddress() {
    const index = fields.length;
    append(createAddress(meta.availableLanguages));
    props.trigger(`${cC.addresses}.${index}`);
  }

  function removeAddress(index: number) {
    remove(index);
  }

  return (
    <div>
      <VisualHeading id='addresses-heading' variant='h4'>
        {t('Ptv.ConnectionDetails.Addresses.Title')}
      </VisualHeading>
      <FormBlock marginTop='20px' marginBottom='20px'>
        <Paragraph>{t('Ptv.ConnectionDetails.Addresses.Description')}</Paragraph>
      </FormBlock>
      {fields.map((item, index) => {
        return (
          <div key={item.id}>
            <RemovableItemBlock onRemove={() => removeAddress(index)}>
              <Address control={props.control} addressIndex={index} trigger={props.trigger} />
            </RemovableItemBlock>
          </div>
        );
      })}

      <Button aria-describedby='addresses-heading' id={toFieldId('add-address')} onClick={addAddress} variant='secondary'>
        {t('Ptv.ConnectionDetails.Addresses.AddNew')}
      </Button>
    </div>
  );
}
