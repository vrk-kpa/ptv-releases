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
import { PTVCheckBox, PTVLabel, PTVIcon } from '../../../../../Components';
import DateTimeValidityInputs from '../../../../Common/DateTimeValidityInputs';
import { ButtonDelete } from '../../../../Common/Buttons';
import * as CommonChannelSelectors from '../../../Common/Selectors';
import classNames from 'classnames';
import styles from './styles.scss';
import moment from 'moment';
import * as PTVValidatorTypes from '../../../../../Components/PTVValidators';

import { openingHoursExtraVisibility } from '../../../../Common/Enums';
import { weekdaysEnum } from '../../../../Common/Enums';
import { openingHoursMessages } from '../../Messages';

const selectors = {
        timestart: CommonChannelSelectors.getDailyOpeningHourTimeFrom,
        timeend: CommonChannelSelectors.getDailyOpeningHourTimeTo,
        timestartIsDefault: CommonChannelSelectors.getIsDefaultDailyOpeningHourTimeFrom,
        timeendIsDefault: CommonChannelSelectors.getIsDefaultDailyOpeningHourTimeTo,
        
        timestartExtra: CommonChannelSelectors.getDailyOpeningHourTimeFromExtra,
        timeendExtra: CommonChannelSelectors.getDailyOpeningHourTimeToExtra,
        timestartExtraIsDefault: CommonChannelSelectors.getIsDefaultDailyOpeningHourTimeFromExtra,
        timeendExtraIsDefault: CommonChannelSelectors.getIsDefaultDailyOpeningHourTimeToExtra,
}

export const OpeningHoursDaily = (props) => {
    const  { id, extra, intl: {formatMessage}, disabled, readOnly, onDayChange, onTimeChange, timeFrom, timeTo, isSetTimeFrom, isSetTimeTo, 
                                     onToggleAdditional, timeFromExtra, timeToExtra, isSetTimeFromExtra, isSetTimeToExtra, onTimeChangeExtra, showAddAdditionalIcon, weekdayId  } = props;


    const getTimeSelectorFor = (type, isValue) => {
        const isDefaultSuffix = isValue ? '' : 'IsDefault';
        return (state) => selectors[`time${type}${isDefaultSuffix}`](state, props) 
    }

    const getTimeExtraSelectorFor = (type, isValue) => {
        const isDefaultSuffix = isValue ? '' : 'IsDefault';
        return (state) => selectors[`time${type}Extra${isDefaultSuffix}`](state, props) 
    }

    const getTresholdSelector = (type) => {
        return type === 'end' ? selectors.timestart : null;
    }

    const getTresholdExtraSelector = (type) => {
        return type === 'end' ? selectors.timestartExtra : selectors.timeend;
    }

    const validators = [PTVValidatorTypes.IS_REQUIRED];

    return (
        <div className="opening-hours-daily-container">
            <div className="row">
                <div className="col-sm-4 col-lg-3">
                    <PTVCheckBox
                        id={ id }
                        openingHoursId = { props.openingHoursId }
                        isSelectedSelector={ CommonChannelSelectors.getDailyOpeningHourDay }
                        isDisabled={ disabled }
                        onClick={ onDayChange } >
                        { formatMessage(openingHoursMessages[`weekday_${weekdayId}`]) }
                    </PTVCheckBox>
                </div>
                <div className="col-sm-8 col-lg-9">
                    <div className="opening-hours-daily">
                        <DateTimeValidityInputs
                            id={ id }
                            openingHoursId = { props.openingHoursId }
                            showAsDropdown
                            getSelectorFor = { getTimeSelectorFor }
                            getTresholdSelector = { getTresholdSelector }
                            disabled={ disabled }
                            mode="timeOnly"
                            onChange={ onTimeChange }
                            readOnly={ readOnly }
                            componentClass="horizontal"
                        />
                        { showAddAdditionalIcon ?
                            extra !== openingHoursExtraVisibility.show ?
                                <PTVIcon
                                    name={"icon-plus"}
                                    onClick={ () => onToggleAdditional(openingHoursExtraVisibility.show) }
                                />
                            :
                                <div className="opening-hours-daily-extra">
                                    <div className="opening-hours-connector">
                                        ja
                                    </div>
                                    <DateTimeValidityInputs
                                        id = { id }
                                        openingHoursId = { props.openingHoursId }
                                        showAsDropdown
                                        getSelectorFor = { getTimeExtraSelectorFor }
                                        getTresholdSelector = { getTresholdExtraSelector }
                                        disabled={ disabled }
                                        mode="timeOnly"
                                        onChange={ onTimeChangeExtra }
                                        readOnly={ readOnly }
                                        componentClass="horizontal"
                                        validators = { validators }                                        
                                    />
                                    <PTVIcon
                                        name={"icon-close"}
                                        onClick={ () => onToggleAdditional(openingHoursExtraVisibility.hide) }
                                    />
                                </div> 
                        : null }
                    </div>
                </div>
            </div>
        </div>
    );

}

OpeningHoursDaily.propTypes = {
  data: PropTypes.any,
  onToggleAdditional: PropTypes.func,
  onTimeChange: PropTypes.func,
  onTimeChangeExtra: PropTypes.func,
  onDayChange: PropTypes.func,
  disabled: PropTypes.bool,
  timeFrom: PropTypes.any,
  timeFromExtra: PropTypes.any,  
  timeTo: PropTypes.any,
  timeToExtra: PropTypes.any,
  defaultId: PropTypes.string,
  extra: PropTypes.string
}

OpeningHoursDaily.defaultProps = {
  defaultId: ''
}

// maps state to props
function mapStateToProps(state, ownProps) {
    return {
        showAddAdditionalIcon: CommonChannelSelectors.getDailyOpeningHourDay(state, ownProps),
        extra: CommonChannelSelectors.getDailyOpeningHourExtra(state, ownProps),
    }
}

export default connect(mapStateToProps)(injectIntl(OpeningHoursDaily));
