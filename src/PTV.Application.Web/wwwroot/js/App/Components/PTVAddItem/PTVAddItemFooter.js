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
import { ButtonAdd } from '../../Containers/Common/Buttons';
import PTVIcon from '../PTVIcon';

class PTVAddItemFooter extends Component {
	constructor(props) {
    super(props);
  };

  onAddButtonClick = () => {
    if (this.props.visible) {
      this.props.onAddButtonClick(this.props.addButtonItem, this.props.countOfItems);
    }

    if (this.props.onShowAddItem) {
      this.props.onShowAddItem();
    }

    this.props.visibility(true);
  }

  render() { 
    const collapsedInfoClass = this.props.messages.collapsedInfo ? "collapsed-info" : null;  

    return (
      <div className="aic-footer">
        
        { !this.props.visible ?
          <div className="collapsed-item">            
            <ButtonAdd 
              onClick={() => this.onAddButtonClick()} 
              className="button-link"
              item = {this.props.addButtonItem}
            >
              { this.props.messages.label }
              <PTVIcon
                name="icon-angle-down"  
              />
            </ButtonAdd>
            <span className={collapsedInfoClass}>{ this.props.messages.collapsedInfo }</span>
          </div>

        : this.props.multiple ?
          <div className="aic-footer-content">  
            <ButtonAdd 
              onClick = {() => this.onAddButtonClick()}
              item = {this.props.addButtonItem}
              className="button-link"
              children = {this.props.messages.addBtnLabel ? this.props.messages.addBtnLabel : null}
            />
          </div>
        : 
          <div className="aic-footer-content empty">
          </div>
        }

      </div>
    );

  };

}

PTVAddItemFooter.propTypes = {
  messages: PropTypes.object,
  multiple: PropTypes.bool,
  onAddButtonClick: PropTypes.func,
  onShowAddItem: PropTypes.func,
  addButtonItem: PropTypes.any,
  visible: PropTypes.bool,
  visibility: PropTypes.func
};

PTVAddItemFooter.defaultProps = {
  messages: {},
};

export default PTVAddItemFooter;
