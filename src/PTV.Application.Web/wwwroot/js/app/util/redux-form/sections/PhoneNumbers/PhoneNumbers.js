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
  getIsChargeTypeOtherSelectedForIndex
} from './selectors'
import cx from 'classnames'
import styles from './styles.scss'

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
      isReadOnly
    } = this.props
    const dialCodeClass = cx(
      {
        [styles.hideValue]: isLocalNumberSelected
      }
    )
    return (
      <div>
        {!isCompareMode && !splitView
          ? <div>
            <div className='collection-form-row'>
              <div className='row'>
                <div className='col-lg-6'>
                  <div className='row'>
                    { !(isReadOnly && isLocalNumberSelected) &&
                      <div className='col-24 mb-2'>
                        <div className={dialCodeClass}>
                          <DialCode
                            isCompareMode={isCompareMode}
                            disabled={isLocalNumberSelected}
                            compare={compare}
                            isReadOnly={isReadOnly}
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
                      />
                    </div>
                  </div>
                </div>
                <div className='col-lg-6'>
                  <PhoneNumber
                    size='full'
                    isCompareMode={isCompareMode}
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
                      {...phoneNumberInfoProps}
                      isReadOnly={isReadOnly}
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
                    {...phoneCostDescriptionProps}
                    required={isChargeTypeOtherSelected}
                    isReadOnly={isReadOnly}
                  />
                </div>
              </div>
            </div>
          </div>
        : <div>
          <div className='collection-form-row'>
            { !(isReadOnly && isLocalNumberSelected) &&
            <div className={dialCodeClass}>
              <DialCode
                size='w240'
                disabled={isLocalNumberSelected}
                isCompareMode={isCompareMode}
                compare={compare}
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
            />
          </div>
          <div className='collection-form-row'>
            <PhoneNumber
              size='full'
              isCompareMode={isCompareMode}
              {...phoneNumberProps}
              isReadOnly={isReadOnly}
            />
          </div>
          <div className='collection-form-row'>
            <PhoneNumberInfo
              isCompareMode={isCompareMode}
              {...phoneNumberInfoProps}
              isReadOnly={isReadOnly}
            />
          </div>
          <div className='collection-form-row'>
            <ChargeType
              isCompareMode={isCompareMode}
              size='w240'
              compare={compare}
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
              {...phoneCostDescriptionProps}
              required={isChargeTypeOtherSelected}
              isReadOnly={isReadOnly}
            />
          </div>
        </div>
        }
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
)(PhoneNumbers)
