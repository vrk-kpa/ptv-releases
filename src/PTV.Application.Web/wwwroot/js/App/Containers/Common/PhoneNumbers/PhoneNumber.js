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
import React, { PropTypes } from 'react'
import { connect } from 'react-redux'
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl'
import { callApiDirect } from '../../../Middleware/Api'

// components
import * as PTVValidatorTypes from '../../../Components/PTVValidators'
import { PTVTextInput, PTVCheckBox, PTVLabel } from '../../../Components'
import { LocalizedAsyncComboBox } from '../../Common/localizedData'
import Info from '../AdditionalInfoComponent'
import cx from 'classnames'

// selectors
import * as CommonSelectors from '../Selectors'

// schemas
import { CommonSchemas } from '../../Common/Schemas'

// actions
import * as commonActions from '../Actions'
import mapDispatchToProps from '../../../Configuration/MapDispatchToProps'

export const PhoneNumber = ({
  phoneId,
  actions,
  intl,
  readOnly,
  language,
  translationMode,
  splitContainer,
  count,
  startOrder,
  infoMaxLength,
  withInfo,
  componentClass,
  messages,
  isNew,
  onAddPhoneNumber,
  shouldValidate,
  phoneNumber,
  additionalInformation,
  prefixNumber,
  isFinnishServiceNumber,
  isLocalServiceNumberVisible,
  isRequired
}) => {
  const onInputChange = input => value => {
     if (!isNew) {
          actions.onEntityInputChange('phoneNumbers', phoneId, input, value)
        } else {
          onAddPhoneNumber({
              id: phoneId,
              [input]: value
            })
        }
   }

  const onCheckBoxInputChange = input => value => {
     if (!isNew) {
          actions.onEntityInputChange('phoneNumbers', phoneId, input, value.target.checked)
        } else {
          onAddPhoneNumber({
              id: phoneId,
              [input]: value.target.checked
            })
        }

     if (value.target.checked)        {
          actions.onEntityInputChange('phoneNumbers', phoneId, 'prefixNumber', null)
        }
   }

  const onPrefixNumberInputChange = input => (value, object) => {
     if (!isNew) {
          actions.onEntityAdd({ id: phoneId, [input]: object }, CommonSchemas.PHONE_NUMBER)
        } else {
          onAddPhoneNumber({
              id: phoneId,
              [input]: object
            })
        }
     actions.onEntityInputChange('phoneNumbers', phoneId, 'isFinnishServiceNumber', false)
   }

  const dialCodeDataOptions = x => {
      return { ...x,
          name: (x.name || x.countryName) + ' (' + x.code + ')'
        }
    }

  const validators = [PTVValidatorTypes.IS_REQUIRED]
  const sharedProps = { readOnly, language, translationMode, splitContainer }
  const { formatMessage } = intl

   // const phoneNumberContainerClass = splitContainer ? "col-xs-12" : "col-lg-7";
  const phonePrefixClass = splitContainer ? 'col-xs-12 col-lg-6' : 'col-xs-12 col-md-4 col-lg-3'
  const phoneNumberClass = splitContainer ? 'col-xs-12 col-lg-6' : 'col-xs-12 col-md-4 col-lg-3'
  const infoClass = splitContainer ? 'col-xs-12' : 'col-md-4 col-lg-6'
  const applyValidators = shouldValidate || isRequired

  return (
     <div>
          <div className='row'>
              <div className={phonePrefixClass}>
                <div className='row'>

                  <LocalizedAsyncComboBox
                    componentClass='col-xs-12'
                    name='phoneNumberPrefix'
                    label={formatMessage(messages.prefixTitle)}
                    placeholder ={formatMessage(messages.prefixPlaceHolder)}
                    tooltip ={formatMessage(messages.prefixTooltip)}
                    endpoint= 'common/GetDialCodes'
                    formatOption={dialCodeDataOptions}
                    formatValue={dialCodeDataOptions}
                    onChange={onPrefixNumberInputChange('prefixNumber')}
                    value={prefixNumber}
                    validators={applyValidators && (!isLocalServiceNumberVisible || !isFinnishServiceNumber) ? validators : []}
                    disabled={translationMode == 'view'}
                    readOnly={readOnly && translationMode == 'none'}
                    className='limited w320'
                    language={language}
                  />
                  { isLocalServiceNumberVisible &&
                    <div className='col-xs-12'>
                      {readOnly && translationMode !== 'view' && isFinnishServiceNumber &&
                        <PTVLabel labelClass='main'>{formatMessage(messages.prefixTitle)}</PTVLabel>
                      }

                      <PTVCheckBox
                        id={phoneId}
                        className='strong'
                        isSelectedSelector={CommonSelectors.getIsFinnishServiceNumber}
                        onClick={onCheckBoxInputChange('isFinnishServiceNumber')}
                        readOnly= {readOnly && translationMode === 'none'}
                        isDisabled= {translationMode === 'view'}
                        language= {language}
                        phoneId= {phoneId}
                      >
                        <FormattedMessage {...messages.finnishServiceNumberName} />
                      </PTVCheckBox>
                    </div>
                  }
                </div>
              </div>

              <PTVTextInput
                  componentClass={phoneNumberClass}
                  inputclass='flex-container flex-reversed'
                  className='phone-number'
                  label= {formatMessage(messages.title)}
                  tooltip ={messages.tooltip ? formatMessage(messages.tooltip) : null}
                  value= {phoneNumber}
                  blurCallback= {onInputChange('number')}
                  maxLength ={20}
                  size= 'w130'
                  name ='phoneNumber'
                  placeholder ={formatMessage(messages.placeholder)}
                  order= {startOrder}
                  validators={applyValidators || (additionalInformation.length > 0) ? validators : []}
                  typingRules='containsPhoneNumberChars'
                  readOnly={readOnly && translationMode == 'none'}
                  disabled={translationMode == 'view'}
                    >
                  <div className='prefix-number'>
                      {prefixNumber ? prefixNumber.code : null}
                    </div>
                </PTVTextInput>

              <div className={infoClass}>
                  { withInfo
                        ? <Info
                          {...sharedProps}
                            // componentClass = {"col-sm-12 col-md-4 col-lg-6"}
                          info= {additionalInformation}
                          handleInfoChange= {onInputChange('additionalInformation')}
                          maxLength ={infoMaxLength}
                          name= {'phoneNumberInfo'}
                          label= {formatMessage(messages.infoTitle)}
                          tooltip ={formatMessage(messages.infoTooltip)}
                          placeholder= {formatMessage(messages.infoPlaceholder)}
                          size= 'full'
                        />
                    : null }
                </div>
            </div>
        </div>
   )
}

