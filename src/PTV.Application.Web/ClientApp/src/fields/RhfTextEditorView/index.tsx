import React from 'react';
import { UseControllerProps, useController } from 'react-hook-form';
import { TextEditorView } from 'components/TextEditorView';
import { parseRawDescription } from 'utils/draftjs';

type RhfTextEditorViewProps = {
  labelText?: string;
  tooltipText?: string;
  id: string;
  value?: string | null | undefined;
};

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export function RhfTextEditorView(props: RhfTextEditorViewProps & UseControllerProps<any>): React.ReactElement {
  const { field } = useController(props);

  const value = parseRawDescription(field.value);

  return <TextEditorView id={props.id} value={value} valueLabel={props.labelText} valueTooltip={props.tooltipText} />;
}
