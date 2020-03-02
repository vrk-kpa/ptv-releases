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
import { connect } from 'react-redux'
import { compose } from 'redux'
import {
  ChargeType,
  DialCode,
  IsLocalNumber,
  PhoneCostDescription,
  PhoneNumber,
  PhoneNumberInfo,
  PhoneType
} from 'util/redux-form/fields'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import withPath from 'util/redux-form/HOC/withPath'
import asSection from 'util/redux-form/HOC/asSection'
import {
  getIsLocalNumberSelectedForIndex,
  getIsChargeTypeOtherSelectedForIndex
} from '../PhoneNumbers/selectors'
import cx from 'classnames'
import styles from './styles.scss'
import shortId from 'shortid'

class PhoneNumbersWithType extends PureComponent {
  static defaultProps = {
    name: 'phoneNumbers'
  }
  render () {
    const {
      dialCodeProps = {},
      phoneNumberProps = {},
      phoneNumberInfoProps = {},
      isCompareMode,
      isLocalNumberSelected,
      splitView,
      required,
      compare,
      isReadOnly,
      isChargeTypeOtherSelected
    } = this.props

    const autocompleteId = shortId.generate()
    const basicCompareModeClass = isCompareMode || splitView ? 'col-lg-24' : 'col-lg-12'
    const dialCodeWrapClass = isCompareMode || splitView ? 'col-lg-24' : 'col-lg-6 mb-2'
    const dialCodeClass = cx(
      'col-lg-24 mb-2',
      {
        [styles.hideValue]: isLocalNumberSelected
      }
    )
    const isLocalNumberClass = 'col-lg-24'
    const phoneNumberClass = isCompareMode || splitView ? 'col-lg-24' : 'col-lg-6'
    return (
      <div>
        <div className='collection-form-row'>
          <div className='row'>
            <div className={basicCompareModeClass}>
              <PhoneType
                isCompareMode={isCompareMode}
                compare={compare}
                required={required}
              />
            </div>
          </div>
        </div>
        <div className='collection-form-row'>
          <div className='row'>
            <div className={dialCodeWrapClass}>
              <div className='row'>
                { !(isReadOnly && isLocalNumberSelected) &&
                  <div className={dialCodeClass}>
                    <DialCode
                      isCompareMode={isCompareMode}
                      disabled={isLocalNumberSelected}
                      required={required}
                      compare={compare}
                      autocomplete={`${autocompleteId}DialCode`}
                      {...dialCodeProps}
                    />
                  </div>
                }
                <div className={isLocalNumberClass}>
                  <IsLocalNumber isCompareMode={isCompareMode} />
                </div>
              </div>
            </div>
            <div className={phoneNumberClass}>
              <PhoneNumber
                isCompareMode={isCompareMode}
                autocomplete={`${autocompleteId}PhoneNumber`}
                {...phoneNumberProps}
                required={required}
              />
            </div>
          </div>
        </div>
        <div className='collection-form-row'>
          <div className='row'>
            <div className={basicCompareModeClass}>
              <PhoneNumberInfo
                isCompareMode={isCompareMode}
                {...phoneNumberInfoProps}
                useQualityAgent
                collectionPrefix='phoneNumbers'
                compare={compare}
              />
            </div>
          </div>
        </div>
        <div className='collection-form-row'>
          <div className='row'>
            <div className={basicCompareModeClass}>
              <ChargeType
                isCompareMode={isCompareMode}
                compare={compare}
              />
            </div>
          </div>
        </div>
        <div className='collection-form-row'>
          <div className='row'>
            <div className={basicCompareModeClass}>
              <PhoneCostDescription
                isCompareMode={isCompareMode}
                counter
                maxLength={150}
                required={isChargeTypeOtherSelected}
                useQualityAgent
                collectionPrefix='phoneNumbers'
                compare={compare}
              />
            </div>
          </div>
        </div>
        {this.props.children}
      </div>
    )
  }
}

export default compose(
  withFormStates,
  asSection(),
  withPath,
  connect((state, ownProps) => ({
    isLocalNumberSelected: getIsLocalNumberSelectedForIndex(state, ownProps),
    isChargeTypeOtherSelected: getIsChargeTypeOtherSelectedForIndex(state, ownProps)
  })),
)(PhoneNumbersWithType)
