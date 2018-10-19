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

import React, { Fragment } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import {
  injectIntl,
  intlShape,
  defineMessages
} from 'util/react-intl'
import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'
import NoDataLabel from 'appComponents/NoDataLabel'
import {
  getIsLocalNumber,
  getPhoneType,
  getPhoneNumber,
  getPhoneNumberTitle
} from './selectors'
import styles from './styles.scss'

const messages = defineMessages({
  isLocalNumber: {
    id: 'ReduxForm.Renders.RenderIsLocalNumber.title',
    defaultMessage: 'Palvelunumero, johon ei voi soittaa ulkomailta.'
  }
})

const PhoneNumberTitle = ({
  className,
  phoneNumberTitle,
  phoneNumber,
  phoneType,
  isLocalNumber,
  withType,
  intl: { formatMessage },
  ...rest
}) => {
  return (
    (phoneNumberTitle || isLocalNumber) && <div className={className}>
      {isLocalNumber && <Fragment>
        {withType && <span>{phoneType}: </span>}
        <span>{phoneNumber}</span>
        <span className={styles.localNumber}>({formatMessage(messages.isLocalNumber)})</span>
        <span>{phoneNumberTitle}</span>
      </Fragment>}
      {!isLocalNumber && withType && <span>{phoneType}: </span>}
      {!isLocalNumber && <span>{phoneNumberTitle}</span>}
    </div> || <NoDataLabel />
  )
}

PhoneNumberTitle.propTypes = {
  className: PropTypes.string,
  phoneNumberTitle: PropTypes.string,
  phoneNumber: PropTypes.string,
  phoneType: PropTypes.string,
  isLocalNumber: PropTypes.bool,
  withType: PropTypes.bool,
  intl: intlShape
}

export default compose(
  injectIntl,
  withLanguageKey,
  connect((state, ownProps) => ({
    isLocalNumber: getIsLocalNumber(state, ownProps),
    phoneType: getPhoneType(state, ownProps),
    phoneNumber: getPhoneNumber(state, ownProps),
    phoneNumberTitle: getPhoneNumberTitle(state, ownProps)
  }))
)(PhoneNumberTitle)
