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
import * as PTVValidatorTypes from '../../Components/PTVValidators';
import { PTVTextInputNotEmpty } from '../../Components/PTVTextInput';

const validatorsEmail = [PTVValidatorTypes.IS_EMAIL];
const validatorsRequired = [PTVValidatorTypes.IS_EMAIL, PTVValidatorTypes.IS_REQUIRED];

const EmailComponent = ({intl, email, handleEmailChange, order, maxLength, name, label, tooltip, placeholder, readOnly, language,
                        translationMode, splitContainer, isRequired, validatedField}) => {
    const { formatMessage } = intl;

    return (
        <PTVTextInputNotEmpty
            //componentClass = { emailClass }
            inputclass = "wrap-content"
            label = { label }
            validatedField={validatedField}
            placeholder = { placeholder }
            tooltip = { tooltip }
            value = { email }
            blurCallback = { handleEmailChange }
            maxLength = { maxLength ? maxLength : 100 }
            name = {name}
            order = { order }
            validators = { isRequired ? validatorsRequired : validatorsEmail }
            readOnly= { readOnly && translationMode == "none" }
            disabled= { translationMode == "view" }
            size = "w320"
        />
    );
};

export default injectIntl(EmailComponent);
