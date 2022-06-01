import React from 'react';
import { Control, UseFormTrigger, useFieldArray } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Button } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ConnectionFormModel, cC, cPhoneNumberLv, createPhoneNumber } from 'types/forms/connectionFormTypes';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import { getFinnishDialCode } from 'utils/phoneNumbers';
import { RemovableItemBlock } from 'features/connection/components/RemovableItemBlock';
import { toFieldId } from 'features/connection/utils/fieldid';
import { PhoneNumber } from './PhoneNumber';

type PhoneNumberListProps = {
  control: Control<ConnectionFormModel>;
  trigger: UseFormTrigger<ConnectionFormModel>;
  language: Language;
};

export function PhoneNumberList(props: PhoneNumberListProps): React.ReactElement {
  const { t } = useTranslation();
  const appContext = useAppContextOrThrow();

  const { fields, append, remove } = useFieldArray({
    name: `${cC.phoneNumbers}.${props.language}`,
    control: props.control,
  });

  function addPhoneNumber() {
    const index = fields.length;
    const dialCode = getFinnishDialCode(appContext.staticData.dialCodes);
    append(createPhoneNumber(dialCode?.id));
    props.trigger(`${cC.phoneNumbers}.${props.language}.${index}.${cPhoneNumberLv.number}`);
  }

  function removePhoneNumber(index: number) {
    remove(index);
  }

  return (
    <div>
      {fields.map((item, index) => {
        return (
          <div key={item.id}>
            <RemovableItemBlock onRemove={() => removePhoneNumber(index)}>
              <PhoneNumber phoneNumberIndex={index} control={props.control} trigger={props.trigger} language={props.language} />
            </RemovableItemBlock>
          </div>
        );
      })}

      <Button
        aria-describedby='phone-numbers-heading'
        id={toFieldId(`${cC.phoneNumbers}.${props.language}.add`)}
        onClick={addPhoneNumber}
        variant='secondary'
      >
        {t('Ptv.ConnectionDetails.PhoneNumbers.AddNew')}
      </Button>
    </div>
  );
}
