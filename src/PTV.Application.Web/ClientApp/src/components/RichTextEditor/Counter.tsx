import React from 'react';
import { EditorState } from 'draft-js';
import 'draft-js/dist/Draft.css';
import { getContentLength } from 'utils/draftjs';
import './styles.css';

type CounterProps = {
  max: number;
  editorState: EditorState;
};

export default function Counter(props: CounterProps): React.ReactElement {
  const length = getContentLength(props.editorState.getCurrentContent());
  return (
    <span>
      {length} / {props.max}
    </span>
  );
}
