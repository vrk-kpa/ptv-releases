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
import React, { PureComponent } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { AddressType, NoAddressAdditionalInformation, FormReceiver } from 'util/redux-form/fields'
import StreetAddress from 'util/redux-form/sections/StreetAddress'
import OtherAddress from 'util/redux-form/sections/OtherAddress'
import POBoxAddress from 'util/redux-form/sections/POBoxAddress'
import AddressPreview from 'appComponents/AddressPreview'
import ForeignAddress from 'util/redux-form/sections/ForeignAddress'
import { formValueSelector } from 'redux-form/immutable'
import { getIsSoteAddress } from './selectors'
import { connect } from 'react-redux'
import { addressUseCases, addressUseCasesEnum } from 'enums'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import asComparable from 'util/redux-form/HOC/asComparable'
import { injectIntl } from 'util/react-intl'
import withPath from 'util/redux-form/HOC/withPath'
import SoteNote from 'appComponents/SoteNote'
import { combinePathAndField } from 'util/redux-form/util'

const AddressPreviewComponent = compose(
  asComparable({ DisplayComponent: AddressPreviewComponent })
)(props => <AddressPreview {...props} isCompareMode={false} />)

class AddressSwitch extends PureComponent {
  static defaultProps = {
    name: 'address'
  }
  render () {
    const {
      addressType,
      addressUseCase,
      additionalInformationProps,
      isCompareMode,
      disableTypes,
      ...rest
    } = this.props
    if (rest.isReadOnly) {
      return (
        <div>
          <AddressPreviewComponent
            addressType={addressType}
            addressUseCase={addressUseCase}
            {...rest}
          />
        </div>)
    }
    return (
      <div>
        <SoteNote showNote={rest.isSoteAddress} />
        {addressUseCase === addressUseCasesEnum.DELIVERY &&
        <div className='collection-form-row'>
          <FormReceiver {...rest} />
        </div>}
        <div className='collection-form-row with-inline-elements'>
          <AddressType
            inline={!isCompareMode && !rest.splitView}
            addressUseCase={addressUseCase}
            disableTypes={disableTypes}
            {...rest}
          />
        </div>
        {addressType === 'Street' && <StreetAddress addressUseCase={addressUseCase}
          addressType={addressType}
          name='address'
          additionalInformationProps={additionalInformationProps}
          {...rest}
        />}
        {addressType === 'Other' && <OtherAddress addressUseCase={addressUseCase}
          addressType={addressType}
          name='address'
          additionalInformationProps={additionalInformationProps}
          {...rest}
        />}
        {addressType === 'PostOfficeBox' && <POBoxAddress addressUseCase={addressUseCase}
          addressType={addressType}
          name='address'
          additionalInformationProps={additionalInformationProps}
          {...rest} />}
        {addressType === 'Foreign' && <ForeignAddress addressUseCase={addressUseCase}
          name='address'
          {...rest} />}
        {addressType === 'NoAddress' && <NoAddressAdditionalInformation
          {...rest} />}
      </div>
    )
  }
}
AddressSwitch.propTypes = {
  addressUseCase: PropTypes.oneOf(addressUseCases),
  addressType: PropTypes.oneOf(['Street', 'PostOfficeBox', 'Foreign', 'NoAddress', 'Other']),
  additionalInformationProps: PropTypes.object,
  isCompareMode: PropTypes.bool,
  disableTypes: PropTypes.bool,
  isSoteAddress: PropTypes.bool
}
AddressSwitch.defaultProps = {
  addressUseCase: addressUseCasesEnum.POSTAL
}

export default compose(
  injectIntl,
  withFormStates,
  withPath,
  injectFormName,
  connect((state, { formName, path }) => ({
    addressType: formValueSelector(formName)(state, combinePathAndField(path, 'streetType')) || 'Street',
    isSoteAddress: getIsSoteAddress(state, { formName, path })
  }))
)(AddressSwitch)
