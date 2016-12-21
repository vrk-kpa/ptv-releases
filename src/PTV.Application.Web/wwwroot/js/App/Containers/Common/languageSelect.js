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
import React, { PropTypes } from 'react';
import { injectIntl, defineMessages } from 'react-intl';
import {connect} from 'react-redux';

import { getTranslationLanguagesObjectArray, getTranslationLanguageId, getLanguageTo } from './Selectors';
import mapDispatchToProps from '../../Configuration/MapDispatchToProps';
import * as commonActions from './Actions';

import { PTVAutoComboBox } from '../../Components';

const messages = defineMessages({
    languageSelectionTitle: {    
        id: "Containers.Common.PageContainer.LanguageSelect.Title",
        defaultMessage: "Kielivalinta"
    },
    languageSelectionTootltip: {    
        id: "Containers.Common.PageContainer.LanguageSelect.Tooltip",
        defaultMessage: "Haku kohdentaa vain yhtä kieltä kohden. Jos haet palvelua ruotsin tai englannin kielisen nimen mukaan, valitse haluamasi kieli pudotusvalikosta."
    },
});

const LanguageSelect = ({ intl: { formatMessage }, keyToState, componentClass, actions, labelClass, autoClass, iconAction, label, tooltip, languages, selectedLanguage, changeCallback, languageProperty }) => {
    const handleLanguageToChange = (id) =>{
        actions.setLanguageTo(keyToState,id)
    }
    
    return (
        <PTVAutoComboBox
            componentClass= { componentClass ? componentClass : 'row'}
            value = { selectedLanguage }
            values = { languages }
            label = { label ? label : formatMessage(messages.languageSelectionTitle)}
            tooltip = { tooltip ? tooltip : formatMessage(messages.languageSelectionTootltip)}
            iconAction = { iconAction }
            changeCallback = { handleLanguageToChange }
            name= { 'languageSelect' }                                       
            readOnly= { false }
            clearable = { false }
            labelClass={ labelClass ? labelClass : 'col-sm-4' }
            autoClass= { autoClass ? autoClass : 'col-sm-8' }
            className = 'limited w320' />
    )
}

const actions = [
    commonActions
];

function mapStateToProps(state, ownProps) {
    const languageTo = getLanguageTo(state, ownProps) || getTranslationLanguageId(state, 'fi');

  return {
    languages: getTranslationLanguagesObjectArray(state),
    selectedLanguage: languageTo,
  }
}

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(LanguageSelect));