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
import classNames from 'classnames';
import { validityTypesEnum } from './../../../../Common/Enums';

// components
import * as PTVValidatorTypes from '../../../../../Components/PTVValidators';
import { PTVCheckBox, PTVRadioGroup } from '../../../../../Components';
import OpeningHoursPreview from './OpeningHoursPreview';
import OpeningHoursAdditionalInfo from './OpeningHoursAdditionalInfo';
import OpeningHoursDayTimes from './OpeningHoursDayTimes';
import { ButtonDelete } from '../../../../Common/Buttons';
import styles from './styles.scss';

// selectors
import * as CommonChannelSelectors from '../../Selectors';

// actions
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps';
import * as commonActions from '../../../../Common/Actions';
import * as commonChannelActions from '../../../Common/Actions';

class OpeningHoursExceptional extends Component {

    constructor(props) {
        super(props);
    }

    onInputChange = input => value => {
        console.log('onInputChange SOH', this.props.isNew, this.props.id, value);
        if (!this.props.isNew) {
            this.props.actions.onOpeningHoursInputChange(this.props.id, input, value, false);
        } else {
            this.props.onAddOpeningHours([{
                id: this.props.id,
                [input]: value
            }])
        }
    }

    onClosedChange = (e) => {
        this.props.actions.onOpeningHoursInputChange(this.props.id, 'closed', e.target.checked, false);
    };

    render () {
        const { intl: {formatMessage}, id, openingHours, openingHoursType, validityTypeExceptional, closed, componentClass, isNew, onAddOpeningHours, validFrom, validTo, timeFrom, timeTo, isFirst, readOnly, openingHoursMessages, previewClass} = this.props;
        const validityTypesExceptional = [
            { id: false, name: formatMessage(openingHoursMessages.validDaySingle) },
            { id: true, name: formatMessage(openingHoursMessages.validDayRange) }
        ];

        return (
            <div className= { classNames("opening-hours-item-row", componentClass) }>
                
                { !readOnly ?
                    <div className="opening-hours-form">

                        <OpeningHoursAdditionalInfo
                            id = { id } 
                            label = { formatMessage(openingHoursMessages.additionalInformation) }
                            readOnly = { readOnly }
                            onInputChange = { this.onInputChange }
                            openingHoursType = { openingHoursType }
                        />
                                        
                        <div className="row form-group">
                            <div className="col-xs-12">
                                <PTVRadioGroup
                                    name = "OpeningHoursExceptionalValidityType"
                                    value = { validityTypeExceptional }
                                    onChange = { this.onInputChange("isDateRange") }
                                    items = { validityTypesExceptional }
                                />
                            </div>
                        </div>

                        <div className="row form-group">
                            <div className="col-sm-6 col-md-12 col-lg-6">
                                <OpeningHoursDayTimes
                                    id = { id }
                                    type = "start"
                                    filter = { validTo }
                                    label = { formatMessage(openingHoursMessages.startDate) }
                                    time = { timeFrom }
                                    date = { validFrom }
                                    readOnly = { readOnly }
                                    onInputChange = { this.onInputChange }
                                    openingHoursType = { openingHoursType }
                                    isClosed = { closed }
                                />
                            </div>
                            <div className="col-sm-6 col-md-12 col-lg-6">
                                <OpeningHoursDayTimes
                                    id = { id }
                                    type = "end"
                                    filter = { validFrom }
                                    label = { formatMessage(openingHoursMessages.endDate) }
                                    time = { timeTo }
                                    date = { validTo }
                                    readOnly = { readOnly }
                                    onInputChange = { this.onInputChange }
                                    openingHoursType = { openingHoursType }
                                    isClosed = { closed }
                                />
                            </div>
                        </div>

                        <div className="row form-group">
                            <div className="col-md-offset-2 col-md-10">
                                <PTVCheckBox
                                    //openingHoursId = { id }
                                    id = { id }
                                    onClick = { this.onClosedChange }
                                    isSelectedSelector = { CommonChannelSelectors.getOpeningHourClosed }
                                >
                                    { validityTypeExceptional ? formatMessage(openingHoursMessages.closedDayRange) : formatMessage(openingHoursMessages.closedDaySingle) }
                                </PTVCheckBox>
                            </div>                
                        </div>
                    </div>
                : null }

            </div>
        );
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
    closed: PropTypes.bool,
    isFirst: PropTypes.bool
}

OpeningHoursExceptional.defaultProps = {
    componentClass: ''
}

/// maps state to props
function mapStateToProps(state, ownProps) {
  return {
      validityTypeExceptional: CommonChannelSelectors.getOpeningHourValidityType(state, ownProps),
      validFrom: CommonChannelSelectors.getOpeningHourValidFrom(state, ownProps),
      validTo: CommonChannelSelectors.getOpeningHourValidTo(state, ownProps),
      timeFrom: CommonChannelSelectors.getOpeningHourTimeFrom(state, ownProps),
      timeTo: CommonChannelSelectors.getOpeningHourTimeTo(state, ownProps),
      closed: CommonChannelSelectors.getOpeningHourClosed(state, ownProps),
  }
}

const actions = [
    commonChannelActions,
    commonActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(OpeningHoursExceptional));
