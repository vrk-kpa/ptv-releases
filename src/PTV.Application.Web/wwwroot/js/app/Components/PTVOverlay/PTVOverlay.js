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
import React,  {Component} from 'react';
import {SkyLightStateless} from 'react-skylight';

class PTVOverlay extends React.Component {
  
  constructor(props){
    super(props);
  }
   
  render() {
                  
  var ptvDialogStyles = {
    backgroundColor: '#FFFFFF',
    color: '#000000',
    width: '50%',
    height: 'auto',
    maxHeight: 700,
    marginTop: -300,
    marginLeft: '-25%',      
  };
  var titleStyle = {     
    marginTop: 0,     
  };
  var contentStyles = {     
    overflow: 'auto',
    maxHeight: 600
  };
  var closeButtonStyle = {     
    textDecoration: 'none',      
  };        

  return (
      <div>
        <SkyLightStateless
          isVisible={this.props.isVisible} 
          dialogStyles={this.props.dialogStyles ? this.props.dialogStyles : ptvDialogStyles} 
          overlayStyles={this.props.overlayStyles}            
          titleStyle={this.props.titleStyle ? this.props.titleStyle : titleStyle} 
          closeButtonStyle={this.props.closeButtonStyle ? this.props.closeButtonStyle : closeButtonStyle} 
          hideOnOverlayClicked
          onCloseClicked = {this.props.onCloseClicked} 
          title={this.props.title}>    
          <div style={this.props.contentStyles ? this.props.contentStyles : contentStyles}>    
             {this.props.children}
          </div>
        </SkyLightStateless>
      </div>
    )
  }
}

export default PTVOverlay
