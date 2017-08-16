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
import sinon from 'sinon';
import { expect } from 'chai';
import { shallowWithIntl, mountWithIntl } from './Helpers/intlEnzymeTestHelper';
import PTVTextInput from '../PTVTextInput';
import PTVLabel from '../PTVLabel';
import { ValidatePTVComponent } from '../PTVComponent';
import {IntlProvider} from 'react-intl';

describe('<PTVTextInput />', () => {
    const intlProvider = new IntlProvider({locale: 'en'}, {});
    const { intl } = intlProvider.getChildContext();
    
    it('should render <label> element when "label" property provided', () => {
        const props = {
            name: 'test_input',
            label: 'test input label'
        };
        const wrapper = mountWithIntl (
            <PTVTextInput { ...props } />
        );
        const inputLabel = wrapper.find(PTVLabel);

        expect(inputLabel).to.have.length(1);
    });
    
    it('should not render <label> element when "label" property is not provided', () => {
        const props = {
            name: 'test_input'
        };
        const wrapper = mountWithIntl (
            <PTVTextInput { ...props } />
        );
        const inputLabel = wrapper.find(PTVLabel);

        expect(inputLabel).to.have.length(0);
    });
    
    it('should render <input> with "ptv-textinput" class when not in readonly mode', () => {
        const props = {
            name: 'test_input',
            className: 'ptv-textinput',
            readOnly: false
        }
        const wrapper = mountWithIntl (
            <PTVTextInput { ...props } />
        );
        const inputText = wrapper.find('input');

        expect(inputText).to.have.length(1);
        expect(inputText.hasClass('ptv-textinput')).to.equal(true);
    });

    it('should not render <input> element when in readonly mode', () => {
        const props = {
            name: 'test_input',
            className: 'ptv-textinput',
            readOnly: true
        }
        const wrapper = mountWithIntl (
            <PTVTextInput { ...props } />
        );
        const inputText = wrapper.find('input');

        expect(inputText).to.have.length(0);
    });    
    
    // it('has small description', () => {
    //     const wrapper = shallow(<PTVTextInput textMuted='test muted' label='test'/>);
    //     let small = wrapper.find('.text-muted');
    //     expect(small).to.have.length(1);
    // });
    
    // it('has value set', () => {
    //     const wrapper = shallow(<PTVTextInput label='tests'/>);
    //     expect(wrapper.find('input').prop('value')).to.equal('');
    //     wrapper.setState({componentValue: 'text', isChangingDirty: true});
    //     expect(wrapper.find('input').prop('value')).to.equal('text');
    // });
    
    // it('has set property to input', () => {
    //     const wrapper = shallow(
    //         <PTVTextInput 
    //             inputclass='input-class' 
    //             label='test'
    //             id='5'
    //             name='textInput'
    //             maxLength={100}
    //             placeholder='test placeholder' />);
    //     let divContainer = wrapper.find('.input-class');
    //     expect(divContainer).to.have.length(1);
    //     let input = divContainer.find('input');
    //     expect(input).to.have.length(1);
    //     expect(input.prop('type')).to.equal('text');
    //     expect(input.prop('id')).to.equal('textInput_5');
    //     expect(input.prop('className')).to.equal('form-control');
    //     expect(input.prop('maxLength')).to.equal(100);
    //     expect(input.prop('placeholder')).to.equal('test placeholder');
    //     expect(input.prop('value')).to.equal('');
    // });
    
    // it('has validation message', () => {
    //     const wrapper = shallow(<PTVTextInput label='test' />);
    //     expect(wrapper.find('.error-message')).to.have.length(0);
    //     wrapper.setState({isValid: false});
    //     expect(wrapper.find('.error-message')).to.have.length(1);
    // });
    
    // it('simulates hitting ENTER key', () => {
    //     const simulateEnterKey = sinon.spy();
    //     const props = {
    //         name: 'test_input',
    //         //onEnterCallBack: () => {}
    //     }
    //     const wrapper = mountWithIntl (
    //         <PTVTextInput onKeyUp={simulateEnterKey} { ...props } />
    //     );

    //     wrapper.find('input').simulate('keyUp', { keyCode: 13 })
    //     expect(simulateEnterKey.calledOnce).to.equal(true);
    // });

    // it('simulates change event', () => {
    //     const simulateChange = sinon.spy();
    //     /*const props = {
    //         name: 'test_input',
    //         //onEnterCallBack: () => {}
    //     }*/
    //     const wrapper = mountWithIntl (
    //         <PTVTextInput onChange={simulateChange} />
    //     );

    //     wrapper.find('input').simulate('change', { target: { value: 'test input value changed' } })
    //     expect(simulateChange.calledOnce).to.equal(true);
    // });   

    it('should handle change', () => {
        const simulateChange = sinon.spy();
        const wrapper = mountWithIntl (
            <PTVTextInput />, { intl }
        );
        console.log(wrapper.find(PTVTextInput).debug());
        console.log('********************************');
        console.log(wrapper.find(PTVTextInput).props());

        expect(wrapper.find(PTVTextInput)).to.have.length(1);
        //wrapper.instance().handleChange();
        //console.log(wrapper);
    });    
    // it('simulate value changed', () => {
    //     const onChange = sinon.spy();
    //     const wrapper = shallow(<PTVTextInput label='test' changeCallback={ onChange }/>);
    //     wrapper.find('input').simulate('change', {target: {value:'test'}})
    //     expect(onChange.calledOnce).to.equal(true);
    //     expect(wrapper.find('input').prop('value')).to.equal('test');
    // });
    
    // it('simulate on blur event', () => {
    //     const onBlurCallBack = sinon.spy();
    //     const wrapper = shallow(<PTVTextInput label='test' blurCallback={ onBlurCallBack }/>);
    //     wrapper.find('input').simulate('blur');
    //     expect(onBlurCallBack.calledOnce).to.equal(true);
    // });
});
