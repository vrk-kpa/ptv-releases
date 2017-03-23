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
import { defineMessages } from 'react-intl';

const messages = defineMessages({
    isRequired: {
        id: "Components.Validators.IsRequired.Message",
        defaultMessage: "Täytä kentän tiedot."
    },
    isEmail: {
        id: "Components.Validators.IsEmail.Message",
        defaultMessage: "Täytä voimassaoleva sähköpostiosoite."
    },
    isUrl: {
        id: "Components.Validators.IsUrl.Message",
        defaultMessage: "Täytä tarkka verkko-osoite."
    },
    atLeastOneDigit: {
        id: "Components.Validators.AtLeastOneDigit.Message",
        defaultMessage: "Kirjoita vähintään yksi merkki."
    },
    isPostalCode: {
        id: "Components.Validators.IsPostalCode.Message",
        defaultMessage: "Syötä postinumero (esimerkiksi 00010)."
    },
    isBusinessId: {
        id: "Components.Validators.IsBusinessId.Message",
        defaultMessage: "Invalid business ID (1234567-8)"
    },
    isValidDateTime: {
        id: "Components.Validators.IsValidDateTime.Message",
        defaultMessage: "Invalid date or time."
    },
    isNotEmpty: {
        id: "Components.Validators.IsNotEmpty.Message",
        defaultMessage: "Täytä kentän tiedot. Jos haluat jättää kentän tyhjäksi, varmista ettei kentässä ole yhtään tyhjää välilyöntiä."
    }
});

export const IS_REQUIRED = { rule: 'isRequired', errorMessage: messages.isRequired };
export const IS_NOT_EMPTY = { rule: 'isNotEmpty', errorMessage: messages.isNotEmpty };
export const IS_EMAIL = { rule: 'isEmail', errorMessage: messages.isEmail };
export const IS_URL = { rule: 'isUrl', errorMessage: messages.isUrl };
export const IS_POSTALCODE = { rule: 'isPostalCode', errorMessage: messages.isPostalCode };
export const IS_BUSINESSID = { rule: 'isBusinessId', errorMessage: messages.isBusinessId };
export const IS_DATETIME = { rule: 'isValidDateTime', errorMessage: messages.isValidDateTime };
