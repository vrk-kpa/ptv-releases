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
import { injectIntl, intlShape } from 'react-intl'
import {connect} from 'react-redux';
import mapDispatchToProps from '../../Configuration/MapDispatchToProps';
import * as appActions from './Actions';
// Selectors
/// Selectors
import { getErrorMessages, getInfos } from './Selectors';

/// PTV Components
import { PTVMessageBox } from '../../Components';

/// App Helperes
import { translateMessages } from '../../ServerMessages/ServerMessages';

class NotificationContainer extends Component {

    constructor(props) {
        super(props);
    }

    onMessageClose = (id) => {
        this.props.actions.resetMessages([this.props.keyToState, this.props.notificationKey, id]);
    }

    render = () => {
        const { intl: { formatMessage}, errors, infos, validationMessages } = this.props;
        return (<div>
                    { validationMessages.length > 0 ? <PTVMessageBox messages={ validationMessages } /> : null}
                    { errors.length > 0 ? <PTVMessageBox referenceId="errors" onClose={this.onMessageClose} messages={ translateMessages(errors, formatMessage) }/> : null}
                    { infos.length > 0 ? <PTVMessageBox referenceId="infos" isValid = { true } onClose={this.onMessageClose} messages={ translateMessages(infos, formatMessage) }/> : null}
                    
        </div>)
    }

}

NotificationContainer.propTypes = {
    validationMessages: PropTypes.array,
    isFetching: PropTypes.bool,
    intl: intlShape.isRequired,
    keyToState: PropTypes.string,
    notificationKey: PropTypes.string
};

NotificationContainer.defaultProps = {
    validationMessages: [],
    isFetching: false,
    keyToState: PropTypes.string,
    notificationKey: 'all'
}


function mapStateToProps(state, ownProps) {
  
  return {
    errors: getErrorMessages(state, ownProps), 
    infos: getInfos(state, ownProps), 
  }
}

const actions = [
    appActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(NotificationContainer));