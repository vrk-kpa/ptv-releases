import React, { FunctionComponent } from 'react';
import { Control } from 'react-hook-form';
import { Block } from 'suomifi-ui-components';
import { ServiceModel } from 'types/forms/serviceFormTypes';
import { useFormMetaContext } from 'context/formMeta';
import { TreeDisplay } from './TreeDisplay';
import { TreeSelect } from './TreeSelect';
import { TreeView } from './TreeView';

type ClassificationItemsProps = {
  setFieldValue: (values: string[]) => void;
  fieldValue: string[];
  control: Control<ServiceModel>;
};

export const ClassificationItems: FunctionComponent<ClassificationItemsProps> = (props: ClassificationItemsProps) => {
  const { mode } = useFormMetaContext();

  return (
    (mode === 'edit' && (
      <Block>
        <TreeSelect setFieldValue={props.setFieldValue} fieldValue={props.fieldValue} control={props.control} />
        <TreeDisplay fieldValue={props.fieldValue} setFieldValue={props.setFieldValue} control={props.control} />
      </Block>
    )) || <TreeView control={props.control} fieldValue={props.fieldValue} />
  );
};
