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
import React, { PropTypes, Component } from 'react'
import { connect } from 'react-redux'
import { injectIntl } from 'react-intl'

// actions
import * as channelActions from '../Actions'
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps'

// components
import * as PTVValidatorTypes from '../../../../Components/PTVValidators'
import PTVAddItem from '../../../../Components/PTVAddItem'
import PublishingStatus from '../../Common/Pages/channelPublishingStatus'
import ChannelDescriptionContainer from '../../Common/Pages/ChannelDescriptionContainer'
import ChannelPhoneNumbers from '../../Common/Pages/ChannelPhoneNumbers'
import ChannelEmailAddress from '../../Common/Pages/channelEmailAddress'
import ChannelUrl from '../../Common/Pages/channelUrl'
import ChannelLanguages from '../../Common/Pages/channelLanguages'
import LanguageLabel from '../../../Common/languageLabel'
import ChannelConnectionType from '../../Common/Pages/channelConnectionTypes'

// types
import { channelTypes } from '../../../Common/Enums'

// selectors
import { getChannelPhoneNumberEntities, getChannelEmailEntities } from 'Containers/Channels/Common/Selectors'

// messages
import * as Messages from '../Messages'

export const Step1 = ({
    intl: { formatMessage },
    readOnly,
    keyToState,
    language,
    translationMode,
    splitContainer,
    isEmailFilled,
    isPhoneFilled }) => {
  const validators = [PTVValidatorTypes.IS_REQUIRED]
  const sharedProps = { readOnly, translationMode, language, keyToState, splitContainer }

  const renderContacts = () => {
    return (
      <div>
        {isPhoneFilled &&
        <ChannelPhoneNumbers
          {...sharedProps}
          messages={Messages.phoneNumberMessages}
          shouldValidate={false}
          collapsible
        />}
        {isEmailFilled &&
        <ChannelEmailAddress
          {...sharedProps}
          messages={Messages.emailMessages}
          collapsible
        />}
      </div>
    )
  }

  return (
    <div className='step-1'>
      <LanguageLabel {...sharedProps}
            />
      <PublishingStatus {...sharedProps}
            />
      <ChannelDescriptionContainer {...sharedProps}
        messages={Messages.channelDescriptionMessages}
            />
      <ChannelUrl {...sharedProps}
        messages={Messages.urlMessages}
        validators={validators}
            />
      {(isEmailFilled || isPhoneFilled) &&
        <PTVAddItem {...sharedProps}
          readOnly={readOnly && translationMode === 'none'}
          collapsible={!readOnly && translationMode !== 'edit'}
          renderItemContent={renderContacts}
          messages={{ 'label': formatMessage(Messages.messages.supportTitle) }}
        />
      }

      <ChannelLanguages {...sharedProps}
        messages={Messages.languageMessages}
        validators={validators}
            />

      <ChannelConnectionType {...sharedProps} />
    </div>
  )
}

function mapStateToProps (state, ownProps) {
  const keyToState = channelTypes.WEB_PAGE
  const phoneNumbers = getChannelPhoneNumberEntities(state, { keyToState, language : ownProps.language })
  const emails = getChannelEmailEntities(state, { keyToState, language : ownProps.language })

  const isEmailFilled = (
        emails.filter(email => email.has('email') &&
        email.get('email') !== '' &&
        email.get('email') !== null).size > 0
      )
  const isPhoneFilled = (
        phoneNumbers.filter(phoneNumber =>
        phoneNumber.has('number') &&
        phoneNumber.get('number') !== '' &&
        phoneNumber.get('number') !== null).size > 0
      )
  return {
    keyToState,
    isEmailFilled: (!ownProps.readOnly || ownProps.readOnly && isEmailFilled),
    isPhoneFilled: (!ownProps.readOnly || ownProps.readOnly && isPhoneFilled)
  }
}

const actions = [
  channelActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(Step1))
