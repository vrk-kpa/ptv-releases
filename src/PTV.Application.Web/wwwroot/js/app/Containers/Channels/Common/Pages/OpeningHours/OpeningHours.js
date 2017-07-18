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
import React, { Component, PropTypes } from 'react'
import { connect } from 'react-redux'
import { injectIntl, intlShape } from 'react-intl'
import cx from 'classnames'

// components
import * as PTVValidatorTypes from '../../../../../Components/PTVValidators'
import OpeningHoursDailySet from './OpeningHoursDailySet'
import OpeningHoursAdditionalInfo from './OpeningHoursAdditionalInfo'
import OpeningHoursValidityTypes from './OpeningHoursValidityTypes'
import DateTimeValidityInputs from '../../../../Common/DateTimeValidityInputs'
import './styles.scss'

// selectors
import * as CommonChannelSelectors from '../../Selectors'

// actions
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps'
import * as commonActions from '../../../../Common/Actions'
import * as commonChannelActions from '../../../Common/Actions'

// import { openingHoursExtraVisibility } from '../../../../Common/Enums'

class OpeningHours extends Component {

  onInputChange = input => value => {
    if (!this.props.isNew) {
      this.props.actions.onOpeningHoursInputChange(this.props.id, input, value, false)
    } else {
      this.props.onAddOpeningHours([{
        id: this.props.id,
        [input]: value
      }])
    }
  }

  onToggleAdditional = (id) => (value) => {
    if (!this.props.isNew) {
      this.props.actions.onDailyOpeningHoursInputChange(id, 'extra', value, false)
    } else {
      this.props.onAddOpeningHours([{
        id: this.props.id,
        dailyOpeningHours: [{ id: id, extra: value }]
      }])
    }
  };

  onDayChange = (id) => (e) => {
    if (!this.props.isNew) {
      this.props.actions.onDailyOpeningHoursInputChange(id, 'day', e.target.checked, false)
    } else {
      this.props.onAddOpeningHours([{
        id: this.props.id,
        dailyOpeningHours: [{ id: id, day: e.target.checked }]
      }])
    }
  };

  onNonstopChange = e => {
    const input = 'nonstop'
    if (!this.props.isNew) {
      this.props.actions.onOpeningHoursInputChange(this.props.id, input, e.target.checked, false)
    } else {
      this.props.onAddOpeningHours([{
        id: this.props.id,
        [input]: e.target.checked
      }])
    }
  };

  onTimeChange = (id) => (dateTimeType, value) => {
    const property = dateTimeType === 'start' ? 'timeFrom' : 'timeTo'
    const obj = { [property]: value, id, day: true }
    if (!this.props.isNew) {
      this.props.actions.onDailyOpeningHoursObjectChange(id, obj, false)
    } else {
      this.props.onAddOpeningHours([{
        id: this.props.id,
        dailyOpeningHours: [obj]
      }])
    }
  };

  onTimeChangeExtra = (id) => (dateTimeType, value) => {
    const property = dateTimeType === 'start' ? 'timeFromExtra' : 'timeToExtra'
    this.props.actions.onDailyOpeningHoursInputChange(id, property, value, false)
  };

  onDateChange = (dateTimeType, value) => {
    const property = dateTimeType === 'start' ? 'validFrom' : 'validTo'
    this.props.actions.onOpeningHoursInputChange(this.props.id, property, value, false)
  };

    // validators = [PTVValidatorTypes.IS_REQUIRED];

