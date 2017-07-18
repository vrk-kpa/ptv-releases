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
import PTVIcon from '../PTVIcon';

class PTVRenderInBody extends Component {
	constructor(props) {
    super(props);
  };

  componentDidMount = () => {
    this.popup = document.createElement("div");
    document.body.appendChild(this.popup);
    this._renderLayer();
  };


  componentDidUpdate = () => {
    this._renderLayer();
  };


  componentWillUnmount = () => {
    ReactDOM.unmountComponentAtNode(this.popup);
    document.body.removeChild(this.popup);
  };


  _renderLayer = () => {
    ReactDOM.render(this.props.children, this.popup);
  };


  render = () => {
    // Render a placeholder
    return null;
  };

}

PTVRenderInBody.propTypes = {
  //tooltip: PropTypes.string
};

PTVRenderInBody.defaultProps = {
  //tooltip: ''
}

export default PTVRenderInBody;