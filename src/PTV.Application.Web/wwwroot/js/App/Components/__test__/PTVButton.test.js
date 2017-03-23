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
import { PTVButton, PTVIcon } from '../../Components';
import { Link } from 'react-router';

import { shallowWithIntl } from './Helpers/intlEnzymeTestHelper.js';

describe('<PTVButton />', () => {
    
    it('should render <a> element when "onClick" and "href" is provided', () => {
        const props = {
            onClick: () => true,
            href: 'http://testhref.cz'
        };
        const wrapper = shallow (
            <PTVButton {...props} />
        );

        expect(wrapper.find('a').length).to.equal(1);
    });

    it('should render <button> element with own class when "onClick" is provided', () => {
        const props = {
            onClick: () => true
        };
        const wrapper = shallow (
            <PTVButton {...props} />
        );

        expect(wrapper.find('button').length).to.equal(1);
        expect(wrapper.props().className).to.have.string('ptv-button');
    });

    it('should render <Link /> when "href" property provided', () => {
        const props = {
            href: 'http://testhref.cz'
        };
        const wrapper = shallow (
            <PTVButton {...props}>
                Test Button
            </PTVButton>
        );

        expect(wrapper.find(Link)).to.have.length(1);
    });

    it('should render <PTVIcon /> when "withIcon" property provided', () => {
        const props = {
            onClick: () => true,
            withIcon: true
        };
        const wrapper = shallow (
            <PTVButton {...props}>
                Test Button
            </PTVButton>
        );

        expect(wrapper.find(PTVIcon)).to.have.length(1);
    });

    it('renders disabled button when "disabled" property is true', () => {
        const props = {
            onClick: () => true,
            disabled: true
        };
        const wrapper = shallow (
            <PTVButton {...props} />
        );

        expect(wrapper.find('button').props()['disabled']).to.equal(true)
    });

    it('simulates click event', () => {
        const simulateClick = sinon.spy();
        const wrapper = shallow (
            <PTVButton onClick={simulateClick} />
        );

        wrapper.find('.ptv-button').simulate('click');
        expect(simulateClick.calledOnce).to.equal(true);
    });
});
