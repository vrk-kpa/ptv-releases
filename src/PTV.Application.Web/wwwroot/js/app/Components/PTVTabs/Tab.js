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
import classnames from 'classnames';
import styles from './styles.scss';

class Tab extends Component {
  static propTypes = {
    active: PropTypes.bool,
    activeClassName: PropTypes.string,
    className: PropTypes.string,
    disabled: PropTypes.bool,
    hidden: PropTypes.bool,
    label: PropTypes.any.isRequired,
    onActive: PropTypes.func,
    onClick: PropTypes.func
  };

  static defaultProps = {
    active: false,
    className: '',
    disabled: false,
    hidden: false
  };

  componentDidUpdate(prevProps) {
    if (!prevProps.active && this.props.active && this.props.onActive) {
      this.props.onActive();
    }
  }

  handleClick = (event) => {
    if (!this.props.disabled && this.props.onClick) {
      this.props.onClick(event);
    }
  };

  render() {
    const {
      onActive, // eslint-disable-line
      active, activeClassName, className, disabled, hidden, ...other
    } = this.props;
    const _className = classnames("tab-label", {
      "active": active,
      "hidden": hidden,
      "disabled": disabled,
    }, className);

    return (
      <label {...other} 
        className={_className}
        onClick={this.handleClick}
      >
        {this.props.label}
      </label>
    );
  }
}

export default Tab;
