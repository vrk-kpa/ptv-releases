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
import React from 'react'
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { injectIntl } from 'react-intl';
import classNames from 'classnames';

// components
import * as PTVValidatorTypes from '../../../Components/PTVValidators';
import { PTVAutoComboBox, PTVTextInputNotEmpty, PTVUrlChecker } from '../../../Components';
import { ButtonDelete } from '../Buttons';
import Info from '../AdditionalInfoComponent'

// selectors
import * as CommonSelectors from '../Selectors';

// schemas
import { CommonSchemas } from '../../Common/Schemas';

// actions
import * as commonActions from '../Actions';
import mapDispatchToProps from '../../../Configuration/MapDispatchToProps';

export const LawContainer = ({readOnly, language, translationMode, id, actions, intl, count,
    componentClass, isFetching, areDataValid, messages,
    urlAddress, urlExists, shouldValidate, withName, name, isNew, onAddLaw, orderNumber}) => {

    const checkUrl = value => {
        actions.apiCall(['laws', id],
            { endpoint: 'url/CheckV2', data: { urlAddress: { [language]: value }, id: id, language: language } }, null, CommonSchemas.LAW);
    }

    const onInputChange = input => value => {
        if (!isNew) {
            // actions.onLocalizedEntityInputChange('laws', id, input, value, language);
          actions.onEntityObjectChange('laws', id, { [input]: { [language]: value } })
        } else {
            onAddLaw([{
                id: id,
                [input]: { [language]: value },
                orderNumber: orderNumber || 1
            }])
        }
    }

    const validators = [PTVValidatorTypes.IS_REQUIRED];

    const renderChecker = () =>{
        const { formatMessage } = intl;

        return  <PTVUrlChecker
                    componentClass = "url-checker"
                    messages= { { tooltip: messages.urlTooltip,
                        label: messages.urlLabel,
                        placeholder: messages.urlPlaceholder,
                        button: messages.urlButton,
                        checkerInfo: messages.urlCheckerInfo } }
                    id = { id }
                    value = { urlAddress }
                    checkUrlCallback = { checkUrl }
                    blurCallback = { onInputChange('urlAddress') }
                    showPreloader = { isFetching }
                    showMessage ={ areDataValid }
                    urlExists = { urlExists }
                    inputValidators = { shouldValidate || ( withName && name.length > 0 ) ? validators : [] }
                    readOnly= { readOnly && translationMode == "none" }
                    disabled= { translationMode == "view" }
                    translationMode = { translationMode }
		            maxLength= { 500 } />
    }

    const { formatMessage } = intl;

    return (
        <div className= { classNames("channel-webPage-container item-row", componentClass) }>
            <div className="row">
                { withName ?
                    <PTVTextInputNotEmpty
                        componentClass="col-xs-12"
                        label={ formatMessage(messages.nameTitle) }
                        tooltip={ formatMessage(messages.nameTooltip) }
                        value={ name }
                        blurCallback={ onInputChange('name') }
                        maxLength = { 500 }
                        name="lawName"
                        readOnly= { readOnly && translationMode == "none" }
                        disabled= { translationMode == "view" }/>
                : null }
            </div>

            { renderChecker() }

        </div>
    );
};

LawContainer.propTypes = {
    id: PropTypes.string.isRequired,
    messages: PropTypes.object.isRequired,
    readOnly: PropTypes.bool,
    shouldValidate: PropTypes.bool,
    isNew: PropTypes.bool,
    componentClass: PropTypes.string,
    count: PropTypes.number,
    withName: PropTypes.bool,
    url: PropTypes.bool,
    onAddLaw: PropTypes.func,
    customTypesSelector: PropTypes.func
};

LawContainer.defaultProps = {
  isNew: false,
  readOnly: false,
  shouldValidate: false,
  withName: false
}

function mapStateToProps(state, ownProps) {
  return {
      urlAddress: CommonSelectors.getLawUrlAddress(state, ownProps),
      name: CommonSelectors.getLawName(state, ownProps),
      urlExists: CommonSelectors.getLawUrlExists(state, ownProps),
      isFetching: CommonSelectors.getLawIsFetching(state, ownProps),
      areDataValid: CommonSelectors.getLawAreDataValid(state, ownProps),
  }
}

const actions = [
    commonActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(LawContainer));
