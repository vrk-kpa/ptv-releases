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
import React, {Component, PropTypes} from 'react';
import SkyLight from 'react-skylight';
import ReactDOM from 'react-dom';
import * as ValidationHelper from '../PTVValidations';
import {is} from 'immutable';
import PTVLabel from '../PTVLabel';
import PTVTextInput from '../PTVTextInput';
import styles from './styles.scss';

export class PTVPopupSearchInput extends Component {

  constructor(props){
    super(props);
    this.state = { isPopupVisible: false};
    
    this.handleAfterClose = this.handleAfterClose.bind(this);
    this.handleInputChange = this.handleInputChange.bind(this);
    this.handleInputOnFocusChange = this.handleInputOnFocusChange.bind(this);
  }

  show()
  {
      this.refs.PTVPopupSearchInput.show();
      this.setState({ isPopupVisible: true });
  }

  hide()
  {
      this.refs.PTVPopupSearchInput.hide(); 
      this.setState({ isPopupVisible: false });  
  }

  handleAfterClose()
  {
      this.setState({ isPopupVisible: false });   
  }
  
  handleInputChange = (value) => { 
             
        if (this.props.inputChangeCallback){
            this.props.inputChangeCallback(value);
        }       
        this.show();
  }

  handleInputOnFocusChange = (value) => { 
      
        if (!this.state.isPopupVisible && this.props.inputChangeCallback)
        {
            this.props.inputChangeCallback(value);                    
        }
        this.show();
    } 

  componentDidMount() {
      var labelInput = ReactDOM.findDOMNode(this.refs.PTVTextInput);

      this.dialogStyles.width = labelInput.getElementsByTagName('input')[0].offsetWidth;
      this.dialogStyles.top = '0px';
      this.dialogStyles.left = '0px';
  }

  
  dialogStyles = {
      height: '400px',
      position: 'absolute',
      marginTop: '0px',
      marginLeft: '0px',
      backgroundColor: '#fff',
      borderRadius: '2px',
      padding: '15px',
      boxShadow: 'none',
      overflow: 'auto',
      border: '1px solid #CCCCCC',
      borderTop: 'none',      
  }
  
  titleStyle = {     
    marginTop: 0,     
  };
 
  render() {
    var overlayStyles = {
      width: '0%',
      height: '0%',
      zIndex: '0',
    }
       
    return ( 

        <div className='popup-search-container'>    
            <PTVTextInput  ref='PTVTextInput'
                name= 'SearchInput'
                value= { this.props.inputValue }
                placeholder={ this.props.inputPlaceholder } 
                inputclass= {this.props.inputClass }                
                componentClass= { this.props.componentClass }               
                changeCallback= { this.handleInputChange }
                focusCallback= { this.handleInputOnFocusChange }
                stopChangeTimeout = {600}                          
            />

            <SkyLight ref="PTVPopupSearchInput" 
                title={this.props.title}
                titleStyle={this.titleStyle}
                dialogStyles= {this.dialogStyles} 
                overlayStyles= {overlayStyles}
                afterClose= {this.handleAfterClose}                                   
                     >
                    {this.props.children}
            </SkyLight>
        </div>
      )
  }
}

PTVPopupSearchInput.propTypes = {
  children: React.PropTypes.element.isRequired,
  beforeOpenCallback: PropTypes.func,
  inputChangeCallback: PropTypes.func
}
  
PTVPopupSearchInput.defaultProps = {
  inputPlaceholder: '',
  inputValue: '',
}

export default PTVPopupSearchInput;
