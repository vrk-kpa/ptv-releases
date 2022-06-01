import React, { ReactElement, ReactNode, RefObject, cloneElement, isValidElement, useState } from 'react';
import { UseControllerProps, useController } from 'react-hook-form';
import { styled } from '@mui/material/styles';
import { Editor, EditorState, convertToRaw } from 'draft-js';
import { HintText, Label } from 'suomifi-ui-components';
import { Mode } from 'types/enumTypes';
import { QualityResult } from 'types/qualityAgentResponses';
import RichTextEditor from 'components/RichTextEditor';
import { TextEditorView } from 'components/TextEditorView';
import { parseRawDescription } from 'utils/draftjs';
import { toFieldStatus } from 'utils/rhf';

export type RhfTextEditorProps = {
  forwardedRef?: RefObject<Editor>;
  className?: string;
  labelText?: string;
  tooltipComponent?: ReactElement;
  optionalText?: string | undefined;
  placeHolder?: string;
  hintText?: string;
  maxCharacters: number;
  mode: Mode;
  id: string;
  value?: string | null | undefined;
  qualityResults?: QualityResult[];
};

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const RhfTextEditor = styled((props: RhfTextEditorProps & UseControllerProps<any>): React.ReactElement => {
  const { field, fieldState } = useController(props);
  const { status, statusText } = toFieldStatus(fieldState);
  const [wrapperRef, setWrapperlRef] = useState<HTMLDivElement | null>(null);

  function onChange(state: EditorState) {
    const contentState = state.getCurrentContent();
    const raw = convertToRaw(contentState);
    field.onChange(JSON.stringify(raw));
  }

  function getTooltipComponent(tooltipComponent: ReactElement | undefined): ReactNode {
    if (isValidElement(tooltipComponent)) {
      return cloneElement(tooltipComponent, {
        anchorElement: wrapperRef,
      });
    }
    return null;
  }

  // If we use RawDraftContentState as the field.value and field is empty
  // (i.e. not null but empty RawDraftContentState so inside it e.g. entityRangers is empty array)
  // then RHF will remove those empty values breaking draftjs's content.
  const value = parseRawDescription(field.value);

  if (props.mode === 'view') {
    return <TextEditorView id={props.id} value={value} valueLabel={props.labelText} />;
  }

  return (
    <div ref={(ref) => setWrapperlRef(ref)} className={props.className}>
      <Label className='custom-label' optionalText={props.optionalText}>
        {props.labelText}
      </Label>
      {!!props.tooltipComponent && getTooltipComponent(props.tooltipComponent)}
      <HintText className='hint-text'>{props.hintText}</HintText>
      <RichTextEditor
        forwardedRef={props.forwardedRef}
        className='rich-text-editor'
        status={status}
        statusText={statusText}
        maxCharacters={props.maxCharacters}
        placeHolder={props.placeHolder}
        value={value}
        onChange={onChange}
        id={props.id}
        qualityResults={props.qualityResults}
      />
    </div>
  );
})(() => ({
  '& .fi-label-text.custom-label': {
    display: 'inline',
    verticalAlign: 'middle',
    marginBottom: '10px',
    '& .fi-label-text_label-span': {
      display: 'inline',
    },
  },
  '& .rich-text-editor': {
    marginTop: '10px',
  },
  '& .hint-text': {
    marginBottom: '10px',
  },
}));
