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
import PTVIcon from '../PTVIcon';

import { shallowWithIntl, mountWithIntl } from '../../../../../Scripts/test/intlEnzymeTestHelper.js';

describe('<PTVIcon />', () => {
    
    it('should render <svg> element with default size 20x20', () => {
        const props = {
            name: 'test-icon'
        }
        const wrapper = shallow (
            <PTVIcon {...props} />
        );

        expect(wrapper.find('svg')).to.have.length(1);
        expect(wrapper.find('svg').props().width).to.equal(20);
        expect(wrapper.find('svg').props().height).to.equal(20);
    });
    
    it('should render tooltip when "tooltip" property provided', () => {
        const props = {
            tooltip: 'test tooltip',
            name: 'test-icon'
        }
        const wrapper = shallow (
            <PTVIcon {...props} />
        );

        expect(wrapper.find('span')).to.have.length(1);
        expect(wrapper.find('span').props()['data-tooltip']).to.equal('test tooltip');
    });

    it('allows us to provide custom class', () => {
        const props = {
            className: 'custom-icon',
            name: 'test-icon'
        }

        const wrapper = shallow (
            <PTVIcon {...props} />
        );
        
        expect(wrapper.find('svg').hasClass('test-icon')).to.equal(true);
        expect(wrapper.find('svg').hasClass('custom-icon')).to.equal(true);
    });
    
});
