import React from 'react';
import { Control, useWatch } from 'react-hook-form';

type OptionalFieldByWatchProps<T> = {
  shouldRender: (value: T) => boolean;
  children: React.ReactElement;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  control: Control<any>;
  fieldName: string;
};

export function OptionalFieldByWatch<T>(props: OptionalFieldByWatchProps<T>): React.ReactElement | null {
  const value = useWatch({
    control: props.control,
    name: props.fieldName,
  }) as T;

  if (!props.shouldRender(value)) {
    return null;
  }

  return props.children;
}
