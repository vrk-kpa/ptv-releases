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
import OpeningHoursPreview from './OpeningHoursPreview';
import OpeningHoursAdditionalInfo from './OpeningHoursAdditionalInfo';
import OpeningHoursValidityTypes from './OpeningHoursValidityTypes';
import OpeningHoursDayTimes from './OpeningHoursDayTimes';
import styles from './styles.scss';

// selectors
import * as CommonChannelSelectors from '../../Selectors';

// actions
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps';
import * as commonActions from '../../../../Common/Actions';
import * as commonChannelActions from '../../../Common/Actions';

class OpeningHoursSpecial extends Component {

    constructor(props) {
        super(props);
    }

    onInputChange = input => value => {
        if (!this.props.isNew) {
            this.props.actions.onOpeningHoursInputChange(this.props.id, input, value, false);
        } else {
            this.props.onAddOpeningHours([{
                id: this.props.id,
                [input]: value
            }])
        }
    }

    validators = [PTVValidatorTypes.IS_REQUIRED];

    render () {
        const { intl: {formatMessage}, id, openingHours, componentClass, isNew, onAddOpeningHours, validFrom, validTo, dayFrom, dayTo, timeFrom, timeTo, readOnly, isFirst, openingHoursMessages, openingHoursType, previewClass, isNotempty} = this.props;
        const validators = isNotempty ? this.validators : null; 
        return (
            <div className= { classNames("opening-hours-item-row", componentClass) }>
                
                { !readOnly ?
                    <div className="opening-hours-form">
                        <OpeningHoursAdditionalInfo
                            id = { id } 
                            label = { formatMessage(openingHoursMessages.alternativeTitle) }
                            readOnly = { readOnly }
                            onInputChange = { this.onInputChange }
                            openingHoursType = { openingHoursType }
                        />                
                        <OpeningHoursValidityTypes 
                            id = { id }
                            messages = { openingHoursMessages }
                            onInputChange = { this.onInputChange }
                        />
                        <div className="row form-group">
                            <div className="col-sm-6 col-md-12 col-lg-6">
                                <OpeningHoursDayTimes
                                    id = { id }
                                    type = "start"
                                    filter = { validTo }
                                    label = { formatMessage(openingHoursMessages.startDate) }
                                    day = { dayFrom }
                                    time = { timeFrom }
                                    date = { validFrom }
                                    readOnly = { readOnly }
                                    openingHoursType = { openingHoursType }
                                    onInputChange = { this.onInputChange }
                                    validators = { validators }
                                />
                            </div>
                            <div className="col-sm-6 col-md-12 col-lg-6">
                                <OpeningHoursDayTimes
                                    id = { id }
                                    type = "end"
                                    filter = { validFrom }
                                    label = { formatMessage(openingHoursMessages.endDate) }
                                    day = { dayTo }
                                    time = { timeTo }
                                    date = { validTo }
                                    readOnly = { readOnly }
                                    openingHoursType = { openingHoursType }
                                    onInputChange = { this.onInputChange }
                                    validators = { validators }
                                />
                            </div>
                        </div>
                    </div>
                : null }

            </div>
        );
    }
}

OpeningHoursSpecial.propTypes = {
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
    isFirst: PropTypes.bool
}

OpeningHoursSpecial.defaultProps = {
    componentClass: ''
}

/// maps state to props
function mapStateToProps(state, ownProps) {
  return {
      isNotempty: CommonChannelSelectors.getIsOpeningHourSpecialNotEmpty(state, ownProps),
      validFrom: CommonChannelSelectors.getOpeningHourValidFrom(state, ownProps),
      validTo: CommonChannelSelectors.getOpeningHourValidTo(state, ownProps),
      dayFrom: CommonChannelSelectors.getOpeningHourDayFrom(state, ownProps),
      dayTo: CommonChannelSelectors.getOpeningHourDayTo(state, ownProps),
      timeFrom: CommonChannelSelectors.getOpeningHourTimeFrom(state, ownProps),
      timeTo: CommonChannelSelectors.getOpeningHourTimeTo(state, ownProps)
  }
}

const actions = [
    commonChannelActions,
    commonActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(OpeningHoursSpecial));
