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
import classNames from 'classnames'

// components
import { PTVCheckBox, PTVRadioGroup } from '../../../../../Components'
import OpeningHoursAdditionalInfo from './OpeningHoursAdditionalInfo'
import OpeningHoursDayTimes from './OpeningHoursDayTimes'
import './styles.scss'

// selectors
import * as CommonChannelSelectors from '../../Selectors'

// actions
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps'
import * as commonActions from '../../../../Common/Actions'
import * as commonChannelActions from '../../../Common/Actions'

class OpeningHoursExceptional extends Component {

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

  onClosedChange = (e) => {
    if (!this.props.isNew) {
      this.props.actions.onOpeningHoursInputChange(this.props.id, 'closed', e.target.checked, false)
    } else {
      this.props.onAddOpeningHours([{
        id: this.props.id,
        'closed': e.target.checked
      }])
    }
        // this.props.actions.onOpeningHoursInputChange(this.props.id, 'closed', e.target.checked, false);
  };

    // validators = [PTVValidatorTypes.IS_REQUIRED];

  render () {
    const {
    intl: { formatMessage },
    id,
    keyToState,
    openingHoursType,
    isDateRange,
    closed,
    componentClass,
    validFrom,
    validTo,
    timeFrom,
    timeTo,
    readOnly,
    openingHoursMessages,
  } = this.props
    const validityTypesExceptional = [
            { id: false, name: formatMessage(openingHoursMessages.validDaySingle) },
            { id: true, name: formatMessage(openingHoursMessages.validDayRange) }
    ]
        // const validators = isEmpty ? this.validators : null;

    return (
      <div className={classNames('opening-hours-item-row', componentClass)}>

        { !readOnly &&
          <div className='opening-hours-form'>

            <OpeningHoursAdditionalInfo
              id={id}
              label={formatMessage(openingHoursMessages.additionalInformation)}
              readOnly={readOnly}
              onInputChange={this.onInputChange}
              openingHoursType={openingHoursType}
            />

            <div className='row form-group'>
              <div className='col-xs-12'>
                <PTVRadioGroup
                  name='OpeningHoursExceptionalValidityType'
                  value={isDateRange}
                  onChange={this.onInputChange('isDateRange')}
                  items={validityTypesExceptional}
                />
              </div>
            </div>

            <div className='row form-group'>
              <div className='col-sm-6 col-md-12 col-lg-6'>
                <OpeningHoursDayTimes
                  id={id}
                  type='start'
                  filter={validTo}
                  label={formatMessage(openingHoursMessages.startDate)}
                  time={timeFrom}
                  date={validFrom}
                  readOnly={readOnly}
                  onInputChange={this.onInputChange}
                  openingHoursType={openingHoursType}
                  closed={closed}
                  keyToState={keyToState}
                />
              </div>
              <div className='col-sm-6 col-md-12 col-lg-6'>
                <OpeningHoursDayTimes
                  id={id}
                  type='end'
                  filter={validFrom}
                  label={formatMessage(openingHoursMessages.endDate)}
                  time={timeTo}
                  date={validTo}
                  readOnly={readOnly}
                  onInputChange={this.onInputChange}
                  openingHoursType={openingHoursType}
                  closed={closed}
                  keyToState={keyToState}
                />
              </div>
            </div>

            <div className='row form-group'>
              <div className='col-md-offset-2 col-md-10'>
                <PTVCheckBox
                  id={id}
                  onClick={this.onClosedChange}
                  isSelectedSelector={CommonChannelSelectors.getOpeningHourClosed}
                >
                  { isDateRange
                    ? formatMessage(openingHoursMessages.closedDayRange)
                    : formatMessage(openingHoursMessages.closedDaySingle)
                  }
                </PTVCheckBox>
              </div>
            </div>
          </div>
        }

      </div>
    )
  }
}

OpeningHoursExceptional.propTypes = {
  isNew: PropTypes.bool,
  onAddOpeningHours: PropTypes.func,
  intl: intlShape.isRequired,
  label: PropTypes.string,
  componentClass: PropTypes.string,
  readOnly: PropTypes.bool,
  actions: PropTypes.object.isRequired,
  id: PropTypes.string.isRequired,
  timeFrom: PropTypes.number,
  timeTo: PropTypes.number,
  dayFrom: PropTypes.string,
  dayTo: PropTypes.string,
  validFrom: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.number
  ]),
  validTo: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.number
  ]),
  openingHoursType: PropTypes.number,
  closed: PropTypes.bool
}

OpeningHoursExceptional.defaultProps = {
  componentClass: ''
}

// / maps state to props
function mapStateToProps (state, ownProps) {
  return {
    isDateRange: CommonChannelSelectors.getOpeningHourValidityType(state, ownProps),
    validFrom: CommonChannelSelectors.getOpeningHourValidFrom(state, ownProps),
    validTo: CommonChannelSelectors.getOpeningHourValidTo(state, ownProps),
    timeFrom: CommonChannelSelectors.getOpeningHourTimeFrom(state, ownProps),
    timeTo: CommonChannelSelectors.getOpeningHourTimeTo(state, ownProps),
    closed: CommonChannelSelectors.getOpeningHourClosed(state, ownProps)
  }
}

const actions = [
  commonChannelActions,
  commonActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(OpeningHoursExceptional))
