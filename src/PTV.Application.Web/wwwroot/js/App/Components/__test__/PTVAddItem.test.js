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
import { PTVAddItem, PTVLabel, PTVIcon } from '../../Components';
import PTVAddItemHeader from '../../Components/PTVAddItem/PTVAddItemHeader';
import PTVAddItemFooter from '../../Components/PTVAddItem/PTVAddItemFooter';
import { ButtonAdd } from '../../Containers/Common/Buttons';
import { shallowWithIntl, mountWithIntl } from './Helpers/intlEnzymeTestHelper.js';
import { IntlProvider, injectIntl } from 'react-intl';

describe('<PTVAddItem />', () => {

    const intlProvider = new IntlProvider({locale: 'en'}, {});
    const { intl } = intlProvider.getChildContext();  
    
    it('should render <PTVAddItemFooter /> and <PTVAddItemHeader /> when visible', () => {
        const props = {
            readOnly: false,
            renderItemContent: () => {}
        };

        const wrapper = shallow (
            <PTVAddItem {...props} />
        );

        wrapper.setState({ visible: true });
        expect(wrapper.find(PTVAddItemFooter)).to.have.length(1);
        expect(wrapper.find(PTVAddItemHeader)).to.have.length(1);
    });

    it('should render <PTVAddItemFooter /> and no <PTVAddItemHeader /> when not visible', () => {
        const props = {
            readOnly: false,
            renderItemContent: () => {}
        };

        const wrapper = shallow (
            <PTVAddItem {...props} />
        );

        expect(wrapper.find(PTVAddItemFooter)).to.have.length(1);
        expect(wrapper.find(PTVAddItemHeader)).to.have.length(0);
    });

    it('should render three rows when there are three items', () => {
        const props = {
            readOnly: false,
            renderItemContent: () => {},
            items: ['one', 'two', 'three']
        };

        const wrapper = mount (
            <PTVAddItem {...props} />
        );

        wrapper.setState({ visible: true });
        console.log(wrapper.debug());
        expect(wrapper.find('.row')).to.have.length(3);
    });

    // it('should render <ButtonAdd /> with button-link className when not visible', () => {
    //     const props = {
    //         readOnly: false,
    //         renderItemContent: () => {}
    //     };

    //     const wrapper = mount (
    //         <PTVAddItem {...props} />
    //     );
        
    //     const addButton = wrapper.find(ButtonAdd);
    //     expect(addButton).to.have.length(1);
    //     expect(addButton.props().className).to.equal('button-link');
    // });

    // it('should render <ButtonAdd /> without button-link className when visible and multiple', () => {
    //     const props = {
    //         readOnly: false,
    //         renderItemContent: () => {},
    //         visible: true,
    //         multiple: true
    //     };

    //     const wrapper = mount (
    //         <PTVAddItem {...props} />
    //     );
        
    //     const addButton = wrapper.find(ButtonAdd);
    //     expect(addButton).to.have.length(1);
    //     expect(addButton.props().className).not.equal('button-link');
    // });

    // it('should render no <ButtonAdd /> when visible and not multiple', () => {
    //     const props = {
    //         readOnly: false,
    //         renderItemContent: () => {}
    //     };

    //     const wrapper = mount (
    //         <PTVAddItem {...props} />
    //     );
        
    //     wrapper.setState({ visible: true });
    //     expect(wrapper.find(ButtonAdd)).to.have.length(0);
    // });
});
