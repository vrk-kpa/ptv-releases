import React from 'react';

export function setFocusEndOfText(inputRef: React.RefObject<HTMLTextAreaElement> | React.RefObject<HTMLInputElement>) {
  if (!inputRef.current) return;
  const end = inputRef.current.value.length;
  inputRef.current.setSelectionRange(end, end);
  inputRef.current.focus();
}
