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
import { PTVHeaderSection, PTVButton } from '../../Components';
import { shallowWithIntl, mountWithIntl } from './Helpers/intlEnzymeTestHelper.js';
import { IntlProvider } from 'react-intl';
import { injectIntl } from 'react-intl';

describe('<PTVHeaderSection />', () => {

    const intlProvider = new IntlProvider({locale: 'en'}, {});
    const { intl } = intlProvider.getChildContext();    
    
    it('should have "header-section" class', () => {
        const wrapper = shallow (
            <PTVHeaderSection />
        );

        expect(wrapper.props().className).to.have.string('header-section');
    });

    it('should render provided description', () => {
        const props = {
            description: 'Test section header description'
        }
        const wrapper = shallow (
            <PTVHeaderSection {...props} />
        );
        
        expect(wrapper.find('p').text()).to.equal('Test section header description');
    });

    it('should allow changing the description', () => {
        const props = {
            description: 'Test section header description'
        }
        const wrapper = shallow (
            <PTVHeaderSection {...props} />
        );
        
        expect(wrapper.find('p').text()).to.equal('Test section header description');
        wrapper.setProps({ description: 'Changed test section header description' });
        expect(wrapper.find('p').text()).to.equal('Changed test section header description');
    });

    it('should render <PTVButton /> component', () => {
        const wrapper = shallow (
            <PTVHeaderSection />
        );
        
        expect(wrapper.find(PTVButton)).to.have.length(1);
    });

});
