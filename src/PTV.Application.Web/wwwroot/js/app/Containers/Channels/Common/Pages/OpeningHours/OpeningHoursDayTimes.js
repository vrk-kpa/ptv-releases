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
import { connect } from 'react-redux'
import { injectIntl, intlShape } from 'react-intl'
import moment from 'moment'

import * as PTVValidatorTypes from '../../../../../Components/PTVValidators'
import PTVDateTimePicker from '../../../../../Components/PTVDateTimePicker'
import PTVAutoComboBox from '../../../../../Components/PTVAutoComboBox'
import PTVTimeSelect from '../../../../../Components/PTVTimeSelect'
import PTVLabel from '../../../../../Components/PTVLabel'
import './styles.scss'

import * as CommonChannelSelectors from '../../../Common/Selectors'
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps'
import * as commonChannelActions from '../../../Common/Actions'

// enums
import { openingHoursTypes, weekdaysEnum } from '../../../../Common/Enums'
import { openingHoursMessages } from '../../Messages'

const weekdaysList = Object.keys(weekdaysEnum).map(x => ({ id: weekdaysEnum[x], name: x }))
const openingHoursTypeList = Object.keys(openingHoursTypes).map(x => (x))

const OpeningHoursDayTimes = props => {
  const {
    intl: { formatMessage },
    closed,
    type,
    openingHoursType,
    label,
    day,
    time,
    date,
    timeTreshold,
    readOnly,
    validityType,
    onInputChange,
    filter,
    hoursValidators
  } = props

  const daySelectClass = 'day-select'
  const dayClass = !validityType && openingHoursType === openingHoursTypes.special
    ? 'col-xs-6 col-md-4'
    : 'col-xs-offset-0 col-xs-6 col-md-offset-2 col-lg-offset-4 col-md-4'
  const dateClass = openingHoursType === openingHoursTypes.exceptional ? 'col-md-4' : 'col-md-5'
  const showDatePicker =
        openingHoursType === openingHoursTypes.exceptional && (validityType || (!validityType && type !== 'end')) ||
        openingHoursType === openingHoursTypes.special && validityType

  const getValidationFunction = (type, filter) => {
    if (!isNaN(Number(filter)) && Math.abs(Number(filter)) > 0) {
      if (type === 'start') {
        return validBefore
      } else if (type === 'end') {
        return validAfter
      }
    }
    return null
  }

  const validAfter = (current) => {
    const treshold = moment(filter)
    return current.isAfter(treshold)
  }

  const validBefore = (current) => {
    const treshold = moment(filter)
    return current.isBefore(treshold)
  }

  const optionRenderer = (option) => {
    return formatMessage(openingHoursMessages[`weekday_short_${option.id}`])
  }

  return (
    <div className='row'>
      { closed && !validityType && type === 'end'
      ? null
      : <div className='col-md-2 col-lg-4'>
        <PTVLabel readOnly={readOnly}> { label } </PTVLabel>
      </div>
      }
      { showDatePicker &&
        <div className={dateClass}>
          <PTVDateTimePicker
            value={date}
            mode='date'
            onChangeValid={type === 'start' ? onInputChange('validFrom') : onInputChange('validTo')}
            relatedValue={filter}
            isValidDate={getValidationFunction(type, filter)}
            readOnly={readOnly}
            validatedField={openingHoursMessages[`dayTimesDate_${openingHoursTypeList[openingHoursType]}_${type}`]}
            inputProps={{ className: 'ptv-textinput' }}
            validators={hoursValidators}
          />
        </div>
      }
      { openingHoursType !== openingHoursTypes.exceptional &&
        <div className={dayClass}>
          <div className={daySelectClass}>
            <PTVAutoComboBox
              value={day}
              values={weekdaysList}
              changeCallback={type === 'start' ? onInputChange('dayFrom') : onInputChange('dayTo')}
              name={daySelectClass}
              readOnly={readOnly}
              optionRenderer={optionRenderer}
              validatedField={openingHoursMessages[`dayTimesDay_${openingHoursTypeList[openingHoursType]}_${type}`]}
              validators={hoursValidators}
            />
          </div>
        </div>
      }
      { !closed &&
        <div className='col-xs-6 col-md-4'>
          <PTVTimeSelect
            value={time}
            onChange={type === 'start' ? onInputChange('timeFrom') : onInputChange('timeTo')}
            name={time}
            step={15}
            treshold={timeTreshold}
            label={
              formatMessage(openingHoursMessages[`dayTimesTime_${openingHoursTypeList[openingHoursType]}_${type}`])
            }
            validatedField={openingHoursMessages[`dayTimesTime_${openingHoursTypeList[openingHoursType]}_${type}`]}
            validators={hoursValidators}
          />
        </div>
      }
    </div>
  )
}

OpeningHoursDayTimes.propTypes = {
  id: PropTypes.string,
  type: PropTypes.string,
  filter: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.number
  ]),
  day: PropTypes.string,
  time: PropTypes.number,
  date: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.number
  ]),
  readOnly: PropTypes.bool,
  onInputChange: PropTypes.func,
  openingHoursType: PropTypes.number,
  closed: PropTypes.bool,
  intl: intlShape.isRequired
}

// maps state to props
function mapStateToProps (state, ownProps) {
  const isValueValid = ownProps.openingHoursType === 2
    ? CommonChannelSelectors.getIsCurrentOpeningHourSpecialValid(state, ownProps)
    : CommonChannelSelectors.getIsCurrentOpeningHourExceptionalValid(state, ownProps)
  const isValueEntered = ownProps.openingHoursType === 2
    ? CommonChannelSelectors.getIsAnyOpeningHoursSpecial(state, ownProps)
    : CommonChannelSelectors.getIsAnyOpeningHoursExceptional(state, ownProps)
  const validators = [PTVValidatorTypes.IS_REQUIRED]
  const hoursValidators = isValueEntered && !isValueValid ? validators : null
  return {
    validityType: CommonChannelSelectors.getOpeningHourValidityType(state, ownProps),
    hoursValidators
  }
}

const actions = [
  commonChannelActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(OpeningHoursDayTimes))
