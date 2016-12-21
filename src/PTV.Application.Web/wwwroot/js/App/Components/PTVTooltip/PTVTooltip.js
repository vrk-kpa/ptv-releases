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
import styles from './styles.scss';
import cx from 'classnames';
import ReactDOM from 'react-dom';
import { ButtonDelete, ButtonCancelUpdate } from '../../Containers/Common/Buttons';
import PTVRenderInBody from '../PTVRenderInBody';
import PTVIcon from '../PTVIcon';

class PTVTooltip extends Component {
	constructor(props) {
    super(props);
    this.state = {
      active: false,
      permanent: false,
      alignment: 'right',
      position: null
    };
  };

  getAlignment = () => {
    const tooltipEl = ReactDOM.findDOMNode(this.refs.tooltip);
    const ttBCR = tooltipEl ? tooltipEl.getBoundingClientRect() : null;

    return window.innerWidth - ttBCR.right > this.props.maxWidth ? 'right' : ttBCR.left > this.props.maxWidth ? 'left' : 'top';
  }

  handleClick = () => {
    if (this.state.active && this.props.attachToBody) {
      this.setState({
        active: false,
        permanent: false
      });
    } else if (this.state.active && this.state.permanent) {
      this.setState({
        active: false,
        permanent: false
      });
    } else if (this.state.active && !this.state.permanent) {
      this.setState({
        permanent: true
      });
    } else {
      this.setState({
        active: true,
        permanent: true
      });
    }
  }

  handleMouseOver = () => {
    this.setState({
      alignment: this.getAlignment(),
      active: true,
      position: this.getPosition(),
    });
  }

  handleMouseOut = () => {
    if (this.state.permanent)
      return null;

    this.setState({active: false});
  }

  hideTooltip = () => {
    this.setState({
      permanent: false,
      active: false
    });
  }

  // helper function to get an element's exact position
  getPosition = () => {
    let el = ReactDOM.findDOMNode(this.refs.tooltip);
    let xPosition = 0;
    let yPosition = el.offsetHeight;
  
    while (el) {
      if (el.tagName == "BODY") {
        // deal with browser quirks with body/window/document and page scroll
        let xScrollPos = el.scrollLeft || document.documentElement.scrollLeft;
        let yScrollPos = el.scrollTop || document.documentElement.scrollTop;      

        xPosition += (el.offsetLeft - xScrollPos + el.clientLeft);
        yPosition += (el.offsetTop - yScrollPos + el.clientTop);
      } else {
        xPosition += (el.offsetLeft - el.scrollLeft + el.clientLeft);
        yPosition += (el.offsetTop - el.scrollTop + el.clientTop);
      }

      el = el.offsetParent;
    }
    return {
      x: xPosition,
      y: yPosition
    };
  }

  getTooltipStyles = () => {
    return {
      position: 'fixed',
      top: this.state.position ? this.state.position.y : 0,
      left: this.state.position ? this.state.position.x : 0,
      width: this.props.maxWidth
    }
  }

  render() {
    const tooltipInlineCSS = {
      width: this.props.maxWidth
    };
    const mouseOver = !this.props.clearAll ? () => this.handleMouseOver() : null;
    const mouseOut = !this.props.clearAll ? () => this.handleMouseOut() : null;

    return (
      !this.props.tooltip || this.props.readOnly ? 
        <span>{this.props.labelContent}</span>
      :
        <span className="ptv-tooltip" ref="tooltip">
          
          { this.props.type !== 'standard' ?
            <span
              onClick={() => this.handleClick()}
              onMouseOver={ mouseOver }
              onMouseOut={ mouseOut } >
                { this.props.labelContent }
            </span>
          : <span>
              { this.props.labelContent }
              <PTVIcon name="icon-info-circle"
                onClick={() => this.handleClick()}
                onMouseOver={ mouseOver }
                onMouseOut={ mouseOut }
              />
            </span> 
          }

          { this.state.active ?
              this.props.attachToBody ?
                <PTVRenderInBody> 
                  <span className={cx("tooltip-content", {"hidden": this.state.active ? false : true})} style={this.getTooltipStyles()}> 
                    { this.props.tooltip }              
                  </span>
                </PTVRenderInBody>
              :
                <span className={cx("tooltip-content", this.state.alignment, {"hidden": this.state.active ? false : true}, {"permanent": this.state.permanent ? true : false})} style={tooltipInlineCSS}> 
                    <span className={cx("tooltip-content-arrow")} />
                    <span className="tooltip-content-body">
                      { this.props.tooltip }
                    </span>
                    
                    { this.props.clearAll ?
                      <div className={cx("button-group", {"left-aligned": this.props.clearAll} )}>
                        <ButtonDelete onClick={this.props.clearAll} />
                        <ButtonCancelUpdate onClick={() => this.hideTooltip()} />
                      </div>
                    : null }

                    { this.state.permanent ?
                      <PTVIcon
                        name="icon-cross"
                        onClick={() => this.hideTooltip()} />
                    : null }
                  </span>
          : null }
      
        </span>
    );

  };

}

PTVTooltip.propTypes = {
  tooltip: PropTypes.string,
  onClick: PropTypes.func,
  onMouseOver: PropTypes.func,
  onMouseOut: PropTypes.func,
  maxWidth: PropTypes.number,
  labelContent: PropTypes.any,
  type: PropTypes.string,
  clearAll: PropTypes.func,
  attachToBody: PropTypes.bool
};

PTVTooltip.defaultProps = {
  tooltip: '',
  maxWidth: 375,
  type: 'standard'
}

export default PTVTooltip;
