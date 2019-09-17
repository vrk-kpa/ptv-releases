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

import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { branch } from 'recompose'
import {
  injectIntl,
  intlShape,
  defineMessages
} from 'util/react-intl'
import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import asComparable from 'util/redux-form/HOC/asComparable'
import AddressTitle from 'appComponents/AddressTitle'
import commonMessages from 'util/redux-form/messages'
import {
  getAddressType,
  getLocalizedAddressAdditionalInformation,
  getIsMainEntrance,
  getIsAdditionalEntrance,
  getIsAccessibilityRegisterValid,
  getInvalidAddress
} from 'selectors/addresses'
import { getAddressTitle } from './selectors'
import { addressTypesEnum } from 'enums'

const messages = defineMessages({
  mainEntraceTitle: {
    id : 'AppComponents.VisitingAddressCollection.VisitingAddressTitle.MainEntrance.Title',
    defaultMessage: 'Main entrance'
  },
  additionalEntraceTitle: {
    id : 'AppComponents.VisitingAddressCollection.VisitingAddressTitle.AdditionalEntrance.Title',
    defaultMessage: 'Additional entrance'
  }
})

class VisitingAddressTitle extends Component {
  getFormatedAddressType = () => {
    const {
      index,
      intl: { formatMessage },
      addressType,
      isMainEntrance,
      isAdditionalEntrance,
      isValid,
      isReadOnly,
      items
    } = this.props
    let translatedAddressType
    if (isValid) {
      if (isMainEntrance) {
        translatedAddressType = formatMessage(messages.mainEntraceTitle)
      } else if (isAdditionalEntrance) {
        translatedAddressType = formatMessage(messages.additionalEntraceTitle)
      }
    } else {
      translatedAddressType = (
        formatMessage(commonMessages[`${addressType.toLowerCase()}AddressType`])
      )
    }
    const showNumbers = isReadOnly && items && items.size > 1
    return showNumbers && `${index + 1}. ${translatedAddressType}` || translatedAddressType
  }
  render () {
    const {
      addressTitle,
      addressType,
      addressInfo,
      ...rest
    } = this.props
    return (
      <AddressTitle
        addressTitle={addressTitle}
        addressType={this.getFormatedAddressType()}
        addressInfo={addressType === addressTypesEnum.FOREIGN ? null : addressInfo}
        prependInfo={addressType === addressTypesEnum.OTHER}
        {...rest}
      />
    )
  }
}
VisitingAddressTitle.propTypes = {
  addressTitle: PropTypes.string,
  addressType: PropTypes.string,
  isMainEntrance: PropTypes.bool,
  isAdditionalEntrance: PropTypes.bool,
  isAccessibilityAddress: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  isValid: PropTypes.bool,
  items: PropTypes.object,
  addressInfo: PropTypes.string,
  index: PropTypes.number,
  intl: intlShape
}

export default compose(
  injectIntl,
  withLanguageKey,
  withFormStates,
  branch(({ comparable, isReadOnly }) =>
    comparable || isReadOnly, asComparable({ DisplayRender: VisitingAddressTitle })),
  connect((state, ownProps) => ({
    addressType: getAddressType(state, ownProps),
    addressTitle: getAddressTitle(state, ownProps),
    addressInfo: getLocalizedAddressAdditionalInformation(state, ownProps),
    isValid: getIsAccessibilityRegisterValid(state, ownProps),
    isMainEntrance: getIsMainEntrance(state, ownProps),
    isAdditionalEntrance: getIsAdditionalEntrance(state, ownProps),
    invalidAddress: getInvalidAddress(state, ownProps)
  }))
)(VisitingAddressTitle)