PhoneNumber.propTypes = {
  isLocalServiceNumberVisible: PropTypes.bool,
  phoneId: PropTypes.string.isRequired,
  messages: PropTypes.object.isRequired,
  readOnly: PropTypes.bool,
  shouldValidate: PropTypes.bool,
  withInfo: PropTypes.bool,
  isNew: PropTypes.bool,
  componentClass: PropTypes.string,
  count: PropTypes.number,
  onAddPhoneNumber: PropTypes.func
}

PhoneNumber.defaultProps = {
  isNew: false,
  readOnly: false,
  shouldValidate: false,
  withInfo: true,
  isLocalServiceNumberVisible: true
}

function mapStateToProps (state, ownProps) {
  const phoneNumber = CommonSelectors.getPhoneNumberPhoneNumber(state, ownProps)
  const prefixNumber = CommonSelectors.getSelectedPrefixNumberJS(state, ownProps)
  const isFinnishServiceNumber = CommonSelectors.getIsFinnishServiceNumber(state, ownProps)
  return {
    phoneNumber,
    additionalInformation: CommonSelectors.getPhoneNumberAdditionalInformation(state, ownProps),
    prefixNumber,
    isFinnishServiceNumber,
    isRequired: phoneNumber || prefixNumber || isFinnishServiceNumber,
    isLocalServiceNumberVisible: ownProps.numberType !== 'fax'
  }
}

const actions = [
  commonActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(PhoneNumber))
