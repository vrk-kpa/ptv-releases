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
import sinon from 'sinon';
import { PTVTag, PTVIcon } from '../../Components';
import { shallowWithIntl, mountWithIntl } from './Helpers/intlEnzymeTestHelper.js';
import { IntlProvider } from 'react-intl';
import { injectIntl } from 'react-intl';

describe('<PTVTag />', () => {

    const intlProvider = new IntlProvider({locale: 'en'}, {});
    const { intl } = intlProvider.getChildContext();    
    
    it('should render <LI> element', () => {
        const wrapper = mountWithIntl (
            <PTVTag />
        );

        expect(wrapper.find('li')).to.have.length(1);
    });

    it('should render <PTVIcon /> component (cross icon) when not readonly', () => {
        const props = {
            readOnly: false
        };
        const wrapper = mountWithIntl (
            <PTVTag {...props} />
        );

        const icon = wrapper.find(PTVIcon);
        expect(icon).to.have.length(1);
        expect(icon.props().name).to.equal('icon-cross');
    });

    it('should not render <PTVIcon /> component (cross icon) when readonly', () => {
        const props = {
            readOnly: true
        };
        const wrapper = mountWithIntl (
            <PTVTag {...props} />
        );

        expect(wrapper.find(PTVIcon)).to.have.length(0);
    });    

    it('should render <SPAN> element with "disabled" class when "isDisabled" prop is true', () => {
        const props = {
            isDisabled: true
        };
        const wrapper = mountWithIntl (
            <PTVTag {...props} />
        );

        expect(wrapper.find('span').first().props().className).to.have.string('disabled');
    });

});
