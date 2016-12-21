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
import { PTVLabel } from '../../../../../Components';
import OpeningHoursPreviewDay from './OpeningHoursPreviewDay';
import * as CommonChannelSelectors from '../../../Common/Selectors';
import cx from 'classnames';
import styles from './styles.scss';
import moment from 'moment';
import { openingHoursTypes } from '../../../../Common/Enums';
import { openingHoursMessages } from '../../Messages';

export const OpeningHoursPreview = ({ intl: {formatMessage}, weekdays, openingHoursType, openingHoursId, validity, alterTitle, defaultMessages,
        nonstop, closed, timeFrom, timeTo, dayFrom, dayTo, validFrom, validTo, validityType, isFirst, readOnly, componentClass } = props) => {

    const yearFormat = (d) => moment(d).format('YYYY');
    const dateFormat = (d) => moment(d).format('DD.MM.'); 
    const fullDateFormat = (d) => moment(d).format('DD.MM.YYYY');
    const timeFormat = (d) => moment.utc(d).format('HH.mm');
    const dayFormat = (day) => formatMessage(openingHoursMessages[`weekday_short_${day}`]).substring(0, 2);
    const textFormat = (arr) => Array.isArray(arr) ? arr.join(" ").replace(/\s+/g, " ").trim() : "";
    
    const formatDateRange = (dateFrom, dateTo) => {
        // return full date when years are different
        if (yearFormat(dateFrom) !== yearFormat(dateTo)) {
            return fullDateFormat(dateFrom) + " - " + fullDateFormat(dateTo) + " ";
        }

        return dateFormat(dateFrom) + " - " + fullDateFormat(dateTo) + " ";
    }

    const renderPreview = (type) => {
        switch(type) {
            case openingHoursTypes.special:
                const dateSpecial = validFrom && validTo && validityType ? formatDateRange(validFrom, validTo) : "";
                const daySpecial = (dayFrom || dayFrom === 0) && (dayTo || dayTo === 0) ? dayFormat(dayFrom) + " - " + dayFormat(dayTo) : "";
                const timeSpecial = timeFrom && timeTo ? timeFormat(timeFrom) + " - " + timeFormat(timeTo) : "";
                
                const specialBody = textFormat([daySpecial, timeSpecial]); 

                return (
                    <div className={componentClass}>
                        <div>
                            <div>{ dateSpecial }
                                <PTVLabel labelClass="alter-title">
                                    { alterTitle ? alterTitle : formatMessage(openingHoursMessages['defaultTitle_' + defaultMessages[openingHoursType]]) }
                                </PTVLabel>
                            </div>
                        </div>
                        <div>
                            { specialBody }
                        </div>
                    </div>
                );
            case openingHoursTypes.exceptional:
                const dateExceptional = validFrom && validTo ? (validityType ? formatDateRange(validFrom, validTo) : fullDateFormat(validFrom)) : validFrom ? fullDateFormat(validFrom) : "";
                const timeExceptional = !closed && timeFrom && timeTo ? timeFormat(timeFrom) + " - " + timeFormat(timeTo) : "";
                const openClosedMessage = closed ? formatMessage(openingHoursMessages.closedMessage) : '';
                const reasonMessage = alterTitle ? alterTitle : "";
                
                const exceptionalBody = textFormat([openClosedMessage, dateExceptional, timeExceptional, reasonMessage]); 

                return (
                    <div className={componentClass}>
                        <div>
                            <PTVLabel labelClass="alter-title">
                                { formatMessage(openingHoursMessages['defaultTitle_' + defaultMessages[openingHoursType]]) }
                            </PTVLabel>
                        </div> 
                        <div className="opening-hours-preview-body">
                            { exceptionalBody }
                        </div>
                    </div>
                );
            default:
                const dateDefault = validFrom && validTo && validityType ? formatDateRange(validFrom, validTo) : "";
                const titleDefault = alterTitle ? alterTitle : formatMessage(openingHoursMessages['defaultTitle_' + defaultMessages[openingHoursType]]);

                return (
                    <div className={componentClass}>
                        <div>{ dateDefault }
                            <PTVLabel labelClass="alter-title">
                                { alterTitle ? alterTitle : formatMessage(openingHoursMessages['defaultTitle_' + defaultMessages[openingHoursType]]) }
                            </PTVLabel>
                        </div>
                    <div>
                        { nonstop ? formatMessage(openingHoursMessages.nonstopOpeningHours)
                        : weekdays.map((weekday) => {
                                return (
                                    <OpeningHoursPreviewDay
                                        key = { CommonChannelSelectors.getDailyId(openingHoursId, weekday) }
                                        id = { CommonChannelSelectors.getDailyId(openingHoursId, weekday) }
                                        openingHoursId = { openingHoursId }
                                        weekdayLabel = { formatMessage(openingHoursMessages[`weekday_short_${weekday}`]) }
                                        timeFormat = {timeFormat}
                                    />
                                )
                            })
                        }
                        </div>
                    </div>
                );
        }
    }

    return renderPreview(openingHoursType);
}

OpeningHoursPreview.propTypes = {
    openingHoursId: PropTypes.string,
    openingHoursMessages: PropTypes.any,
    openingHoursType: PropTypes.number,
    weekdays: PropTypes.object,
    isFirst: PropTypes.bool,
    componentClass: PropTypes.string,
    defaultMessages: PropTypes.object
}

// maps state to props
function mapStateToProps(state, ownProps) {
    return {
        alterTitle: CommonChannelSelectors.getOpeningHourAlterTitle(state, ownProps),
        weekdays: CommonChannelSelectors.getWeekdaysList(state),
        nonstop: CommonChannelSelectors.getOpeningHourNonstop(state, ownProps),
        closed: CommonChannelSelectors.getOpeningHourClosed(state, ownProps),
        timeFrom: CommonChannelSelectors.getOpeningHourTimeFrom(state, ownProps),
        timeTo: CommonChannelSelectors.getOpeningHourTimeTo(state, ownProps),
        dayFrom: CommonChannelSelectors.getOpeningHourDayFrom(state, ownProps),
        dayTo: CommonChannelSelectors.getOpeningHourDayTo(state, ownProps),
        validFrom: CommonChannelSelectors.getOpeningHourValidFrom(state, ownProps),
        validTo: CommonChannelSelectors.getOpeningHourValidTo(state, ownProps),
        validityType: CommonChannelSelectors.getOpeningHourValidityType(state, ownProps)
    }
}

export default connect(mapStateToProps)(injectIntl(OpeningHoursPreview));
