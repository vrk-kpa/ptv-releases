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
import {
  AuthenticationSignatureCount,
  AuthenticationSignatureSwitch,
  OnlineAuthentication } from 'util/redux-form/fields'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { injectFormName, asGroup, withFormStates } from 'util/redux-form/HOC'
import { getIsOnlineSign } from './selectors'
import { defineMessages, injectIntl, FormattedMessage } from 'react-intl'

export const messages = defineMessages({
  groupTitle: {
    id: 'Routes.Channels.Electronic.Authentication.GroupTitle',
    defaultMessage: 'Sähköinen tunnistus ja allekirjoitus'
  }
})

const AuthenticationSign = ({
  validate,
  isOnlineSign,
  isCompareMode,
  ...rest
}) => {
  const basicCompareModeClass = isCompareMode ? 'col-lg-24' : 'col-lg-12'
  return (
    <div>
      <div className='form-row'>
        <OnlineAuthentication inline required defaultValue={false} />
      </div>
      <div className='row'>
        <div className={basicCompareModeClass}>
          <div className='form-row'>
            <AuthenticationSignatureSwitch inline defaultValue={false} />
          </div>
        </div>
        { isOnlineSign &&
          <div className={basicCompareModeClass}>
            <div className='form-row'>
              <AuthenticationSignatureCount required />
            </div>
          </div>
        }
      </div>
    </div>
  )
}
AuthenticationSign.propTypes = {
  validate: PropTypes.func,
  isOnlineSign: PropTypes.bool,
  isCompareMode: PropTypes.bool
}

export default compose(
  injectFormName,
  injectIntl,
  withFormStates,
  connect((state, ownProps) => {
    return ({
      isOnlineSign: getIsOnlineSign(state, ownProps)
    })
  }),
  asGroup({
    title: messages.groupTitle
  })
)(AuthenticationSign)
