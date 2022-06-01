import React from 'react';
import { Control, UseFormTrigger, useFieldArray } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Button } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ConnectionFormModel, cC, createFaxNumber } from 'types/forms/connectionFormTypes';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import { getFinnishDialCode } from 'utils/phoneNumbers';
import { RemovableItemBlock } from 'features/connection/components/RemovableItemBlock';
import { toFieldId } from 'features/connection/utils/fieldid';
import { FaxNumber } from './FaxNumber';

type FaxNumberListProps = {
  control: Control<ConnectionFormModel>;
  trigger: UseFormTrigger<ConnectionFormModel>;
  language: Language;
};

export function FaxNumberList(props: FaxNumberListProps): React.ReactElement {
  const { t } = useTranslation();
  const appContext = useAppContextOrThrow();

  const { fields, append, remove } = useFieldArray({
    name: `${cC.faxNumbers}.${props.language}`,
    control: props.control,
  });

  function addFaxNumber() {
    const index = fields.length;
    const dialCode = getFinnishDialCode(appContext.staticData.dialCodes);
    append(createFaxNumber(dialCode?.id));
    props.trigger(`${cC.faxNumbers}.${props.language}.${index}`);
  }

  function removeFaxNumber(index: number) {
    remove(index);
  }

  return (
    <div>
      {fields.map((item, index) => {
        return (
          <div key={item.id}>
            <RemovableItemBlock onRemove={() => removeFaxNumber(index)}>
              <FaxNumber faxNumberIndex={index} control={props.control} language={props.language} />
            </RemovableItemBlock>
          </div>
        );
      })}

      <Button
        aria-describedby='fax-numbers-heading'
        id={toFieldId(`${cC.faxNumbers}.${props.language}.add`)}
        onClick={addFaxNumber}
        variant='secondary'
      >
        {t('Ptv.ConnectionDetails.FaxNumbers.AddNew')}
      </Button>
    </div>
  );
}
