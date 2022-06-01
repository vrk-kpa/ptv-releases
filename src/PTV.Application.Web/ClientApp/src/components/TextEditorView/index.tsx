import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { styled } from '@mui/material/styles';
import { Editor, EditorState, RawDraftContentState, convertFromRaw } from 'draft-js';
import { Text } from 'suomifi-ui-components';
import { Tooltip } from 'components/Tooltip';

type DraftValue = RawDraftContentState | string | null | undefined;

interface TextEditorViewInterface {
  id: string;
  value: DraftValue;
  valueLabel?: string;
  valueTooltip?: string;
  className?: string;
}

export const TextEditorView = styled(({ id, value, valueLabel, valueTooltip, className, ...rest }: TextEditorViewInterface) => {
  const [wrapperRef, setWrapperlRef] = useState<HTMLDivElement | null>(null);
  const { t } = useTranslation();

  function createEditorState(value: DraftValue): EditorState {
    if (!value) {
      return EditorState.createEmpty();
    }

    if (typeof value === 'string') {
      return EditorState.createWithContent(convertFromRaw(JSON.parse(value)));
    }

    return EditorState.createWithContent(convertFromRaw(value));
  }

  const editorState = createEditorState(value);

  return (
    <div ref={(ref) => setWrapperlRef(ref)} className={className}>
      {valueLabel && (
        <Text smallScreen variant='bold' className='value-label'>
          {valueLabel}
        </Text>
      )}
      {!!valueLabel && !!valueTooltip && (
        <Tooltip
          anchorElement={wrapperRef}
          ariaInfoButtonLabelText={t('Ptv.Common.Tooltip.Button.Label', {
            label: valueLabel,
          })}
          ariaCloseButtonLabelText={t('Ptv.Common.Tooltip.CloseButton.Label', {
            label: valueLabel,
          })}
        >
          {t('Ptv.Service.Form.Field.FeeExtraInfo.Tooltip')}
        </Tooltip>
      )}
      <Editor
        readOnly={true}
        onChange={() => {
          return;
        }}
        editorState={editorState}
      />
    </div>
  );
})(() => ({
  '& > div.DraftEditor-root': {
    marginLeft: '0',
  },
  '& .fi-text.value-label': {
    verticalAlign: 'middle',
    display: 'inline',
  },
}));
