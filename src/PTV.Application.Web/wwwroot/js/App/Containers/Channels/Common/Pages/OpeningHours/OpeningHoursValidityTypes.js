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
import { connect } from 'react-redux';
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl';

import { PTVRadioGroup } from '../../../../../Components';
import classNames from 'classnames';
import styles from './styles.scss';

import * as CommonChannelSelectors from '../../../Common/Selectors';
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps';
import * as commonChannelActions from '../../../Common/Actions';

export const OpeningHoursValidityTypes = ({ intl: {formatMessage}, validityType, items, actions, id, onInputChange, messages } = props) => {
    
    const validityTypes = [
        { id: false, name: formatMessage(messages.validOnward) },
        { id: true, name: formatMessage(messages.validPeriod) }
    ];

    return (
        <div className="row form-group">
            <div className="col-xs-12">
                <PTVRadioGroup
                    name = "OpeningHoursValidityType"
                    value = { validityType }
                    onChange = { onInputChange("isDateRange") }
                    items = { validityTypes }
                />
            </div>
        </div>
    );
}

OpeningHoursValidityTypes.propTypes = {
    id: PropTypes.string,
    items: PropTypes.array,
    onInputChange: PropTypes.func
}

// maps state to props
function mapStateToProps(state, ownProps) {
    return {
        validityType: CommonChannelSelectors.getOpeningHourValidityType(state, ownProps)
    }
}

const actions = [
    commonChannelActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(OpeningHoursValidityTypes));        