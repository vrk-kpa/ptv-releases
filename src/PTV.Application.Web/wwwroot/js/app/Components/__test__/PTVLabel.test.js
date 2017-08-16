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
import PTVLabel from '../PTVLabel';

import { shallowWithIntl, mountWithIntl } from './Helpers/intlEnzymeTestHelper.js';

describe('<PTVLabel />', () => {
    
    it('should render <label> element with own class', () => {
        const wrapper = shallow (
            <PTVLabel />
        );
        
        expect(wrapper.find('label')).to.have.length(1);
        expect(wrapper.props().className).to.equal('ptv-label');
    });
    
    it('should render tooltip when "tooltip" property provided', () => {
        const props = {
            tooltip: 'test tooltip',
            readOnly: false
        }
        const wrapper = shallow (
            <PTVLabel {...props} />
        );

        expect(wrapper.find('span')).to.have.length(1);
        expect(wrapper.find('span').props()['data-tooltip']).to.equal('test tooltip');
        expect(wrapper.find('i')).to.have.length(1);
        expect(wrapper.find('i').props().className).to.equal('tooltip');
    });

    it('allows us to provide custom class', () => {
        const props = {
            labelClass: 'custom-label'
        }

        const wrapper = shallow (
            <PTVLabel {...props} />
        );
        
        expect(wrapper.props().className).to.have.string('custom-label');
    });
    
    it('allows us to set props', () => {
        const props = {
            tooltip: 'test tooltip'
        }
        const wrapper = mount (
            <PTVLabel {...props} />
        );
        
        expect(wrapper.prop('tooltip')).to.equal('test tooltip');
        
        wrapper.setProps({tooltip: 'changed test tooltip'});
        
        expect(wrapper.prop('tooltip')).to.equal('changed test tooltip');
    });
});
