import React from 'react';
import { Control, UseFormTrigger, useFieldArray } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Button, Heading } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ConnectionFormModel, cC, createWebPage } from 'types/forms/connectionFormTypes';
import { FormBlock } from 'features/connection/components/FormLayout';
import { RemovableItemBlock } from 'features/connection/components/RemovableItemBlock';
import { toFieldId } from 'features/connection/utils/fieldid';
import { WebPage } from './WebPage';

type WebPageListProps = {
  control: Control<ConnectionFormModel>;
  language: Language;
  trigger: UseFormTrigger<ConnectionFormModel>;
};

export function WebPageList(props: WebPageListProps): React.ReactElement {
  const { t } = useTranslation();

  const { fields, append, remove } = useFieldArray({
    name: `${cC.webPages}.${props.language}`,
    control: props.control,
  });

  function addWebPage() {
    const index = fields.length;
    append(createWebPage());
    props.trigger(`${cC.webPages}.${props.language}.${index}`);
  }

  function removeWebPage(index: number) {
    remove(index);
  }

  return (
    <div>
      <FormBlock marginBottom='20px'>
        <Heading id='webpages-heading' variant='h4'>
          {t('Ptv.ConnectionDetails.WebPages.Title')}
        </Heading>
      </FormBlock>

      {fields.map((item, index) => {
        return (
          <div key={item.id}>
            <RemovableItemBlock onRemove={() => removeWebPage(index)}>
              <WebPage webPageIndex={index} control={props.control} language={props.language} />
            </RemovableItemBlock>
          </div>
        );
      })}

      <Button
        aria-describedby='webpages-heading'
        id={toFieldId(`${cC.webPages}.${props.language}.add`)}
        onClick={addWebPage}
        variant='secondary'
      >
        {t('Ptv.ConnectionDetails.WebPages.AddNew')}
      </Button>
    </div>
  );
}
