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
import React, {PropTypes, Component} from 'react';
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl';
import PTVTextInput from '../../Components/PTVTextInput';

const AdditionalInfoComponent = ({intl, info, readOnly, language, translationMode, splitContainer, handleInfoChange, order, maxLength, name, label, size ,tooltip, placeholder, componentClass}) => {
    const { formatMessage } = intl;
    //const additionalInfoClass = splitContainer ? "col-xs-12" : ( componentClass ? componentClass : "col-sm-8 col-md-6 col-lg-4");
    return (
        <PTVTextInput                    
            //componentClass = { additionalInfoClass }
            label = { label }
            placeholder = { placeholder }
            tooltip = { tooltip }
            value = { info }
            blurCallback = { handleInfoChange }
            maxLength = { maxLength ? maxLength : 100 }
            name = {name}
            order = { order }
            readOnly= { readOnly && translationMode == "none" } 
            disabled= { translationMode == "view" }
            size = { size ? size :"w320" }
        />         
    );
};

export default injectIntl(AdditionalInfoComponent);
