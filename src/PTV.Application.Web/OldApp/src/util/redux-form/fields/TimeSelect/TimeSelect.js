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
import { Field } from 'redux-form/immutable'
import { RenderSelect, RenderSelectDisplay } from 'util/redux-form/renders'
import { range } from 'lodash'
import moment from 'moment'
import { compose } from 'redux'
import { connect } from 'react-redux'
import styles from './styles.scss'
import asComparable from 'util/redux-form/HOC/asComparable'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import { injectIntl } from 'util/react-intl'

const timeOptions = range(0, 96).map(index => {
  const minute = 60 * 1000
  const time = moment.utc(index * minute * 15)
  return {
    label: time.format('HH:mm'),
    value: time.valueOf()
  }
})

const getFilteredTimeOptions = (timeOptions, filterValue, filterBefore) => {
  return filterValue
    ? filterBefore
      ? timeOptions.filter(x => x.value < filterValue)
      : timeOptions.filter(x => x.value > filterValue)
    : timeOptions
}

const TimeSelect = props => {
  return (
    <div className={styles.time}>
      <Field
        className={styles.timeSelect}
        name='timeSelect'
        component={RenderSelect}
        {...props}
      />
    </div>
  )
}

TimeSelect.propTypes = {
  name: PropTypes.string.isRequired
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderSelectDisplay }),
  asDisableable,
  connect(
    (state, { filterValue, options, filterBefore }) => {
      return {
        options: options || getFilteredTimeOptions(timeOptions, filterValue, filterBefore)
      }
    }
  )
)(TimeSelect)
