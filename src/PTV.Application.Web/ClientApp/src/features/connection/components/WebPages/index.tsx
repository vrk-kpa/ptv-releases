import React from 'react';
import { Control, UseFormTrigger } from 'react-hook-form';
import { ConnectionFormModel } from 'types/forms/connectionFormTypes';
import { ComparisonView } from 'components/ComparisonView';
import { useFormMetaContext } from 'context/formMeta';
import { WebPageList } from './WebPageList';

type WebPagesProps = {
  control: Control<ConnectionFormModel>;
  trigger: UseFormTrigger<ConnectionFormModel>;
};

export function WebPages(props: WebPagesProps): React.ReactElement {
  const meta = useFormMetaContext();

  function renderRight(): React.ReactElement | undefined {
    if (meta.compareLanguageCode) {
      return <WebPageList control={props.control} language={meta.compareLanguageCode} trigger={props.trigger} />;
    }

    return undefined;
  }

  return (
    <div>
      <ComparisonView
        left={<WebPageList control={props.control} language={meta.selectedLanguageCode} trigger={props.trigger} />}
        right={renderRight()}
      />
    </div>
  );
}
