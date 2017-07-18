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
import { shallow, mount, render } from 'enzyme';
import { expect } from 'chai';
import sinon from 'sinon';
import { PTVLabel, PTVIcon } from '../../Components';
import PTVAddItemHeader from '../../Components/PTVAddItem/PTVAddItemHeader';
import { shallowWithIntl, mountWithIntl } from './Helpers/intlEnzymeTestHelper.js';
import { IntlProvider, injectIntl } from 'react-intl';

describe('<PTVAddItemHeader />', () => {

    const intlProvider = new IntlProvider({locale: 'en'}, {});
    const { intl } = intlProvider.getChildContext();  

    it('should render div with aic-header class', () => {
        const props = {
            readOnly: false
        };

        const wrapper = shallow (
            <PTVAddItemHeader {...props} />
        );
        
        expect(wrapper.props().className).to.equal('aic-header');
    });

    it('should render one <PTVLabel /> when readonly', () => {
        const props = {
            readOnly: true,
            messages: { label: 'Test label' }
        };

        const wrapper = shallow (
            <PTVAddItemHeader {...props} />
        );
        
        const label = wrapper.find(PTVLabel);
        expect(label).to.have.length(1);
    });

    // it('should render no <ButtonAdd /> when visible and not multiple', () => {
    //     const props = {
    //         readOnly: false,
    //         visible: true
    //     };

    //     const wrapper = shallow (
    //         <PTVAddItemHeader {...props} />
    //     );
        
    //     wrapper.setState({ visible: true });
    //     expect(wrapper.find(ButtonAdd)).to.have.length(0);
    // });
});
