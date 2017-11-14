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
import PropTypes from 'prop-types'
import { RenderSelect, RenderSelectDisplay } from 'util/redux-form/renders'
import { Field } from 'redux-form/immutable'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import { injectIntl, intlShape } from 'react-intl'
import { compose } from 'redux'
import { asDisableable, asComparable } from 'util/redux-form/HOC'
import { messages as dayMessages } from './messages'
import styles from './styles.scss'

const daysOfTheWeek = [
  'monday',
  'tuesday',
  'wednesday',
  'thursday',
  'friday',
  'saturday',
  'sunday'
]
const options = daysOfTheWeek.map(dayName => ({
  label: dayName,
  value: dayName
}))

const DaySelect = ({
  intl: { formatMessage },
  validate,
  ...rest
}) => {
  const optionRenderer = (option) => formatMessage(dayMessages[option.value])
  const valueRenderer = (option) => formatMessage(dayMessages[option.value])
  return (
    <div className={styles.day}>
      <Field
        name='daySelect'
        component={RenderSelect}
        options={options}
        optionRenderer={optionRenderer}
        valueRenderer={valueRenderer}
        // validate={translateValidation(validate, formatMessage, messages.label)}
        {...rest}
      />
    </div>
  )
}
DaySelect.propTypes = {
  intl: intlShape,
  validate: PropTypes.func
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderSelectDisplay }),
  asDisableable,
  injectSelectPlaceholder()
)(DaySelect)
