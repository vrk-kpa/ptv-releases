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
import { PTVNavigation, PTVNavigationItem } from '../../Components';
import { shallowWithIntl, mountWithIntl } from './Helpers/intlEnzymeTestHelper.js';
import { IntlProvider } from 'react-intl';
import { injectIntl } from 'react-intl';

describe('<PTVNavigation />', () => {

    const intlProvider = new IntlProvider({locale: 'en'}, {});
    const { intl } = intlProvider.getChildContext();    
    
    it('should contain <ul> element with "role-selection" class', () => {
        const props = {
            menuItems: ['item1', 'item2', 'item3'],
            defaultMenuItemUid: 'a01'
        }
        const wrapper = shallow (
            <PTVNavigation {...props} />
        );
        console.log(wrapper.state());
        expect(wrapper.find('ul').props().className).to.have.string('role-selection');
    });

    it('should render defined amount of menu items', () => {
        const props = {
            menuItems: ['item1', 'item2', 'item3'],
            defaultMenuItemUid: 'a01'
        }
        const wrapper = shallow (
            <PTVNavigation {...props} />
        );

        console.log(wrapper.props());
        expect(wrapper.find(PTVNavigationItem)).to.have.length(3);
    });

    it('should set clicked menu item as active', () => {
        //const simulateSelect = sinon.spy();
        const props = {
            menuItems: ['item1', 'item2', 'item3'],
            defaultMenuItemUid: 'a01'
        };
        const wrapper = mount (
            <PTVNavigation {...props} />
        );
        
        expect(wrapper.state().activeMenuItemUid).to.equal('a01');
        // wrapper.instance().find(PTVNavigationItem).last().setActiveMenuItem('c03');
        // expect(wrapper.state().activeMenuItemUid).to.equal('c03');
    });

    // it('should render as many <PTVLabel> components as there are messages', () => {
    //     const props = {
    //         messages: ['Hello there, ', 'I am a test message.', 'You can see me during test run!'],
    //         isValid: true
    //     };
    //     const wrapper = shallow (
    //         <PTVNavigation {...props} />
    //     );
        
    //     expect(wrapper.find(PTVLabel)).to.have.length(3);
    // });

    // it('should be possible to remove <PTVNavigation> component', () => {
    //     const simulateClick = sinon.spy();
    //     const props = {
    //         messages: ['Hello there, ', 'I am a test message.', 'You can see me during test run!']
    //     };
    //     const wrapper = mountWithIntl (
    //         <PTVNavigation {...props} />, { intl }
    //     );
                
    //     expect(wrapper.state().show).to.equal(true);
    //     wrapper.find(ButtonDelete).simulate('click');
    //     expect(wrapper.state().show).to.equal(false);
    // });
    
});
