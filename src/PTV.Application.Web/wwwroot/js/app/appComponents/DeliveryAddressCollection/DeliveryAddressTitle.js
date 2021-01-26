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
import { branch } from 'recompose'
import {
  injectIntl,
  intlShape
} from 'util/react-intl'
import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import asComparable from 'util/redux-form/HOC/asComparable'
import commonMessages from 'util/redux-form/messages'
import {
  getAddressType,
  getLocalizedAddressAdditionalInformation,
  getInvalidAddress
} from 'selectors/addresses'
import { getAddressTitle } from './selectors'
import AddressTitle from 'appComponents/AddressTitle'

const DeliveryAddressTitle = ({
  addressTitle,
  addressType,
  addressInfo,
  intl: { formatMessage },
  ...rest
}) => {
  return (
    <AddressTitle
      addressTitle={addressTitle}
      addressType={formatMessage(commonMessages[`${addressType.toLowerCase()}AddressType`])}
      addressInfo={addressInfo}
      {...rest}
    />
  )
}

DeliveryAddressTitle.propTypes = {
  addressTitle: PropTypes.string,
  addressType: PropTypes.string,
  addressInfo: PropTypes.string,
  index: PropTypes.number,
  intl: intlShape
}

export default compose(
  injectIntl,
  withLanguageKey,
  withFormStates,
  branch(({ comparable, isReadOnly }) =>
    comparable || isReadOnly, asComparable({ DisplayRender: DeliveryAddressTitle })),
  connect((state, ownProps) => ({
    addressType: getAddressType(state, ownProps),
    addressTitle: getAddressTitle(state, ownProps),
    addressInfo: getLocalizedAddressAdditionalInformation(state, ownProps),
    invalidAddress: getInvalidAddress(state, ownProps)
  }))
)(DeliveryAddressTitle)
