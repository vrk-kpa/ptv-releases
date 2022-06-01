import React from 'react';
import { Control, UseFormTrigger, useFieldArray } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Button } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ConnectionFormModel, cC, cEmailLv, createEmailAddress } from 'types/forms/connectionFormTypes';
import { RemovableItemBlock } from 'features/connection/components/RemovableItemBlock';
import { toFieldId } from 'features/connection/utils/fieldid';
import { Email } from './Email';

type EmailAddressListProps = {
  control: Control<ConnectionFormModel>;
  language: Language;
  trigger: UseFormTrigger<ConnectionFormModel>;
};

export function EmailAddressList(props: EmailAddressListProps): React.ReactElement {
  const { t } = useTranslation();
  const { fields, append, remove } = useFieldArray({
    name: `${cC.emails}.${props.language}`,
    control: props.control,
  });

  function addEmail() {
    const index = fields.length;
    append(createEmailAddress());
    props.trigger(`${cC.emails}.${props.language}.${index}.${cEmailLv.value}`);
  }

  function removeEmail(index: number) {
    remove(index);
  }

  return (
    <div>
      {fields.map((item, index) => {
        return (
          <div key={item.id}>
            <RemovableItemBlock onRemove={() => removeEmail(index)}>
              <Email emailIndex={index} control={props.control} language={props.language} />
            </RemovableItemBlock>
          </div>
        );
      })}

      <Button
        aria-describedby='email-addresses-heading'
        id={toFieldId(`${cC.emails}.${props.language}.add`)}
        onClick={addEmail}
        variant='secondary'
      >
        {t('Ptv.ConnectionDetails.EmailAddresses.AddNew')}
      </Button>
    </div>
  );
}
