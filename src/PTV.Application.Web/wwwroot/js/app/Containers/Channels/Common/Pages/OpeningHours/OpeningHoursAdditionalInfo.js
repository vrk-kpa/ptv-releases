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

import { PTVLabel, PTVTextInputNotEmpty } from '../../../../../Components';
import classNames from 'classnames';
import styles from './styles.scss';

import * as CommonChannelSelectors from '../../../Common/Selectors';
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps';
import * as commonChannelActions from '../../../Common/Actions';

import { openingHoursTypes } from '../../../../Common/Enums';
import { openingHoursMessages } from '../../Messages';

const defaultMessages = {
    [openingHoursTypes.exceptional]: 'openingHoursExceptional',
    [openingHoursTypes.special]: 'openingHoursSpecial',
    [openingHoursTypes.normal]: 'openingHoursNormal',
}

export const OpeningHoursAdditionalInfo = ({ intl: { formatMessage }, label, readOnly, language, alterTitle, actions, id, onInputChange, openingHoursType } = props) => {
    const test = openingHoursType;
    const placeHolder = alterTitle ?
            alterTitle
        : openingHoursType === openingHoursTypes.exceptional ?
            ''
        : formatMessage(openingHoursMessages['defaultTitle_' + defaultMessages[openingHoursType]]);

    return (
        <div className="row form-group">
            <div className="col-md-2">
                <PTVLabel readOnly = {readOnly} >
                    <strong>{ label }</strong>
                </PTVLabel>
            </div>
            <div className="col-md-9">
                <PTVTextInputNotEmpty
                    value = { placeHolder }
                    name = "OpeningHoursAdditionalInfo"
                    blurCallback = { onInputChange("alterTitle") }
                    maxLength = { 100 }
                    readOnly = { readOnly } />
            </div>
        </div>
    );
}

OpeningHoursAdditionalInfo.propTypes = {
    id: PropTypes.string,
    label: PropTypes.string,
    readOnly: PropTypes.bool,
    onInputChange: PropTypes.func,
    openingHoursType: PropTypes.number
}

// maps state to props
function mapStateToProps(state, ownProps) {
    return {
        alterTitle: CommonChannelSelectors.getOpeningHourAlterTitle(state, ownProps)
    }
}

const actions = [
    commonChannelActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(OpeningHoursAdditionalInfo));
