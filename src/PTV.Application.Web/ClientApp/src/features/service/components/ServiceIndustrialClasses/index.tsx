import React from 'react';
import { Control, useController } from 'react-hook-form';
import { ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { GeneralDescriptionModel } from 'types/generalDescriptionTypes';
import { ClassificationItems } from 'features/service/components/ClassificationItems';
import { ClassificationItemsContextProvider } from 'features/service/contexts/ClassificationItems/ClassificationItemsContextProvider';

type ServiceIndustrialClassesProps = {
  gd: GeneralDescriptionModel | null | undefined;
  control: Control<ServiceModel>;
  namespace: string;
};

export function ServiceIndustrialClasses(props: ServiceIndustrialClassesProps): React.ReactElement {
  const { field } = useController({
    control: props.control,
    name: `${cService.industrialClasses}`,
  });

  const gdItems = props.gd?.industrialClasses || [];

  function setFieldValue(values: string[]) {
    field.onChange(values);
  }

  return (
    <ClassificationItemsContextProvider
      classification={cService.industrialClasses}
      namespace={`${props.namespace}.${cService.industrialClasses}`}
      gdItems={gdItems}
      selectedItems={field.value}
    >
      <ClassificationItems setFieldValue={setFieldValue} fieldValue={field.value} control={props.control} />
    </ClassificationItemsContextProvider>
  );
}
