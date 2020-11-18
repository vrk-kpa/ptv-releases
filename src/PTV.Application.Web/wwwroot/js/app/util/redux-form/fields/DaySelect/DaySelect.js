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
import React from 'react'
import PropTypes from 'prop-types'
import { RenderSelect, RenderSelectDisplay } from 'util/redux-form/renders'
import { Field } from 'redux-form/immutable'
import { localizeList, languageTranslationTypes } from 'appComponents/Localize'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import { injectIntl, intlShape } from 'util/react-intl'
import { compose } from 'redux'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'
import { messages as dayMessages } from './messages'
import styles from './styles.scss'
import { daysOfTheWeek } from 'enums'

const options = daysOfTheWeek.map(dayName => ({
  label: dayName,
  value: dayName,
  code: dayMessages[dayName].id
}))

const RenderDays = compose(
  localizeList({
    input: 'options',
    idAttribute: 'code',
    nameAttribute: 'label',
    languageTranslationType: languageTranslationTypes.locale
  }),
)(RenderSelect)

const DaySelect = ({
  intl: { formatMessage },
  validate,
  ...rest
}) => (
  <div className={styles.day}>
    <Field
      name='daySelect'
      component={RenderDays}
      options={options}
      {...rest}
    />
  </div>
)
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
