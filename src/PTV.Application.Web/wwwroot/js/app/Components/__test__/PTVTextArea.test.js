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
import { PTVTextArea, PTVLabel } from '../../Components';

import { shallowWithIntl, mountWithIntl } from './Helpers/intlEnzymeTestHelper.js';

import {IntlProvider} from 'react-intl';
import { injectIntl } from 'react-intl';

describe('<PTVTextArea />', () => {

    const intlProvider = new IntlProvider({locale: 'en'}, {});
    const { intl } = intlProvider.getChildContext();
    
    it('should contain <textarea> element', () => {
        const props = {
            name: "test_textarea"
        }
        const wrapper = mountWithIntl (
            <PTVTextArea { ...props } />, { intl }
        );

        expect(wrapper.find('textarea')).to.have.length(1);
    });

    it('should contain <div> element with "ptv-textarea" class', () => {
        const props = {
            name: "test_textarea"
        }
        const wrapper = mountWithIntl (
            <PTVTextArea { ...props } />, { intl }
        );

        expect(wrapper.find('.ptv-textarea')).to.have.length(1);
    });
    
    it('should render one <PTVLabel /> when "label" property provided in readonly mode', () => {
        const props = {
            name: "Textarea test name",
            label: "Test Textarea label",
            readOnly: true
        };
        const wrapper = mountWithIntl (
            <PTVTextArea {...props} />, { intl }
        );

        expect(wrapper.find(PTVLabel)).to.have.length(1);
    });

    it('should render two <PTVLabel /> components when "label" property provided in edit mode', () => {
        const props = {
            name: "Textarea test name",
            label: "Test Textarea label",
            readOnly: false
        };
        const wrapper = mountWithIntl (
            <PTVTextArea {...props} />, { intl }
        );

        expect(wrapper.find(PTVLabel)).to.have.length(2);
    });

    it('should render one <PTVLabel /> when "label" property is not provided in edit mode', () => {
        const props = {
            name: "Textarea test name",
            counterClass: false,
            readOnly: false
        };
        const wrapper = mountWithIntl (
            <PTVTextArea {...props} />, { intl }
        );
        const counterLabel = wrapper.find(PTVLabel);

        expect(counterLabel).to.have.length(1);
    });
    
    it('should allow us to set props', () => {
        const props = {
            name: "Textarea test name",
            size: "full",
            readOnly: false
        };
        const wrapper = mountWithIntl (
            <PTVTextArea {...props} />, { intl }
        );

        expect(wrapper.find('.full')).to.have.length(1);
        expect(wrapper.find('.w320')).to.have.length(0);
        
        wrapper.setProps ({ 
            size: 'w320' 
        });

        expect(wrapper.find('.full')).to.have.length(0);
        expect(wrapper.find('.w320')).to.have.length(1);
    });
});
