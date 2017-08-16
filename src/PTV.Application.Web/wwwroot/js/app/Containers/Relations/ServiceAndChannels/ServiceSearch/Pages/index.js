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
import React, { PropTypes, Component } from 'react';
import { Link } from 'react-router';
import { browserHistory } from 'react-router';
import { List } from 'immutable';
import { connect } from 'react-redux';
import { injectIntl, intlShape, FormattedMessage } from 'react-intl'
import shortId from 'shortid';

// Components and containers
import { PTVPreloader, PTVTextInput, PTVLabel, PTVIcon } from '../../../../../Components';
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps';
import ServiceSearchCriteria from '../../Common/Pages/ServiceSearchCriteria';
import ServiceSearchCriteriaTag from '../../Common/Pages/ServiceSearchCriteriaTags';
import ServiceAndChannelCollapsible from '../../Common/Pages/ServiceAndChannelCollapsible';
import ServiceSearchResultTable from '../../Common/Pages/ServiceSearchResultTable';


// Messages
import commonMessages from '../../../../Common/LocalizedMessages';
import * as Messages from '../Messages';

// Actions
import * as serviceSearchActions from '../Actions';
import * as serviceActions from '../../../../Services/Service/Actions';
import * as commonServiceAndChannelActions from '../../Common/Actions'

// Styles
import '../Styles/ServiceSearch.scss';

// Selectors
import * as CommonSelectors from '../../../../Common/Selectors';
import * as ServiceSearchSelectors from '../Selectors';
import * as CommonServiceAndChannelSelectors from '../../Common/Selectors';

class ServiceSearch extends Component {

    onInputChange = (input, isSet=false) => value => {
        this.props.actions.onServiceSearchInputChange(input, value, isSet);
    }

    componentDidMount() {
        const { areDataValid } = this.props;
        if (!areDataValid) {
            this.props.actions.loadServiceSearch();
        }
    }

    handleSubmit = () => {
        this.props.actions.loadServices(false, this.props.language);
        this.props.actions.setServiceSearchExpanded(false);
    }

    render() {

        const { isFetching, serviceName, keyToState, defaultChannelId } = this.props;

        const { formatMessage } = this.props.intl;

        return (
                <div>
                    { isFetching ? <PTVPreloader /> :
                    <div className="form-inline  service-search" role="form">
                        <h3 className="search-title"> {formatMessage(Messages.messages.serviceSearchBoxHeaderTitle)} </h3>
                        <div className="form-group">
                            <PTVTextInput
                                className="with-icon"
                            	inputclass="list-search-field"value={serviceName}
                                id="1"
                                placeholder={formatMessage(Messages.messages.namePlaceholder)}
                                changeCallback={this.onInputChange('serviceName')}
                                onEnterCallBack={this.handleSubmit}
                                name='serviceName'
                                maxLength={100}>
                                <PTVIcon componentClass ="magnifying-glass" name="icon-search" onClick={ this.handleSubmit } />
                            </PTVTextInput>
                        </div>
                        <ServiceAndChannelCollapsible
                            title = { formatMessage(Messages.messages.serviceSearchBoxCollapsibleHeaderTitle) }
                                renderTags = { () => { return ( <ServiceSearchCriteriaTag keyToState = { keyToState } /> )} }
                            renderCriteria = { () => { return ( <ServiceSearchCriteria keyToState = { keyToState } messages = { Messages.messages } />)} }
                            expandedSelector = { ServiceSearchSelectors.getIsExpanded }
                            setExpanded = {  this.props.actions.setServiceSearchExpanded }
                            keyToState = {keyToState}
                        />
                        <ServiceSearchResultTable
                            defaultChannelId = { defaultChannelId }
                            messages = { Messages.messages }
                        />
                    </div> }
                </div>
        );
    }
}

ServiceSearch.propTypes = {
        actions: PropTypes.object,
        intl: intlShape.isRequired,
        language: PropTypes.string
    };

ServiceSearch.defaultProps = {
        keyToState: 'serviceAndChannelServiceSearch',
    };

function mapStateToProps(state, ownProps) {
    const props = { keyToState: 'serviceAndChannelServiceSearch', keyToEntities: 'services' };
  return {
    isFetching: CommonSelectors.getSearchIsFetching(state, props),
    areDataValid: CommonSelectors.getSearchAreDataValid(state, props),
    serviceName: ServiceSearchSelectors.getServiceName(state)
  }
}

const actions = [
    serviceSearchActions,
    serviceActions,
    commonServiceAndChannelActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceSearch));
