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
import React from 'react'
import { Field } from 'redux-form/immutable'
import { connect } from 'react-redux'
import { injectIntl, intlShape } from 'util/react-intl'
import { localizeItem, languageTranslationTypes } from 'appComponents/Localize'
import { compose } from 'redux'
import { RenderTreeView } from 'util/redux-form/renders'
import {
  Nodes,
  Node,
  NodeLabelCheckBox,
  withCustomLabel,
  withFocus
} from 'util/redux-form/renders/RenderTreeView/TreeView'
import {
  getLanguagesIds,
  getLanguage
} from './selectors'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import injectFormName from 'util/redux-form/HOC/injectFormName'

const LanguageNodes = compose(
  injectFormName,
  connect((state, ownProps) => {
    const nodes = getLanguagesIds(state, ownProps)
    return { nodes, listType: 'simple' }
  }),
  withFocus()
)(Nodes)

const LanguageNode = compose(
  injectFormName,
  connect(
    (state, ownProps) => {
      const node = getLanguage(state, ownProps)
      const checked = ownProps.value && ownProps.value.contains(ownProps.id) || false
      return {
        node,
        checked: checked,
        isLeaf: true
      }
    }
  ),
  localizeItem({
    input: 'node',
    output: 'node',
    languageTranslationType: languageTranslationTypes.locale
  }),
  withCustomLabel(NodeLabelCheckBox)
)(Node)

const LanguageSearchTree = ({
  ...rest
}) => (
  <Field
    name='languages'
    component={RenderTreeView}
    NodesComponent={LanguageNodes}
    NodeComponent={LanguageNode}
    simple
    {...rest}
  />
)
LanguageSearchTree.propTypes = {
  intl: intlShape
}

export default compose(
  injectIntl,
  withFormStates
)(LanguageSearchTree)