  render () {
    const {
      intl: { formatMessage },
      id,
      keyToState,
      validityType,
      nonstop,
      readOnly,
      openingHoursMessages,
      weekdays,
      openingHoursType,
      dateValidators,
      hoursValidators,
      valueToValidate
    } = this.props
    const startDateMsg = formatMessage(openingHoursMessages.startDate)
    const endDateMsg = formatMessage(openingHoursMessages.endDate)
    const startDateValidationMsg = openingHoursMessages.dayTimesDate_normal_start
    const endDateValidationMsg = openingHoursMessages.dayTimesDate_normal_end

    return (
      <div className={cx('opening-hours-item-row', this.props.componentClass)}>

        { !readOnly &&
          <div className='opening-hours-form'>

            <OpeningHoursAdditionalInfo
              id={id}
              label={formatMessage(openingHoursMessages.alternativeTitle)}
              readOnly={readOnly}
              onInputChange={this.onInputChange}
              openingHoursType={openingHoursType}
            />

            <OpeningHoursValidityTypes
              id={id}
              messages={openingHoursMessages}
              onInputChange={this.onInputChange}
            />

            { validityType &&
              <div className='row form-group'>
                <div className='col-xs-12'>
                  <DateTimeValidityInputs
                    start={this.props.validFrom}
                    end={this.props.validTo}
                    filterDateTime={{ start: this.props.validTo, end: this.props.validFrom }}
                    mode='date'
                    onChange={this.onDateChange}
                    readOnly={readOnly}
                    timeConstraints={{ minutes:{ step:30 } }}
                    showLabels={{ start: startDateMsg, end: endDateMsg }}
                    showSeparator={false}
                    componentClass='horizontal'
                    inputProps={{ className: 'ptv-textinput' }}
                    validatedFieldLabels={{ start: startDateValidationMsg, end: endDateValidationMsg }}
                    validators={dateValidators}
                  />
                </div>
              </div>
            }

            <OpeningHoursDailySet
              id={id}
              readOnly={readOnly}
              messages={openingHoursMessages}
              label={formatMessage(openingHoursMessages.openingHoursDailySetTitle)}
              validatedField={openingHoursMessages.openingHoursDailySetTitle}
              onNonstopChange={this.onNonstopChange}
              onTimeChange={this.onTimeChange}
              onTimeChangeExtra={this.onTimeChangeExtra}
              onToggleAdditional={this.onToggleAdditional}
              onDayChange={this.onDayChange}
              validators={hoursValidators}
              keyToState={keyToState}
              weekdays={weekdays}
              nonstop={nonstop}
              valueToValidate={valueToValidate}
            />

          </div>
        }

      </div>
    )
  }
}

OpeningHours.propTypes = {
  id: PropTypes.string.isRequired,
  validFrom: PropTypes.any,
  validTo: PropTypes.any,
  nonstop: PropTypes.bool,
  isNew: PropTypes.bool,
  intl: intlShape.isRequired,
  label: PropTypes.string,
  componentClass: PropTypes.string,
  readOnly: PropTypes.bool,
  actions: PropTypes.object.isRequired,
  onAddOpeningHours: PropTypes.func,
  openingHoursType: PropTypes.number
}

OpeningHours.defaultProps = {
  componentClass: ''
}

// maps state to props
function mapStateToProps (state, ownProps) {
  const isDateEmpty = CommonChannelSelectors.getIsOpeningHourDateEmpty(state, ownProps)
  const isValueValid = CommonChannelSelectors.getIsCurrentOpeningHourNormalValid(state, ownProps)
  const isValueEntered = CommonChannelSelectors.getIsAnyOpeningHoursNormal(state, ownProps)
  const validators = [PTVValidatorTypes.IS_REQUIRED]
  const dateValidators = isValueEntered && isDateEmpty ? validators : null
  const valueToValidate = isValueEntered && !isValueValid ? '' : 'valid'
  return {
    validityType: CommonChannelSelectors.getOpeningHourValidityType(state, ownProps),
    validFrom: CommonChannelSelectors.getOpeningHourValidFrom(state, ownProps),
    validTo: CommonChannelSelectors.getOpeningHourValidTo(state, ownProps),
    nonstop: CommonChannelSelectors.getOpeningHourNonstop(state, ownProps),
    weekdays: CommonChannelSelectors.getWeekdaysList(state),
    dateValidators,
    hoursValidators: [{
      ...PTVValidatorTypes.IS_REQUIRED,
      errorMessage: ownProps.openingHoursMessages.errorDayNotSelected
    }],
    valueToValidate
  }
}

const actions = [
  commonChannelActions,
  commonActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(OpeningHours))
