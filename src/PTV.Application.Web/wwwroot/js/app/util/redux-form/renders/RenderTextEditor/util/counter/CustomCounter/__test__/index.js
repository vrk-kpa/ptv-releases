/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
import React from 'react';
import { mount } from 'enzyme';
import { expect } from 'chai';
import { EditorState, ContentState } from 'draft-js';
import createCounterPlugin from '../../index';

describe('CounterPlugin Line Counter', () => {
  const createEditorStateFromText = (text) => {
    const contentState = ContentState.createFromText(text);
    return EditorState.createWithContent(contentState);
  };

  let counterPlugin;

  beforeEach(() => {
    counterPlugin = createCounterPlugin();
  });

  it('instantiates plugin with word counter and counts 5 words', () => {
    const text = 'Hello there, how are you?';
    const editorState = createEditorStateFromText(text);
    counterPlugin.initialize({
      getEditorState: () => editorState,
    });
    const { CustomCounter } = counterPlugin;

    // a function that takes a string and returns the number of words
    const countFunction = (str) => {
      const wordArray = str.match(/\S+/g);  // matches words according to whitespace
      return wordArray ? wordArray.length : 0;
    };

    const result = mount(
      <CustomCounter countFunction={countFunction} />
    );
    expect(result).to.have.text('5');
  });

  it('instantiates plugin with number counter and counts 6 number characters', () => {
    const text = 'I am a 1337 h4x0r';
    const editorState = createEditorStateFromText(text);
    counterPlugin.initialize({
      getEditorState: () => editorState,
    });
    const { CustomCounter } = counterPlugin;

    // a function that takes a string and returns the number of number characters
    const countFunction = (str) => {
      const numArray = str.match(/\d/g);  // matches only number characters
      return numArray ? numArray.length : 0;
    };

    const result = mount(
      <CustomCounter countFunction={countFunction} />
    );
    expect(result).to.have.text('6');
  });
});
