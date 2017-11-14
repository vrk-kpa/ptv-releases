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
import React, {PropTypes} from 'react';
import cx from 'classnames';
import PTVLabel from '../PTVLabel';
import styles from './styles.scss';

class PTVRadioButton extends React.Component {

  handleClick = event => {
    const {checked, disabled, onChange, value} = this.props;
    if (event.pageX !== 0 && event.pageY !== 0) this.blur();
    if (!disabled && !checked && onChange) onChange(value);
  };

  blur() {
    this.refs.input.blur();
  }

  focus() {
    this.refs.input.focus();
  }

  render() {
    const className = cx(styles[this.props.disabled ? 'disabled' : 'field'],
      {[styles.small]: this.props.small},
      this.props.className);
    const {onChange, small, ...others} = this.props; // eslint-disable-line no-unused-vars

    return (
      <div className={cx({"checked": this.props.checked}, "ptv-radio")}>
        <label className={cx(className, 'ptv-label')}> 
            <input
              {...others}
              className={styles.input}
              onClick={this.handleClick}
              readOnly
              ref="input"
              type="radio"
            />
            <div className="check"></div>
          {this.props.label ? <span>{this.props.label}</span> : null}
        </label>
      </div>
    );
  }
}

PTVRadioButton.propTypes = {
  checked: PropTypes.bool,
  className: PropTypes.string,
  disabled: PropTypes.bool,
  label: PropTypes.string,
  name: PropTypes.string,
  onBlur: PropTypes.func,
  onChange: PropTypes.func,
  onFocus: PropTypes.func,
  small: PropTypes.bool,
  value: PropTypes.any
};

PTVRadioButton.defaultProps = {
  checked: false,
  className: '',
  disabled: false,
  small: false
};

export default PTVRadioButton;
