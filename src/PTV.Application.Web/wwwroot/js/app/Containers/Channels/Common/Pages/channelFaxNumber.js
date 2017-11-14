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
import { injectIntl } from 'react-intl'

// actions
import * as channelActions from '../../Common/Actions'
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps'

// components
import PhoneNumbers from '../../../Common/PhoneNumbers'

// selectors
import * as CommonChannelSelectors from '../Selectors'

export const channelFaxNumber = ({
  messages,
  readOnly,
  intl,
  channelId,
  fax,
  componentClass,
  order,
  actions,
  shouldValidate,
  language,
  translationMode,
  splitContainer,
}) => {
  const onAddPhoneNumber = (entity) => {
    actions.onChannelEntityAdd('fax', entity, channelId, language)
  }
  const sharedProps = { readOnly, language, translationMode, splitContainer }
  return (
    <div>
      <PhoneNumbers
        {...sharedProps}
        messages={messages}
        item={fax}
        onAddPhoneNumber={onAddPhoneNumber}
        shouldValidate={shouldValidate}
        startOrder={order}
        numberType='fax'
        withInfo={false}
      />
    </div>
  )
}

channelFaxNumber.propTypes = {
  messages: PropTypes.object.isRequired
}

function mapStateToProps (state, ownProps) {
  return {
    fax: CommonChannelSelectors.getFax(state, ownProps),
    channelId: CommonChannelSelectors.getChannelId(state, ownProps)
  }
}

const actions = [
  channelActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(channelFaxNumber))
