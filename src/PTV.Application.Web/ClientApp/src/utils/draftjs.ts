import { RefObject } from 'react';
import { ContentState, Editor, EditorState, RawDraftContentState, convertFromRaw } from 'draft-js';
import _ from 'lodash';

export function parseRawDescription(input: string | null | undefined): RawDraftContentState {
  return !input ? null : JSON.parse(input);
}

export function getSelectionStartBlockType(editorState: EditorState): string {
  const selection = editorState.getSelection();
  const content = editorState.getCurrentContent();
  return content.getBlockForKey(selection.getStartKey()).getType();
}

export function getContentLength(content: ContentState): number {
  const text = content.getPlainText();
  return Array.from(text).length;
}

export function getContentLengthFromRaw(content: RawDraftContentState): number {
  return getContentLength(convertFromRaw(content));
}

export function getPlainText(value: RawDraftContentState | null | undefined): string {
  const editorContentState = getContentState(value);
  return editorContentState.getPlainText();
}

export function getContentState(value: RawDraftContentState | null | undefined): ContentState {
  let editorState;
  if (!value) {
    editorState = EditorState.createEmpty();
  } else {
    editorState = EditorState.createWithContent(convertFromRaw(value));
  }

  return editorState.getCurrentContent();
}

export function areEqual(left: RawDraftContentState, right: RawDraftContentState): boolean {
  // Using isEqual because the actual order of keys inside the object might not be the
  // same even though the content is (e.g. entityMap might be first or last key)
  return _.isEqual(left, right);
}

export function setFocusToEditor(ref?: RefObject<Editor>) {
  if (!ref?.current?.editor) return;
  ref.current.editor.focus();
}
