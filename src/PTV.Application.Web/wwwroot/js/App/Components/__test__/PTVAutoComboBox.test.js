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
import { PTVAutoComboBox, PTVLabel } from '../../Components';
import Select from 'react-select';

import { shallowWithIntl, mountWithIntl } from './Helpers/intlEnzymeTestHelper.js';

import {IntlProvider} from 'react-intl';
import { injectIntl } from 'react-intl';

describe('<PTVAutoComboBox />', () => {

    const intlProvider = new IntlProvider({locale: 'en'}, {});
    const { intl } = intlProvider.getChildContext();
    
    // it('should contain <textarea> element', () => {
    //     const props = {
    //         name: "test_textarea"
    //     }
    //     const wrapper = mountWithIntl (
    //         <PTVAutoComboBox { ...props } />, { intl }
    //     );

    //     expect(wrapper.find('textarea')).to.have.length(1);
    // });

    // it('should contain <div> element with "ptv-textarea" class', () => {
    //     const props = {
    //         name: "test_textarea"
    //     }
    //     const wrapper = mountWithIntl (
    //         <PTVAutoComboBox { ...props } />, { intl }
    //     );

    //     expect(wrapper.find('.ptv-textarea')).to.have.length(1);
    // });
    
    // it('should render one <PTVLabel /> when "label" property provided in readonly mode', () => {
    //     const props = {
    //         name: "Textarea test name",
    //         label: "Test Textarea label",
    //         readOnly: true
    //     };
    //     const wrapper = mountWithIntl (
    //         <PTVAutoComboBox {...props} />, { intl }
    //     );

    //     expect(wrapper.find(PTVLabel)).to.have.length(1);
    // });

    it('should render <Async /> component when "async" property is provided in edit mode', () => {
        const props = {
            async: true,
            values: () => { return "test" },
            readOnly: false
        };
        const wrapper = mountWithIntl (
            <PTVAutoComboBox {...props} />, { intl }
        );
        console.log(wrapper);
        expect(wrapper.find(Select.Async)).to.have.length(1);
    });

});