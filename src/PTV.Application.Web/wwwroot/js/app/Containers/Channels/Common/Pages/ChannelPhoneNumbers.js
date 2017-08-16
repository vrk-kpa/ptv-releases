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
import PhoneNumbers from '../../../Common/PhoneNumbers'

// selectors
import * as CommonSelectors from '../Selectors'

export const ChannelPhoneNumbers = ({
  messages,
  readOnly,
  language,
  translationMode,
  phoneNumbers,
  actions,
  channelId,
  shouldValidate,
  children,
  collapsible,
  withType,
  splitContainer
}) => {
  const onAddPhoneNumber = (entity) => {
    actions.onChannelEntityAdd('phoneNumbers', entity, channelId, language)
  }
  const onRemovePhoneNumber = (id) => {
    actions.onChannelListChange('phoneNumbers', channelId, id, language)
  }
  const sharedProps = { readOnly, language, translationMode, splitContainer }
  const body = (
    <PhoneNumbers {...sharedProps}
      messages={messages}
      items={phoneNumbers}
      shouldValidate={shouldValidate}
      onAddPhoneNumber={onAddPhoneNumber}
      onRemovePhoneNumber={onRemovePhoneNumber}
      children={children}
      withType={withType}
      collapsible={collapsible}
      withTypeEdit
    />
  )
  return (!readOnly || readOnly && phoneNumbers.size !== 0) && body
}

ChannelPhoneNumbers.propTypes = {
  shouldValidate: PropTypes.bool,
  collapsible: PropTypes.bool,
  withType: PropTypes.bool
}

ChannelPhoneNumbers.defaultProps = {
  shouldValidate: false,
  collapsible: false,
  withType: false
}

function mapStateToProps (state, ownProps) {
  return {
    phoneNumbers: CommonSelectors.getChannelPhoneNumbers(state, ownProps),
    channelId: CommonSelectors.getChannelId(state, ownProps)
  }
}

const actions = [
  channelActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(ChannelPhoneNumbers)
