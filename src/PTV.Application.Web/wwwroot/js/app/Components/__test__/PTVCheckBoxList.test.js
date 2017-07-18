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
import { PTVCheckBoxList, PTVCheckBox, PTVLabel } from '../../Components';

import { shallowWithIntl, mountWithIntl } from './Helpers/intlEnzymeTestHelper.js';

import {IntlProvider} from 'react-intl';

describe('<PTVCheckBoxList />', () => {

    const intlProvider = new IntlProvider({locale: 'en'}, {});
    const { intl } = intlProvider.getChildContext();
    
    it('should render two <PTVLabel /> components in readonly mode', () => {
        const props = {
            readOnly: true,
            label: 'test checkbox list',
            box: ['chb1', 'chb2', 'chb3'],
            onChangeBox: () => {}
        }
        const wrapper = mountWithIntl (
            <PTVCheckBoxList { ...props } />, { intl }
        );

        expect(wrapper.find(PTVLabel)).to.have.length(2);
    });

    it('should render <fieldset> element with "checkbox-list" class', () => {
        const props = {
            readOnly: false,
            box: ['chb1', 'chb2', 'chb3'],
            onChangeBox: () => {}
        }
        const wrapper = mountWithIntl (
            <PTVCheckBoxList { ...props } />, { intl }
        );

        expect(wrapper.find('fieldset').prop('className')).to.have.string('checkbox-list');
    });

    it('should contain <ul> element with "checkbox-group-horizontal" class when "horizontalLayout" property is true components in readonly mode', () => {
        const props = {
            readOnly: false,
            horizontalLayout: true,
            box: ['chb1', 'chb2', 'chb3'],
            onChangeBox: () => {}
        }
        const wrapper = mountWithIntl (
            <PTVCheckBoxList { ...props } />, { intl }
        );

        expect(wrapper.find('ul').prop('className')).to.have.string('checkbox-group-horizontal');
    });    

    it('should render three <PTVCheckbox /> components when "box" property has three items', () => {
        const props = {
            readOnly: false,
            box: ['chb1', 'chb2', 'chb3'],
            onChangeBox: () => {}
        }
        const wrapper = mountWithIntl (
            <PTVCheckBoxList { ...props } />, { intl }
        );

        expect(wrapper.find(PTVCheckBox)).to.have.length(3);
    });
    
});
