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
import React from 'react'
import { compose } from 'redux'
import PropTypes from 'prop-types'
import styles from './styles.scss'
import {
  PhoneNumber,
  PhoneNumberCode,
  PhoneNumberNumber,
  IsLocalNumber
} from 'util/redux-form/fields'
import { Label } from 'sema-ui-components'
import { change } from 'redux-form/immutable'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import enumSelector from 'selectors/enums'
import { getDialCodeOptionsJS } from 'util/redux-form/fields/DialCode/selectors'
import {
  getIsLocalNumberSelectedForIndex,
  getIsLocalNumberParsedForIndex,
  getUserPhoneNumberForIndex,
  getDialCodeForIndex
} from 'util/redux-form/sections/PhoneNumbers/selectors'
import { connect } from 'react-redux'
import withPath from 'util/redux-form/HOC/withPath'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'

const messages = defineMessages({
  title:  {
    id: 'AppComponents.OpeningHoursPreview.OpeningHoursPreview.Title',
    defaultMessage: 'Esikatselu'
  },
  serviceNumber:  {
    id: 'AppComponents.PhoneNumberParser.ServiceNumber.Title',
    defaultMessage: 'Kansallinen palvelunumero'
  }
})

const PhoneNumberParser = ({
  intl: { formatMessage },
  phoneNumberProps = {},
  formName,
  path,
  dialCodes,
  serviceNumbers,
  isLocalNumberSelected,
  isCompareMode,
  required,
  splitView,
  dispatch,
  isServiceNumber = true,
  isReadOnly,
  localServiceNumberLabel,
  disabled,
  isLocalNumberParsed,
  userNumber,
  dialCode,
  showAll = true,
  ...rest
}) => {
  const phoneNumberClass = isCompareMode || splitView ? 'col-lg-24' : 'col-lg-6'
  const previewClass = isCompareMode || splitView ? 'col-lg-24' : 'col-lg-9 mb-2'

  const handleParse = (e, value) => {
    parse(value, true)
  }
  const handleKey = (e) => {
    !showAll && e.value && parse(e.value, true)
  }

  const parse = (value, handleService) => {
    let defaultDialCode = dialCodes.filter(x => x.isDefault).first().value
    let isService = false
    let phone = value
    let customPath = path && path + '.' || ''

    const servicePrefixies = isServiceNumber && serviceNumbers.filter(serviceNumber => value.startsWith(serviceNumber))
    if (isServiceNumber && servicePrefixies.size && handleService) {
      isService = true
    } else if (value.startsWith('0')) {
      phone = value.substring(1)
    } else {
      let codes = dialCodes.filter(x => value.startsWith(x.code))
      phone = codes.size ? value.replace(codes.first().code + '0', '').replace(codes.first().code, '') : value
      defaultDialCode = codes.size ? codes.first().value : value.startsWith('+') ? '' : defaultDialCode
    }
    dispatch(change(formName, customPath + 'phoneNumber', phone.replace('+', '')))
    dispatch(change(formName, customPath + 'dialCode', defaultDialCode))
    dispatch(change(formName, customPath + 'isLocalNumber', isService))
    dispatch(change(formName, customPath + 'isLocalNumberParsed', isService))
  }
  const handleNationalFormat = (e, value) => {
    let customPath = path && path + '.' || ''
    if (value) {
      let parseNumber = userNumber
      if (dialCode) {
        parseNumber = parseNumber.replace(dialCodes.get(dialCode).code, '')
      }
      dispatch(change(formName, customPath + 'phoneNumber', parseNumber.replace('+', '')))
    } else {
      parse(userNumber, false)
    }
  }
  return (
    !showAll && (
      <PhoneNumber
        isCompareMode={isCompareMode}
        {...phoneNumberProps}
        required={required}
        onBlur={handleParse}
        customOnChange={handleKey}
        name={'wholePhoneNumber'}
        autocomplete='off'
        {...rest}
      />
    ) || (
      <div>
        <div className='row'>
          <div className={phoneNumberClass}>
            <div className='row'>
              <div className='col-24 mb-2'>
                <PhoneNumber
                  isCompareMode={isCompareMode}
                  {...phoneNumberProps}
                  required={required}
                  onBlur={handleParse}
                  name={'wholePhoneNumber'}
                  {...rest}
                />
              </div>
              <div className='col-24'>
                {isServiceNumber && <IsLocalNumber
                  {...rest}
                  isCompareMode={isCompareMode}
                  label={localServiceNumberLabel}
                  disabled={disabled || isLocalNumberParsed}
                  onChange={handleNationalFormat}
                />}
              </div>
            </div>
          </div>
          <div className={previewClass}>
            <div className='row'>
              <div className={styles.previewLabel}>
                <Label labelText={formatMessage(messages.title)} />
              </div>
            </div>
            <div className='row'>
              {!isLocalNumberSelected &&
              <div className={styles.previewDial}>
                <PhoneNumberCode isCompareMode={isCompareMode} />
              </div>}
              <div className={styles.previewNumber}>
                <PhoneNumberNumber isCompareMode={isCompareMode} />
              </div>
              <div className={styles.previewNumber}>
                {isLocalNumberSelected && formatMessage(messages.serviceNumber)}
              </div>
            </div>
          </div>
        </div>
      </div>
    )
  )
}

PhoneNumberParser.propTypes = {
  formName: PropTypes.string.isRequired,
  dialCodes: PropTypes.any,
  path: PropTypes.string,
  isLocalNumberSelected: PropTypes.bool,
  isCompareMode: PropTypes.bool,
  required: PropTypes.bool,
  splitView: PropTypes.bool,
  dispatch: PropTypes.func,
  phoneNumberProps: PropTypes.object,
  serviceNumbers: PropTypes.any,
  intl: intlShape,
  isFaxNumber: PropTypes.any,
  showAll: PropTypes.bool
}

export default compose(
  injectIntl,
  withFormStates,
  injectFormName,
  withPath,
  connect((state, ownProps) => ({
    isLocalNumberSelected: getIsLocalNumberSelectedForIndex(state, ownProps),
    isLocalNumberParsed: getIsLocalNumberParsedForIndex(state, ownProps),
    userNumber: getUserPhoneNumberForIndex(state, ownProps),
    dialCode: getDialCodeForIndex(state, ownProps),
    dialCodes: getDialCodeOptionsJS(state),
    serviceNumbers: enumSelector.serviceNumbers.getEnums(state)
  }), {
    change
  }),
)(PhoneNumberParser)
