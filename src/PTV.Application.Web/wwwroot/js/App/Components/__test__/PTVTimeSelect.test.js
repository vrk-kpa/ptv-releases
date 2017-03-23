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
import { PTVTimeSelect, PTVAutoComboBox } from '../../Components';

import { shallowWithIntl, mountWithIntl } from './Helpers/intlEnzymeTestHelper.js';

describe('<PTVTimeSelect />', () => {
    
    it('should render <PTVAutoComboBox /> component with "ptv-timeselect" class', () => {
        const props = {
            onChange: () => {},
            minTicks: [0, 30]
        }
        const wrapper = shallow (
            <PTVTimeSelect {...props} />
        );
        console.log(wrapper.props());
        expect(wrapper.find(PTVAutoComboBox)).to.have.length(1);
        expect(wrapper.props().className).to.equal('ptv-timeselect');
    });
    
    it('should have placeholder property set to 00.00 by default', () => {
        const props = {
            onChange: () => {},
            minTicks: [0, 30]
        }
        const wrapper = shallow (
            <PTVTimeSelect {...props} />
        );

        expect(wrapper.instance().props.placeholder).to.have.string('00.00');
    });
});
