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
import { PTVOverlay } from '../../Components';
import { Link } from 'react-router';
import SkyLight from 'react-skylight';

describe('<PTVOverlay />', () => {
    
    it('should render <SkyLight> component', () => {
        const wrapper = shallow (
            <PTVOverlay />
        );

        expect(wrapper.find(SkyLight)).to.have.length(1);
    });

    it('should have default styles set when "dialogStyles" property not provided', () => {
        const wrapper = shallow (
            <PTVOverlay />
        );

        expect(wrapper.find(SkyLight).props().dialogStyles).to.have.property('backgroundColor', '#FFFFFF');
        expect(wrapper.find(SkyLight).props().dialogStyles).to.have.property('color', '#000000');
        expect(wrapper.find(SkyLight).props().dialogStyles).to.have.property('width', '50%');
        expect(wrapper.find(SkyLight).props().dialogStyles).to.have.property('height', 700);
        expect(wrapper.find(SkyLight).props().dialogStyles).to.have.property('marginTop', -300);
        expect(wrapper.find(SkyLight).props().dialogStyles).to.have.property('marginLeft', '-25%');
    });

    it('should override default styles set when "dialogStyles" property is provided', () => {
        const customStyles = {
            'backgroundColor': '#ABCDEF',
            'color': '#123456'
        };
        const props = {
            dialogStyles: customStyles
        }
        const wrapper = shallow (
            <PTVOverlay {...props} />
        );

        expect(wrapper.find(SkyLight).props().dialogStyles).to.have.property('backgroundColor', '#ABCDEF');
        expect(wrapper.find(SkyLight).props().dialogStyles).to.have.property('color', '#123456');
    });

});
