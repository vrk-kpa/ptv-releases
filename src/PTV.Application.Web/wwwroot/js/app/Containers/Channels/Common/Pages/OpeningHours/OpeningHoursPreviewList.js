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
import OpeningHoursPreview from './OpeningHoursPreview';
import * as CommonChannelSelectors from '../../../Common/Selectors';
import cx from 'classnames';
import styles from './styles.scss';
import { openingHoursTypes } from '../../../../Common/Enums';
import { openingHoursMessages } from '../../Messages';

const defaultMessages = {
    [openingHoursTypes.exceptional]: 'openingHoursExceptional',
    [openingHoursTypes.special]: 'openingHoursSpecial',
    [openingHoursTypes.normal]: 'openingHoursNormal',
}
export const OpeningHoursPreviewList = ({ intl: {formatMessage}, openingHoursType, openingHoursId, readOnly,
    ids, activeHours, language, componentSectionClass, componentSectionItemClass } = props) => {

    const renderDefaultPreview = (type) => {
        return (
            <div className={componentSectionClass}>
                <PTVLabel labelClass="alter-title">
                    { formatMessage(openingHoursMessages['defaultTitle_' + defaultMessages[type]]) }
                </PTVLabel>
                <p>{ formatMessage(openingHoursMessages['previewInstructions1_' + defaultMessages[type]]) }</p>
                <p>{ formatMessage(openingHoursMessages['previewInstructions2_' + defaultMessages[type]]) }</p>
            </div>
        )
    }

    const renderPreview = (ids, type) => {
        return ( ids.map((id, index) => {
            return (
                <OpeningHoursPreview
                    key = { id + index }
                    id = { id }
                    openingHoursId = { id }
                    openingHoursType = { type }
                    readOnly = { readOnly }
                    componentClass = { componentSectionItemClass }
                    defaultMessages = { defaultMessages }
                    language={language}
                />
            )
        }) )
    }
    return (
        activeHours === -1 && !readOnly ?
            null
        : ids.size === 0 ?
            activeHours === openingHoursType && !readOnly ?
                renderDefaultPreview(openingHoursType)
            : null
        :
            <div className={componentSectionClass}>
                { renderPreview(ids, openingHoursType) }
            </div>
    );
}

OpeningHoursPreviewList.propTypes = {
    componentSectionClass: PropTypes.string,
    componentSectionItemClass: PropTypes.string,
    ids: PropTypes.object,
    openingHoursType: PropTypes.number,
    activeHours: PropTypes.number
}

export default injectIntl(OpeningHoursPreviewList);
