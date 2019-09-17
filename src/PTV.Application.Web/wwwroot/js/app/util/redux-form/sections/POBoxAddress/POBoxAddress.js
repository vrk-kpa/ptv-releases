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
import {
  POBox,
  PostalCode,
  PostOffice,
  AdditionalInformation
} from 'util/redux-form/fields'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { injectIntl } from 'util/react-intl'

class POBoxAddress extends PureComponent {
  static defaultProps = {
    name: 'poBoxAddress'
  }
  render () {
    const {
      isCompareMode,
      additionalInformationProps,
      addressUseCase,
      required,
      layoutClass
    } = this.props
    const poBoxLayoutClass = isCompareMode
      ? 'col-lg-24 mb-4'
      : {
        'default': 'col-lg-8 mb-2 mb-lg-0',
        'bigConnection': 'col-lg-12 mb-2 mb-lg-0'
      }[layoutClass]
    const postalCodeLayoutClass = isCompareMode
      ? 'col-lg-24 mb-2'
      : {
        'default': 'col-lg-8 mb-2 mb-lg-0',
        'bigConnection': 'col-lg-12 mb-2 mb-lg-0'
      }[layoutClass]
    const postalOfficeLayoutClass = isCompareMode
      ? 'col-lg-24 mb-2'
      : {
        'default': 'col-lg-8 mb-2 mb-lg-0',
        'bigConnection': 'col-lg-12 mb-2 mb-lg-0'
      }[layoutClass]
    const additionalInformationLayoutClass = isCompareMode
      ? 'col-lg-24'
      : {
        'default': 'col-lg-12',
        'bigConnection': 'col-lg-18'
      }[layoutClass]
    return (
      <div>
        <div className='form-row'>
          <div className='row'>
            <div className={poBoxLayoutClass}>
              <POBox addressUseCase={addressUseCase} required={required} />
            </div>
          </div>
        </div>
        <div className='form-row'>
          <div className='row'>
            <div className={postalCodeLayoutClass}>
              <PostalCode required={required} skipValidation={!required} />
            </div>
            <div className={postalOfficeLayoutClass}>
              <PostOffice />
            </div>
          </div>
        </div>
        <div className='form-row'>
          <div className='row'>
            <div className={additionalInformationLayoutClass}>
              <AdditionalInformation
                {...additionalInformationProps}
                maxLength={150}
               />
            </div>
          </div>
        </div>
      </div>
    )
  }
}
POBoxAddress.propTypes = {
  addressUseCase: PropTypes.string.isRequired,
  isCompareMode: PropTypes.bool.isRequired,
  additionalInformationProps: PropTypes.object.isRequired,
  required: PropTypes.bool,
  layoutClass: PropTypes.string
}

POBoxAddress.defaultProps = {
  layoutClass: 'default'
}

export default compose(
  injectIntl,
  withFormStates
)(POBoxAddress)
