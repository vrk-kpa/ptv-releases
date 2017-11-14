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
import { PTVIcon, PTVLabel } from '../../Components';

class PTVAddItemHeader extends Component {
	constructor(props) {
    super(props);
  };

  onCloseButtonClick = () => {
    if (this.props.onHideAddItem) {
      this.props.onHideAddItem();
    }

    this.props.visibility(false);
  }

  render() {

    return (
      <div className="aic-header">
        <div className="aic-header-content">

          <PTVLabel
            tooltip = { this.props.messages ? this.props.messages.tooltip : ''}
            readOnly = { this.props.readOnly } >
              { this.props.messages.label }
          </PTVLabel>          
          { !this.props.readOnly && this.props.clearAll ?
            <div className="aic-remove-all">
              <PTVLabel
                tooltip = "Test tooltip for removing whole section"
                type = "bare"
                clearAll = { this.props.clearAll }
                labelClass = "button-link" >
                  <PTVIcon name="icon-trash" />
                  Test message
              </PTVLabel>
            </div>
          : null }
          
          { !this.props.readOnly && this.props.collapsible ?
            <PTVIcon
              name="icon-angle-up"
              componentClass='top-align'
              onClick = {() => this.onCloseButtonClick()}
            />
          : null }
        </div>
      </div>
    );

  };

}

PTVAddItemHeader.propTypes = {
  messages: PropTypes.object,
  readOnly: PropTypes.bool,
  collapsible: PropTypes.bool,
  clearAll: PropTypes.func,
  visibility: PropTypes.func,
  onHideAddItem: PropTypes.func
};

PTVAddItemHeader.defaultProps = {
  messages: {},
};

export default PTVAddItemHeader;
