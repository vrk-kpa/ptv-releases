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
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl';
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps';
import { PTVTextInput, PTVPreloader,  PTVIcon } from '../../../../../Components';

// Components
import ServiceAndChannelCollapsible from '../../Common/Pages/ServiceAndChannelCollapsible';
import ChannelSearchCriteriaTags from '../../Common/Pages/ChannelSearchCriteriaTags';
import ChannelSearchCriteria from '../../Common/Pages/ChannelSearchCriteria';
import ChannelSearchResultTable from '../../Common/Pages/ChannelSearchResultTable';

// selectors
import * as ChannelSearchSelectors from '../Selectors';
import * as CommonSelectors from '../../../../Common/Selectors';
import { getChannels } from '../../../../Channels/Common/Selectors';
import * as CommonServiceAndChannelSelectors from '../../Common/Selectors';

// actions
import * as channelActions from '../Actions';
import * as commonServiceAndChannelActions from '../../Common/Actions'

// messages
import * as Messages from '../Messages';

// Styles
import '../Styles/ChannelSearch.scss';

class ChannelSearch extends Component {

	    static propTypes = {
        actions: PropTypes.object,
        intl: intlShape.isRequired,
        //channelType: PropTypes.any.isRequired
    };

    onInputChange = (input, isSet=false) => value => {
        this.props.actions.onChannelSearchInputChange(input, value, isSet);
    };
    
    handleSubmit = () => {
        this.props.actions.loadChannelSearchResults();
        this.props.actions.setChannelSearchExpanded(false)
    }

    loadData = props => {
        const { areDataValid, isFetching} = props;
        if (!isFetching && !areDataValid) {
            this.props.actions.loadChannelSearch();
        }
    }

    componentWillReceiveProps(newProps) {
        this.loadData(newProps);
    }

    componentDidMount() {
        this.loadData(this.props);
    }

	render() {
    
    	const { formatMessage } = this.props.intl;

        const { isFetching, channelName, keyToState } = this.props;               

     	return (
            <div>
                <div className="box box-white">
                    <div className="form-wrap channel-search-form">
                         { isFetching ? <PTVPreloader /> :
                        <div className="form-inline channel-search" role="form">
                            <h3 className="search-title"> {formatMessage(Messages.messages.channelSearchBoxHeaderTitle)} </h3>                                                      
                            <div className="form-group">
                                <PTVTextInput
                                    className="with-icon"
                                    inputclass="list-search-field"
                                    value={ channelName }
                                    placeholder={formatMessage(Messages.messages.channelSearchBoxNamePlaceholder)}                                    
                                    name='channelName'
                                    changeCallback={this.onInputChange('channelName')}
                                    onEnterCallBack={this.handleSubmit}
                                    maxLength={100}>
                                    <PTVIcon componentClass ="right" name="icon-search" onClick={ this.handleSubmit } />  
                                </PTVTextInput>
                            </div> 
                            <ServiceAndChannelCollapsible
                                title = { formatMessage(Messages.messages.channelSearchBoxCollapsibleHeaderTitle) }
                                renderTags = { () => { return ( <ChannelSearchCriteriaTags keyToState = { keyToState } />  )} }
                                renderCriteria = { () => { return ( <ChannelSearchCriteria keyToState = { keyToState } messages = { Messages.messages } />)} }
                                expandedSelector = { ChannelSearchSelectors.getIsExpanded }
                                setExpanded = {  this.props.actions.setChannelSearchExpanded }
                                keyToState = {keyToState}
                            />  
                            <ChannelSearchResultTable
                                messages = { Messages.messages }
                                keyToState = {keyToState}
                            />
                        </div>}
                        <div className='clearfix'></div>
                    </div>
                </div>
	  		</div>
	    )
    }
}

ChannelSearch.defaultProps = {
        keyToState: 'serviceAndChannelChannelSearch',
    };

function mapStateToProps(state, ownProps) {
    const props = {keyToState: 'serviceAndChannelChannelSearch', keyToEntities: 'channels'};
    return {
        isFetching: CommonSelectors.getSearchIsFetching(state, props),
        channelName: ChannelSearchSelectors.getChannelName(state, props),
        areDataValid: CommonSelectors.getSearchAreDataValid(state, props)             
    }
}

const actions = [
   channelActions,
   commonServiceAndChannelActions
 ];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ChannelSearch));
