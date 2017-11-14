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
import sinon from 'sinon';
import { expect } from 'chai';
import { PTVCheckBox, PTVLabel } from '../../Components';

import { shallowWithIntl, mountWithIntl } from './Helpers/intlEnzymeTestHelper.js';

import {IntlProvider} from 'react-intl';

describe('<PTVCheckBox />', () => {

    const intlProvider = new IntlProvider({locale: 'en'}, {});
    const { intl } = intlProvider.getChildContext();
    
    it('should have "ptv-checkbox" class', () => {
        const props = {
            onClick: () => {}
        }
        const wrapper = mountWithIntl (
            <PTVCheckBox { ...props } />, { intl }
        );

        expect(wrapper.find('.ptv-checkbox')).to.have.length(1);
    });

    it('should render only <PTVLabel /> component in readonly mode', () => {
        const props = {
            readOnly: true,
            onClick: () => {}
        }
        const wrapper = mountWithIntl (
            <PTVCheckBox { ...props } />, { intl }
        );

        expect(wrapper.find(PTVLabel)).to.have.length(1);
        expect(wrapper.find('input')).to.have.length(0);
    });

    it('should render fake checkbox when in edit mode', () => {
        const props = {
            readOnly: false,
            onClick: () => {}
        }
        const wrapper = mountWithIntl (
            <PTVCheckBox { ...props } />, { intl }
        );

        expect(wrapper.find('.check')).to.have.length(1);
    });

    it('should render disabled <input> when "isDisabled" property provided', () => {
        const props = {
            isDisabled: true,
            onClick: () => {}
        }
        const wrapper = mountWithIntl (
            <PTVCheckBox { ...props } />, { intl }
        );

        expect(wrapper.find('.disabled')).to.have.length(1);
        expect(wrapper.find('input').prop('disabled')).to.equal(true);
    });    

    it('should render checked <input> when "isSelected" property provided', () => {
        const props = {
            isSelected: true,
            onClick: () => {}
        }
        const wrapper = mountWithIntl (
            <PTVCheckBox { ...props } />, { intl }
        );

        expect(wrapper.find('.checked')).to.have.length(1);
        expect(wrapper.find('input').prop('checked')).to.equal(true);
    });

    it('simulates click event', () => {
        const simulateClick = sinon.spy();
        const props = {
        }
        const wrapper = mountWithIntl (
            <PTVCheckBox onClick={simulateClick} { ...props } />
        );

        wrapper.find('input').simulate('click');
        expect(simulateClick.calledOnce).to.equal(true);
    });
    
});
