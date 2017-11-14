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
import { PTVTagInput, PTVLabel } from '../../Components';
import { shallowWithIntl, mountWithIntl } from './Helpers/intlEnzymeTestHelper.js';
import { IntlProvider } from 'react-intl';
import { injectIntl } from 'react-intl';

describe('<PTVTagInput />', () => {

    const intlProvider = new IntlProvider({locale: 'en'}, {});
    const { intl } = intlProvider.getChildContext();    
    
    it('should have "tag-input" class', () => {
        const props = {
            readOnly: true,
            options: ['one', 'two', 'three'],
            value: ['two', 'three']
        }
        const wrapper = shallow (
            <PTVTagInput {...props} />
        );

        expect(wrapper.props().className).to.have.string('tag-input');
    });

    it('should render two <PTVLabel /> components when readonly', () => {
        const props = {
            readOnly: true,
            options: ['one', 'two', 'three'],
            value: ['two']
        }
        const wrapper = shallow (
            <PTVTagInput {...props} />
        );

        expect(wrapper.find(PTVLabel)).to.have.length(2);
    });

    // it('should render selected values separated with comma', () => {
    //     const props = {
    //         readOnly: true,
    //         options: ['one', 'two', 'three'],
    //         value: ['one', 'two']
    //     }
    //     const wrapper = mount (
    //         <PTVTagInput {...props} />
    //     );
    //     console.log(wrapper.state());
    //     expect(wrapper.find(PTVLabel).last().render().find('label').html()).to.have.string('one, two');
    //     // wrapper.setProps({ description: 'Changed test section header description' });
    //     // expect(wrapper.find('p').text()).to.equal('Changed test section header description');
    // });

    // it('should render <PTVButton /> component', () => {
    //     const wrapper = shallow (
    //         <PTVTagInput />
    //     );
        
    //     expect(wrapper.find(PTVButton)).to.have.length(1);
    // });

    // it('should be possible to remove <PTVTagInput> component', () => {
    //     const simulateClick = sinon.spy();
    //     const props = {
    //         selected: [{ id: 'abcd1234', name: 'one' }];
    //     };
    //     const wrapper = mountWithIntl (
    //         <PTVTagInput {...props} />, { intl }
    //     );
                
    //     expect(wrapper.state().show).to.equal(true);
    //     wrapper.find(ButtonDelete).simulate('click');
    //     expect(wrapper.state().show).to.equal(false);
    // });

    // it('should open the popup when receives focus', () => {
    //     const simulateClick = sinon.spy();
    //     const wrapper = mountWithIntl (
    //         <PTVTagInput />, { intl }
    //     );
    //     console.log(wrapper);
    //     expect(wrapper.state().isPopupVisible).to.equal(false);
    //     wrapper.instance().handleInputOnFocusChange();
    //     expect(wrapper.state().isPopupVisible).to.equal(true);
    //     wrapper.instance().handleAfterClose();
    //     expect(wrapper.state().isPopupVisible).to.equal(false);        
    // });

});
