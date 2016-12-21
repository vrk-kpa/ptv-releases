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
import { PTVMessageBox, PTVLabel, PTVIcon, PTVTextArea, PTVOverlay } from '../../Components';
import { ButtonDelete } from '../../Containers/Common/Buttons';
import { Link } from 'react-router';
import { shallowWithIntl, mountWithIntl } from './Helpers/intlEnzymeTestHelper.js';
import { IntlProvider } from 'react-intl';
import { injectIntl } from 'react-intl';

describe('<PTVMessageBox />', () => {

    const intlProvider = new IntlProvider({locale: 'en'}, {});
    const { intl } = intlProvider.getChildContext();    
    
    it('should contain <DeleteButton>', () => {
        const props = {
            messages: ['Hello there, ', 'I am a test message.', 'You can see me during test run!']
        };
        const wrapper = shallow (
            <PTVMessageBox {...props} />
        );
        
        expect(wrapper.find(ButtonDelete)).to.have.length(1);
    });

    it('should have "error-box" class by default', () => {
        const props = {
            messages: ['Hello there, ', 'I am a test message.', 'You can see me during test run!']
        };
        const wrapper = shallow (
            <PTVMessageBox {...props} />
        );
        const msgIcon = wrapper.find(PTVIcon);
        
        expect(wrapper.props().className).to.have.string('msg-box');
        expect(wrapper.props().className).to.have.string('error-box');
        expect(msgIcon).to.have.length(1);
        expect(msgIcon.props().className).to.have.string('color-chili-crimson');
        expect(msgIcon.props().name).to.be.equal('icon-exclamation');
    });

    it('should be a success message with proper class when "isValid" property is set', () => {
        const props = {
            messages: ['Hello there, ', 'I am a test message.', 'You can see me during test run!'],
            isValid: true
        };
        const wrapper = shallow (
            <PTVMessageBox {...props} />
        );
        const msgIcon = wrapper.find(PTVIcon);
        
        expect(wrapper.props().className).to.have.string('success-box');
        expect(msgIcon).to.have.length(1);
        expect(msgIcon.props().className).to.have.string('color-leaf');
        expect(msgIcon.props().name).to.be.equal('icon-check');
    });

    it('should render as many <PTVLabel> components as there are messages', () => {
        const props = {
            messages: ['Hello there, ', 'I am a test message.', 'You can see me during test run!'],
            isValid: true
        };
        const wrapper = shallow (
            <PTVMessageBox {...props} />
        );
        
        expect(wrapper.find(PTVLabel)).to.have.length(3);
    });

    it('should be possible to remove <PTVMessageBox> component', () => {
        const simulateClick = sinon.spy();
        const props = {
            messages: ['Hello there, ', 'I am a test message.', 'You can see me during test run!']
        };
        const wrapper = mountWithIntl (
            <PTVMessageBox {...props} />, { intl }
        );
                
        expect(wrapper.state().show).to.equal(true);
        wrapper.find(ButtonDelete).simulate('click');
        expect(wrapper.state().show).to.equal(false);
    });
    
});
