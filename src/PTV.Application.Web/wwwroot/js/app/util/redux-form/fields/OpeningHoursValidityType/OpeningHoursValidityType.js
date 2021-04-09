/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the 'Software'), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
import React from 'react'
import PropTypes from 'prop-types'
import { Field } from 'redux-form/immutable'
import { RadioGroup, RadioButton } from 'sema-ui-components'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { compose } from 'redux'
import styles from './styles.scss'
import cx from 'classnames'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import withFormStates from 'util/redux-form/HOC/withFormStates'

const messages = defineMessages({
  validOnward: {
    id: 'Containers.Channels.Common.OpeningHours.ValidityType.Onward',
    defaultMessage: 'Toistaiseksi voimassa oleva'
  },
  validPeriod: {
    id: 'Containers.Channels.Common.OpeningHours.ValidityType.Period',
    defaultMessage: 'Voimassa ajanjaksolla'
  },
  validDaySingle: {
    id: 'Containers.Channels.Common.OpeningHours.ValidityType.DaySingle',
    defaultMessage: 'Päivä'
  },
  validDayRange: {
    id: 'Containers.Channels.Common.OpeningHours.ValidityType.DayRange',
    defaultMessage: 'Ajanjakso'
  }
})

const renderValidityTypeRadioGroup = injectIntl(({
  input,
  label1,
  label2,
  intl: { formatMessage },
  isCompareMode,
  children
}) => {
  const validityTypesClass = cx(
    styles.validityTypes,
    {
      [styles.notInCompareMode]: !isCompareMode
    }
  )
  return (
    <div>
      {children
        ? <div>
          <RadioGroup small {...input} value={input.value || false} className={styles.validityType}>
            <RadioButton label={label1} value={false} />
          </RadioGroup>
          {!input.value && children}
          <RadioGroup small {...input} value={input.value || false} className={styles.validityType}>
            <RadioButton label={label2} value />
          </RadioGroup>
          {input.value && children}
        </div>
        : <RadioGroup small inline={!isCompareMode} {...input} value={input.value || false} className={validityTypesClass}>
          <RadioButton label={label1} value={false} />
          <RadioButton label={label2} value />
        </RadioGroup>
      }
    </div>
  )
})

const OpeningHoursValidityType = ({
  intl: { formatMessage },
  dayRange,
  ...rest
}) => (
  <div className={styles.validity}>
    <Field
      name='isPeriod'
      component={renderValidityTypeRadioGroup}
      label1={formatMessage(dayRange && messages.validDaySingle || messages.validOnward)}
      label2={formatMessage(dayRange && messages.validDayRange || messages.validPeriod)}
      {...rest}
    />
  </div>
)

OpeningHoursValidityType.propTypes = {
  intl: intlShape,
  dayRange: PropTypes.bool
}

export default compose(
  injectIntl,
  withFormStates,
  asDisableable
)(OpeningHoursValidityType)
