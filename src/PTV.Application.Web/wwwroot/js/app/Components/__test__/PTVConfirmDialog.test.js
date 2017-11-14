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
import React from 'react';
import { shallow, mount } from 'enzyme';
import { expect } from 'chai';
import { PTVConfirmDialog, PTVButton, PTVIcon } from '../../Components';
import { ButtonCancel, ButtonContinue } from '../../Containers/Common/Buttons';

import { shallowWithIntl, mountWithIntl } from './Helpers/intlEnzymeTestHelper.js';

import {IntlProvider} from 'react-intl';

describe('<PTVConfirmDialog />', () => {

    const intlProvider = new IntlProvider({locale: 'en'}, {});
    const { intl } = intlProvider.getChildContext();
    
    it('should have "ptv-dialog" class', () => {
        const wrapper = mountWithIntl (
            <PTVConfirmDialog />, { intl }
        );

        expect(wrapper.find('.ptv-dialog')).to.have.length(1);
    });

    it('should have "closed" class when "show" property is false', () => {
        const props = {
            show: false
        }
        const wrapper = mountWithIntl (
            <PTVConfirmDialog { ...props } />, { intl }
        );

        expect(wrapper.find('.closed')).to.have.length(1);
    });

    it('should contain <PTVIcon /> component with "dialog-type" className', () => {
        const wrapper = mountWithIntl (
            <PTVConfirmDialog />, { intl }
        );
        const dialogIcon = wrapper.find(PTVIcon);

        expect(dialogIcon).to.have.length(1);
        expect(dialogIcon.prop('className')).to.equal('dialog-type');
    });

    it('should contain two buttons (cancel and continue)', () => {
        const wrapper = mountWithIntl (
            <PTVConfirmDialog />, { intl }
        );
        const cancelButton = wrapper.find(ButtonCancel);
        const continueButton = wrapper.find(ButtonContinue);

        expect(cancelButton).to.have.length(1);
        expect(continueButton).to.have.length(1);
        expect(wrapper.find(PTVButton)).to.have.length(2);
    });

    it('should display provided text', () => {
        const props = {
            text: 'This is a test message for PTVConfirmDialog'
        }
        const wrapper = mountWithIntl (
            <PTVConfirmDialog { ...props } />, { intl }
        );
        const message = wrapper.find('p');

        expect(message).to.have.length(1);
        expect(message.text()).to.equal('This is a test message for PTVConfirmDialog');
    });
    
});
