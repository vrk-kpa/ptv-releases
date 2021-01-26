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
  PhoneCostDescription,
  PhoneNumberInfo,
  PhoneType
} from 'util/redux-form/fields'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import withPath from 'util/redux-form/HOC/withPath'
import asSection from 'util/redux-form/HOC/asSection'
import {
  getIsLocalNumberSelectedForIndex,
  getIsChargeTypeOtherSelectedForIndex,
  getIsPhoneTypePhoneSelectedForIndex
} from '../PhoneNumbers/selectors'
import PhoneNumberParser from 'appComponents/PhoneNumberParser'
import { getDialCodeOptionsJS } from 'util/redux-form/fields/DialCode/selectors'
import { change } from 'redux-form/immutable'
import injectFormName from 'util/redux-form/HOC/injectFormName'

class PhoneNumbersWithType extends PureComponent {
  static defaultProps = {
    name: 'phoneNumbers'
  }
  render () {
    const {
      phoneNumberProps = {},
      phoneNumberInfoProps = {},
      isCompareMode,
      splitView,
      required,
      compare,
      isReadOnly,
      isChargeTypeOtherSelected,
      isPhoneTypePhoneSelected,
      path,
      dialCodes,
      dispatch,
      formName
    } = this.props

    const clearAll = input => value => {
      let defaultDialCode = dialCodes.filter(x => x.isDefault).first().value
      let customPath = path && path + '.' || ''
      dispatch(change(formName, customPath + 'phoneNumber', ''))
      dispatch(change(formName, customPath + 'wholePhoneNumber', ''))
      dispatch(change(formName, customPath + 'dialCode', defaultDialCode))
      dispatch(change(formName, customPath + 'isLocalNumber', false))
      dispatch(change(formName, customPath + 'isLocalNumberParsed', false))
      input.onChange(value)
    }

    const basicCompareModeClass = isCompareMode || splitView ? 'col-lg-24' : 'col-lg-12'
    return (
      <div>
        <div className='collection-form-row'>
          <div className='row'>
            <div className={basicCompareModeClass}>
              <PhoneType
                isCompareMode={isCompareMode}
                compare={compare}
                required={required}
                customOnChange={clearAll}
              />
            </div>
          </div>
        </div>
        <div className='collection-form-row'>
          <PhoneNumberParser
            isCompareMode={isCompareMode}
            compare={compare}
            required={required}
            splitView={splitView}
            phoneNumberProps={phoneNumberProps}
            isServiceNumber={isPhoneTypePhoneSelected}
          />
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
  injectFormName,
  asSection(),
  withPath,
  connect((state, ownProps) => ({
    isLocalNumberSelected: getIsLocalNumberSelectedForIndex(state, ownProps),
    isChargeTypeOtherSelected: getIsChargeTypeOtherSelectedForIndex(state, ownProps),
    isPhoneTypePhoneSelected: getIsPhoneTypePhoneSelectedForIndex(state, ownProps),
    dialCodes: getDialCodeOptionsJS(state)
  }), {
    change
  }),
)(PhoneNumbersWithType)
