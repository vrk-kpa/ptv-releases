import React, { RefObject, useEffect, useState } from 'react';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';
import {
  CompositeDecorator,
  ContentBlock,
  Editor,
  EditorState,
  RawDraftContentState,
  RichUtils,
  convertFromRaw,
  convertToRaw,
} from 'draft-js';
import 'draft-js/dist/Draft.css';
import { escapeRegExp } from 'lodash';
import { InputStatus } from 'types/enumTypes';
import { QualityResult } from 'types/qualityAgentResponses';
import ValidationMessage from 'components/ValidationMessage';
import { useFormMetaContext } from 'context/formMeta';
import { useGetQualityAgentLoading } from 'context/qualityAgent/useGetQualityAgentLoading';
import { useRefOrCreate } from 'hooks/useRefOrCreate';
import { areEqual, getSelectionStartBlockType } from 'utils/draftjs';
import { matchingTokenRegExp } from 'utils/qualityAssurance';
import { issuesForGuidedCorrection } from 'features/qualityAgent/utility';
import Highlight from './Highlight';
import { OrderedListItem, Toolbar, UnorderedListItem } from './Toolbar';
import './styles.css';

const useStyles = makeStyles((theme) => ({
  focused: {
    textDecoration: 'none',
    position: 'relative',
    '&::after': {
      content: '""',
      position: 'absolute',
      top: '-4px',
      bottom: '-4px',
      left: '-4px',
      right: '-4px',
      borderRadius: '4px',
      border: '2px solid',
      borderColor: theme.suomifi.values.colors.accentSecondary.hsl,
      pointerEvents: 'none',
    },
  },
  editorContainer: {
    border: '1px solid',
    borderColor: theme.suomifi.values.colors.depthDark3.hsl,
    borderRadius: '2px',
  },
  editor: {
    borderTop: '1px solid',
    borderColor: theme.suomifi.values.colors.depthDark3.hsl,
    minHeight: 210,
    cursor: 'text',
  },
  error: {
    borderColor: theme.suomifi.values.colors.alertBase.hsl,
    borderWidth: '2px',
  },
  warning: {
    borderColor: theme.suomifi.values.colors.warningBase.hsl,
  },
}));

type RichTextEditorProps = {
  forwardedRef?: RefObject<Editor>;
  className?: string;
  value?: RawDraftContentState | null;
  placeHolder?: string;
  maxCharacters: number;
  onChange: (state: EditorState) => void;
  status: InputStatus;
  statusText?: string;
  id: string;
  qualityResults?: QualityResult[];
};

export default function RichTextEditor(props: RichTextEditorProps): React.ReactElement {
  const isQualityCheckLoading = useGetQualityAgentLoading();
  const { displayComparison, compareLanguageCode } = useFormMetaContext();
  const { id, onChange, value } = props;

  const errorHighlightStrategy = (contentBlock: ContentBlock, callback: (start: number, end: number) => void): void => {
    const matches = props.qualityResults ? issuesForGuidedCorrection(props.qualityResults) : [];
    const contentBlockText = contentBlock.getText();

    matches.forEach((match) => {
      // Match the matchToken only as a separate word.
      // Separate words have either unicode white space \p{Z} or \p{P} punctuation between them,
      // or they are at the beginning or the of the string.
      const exactMatchToken = matchingTokenRegExp(match.matchToken);
      const matchStartIndex = contentBlockText.search(exactMatchToken);
      if (matchStartIndex >= 0) {
        const startsWithTheWord = contentBlockText.search(`^${escapeRegExp(match.matchToken)}`) === 0;
        const wordStartIndex = startsWithTheWord ? matchStartIndex : matchStartIndex + 1;
        callback(wordStartIndex, wordStartIndex + match.matchLength);
      }
    });
  };

  const createDecorator = (): CompositeDecorator =>
    new CompositeDecorator([
      {
        strategy: errorHighlightStrategy,
        component: Highlight,
      },
    ]);

  const [editorState, setEditorState] = useState(() => createEditorState());
  const [hasFocus, setHasFocus] = useState(false);
  const editorHighlighState =
    !isQualityCheckLoading && !displayComparison && compareLanguageCode === undefined
      ? EditorState.set(editorState, { decorator: createDecorator() })
      : editorState;

  const editor = useRefOrCreate<Editor>(props.forwardedRef);

  function createEditorState(): EditorState {
    if (!props.value) {
      return EditorState.createEmpty();
    }

    return EditorState.createWithContent(convertFromRaw(props.value));
  }

  useEffect(() => {
    if (!value) return;

    const currentContent = editorState.getCurrentContent();
    const currentContentRaw = convertToRaw(currentContent);
    const fieldValue = value as RawDraftContentState;

    // Values differ if the field value has been changed outside of the editor (e.g. guided correction).
    if (currentContent.hasText() && !areEqual(fieldValue, currentContentRaw)) {
      const newState = EditorState.push(editorState, convertFromRaw(fieldValue), 'spellcheck-change');
      setEditorState(newState);
      // If the value has changed we need to populate the change back to the form model. Otherwise we end up
      // in forever loop because the form model (props.value) is different from the state draft-js has. Noticed
      // during this fix that editor state keeps having extra "data": {} key which might not be present in the
      // original value we receive from the API. There might be other differencies too. Alternative way would be
      // to compare only the text content but it is unclear if corrections (underlining) and formatting changes
      // (bullet list to numbered list) would work correctly.
      onChange(newState);
    }
  }, [id, editorState, value, onChange]);

  function setFocusOnEditor() {
    if (editor && editor.current) {
      editor.current.focus();
      setHasFocus(true);
    }
  }

  function onEditorChange(state: EditorState) {
    setEditorState(state);
    onChange(state);
  }

  function onBlur() {
    setHasFocus(false);
  }

  function onToolbarStyleChange(value: string) {
    onChange(RichUtils.toggleBlockType(editorState, value));
  }

  function shouldHidePlaceholder(editorState: EditorState): boolean {
    const contentState = editorState.getCurrentContent();
    if (contentState.hasText()) {
      return true;
    }

    const first = contentState.getFirstBlock();
    const firstType = first.getType();

    if (firstType === UnorderedListItem || firstType === OrderedListItem) {
      return true;
    }

    return first.getText() !== '';
  }

  const blockType = getSelectionStartBlockType(editorState);

  const classes = useStyles();

  const outerClassName = hasFocus ? classes.focused : '';

  const editorContainerClassName = clsx(classes.editorContainer, {
    [classes.error]: props.status === 'error',
    [classes.warning]: props.status === 'warning',
  });

  const editorClassName = clsx(classes.editor, {
    'Editor-hide-placeholder': shouldHidePlaceholder(editorState),
  });

  return (
    <div className={props.className}>
      <div className={outerClassName}>
        <div className={editorContainerClassName}>
          <Toolbar maxCharacters={props.maxCharacters} editorState={editorState} onStyleChange={onToolbarStyleChange} style={blockType} />
          <div onClick={setFocusOnEditor} className={editorClassName} onFocus={setFocusOnEditor} tabIndex={0} id={props.id}>
            <Editor
              onBlur={onBlur}
              stripPastedStyles={true}
              placeholder={props.placeHolder}
              ref={editor}
              editorState={editorHighlighState}
              onChange={onEditorChange}
            />
          </div>
        </div>
      </div>
      <ValidationMessage message={props.statusText} />
    </div>
  );
}
