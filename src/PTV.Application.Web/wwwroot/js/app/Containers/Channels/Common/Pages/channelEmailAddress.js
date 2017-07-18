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
import { injectIntl } from 'react-intl'

// Actions
import * as channelActions from '../../Common/Actions'
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps'

// components
import Email from '../../../Common/Emails'

// selectors
import * as CommonChannelSelectors from '../Selectors'

export const ChannelEmailAddress = ({
  messages,
  readOnly,
  language,
  translationMode,
  intl,
  channelId,
  emails,
  collapsible,
  order,
  actions,
  shouldValidate,
  splitContainer
}) => {
  const onAddEmail = (entity) => {
    actions.onChannelEntityAdd('emails', entity, channelId, language)
  }
  const onRemoveEmail = (id) => {
    actions.onChannelListChange('emails', channelId, id, language)
  }
  const sharedProps = { readOnly, language, translationMode, splitContainer }
  return (
    <div>
      <Email {...sharedProps}
        messages={messages}
        items={emails}
        onAddEmail={onAddEmail}
        onRemoveEmail={onRemoveEmail}
        shouldValidate={shouldValidate}
        startOrder={order}
        collapsible={collapsible}
        withInfo={false}
      />
    </div>
  )
}

ChannelEmailAddress.propTypes = {
  messages: PropTypes.object.isRequired
}

function mapStateToProps (state, ownProps) {
  return {
    emails: CommonChannelSelectors.getChannelEmails(state, ownProps),
    channelId: CommonChannelSelectors.getChannelId(state, ownProps)
  }
}

const actions = [
  channelActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ChannelEmailAddress))
