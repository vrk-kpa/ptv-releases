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
import React,  {Component, PropTypes} from 'react';
import styles from './styles.scss';
import cx from 'classnames';

const PTVIcon = (props) => (
  <span className={cx(props.componentClass,"icon-wrap")} style={{"height":props.height}} data-tooltip={props.tooltip}>
    <svg className={cx(props.name, props.className)} height={props.height} width={props.width} onClick={props.onClick} 
      onMouseOver={props.onMouseOver} onMouseOut={props.onMouseOut}>
      <use xlinkHref={ `/images/svgdefs.svg#${props.name}` }></use>
    </svg>
  </span>
);

PTVIcon.propTypes = {
  tooltip: PropTypes.string,
  width: PropTypes.number,
  height: PropTypes.number,
  name: PropTypes.string.isRequired,
  className: PropTypes.string,
  componentClass: PropTypes.string,
  onClick: PropTypes.func,
  onMouseOver: PropTypes.func,
  onMouseOut: PropTypes.func
};

PTVIcon.defaultProps = {
  width: 20,
  height: 20
}

export default PTVIcon;
