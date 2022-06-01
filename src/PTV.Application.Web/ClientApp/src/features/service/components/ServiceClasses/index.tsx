import React from 'react';
import { Control, useController } from 'react-hook-form';
import { Block } from 'suomifi-ui-components';
import { ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { GeneralDescriptionModel } from 'types/generalDescriptionTypes';
import { Message } from 'components/Message';
import { toFieldStatus } from 'utils/rhf';
import { ClassificationItems } from 'features/service/components/ClassificationItems';
import { ClassificationItemsContextProvider } from 'features/service/contexts/ClassificationItems/ClassificationItemsContextProvider';

type ServiceClassesProps = {
  gd: GeneralDescriptionModel | null | undefined;
  control: Control<ServiceModel>;
  namespace: string;
};

export function ServiceClasses(props: ServiceClassesProps): React.ReactElement {
  const { field, fieldState } = useController({
    control: props.control,
    name: `${cService.serviceClasses}`,
  });

  const { status, statusText } = toFieldStatus(fieldState);

  const gdItems = props.gd?.serviceClasses || [];

  function setFieldValue(values: string[]) {
    field.onChange(values);
  }

  return (
    <ClassificationItemsContextProvider
      classification={cService.serviceClasses}
      namespace={`${props.namespace}.${cService.serviceClasses}`}
      gdItems={gdItems}
      selectedItems={field.value}
    >
      <Block>
        <ClassificationItems setFieldValue={setFieldValue} fieldValue={field.value} control={props.control} />
        {status === 'error' && <Message type='error'>{statusText}</Message>}
      </Block>
    </ClassificationItemsContextProvider>
  );
}
