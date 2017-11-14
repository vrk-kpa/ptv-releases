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
import React from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'

// actions
import * as channelActions from '../../Common/Actions'
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps'

// components
import PhoneNumbers from '../../../Common/PhoneNumbers'

// selectors
import * as CommonSelectors from '../Selectors'

export const ChannelFaxNumbers = ({
  messages,
  readOnly,
  language,
  translationMode,
  faxNumbers,
  actions,
  channelId,
  shouldValidate,
  children,
  collapsible,
  splitContainer
}) => {
  const onAddFaxNumber = (entity) => {
    actions.onChannelEntityAdd('faxNumbers', entity, channelId, language)
  }
  const onRemoveFaxNumber = (id) => {
    actions.onChannelListChange('faxNumbers', channelId, id, language)
  }
  const sharedProps = { readOnly, language, translationMode, splitContainer }
  const body = (
    <PhoneNumbers
      {...sharedProps}
      messages={messages}
      items={faxNumbers}
      onAddPhoneNumber={onAddFaxNumber}
      onRemovePhoneNumber={onRemoveFaxNumber}
      children={children}
      shouldValidate={shouldValidate}
      numberType='fax'
      withInfo={false}
      collapsible={collapsible}
      />
  )
  return (!readOnly || readOnly && faxNumbers.size !== 0) && body
}

ChannelFaxNumbers.propTypes = {
  shouldValidate: PropTypes.bool,
  collapsible: PropTypes.bool,
  withType: PropTypes.bool
}

ChannelFaxNumbers.defaultProps = {
  shouldValidate: false,
  collapsible: false,
  withType: false
}

function mapStateToProps (state, ownProps) {
  return {
    faxNumbers: CommonSelectors.getChannelFaxNumbers(state, ownProps),
    channelId: CommonSelectors.getChannelId(state, ownProps)
  }
}

const actions = [
  channelActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(ChannelFaxNumbers)
