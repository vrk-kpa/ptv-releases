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
import React, {PropTypes, Component} from 'react';
import {Map} from 'immutable';
import {bindActionCreators} from 'redux';
import {connect} from 'react-redux';
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl'
import * as PTVValidatorTypes from '../../Components/PTVValidators';
import { PTVButton } from '../../Components';
import './Styles.scss';
import PTVValidationManager from '../../Components/PTVValidationManager';

class ToolsContainer extends Component {
    static propTypes = {
    };

    static childContextTypes = {
        ValidationManager: React.PropTypes.object
      };

    getChildContext(){
        return { ValidationManager: this.validationManager};
    };

    validationManager = new PTVValidationManager();

    validators = [PTVValidatorTypes.IS_REQUIRED];

    constructor(props) {
        super(props);
        this.state = {forceLoadState: localStorage.getItem('forceLoadState')};
    }

    componentDidMount() {
    }

    componentWillUpdate(newState){
    }

    onClick = () => {
        if (this.state.forceLoadState === 'true') {
            localStorage.setItem('forceLoadState', false);
            this.setState({forceLoadState: 'false'});
        }
        else {
            localStorage.setItem('forceLoadState', true);
            this.setState({forceLoadState: 'true'});
        }
    }

    render() {
    	const { formatMessage } = this.props.intl;
        return (
            <div className="card service-page">
            <div className="step-2">
                <PTVButton onClick = { this.onClick } >Toggle load state { this.state.forceLoadState }</PTVButton>
                <div className="clearfix"></div>
            </div>
            </div>

       );
    }
}

export default (injectIntl(ToolsContainer));
