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
import { connect } from 'react-redux';
import { injectIntl, defineMessages } from 'react-intl';

import * as ServiceSelectors from '../Services/Service/Selectors';
import * as CommonServiceSelectors from '../Services/Common/Selectors';
import { PTVTagInput } from '../../Components';

const messages = defineMessages({
    keyWordsTitle: {
        id: "Containers.Services.AddService.Step2.KeyWords.Title",
        defaultMessage: "Vapaat asiasanat"
    },
    KeyWordPlaceholder: {
        id: "Containers.Services.AddService.Step2.KeyWords.Placeholder",
        defaultMessage: "Lisää vapaita asiasanoja harkiten."
    },
    keyWordsTooltip: {
        id: "Containers.Services.AddService.Step2.KeyWords.Tooltip",
        defaultMessage: "Kirjoita vapaisiin asiasanoihin ainoastaan sellaisia käsitteitä, joita ei löydy ontologiakentästä, esimerkiksi puhekielisiä sanoja (työkkäri) tai käytöstä poistuneita nimiä (työvoimatoimisto), joilla käyttäjät saattavat etsiä palvelua. Kirjoita asiasanat kenttään pilkulla ja välilyönnillä erotettuina."
    },
});

const KeyWords = ({intl, selectedKeywords, readOnly, keyWords, onKeyWordAdd, onKeyWordRemove, language }) => {
            const { formatMessage } = intl;

            const onKewordSelectClick = (item) => {
                if (item.isNew) {
                    onKeyWordAdd([item], 'newKeyWords',language)
                }
                else {
                    onKeyWordAdd([item], 'keyWords',language);
                }
            }
            
            const onKewordRemoveClick = (item) => {
                if (item.isNew) {
                    onKeyWordRemove('newKeyWords', item.id, false, language)
                }
                else {
                    onKeyWordRemove('keyWords', item.id, false, language);
                }
            }

            return (<PTVTagInput
                        autoClass="row"
                        labelClass="col-xs-12"
                        tagInputClass="col-xs-12"
                        placeholder={ formatMessage(messages.KeyWordPlaceholder) }
                        tooltip={ formatMessage(messages.keyWordsTooltip) }
                        label={ formatMessage(messages.keyWordsTitle) }
                        options={ keyWords }
                        value = { selectedKeywords }
                        input = { '' }
                        onSelect={ onKewordSelectClick /*this.handleKeywordsTargetChange*/ }
                        onRemove={ onKewordRemoveClick /*this.handleKeywordsInputChange*/ }
                        changeIdCallback={ onKewordSelectClick /*this.handleKeywordsIdChange*/ }
                        id = { 'keyWordsId' }
                        take = { 10 }
                        readOnly = { readOnly }/>)
}

KeyWords.propTypes = {
  selectedKeyWordsSelector: PropTypes.func.isRequired,
};

function mapStateToProps(state, ownProps) {
  return {
    keyWords: CommonServiceSelectors.getKeyWords(state, ownProps),
    selectedKeywords: ownProps.selectedKeyWordsSelector(state, ownProps),
  }
}

export default connect(mapStateToProps)(injectIntl(KeyWords));