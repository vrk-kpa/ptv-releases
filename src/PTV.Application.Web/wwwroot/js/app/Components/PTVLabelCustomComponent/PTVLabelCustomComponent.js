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
import PTVLabel from '../PTVLabel';
import * as ValidationHelper from '../PTVValidations';
import shortid from 'shortid';
import cx from 'classnames';
import { composePTVComponent, getRequiredLabel, ValidatePTVComponent } from '../PTVComponent';

export class PTVLabelCustomComponent extends Component{

    constructor(props) {
        super(props);
    }


    render() {
        const { disabled } = this.props;

        return (
        <div className={ cx(this.props.componentClass) }>
                <PTVLabel labelClass={ this.props.labelClass }
                          tooltip={ this.props.tooltip }
                          readOnly={ this.props.labelReadOnly }>{ getRequiredLabel(this.props) }</PTVLabel>
                {this.props.children}
                <ValidatePTVComponent {...this.props} valueToValidate={ this.props.value || '' } />
        </div>
        );
    }
};

PTVLabelCustomComponent.propTypes = {
  label: PropTypes.string.isRequired,
  componentClass: PropTypes.string,
  children: PropTypes.any.isRequired,
  labelClass: PropTypes.string,
  labelReadOnly: PropTypes.bool
}

PTVLabelCustomComponent.defaultProps = {
  componentClass: ''
}

export default composePTVComponent(PTVLabelCustomComponent);
