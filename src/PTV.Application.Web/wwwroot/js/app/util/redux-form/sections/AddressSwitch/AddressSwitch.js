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
import { AddressType, NoAddressAdditionalInformation } from 'util/redux-form/fields'
import { StreetAddress, POBoxAddress, ForeignAddress } from 'util/redux-form/sections'
import AddressPreview from 'appComponents/AddressPreview'
import { formValueSelector } from 'redux-form/immutable'
import { connect } from 'react-redux'
import { addressUseCases, addressUseCasesEnum } from 'enums'
import { asSection, injectFormName, withFormStates, asComparable } from 'util/redux-form/HOC'
import { injectIntl } from 'react-intl'
import withPath from 'util/redux-form/HOC/withPath'

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
      ...rest
    } = this.props
    if (rest.isReadOnly) {
      return <AddressPreviewComponent
        addressType={addressType}
        addressUseCase={addressUseCase}
        {...rest}
      />
    }
    return (
      <div>
        <div className='form-row'>
          <AddressType
            onChange={this.handleAddressTypeOnChange}
            inline={!isCompareMode && !rest.splitView}
            addressUseCase={addressUseCase}
            {...rest}
          />
        </div>
        {addressType === 'Street' && <StreetAddress addressUseCase={addressUseCase}
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
          addressType={addressType}
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
  addressType: PropTypes.oneOf(['Street', 'PostOfficeBox', 'Foreign', 'NoAddress']),
  additionalInformationProps: PropTypes.object,
  isCompareMode: PropTypes.bool
}
AddressSwitch.defaultProps = {
  addressUseCase: addressUseCasesEnum.POSTAL
}

export default compose(
  injectIntl,
  asSection(),
  withFormStates,
  withPath,
  injectFormName,
  connect((state, { formName, path }) => ({
    addressType: formValueSelector(formName)(state, `${path}.streetType`) || 'Street'
  }))
)(AddressSwitch)
