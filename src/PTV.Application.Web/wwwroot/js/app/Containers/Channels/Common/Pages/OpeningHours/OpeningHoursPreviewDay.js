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
import React, { Component, PropTypes } from 'react';
import {connect} from 'react-redux';
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl';
import ImmutablePropTypes from 'react-immutable-proptypes';
import { PTVLabel } from '../../../../../Components';
import DateTimeValidityInputs from '../../../../Common/DateTimeValidityInputs';
import { ButtonDelete } from '../../../../Common/Buttons';
import * as CommonChannelSelectors from '../../../Common/Selectors';
import classNames from 'classnames';
import styles from './styles.scss';
import { openingHoursExtraVisibility } from '../../../../Common/Enums';

const DayLabel = ({weekdayLabel}) => (<div className="day-label">{ weekdayLabel }</div>);
const Hours = ({hoursValid, timeFormat, timeFrom, timeTo }) => {
        
      if (hoursValid){
          return (<div className="hours">{`${timeFormat(timeFrom)} - ${timeFormat(timeTo)}`}</div>);
      }

      return null;
};

const ExtraHours = ({extra, hoursValid, timeFormat, timeFromExtra, timeToExtra}) => {
  const extraHoursValid = hoursValid && timeFromExtra && timeToExtra;
   if ((extra === openingHoursExtraVisibility.show) && extraHoursValid ){
      return (
            <div className="extra-hours">
                {`${timeFormat(timeFromExtra)} - ${timeFormat(timeToExtra)}`}
            </div>
      );
   }
   return null;
};

export const OpeningHoursPreviewDay = (props) => {
    const hoursValid = props.timeFrom && props.timeTo;
    
    // console.log('OHPD: ', props);  
    if (!props.day){
      return null;
    }
    return (
        <div className="opening-hours-daily clearfix">
            <div className="float-children">
                <DayLabel weekdayLabel = {props.weekdayLabel} />
                <Hours 
                  timeFormat = { props.timeFormat } 
                  hoursValid = { hoursValid }
                  timeFrom = {props.timeFrom } 
                  timeTo = { props.timeTo } />
                <ExtraHours 
                  extra={ props.extra } 
                  hoursValid = { hoursValid } 
                  timeFormat = { props.timeFormat } 
                  timeFromExtra = { props.timeFromExtra }
                  timeToExtra = { props.timeToExtra } />
            </div>
        </div>
    );

}

OpeningHoursPreviewDay.propTypes = {
  timeFormat: PropTypes.func,
  weekdayLabel: PropTypes.string
}

// maps state to props
function mapStateToProps(state, ownProps) {
    return {
        day: CommonChannelSelectors.getDailyOpeningHourDay(state, ownProps),
        timeFrom: CommonChannelSelectors.getDailyOpeningHourTimeFrom(state, ownProps),
        timeFromExtra: CommonChannelSelectors.getDailyOpeningHourTimeFromExtra(state, ownProps),
        timeTo: CommonChannelSelectors.getDailyOpeningHourTimeTo(state, ownProps),
        timeToExtra: CommonChannelSelectors.getDailyOpeningHourTimeToExtra(state, ownProps),
        extra: CommonChannelSelectors.getDailyOpeningHourExtra(state, ownProps)
    }
}

export default connect(mapStateToProps)(OpeningHoursPreviewDay);
