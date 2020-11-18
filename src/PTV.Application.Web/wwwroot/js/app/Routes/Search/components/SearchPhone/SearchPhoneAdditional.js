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
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import {
  PhoneNumberCode,
  PhoneNumberNumber
} from 'util/redux-form/fields'
import Tooltip from 'appComponents/Tooltip'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import styles from '../styles.scss'
import {
  getIsLocalNumberSelectedForIndex
} from 'util/redux-form/sections/PhoneNumbers/selectors'

const messages = defineMessages({
  serviceNumber:  {
    id: 'AppComponents.PhoneNumberParser.ServiceNumber.Title',
    defaultMessage: 'Kansallinen palvelunumero'
  }
})

const SearchPhoneAdditional = ({
  localNumberTooltip,
  isLocalNumberSelected,
  intl: { formatMessage }
}) => {
  return (
    <div className={styles.localNumber}>
      {!isLocalNumberSelected &&
      <div className={styles.previewDial}>
        <PhoneNumberCode />
      </div>}
      <div className={styles.previewNumber}>
        <PhoneNumberNumber />
      </div>
      <div className={styles.previewNumber}>
        {isLocalNumberSelected && formatMessage(messages.serviceNumber)}
      </div>
      <Tooltip tooltip={formatMessage(localNumberTooltip)} indent='i5' />
    </div>
  )
}

SearchPhoneAdditional.propTypes = {
  localNumberTooltip: PropTypes.object,
  isLocalNumberSelected: PropTypes.bool,
  intl: intlShape
}

export default compose(
  injectIntl,
  injectFormName,
  connect((state, ownProps) => ({
    isLocalNumberSelected: getIsLocalNumberSelectedForIndex(state, ownProps)
  })),
)(SearchPhoneAdditional)
