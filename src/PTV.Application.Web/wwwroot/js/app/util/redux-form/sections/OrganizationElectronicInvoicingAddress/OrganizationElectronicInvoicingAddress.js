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
import React, { PureComponent } from 'react'
import { OperatorCode, ElectronicInvoicingAddress, AdditionalInformation } from 'util/redux-form/fields'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { injectIntl } from 'util/react-intl'
import styles from './styles.scss'

class OrganizationElectronicInvoicingAddress extends PureComponent {
  render () {
    const {
      isCompareMode,
      splitView,
      additionalInformationProps,
      ...rest
    } = this.props

    const basicCompareModeClass = isCompareMode || splitView ? 'col-lg-24' : 'col-lg-14'
    const fieldClass = isCompareMode || splitView ? 'col-lg-24' : 'col-lg-7'

    return (
      <div>
        <div className='collection-form-row'>
          <div className='row'>
            <div className={fieldClass}>
              <ElectronicInvoicingAddress
                {...rest}
                isLocalized={false}
                isCompareMode={isCompareMode}
              />
            </div>
            <div className={fieldClass}>
              <OperatorCode {...rest} isLocalized={false} isCompareMode={isCompareMode} />
            </div>
          </div>
        </div>
        <div className='collection-form-row'>
          <div className='row'>
            <div className={basicCompareModeClass}>
              <AdditionalInformation
                {...additionalInformationProps}
                maxLength={150}
                isCompareMode={isCompareMode}
                useQualityAgent
                collectionPrefix={'electronicInvoicingAddresses'}
                compare={this.props.compare}
              />
            </div>
          </div>
        </div>
      </div>
    )
  }
}

OrganizationElectronicInvoicingAddress.propTypes = {
  isCompareMode: PropTypes.bool,
  splitView: PropTypes.bool,
  additionalInformationProps: PropTypes.object
}

export default compose(
  injectIntl,
  withFormStates
)(OrganizationElectronicInvoicingAddress)
