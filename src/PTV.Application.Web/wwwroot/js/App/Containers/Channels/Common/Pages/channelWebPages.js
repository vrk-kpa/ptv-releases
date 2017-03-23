/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
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
import React, { PropTypes } from 'react'
import { connect } from 'react-redux'

// actions
import * as channelActions from '../../Common/Actions'
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps'

// components
import WebPages from '../../../Common/WebPages'

// selectors
import * as CommonSelectors from '../Selectors'

export const ChannelWebPages = ({
  messages,
  channelType,
  readOnly,
  language,
  translationMode,
  webPages,
  actions,
  channelId,
  withName,
  withTypes,
  withOrder,
  shouldValidate,
  customTypesSelector,
  splitContainer,
  collapsible
}) => {
  const onAddWebPage = (entity) => {
    actions.onChannelEntityAdd('webPages', entity, channelId, language)
  }
  const onRemoveWebPage = (id) => {
    actions.onChannelListChange('webPages', channelId, id, language)
  }
  const sharedProps = { readOnly, language, translationMode, splitContainer }
  const body = (
    <WebPages
      {...sharedProps}
      messages={messages}
      items={webPages}
      shouldValidate={shouldValidate}
      withName={withName}
      withTypes={withTypes}
      customTypesSelector={customTypesSelector}
      withOrder={withOrder}
      onAddWebPage={onAddWebPage}
      onRemoveWebPage={onRemoveWebPage}
      collapsible={collapsible}
    />
  )
  return (!readOnly || readOnly && webPages.size !== 0) && body
}
ChannelWebPages.propTypes = {
  shouldValidate: PropTypes.bool
}
ChannelWebPages.defaultProps = {
  shouldValidate: true
}

function mapStateToProps (state, ownProps) {
  return {
    webPages: CommonSelectors.getSelectedWebPages(state, ownProps),
    channelId: CommonSelectors.getChannelId(state, ownProps)
  }
}

const actions = [
  channelActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(ChannelWebPages)
