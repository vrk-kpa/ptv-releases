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
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import withPath from 'util/redux-form/HOC/withPath'
import asSection from 'util/redux-form/HOC/asSection'
import {
  ChargeType,
  DialCode,
  IsLocalNumber,
  PhoneCostDescription,
  PhoneNumber,
  PhoneNumberInfo
} from 'util/redux-form/fields'
import {
  getIsLocalNumberSelectedForIndex,
  getIsChargeTypeOtherSelectedForIndex,
  getIsSotePhoneNumber
} from './selectors'
import SoteNote from 'appComponents/SoteNote'
import cx from 'classnames'
import styles from './styles.scss'
import shortId from 'shortid'

class PhoneNumbers extends PureComponent {
  render () {
    const {
      dialCodeProps = {},
      phoneNumberProps = {},
      phoneNumberInfoProps = {},
      localServiceNumberLabel,
      chargeTypeProps = {},
      phoneCostDescriptionProps = {},
      isCompareMode,
      compare,
      isLocalNumberSelected,
      isChargeTypeOtherSelected,
      splitView,
      isReadOnly,
      isSotePhoneNumber
    } = this.props

    const autocompleteId = shortId.generate()

    const dialCodeClass = cx(
      {
        [styles.hideValue]: isLocalNumberSelected
      }
    )

    return (
      <div>
        <SoteNote showNote={isSotePhoneNumber} />
        {!isCompareMode && !splitView
          ? <div>
            <div className='collection-form-row'>
              <div className='row'>
                <div className='col-lg-6'>
                  <div className='row'>
                    {!(isReadOnly && isLocalNumberSelected) &&
                      <div className='col-24 mb-2'>
                        <div className={dialCodeClass}>
                          <DialCode
                            isCompareMode={isCompareMode}
                            disabled={isLocalNumberSelected || isSotePhoneNumber}
                            compare={compare}
                            isReadOnly={isReadOnly}
                            autocomplete={`${autocompleteId}DialCode`}
                            {...dialCodeProps}
                          />
                        </div>
                      </div>
                    }
                    <div className='col-24'>
                      <IsLocalNumber
                        isCompareMode={isCompareMode}
                        label={localServiceNumberLabel}
                        isReadOnly={isReadOnly}
                        disabled={isSotePhoneNumber}
                      />
                    </div>
                  </div>
                </div>
                <div className='col-lg-6'>
                  <PhoneNumber
                    size='full'
                    isCompareMode={isCompareMode}
                    disabled={isSotePhoneNumber}
                    autocomplete={`${autocompleteId}PhoneNumber`}
                    {...phoneNumberProps}
                    isReadOnly={isReadOnly}
                  />
                </div>
              </div>
              <div className='collection-form-row'>
                <div className='row'>
                  <div className='col-lg-12'>
                    <PhoneNumberInfo
                      isCompareMode={isCompareMode}
                      disabled={isSotePhoneNumber}
                      {...phoneNumberInfoProps}
                      isReadOnly={isReadOnly}
                      useQualityAgent
                      collectionPrefix='phoneNumbers'
                      compare={compare}
                    />
                  </div>
                </div>
              </div>
            </div>
            <div className='collection-form-row'>
              <div className='row'>
                <div className='col-lg-6'>
                  <ChargeType
                    isCompareMode={isCompareMode}
                    compare={compare}
                    disabled={isSotePhoneNumber}
                    {...chargeTypeProps}
                    isReadOnly={isReadOnly}
                  />
                </div>
              </div>
            </div>
            <div className='collection-form-row'>
              <div className='row'>
                <div className='col-lg-12'>
                  <PhoneCostDescription
                    size='full'
                    counter
                    maxLength={150}
                    isCompareMode={isCompareMode}
                    disabled={isSotePhoneNumber}
                    {...phoneCostDescriptionProps}
                    required={isChargeTypeOtherSelected}
                    isReadOnly={isReadOnly}
                    useQualityAgent
                    collectionPrefix='phoneNumbers'
                    compare={compare}
                  />
                </div>
              </div>
            </div>
          </div>
          : <div>
            <div className='collection-form-row'>
              {!(isReadOnly && isLocalNumberSelected) &&
                <div className={dialCodeClass}>
                  <DialCode
                    size='w240'
                    disabled={isLocalNumberSelected || isSotePhoneNumber}
                    isCompareMode={isCompareMode}
                    compare={compare}
                    autocomplete={`${autocompleteId}DialCode`}
                    {...dialCodeProps}
                    isReadOnly={isReadOnly}
                  />
                </div>
              }
            </div>
            <div className='collection-form-row'>
              <IsLocalNumber
                isCompareMode={isCompareMode}
                label={localServiceNumberLabel}
                isReadOnly={isReadOnly}
                disabled={isSotePhoneNumber}
              />
            </div>
            <div className='collection-form-row'>
              <PhoneNumber
                size='full'
                isCompareMode={isCompareMode}
                disabled={isSotePhoneNumber}
                autocomplete={`${autocompleteId}PhoneNumber`}
                {...phoneNumberProps}
                isReadOnly={isReadOnly}
              />
            </div>
            <div className='collection-form-row'>
              <PhoneNumberInfo
                isCompareMode={isCompareMode}
                disabled={isSotePhoneNumber}
                {...phoneNumberInfoProps}
                isReadOnly={isReadOnly}
                useQualityAgent
                collectionPrefix='phoneNumbers'
                compare={compare}
              />
            </div>
            <div className='collection-form-row'>
              <ChargeType
                isCompareMode={isCompareMode}
                size='w240'
                compare={compare}
                disabled={isSotePhoneNumber}
                {...chargeTypeProps}
                isReadOnly={isReadOnly}
              />
            </div>
            <div className='collection-form-row'>
              <PhoneCostDescription
                size='full'
                counter
                maxLength={150}
                isCompareMode={isCompareMode}
                disabled={isSotePhoneNumber}
                {...phoneCostDescriptionProps}
                required={isChargeTypeOtherSelected}
                isReadOnly={isReadOnly}
                useQualityAgent
                collectionPrefix='phoneNumbers'
                compare={compare}
              />
            </div>
          </div>
        }
        {this.props.children}
      </div>
    )
  }
}

PhoneNumbers.propTypes = {
  dialCodeProps: PropTypes.object,
  phoneNumberProps: PropTypes.object,
  phoneNumberInfoProps: PropTypes.object,
  localServiceNumberLabel: PropTypes.string,
  chargeTypeProps: PropTypes.object,
  phoneCostDescriptionProps: PropTypes.object,
  isCompareMode: PropTypes.bool,
  compare: PropTypes.bool,
  isLocalNumberSelected: PropTypes.bool,
  isChargeTypeOtherSelected: PropTypes.bool,
  splitView: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  isSotePhoneNumber: PropTypes.bool,
  children: PropTypes.any
}

export default compose(
  withFormStates,
  asSection(),
  withPath,
  connect((state, ownProps) => ({
    isLocalNumberSelected: getIsLocalNumberSelectedForIndex(state, ownProps),
    isChargeTypeOtherSelected: getIsChargeTypeOtherSelectedForIndex(state, ownProps),
    isSotePhoneNumber: getIsSotePhoneNumber(state, ownProps)
  })),
)(PhoneNumbers)
