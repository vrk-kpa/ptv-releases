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
import { connect } from 'react-redux'
import { injectIntl } from 'react-intl'

// actions
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps'
import * as locationChannelActions from '../Actions'

// selectors
// import * as CommonSelectors from '../../../Common/Selectors'

// messages
import * as Messages from '../Messages'

// types
import { channelTypes } from '../../../Common/Enums'
import { addressTypes } from '../../../Common/Helpers/types'

// components
import * as PTVValidatorTypes from '../../../../Components/PTVValidators'
import PTVAddItem from '../../../../Components/PTVAddItem'
import LanguageLabel from '../../../Common/languageLabel'
import PublishingStatus from '../../Common/Pages/channelPublishingStatus'
import ChannelDescriptionContainer from '../../Common/Pages/ChannelDescriptionContainer'
import ChannelRestrictionRegion from '../../Common/Pages/ChannelRestrictionRegion'
import ChannelLanguages from '../../Common/Pages/channelLanguages'
import ChannelAddresses from '../../Common/Pages/channelAddresses'
import ChannelEmailAddress from '../../Common/Pages/channelEmailAddress'
import ChannelFaxNumber from '../../Common/Pages/channelFaxNumber'
import ChannelPhoneNumber from '../../Common/Pages/channelPhoneNumber'
import ChannelWebPages from '../../Common/Pages/channelWebPages'

export const Step1 = ({
    intl: { formatMessage },
    readOnly,
    keyToState,
    language,
    translationMode,
    splitContainer
  }) => {
  const sharedProps = { readOnly, translationMode, language, keyToState, splitContainer }
  const validators = [PTVValidatorTypes.IS_REQUIRED]

  const renderContacts = () => {
    return (
      <div>

        <ChannelEmailAddress
          {...sharedProps}
          messages={Messages.emailMessages}
        />

        <ChannelFaxNumber
          {...sharedProps}
          messages={Messages.faxMessages}
        />

        <ChannelPhoneNumber
          {...sharedProps}
          messages={Messages.phoneNumberMessages}
          shouldValidate={false}
          collapsible
        />

        <ChannelWebPages
          {...sharedProps}
          messages={Messages.webPageMessages}
          shouldValidate={false}
          withName
        />

        <ChannelAddresses
          {...sharedProps}
          addressType={addressTypes.POSTAL}
        />

      </div>
    )
  }

  return (
    <div className='step-1'>
      <LanguageLabel
        {...sharedProps}
      />

      <PublishingStatus
        {...sharedProps}
      />

      <ChannelDescriptionContainer
        {...sharedProps}
        messages={Messages.channelDescriptionMessages}
      />

      <ChannelRestrictionRegion
        {...sharedProps}
        messages={Messages.restrictionRegionMessages}
      />

      <ChannelLanguages
        {...sharedProps}
        messages={Messages.supportLanguageProvidedMessages}
        validators={validators}
      />

      <ChannelAddresses
        {...sharedProps}
        addressType={addressTypes.VISITING}
      />

      <PTVAddItem {...sharedProps}
        readOnly={readOnly && translationMode === 'none'}
        collapsible={!readOnly && translationMode !== 'edit'}
        renderItemContent={renderContacts}
        messages={{ 'label': formatMessage(Messages.messages.showContacts) }}
      />

    </div>
  )
}

function mapStateToProps (state, ownProps) {
  const keyToState = channelTypes.SERVICE_LOCATION
  return {
    keyToState
  }
}

const actions = [
  locationChannelActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(Step1))
