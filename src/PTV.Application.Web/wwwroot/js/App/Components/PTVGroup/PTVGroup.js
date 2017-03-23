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
import PTVLabel from '../PTVLabel';
import '../Styles/PTVCommon.scss'
import { composePTVComponent, ValidatePTVComponent, getRequiredLabel } from '../PTVComponent';
import cx from 'classnames';

export class PTVGroup extends Component {

	constructor(props) {
		super(props);
	}


	render() {
		// console.log('ptv group', this.props.isAnySelected);
		return (
			<div className={cx('form-group', this.props.className)}>
				<div className="row">
          <div className={this.props.labelClassName}>
					  <PTVLabel readOnly={this.props.readOnly} tooltip={this.props.labelTooltip}>{getRequiredLabel(this.props, this.props.labelContent)}</PTVLabel>  
				  </div>
        </div>
        <div className="row">
          {this.props.children}
        </div>
				<ValidatePTVComponent {...this.props} valueToValidate={ this.props.isAnySelected ? 'valid' : '' } />
			</div>
			);
	}
}

export default composePTVComponent(PTVGroup);
