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
import Intervals from '../../OpeningDays/Intervals'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { injectIntl, intlShape } from 'util/react-intl'
import { Checkbox } from 'sema-ui-components'
import styles from '../styles.scss'
import cx from 'classnames'
import { localizeProps, languageTranslationTypes } from 'appComponents/Localize'
import HolidayType from '../HolidayType/HolidayType'
import { isHolidayOpen } from '../selectors'
import withPath from 'util/redux-form/HOC/withPath'
import injectFormName from 'util/redux-form/HOC/injectFormName'

const Holiday = ({
  holidayName,
  label,
  intervals,
  onActivate,
  intl: { formatMessage },
  isCompareMode,
  compare,
  disabled,
  checked,
  isOpen,
  ...rest
}) => {
  const hasInterval = intervals && intervals.length && (intervals[0].from.input.value || intervals[0].to.input.value)
  const holidayItemClass = cx(
    styles.holidayItem,
    {
      [styles.selected]: checked
    }
  )
  return (
    <div className={holidayItemClass}>
      <div className='row'>
        <div className='col-sm-9'>
          <Checkbox
            name={`holidayHours.${holidayName}.active`}
            onChange={onActivate}
            label={label}
            disabled={disabled}
            checked={checked}
            small
          />
        </div>
        <div className='col-sm-15'>
          <div className='row'>
            <div className='col-sm-12'>
              <div className={styles.holidayType}>
                {checked && !disabled &&
                <HolidayType
                  name={`holidayHours.${holidayName}.type`}
                  isCompareMode={isCompareMode}
                  compare={compare}
                  hasInterval={hasInterval}
                />
                }
              </div>
            </div>
          </div>
          <div className='row'>
            <div className='col-sm-12'>
              <div className={styles.holidayInterval}>
                {checked && !disabled && isOpen &&
                <Intervals
                  name={`holidayHours.${holidayName}.intervals`}
                  intervals={intervals}
                  isCompareMode={isCompareMode}
                  compare={compare}
                  maxIntervals={1}
                />
                }
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
Holiday.propTypes = {
  holidayName: PropTypes.string,
  intl: intlShape,
  compare: PropTypes.bool,
  isOpen: PropTypes.bool,
  isCompareMode: PropTypes.bool
}

export default compose(
  injectIntl,
  withPath,
  injectFormName,
  connect((state, { formName, path, holidayName }) => ({
    isOpen: isHolidayOpen(formName, path + '.holidayHours.' + holidayName)(state)
  })),
  localizeProps({
    languageTranslationType: languageTranslationTypes.data,
    idAttribute: 'holidayNameId',
    nameAttribute: 'label'
  })
)(Holiday)
