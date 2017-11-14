/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the 'Software'), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { Link, browserHistory } from 'react-router'
import { intlShape, defineMessages } from 'react-intl'
import shortId from 'shortid'
import { PTVValidationManager, PTVHeaderSection, PTVLabel, PTVIcon } from 'Components'
import { Tabs, Tab } from 'Components/PTVTabs'
import ServiceAndChannelInfoSection from 'Containers/Relations/ServiceAndChannels/Common/Pages/ServiceAndChannelInfoSection'
import ServiceAndChannelButtons from 'Containers/Relations/ServiceAndChannels/Common/Pages/ServiceAndChannelButtons'
import ServiceAndChannelConfirmOverlay from 'Containers/Relations/ServiceAndChannels/Common/Pages/ServiceAndChannelConfirmOverlay'
import ServiceSearch from 'Containers/Relations/ServiceAndChannels/ServiceSearch/Pages'
import ChannelSearch from 'Containers/Relations/ServiceAndChannels/ChannelSearch/Pages'
import AccordionGrid from 'Containers/Relations/ServiceAndChannels/Common/Pages/RelationAccordionGrid/AccordionGrid'
import NotificationContainer from 'Containers/Common/NotificationContainer'
import { confirmationMessages } from 'Containers/Relations/ServiceAndChannels/Common/Messages'
import { ButtonClearList } from 'Containers/Common/Buttons'
import { confirmationTypes } from 'Containers/Relations/ServiceAndChannels/Common/Helpers/confirmationTypes'

const messages = defineMessages({
  pageHeaderTitle: {
    id: 'Containers.ServiceAndChannelRelations.ServiceAndChannelPage.Header.Title',
    defaultMessage: 'Liitokset'
  },
  pageHeaderDescription: {
    id: 'Containers.ServiceAndChannelRelations.ServiceAndChannelPage.Header.Description',
    defaultMessage: 'Tässä osiossa voit lisätä, katsella tai muokata liitoksia palvelujen ja asiointikanavien välillä. Hae ja valitse ensin palvelut, jolloin näet palveluun jo liitetyt asiointikanavat. Liitä uusia asiointikanavia hakemalla ja valitsemalla sivulla olevasta Hae ja lisää   asiointikanavia hausta.'
  },
  linkGoBack: {
    id: 'Containers.ServiceAndChannelRelations.ServiceAndChannelPage.Link.Back',
    defaultMessage: 'Takaisin'
  },
  tabItemServices: {
    id: 'Containers.ServiceAndChannelRelations.ServiceAndChannelPage.TabItem.Services.Title',
    defaultMessage: 'Palvelut'
  },
  tabItemChannels: {
    id: 'Containers.ServiceAndChannelRelations.ServiceAndChannelPage.TabItem.Channels.Title',
    defaultMessage: 'Asiointikanavat'
  },
  tabItemNoServicesSelected: {
    id: 'Containers.ServiceAndChannelRelations.ServiceAndChannelPage.TabContent.ServicesNotSelected.Title',
    defaultMessage: 'Palveluja ei valittu'
  }
})

class Relations extends Component {
  getChildContext = () => ({
    ValidationManager: this.validationManager
  })

  validationManager = new PTVValidationManager()

  constructor (props) {
    super(props)
    this.state = {
      redirectLoad: false
    }
  }

  handleTabChange = index => {
    this.props.actions.setChannelSearchTab(index)
  }

  goBack = () => {
    if (this.props.location.state && this.props.location.state.serviceId) {
      this.props.actions.clearApiCall(['service', 'step4Form'], { model: { result: null } })
      browserHistory.push({ pathname :'/service/manageService' })
    }
    if (this.props.location.state && this.props.location.state.channelId) {
      this.props.actions.clearApiCall([this.props.location.state.keyToState,'channelServiceStep'], { model: { result: null } })
      browserHistory.push({ pathname: '/channels/manage/' + this.props.location.state.keyToState })
    }
  }

  componentWillReceiveProps(nextProps) {
    if ((this.props.isFetching != nextProps.isFetching) || (this.props.areDataValid != nextProps.areDataValid)) {
      if (!nextProps.isFetching && nextProps.areDataValid) {
        const {actions, newConnectedChannelIds} = this.props
        actions.onServiceSearchListRemove()
        actions.onChannelSearchListRemove()
        newConnectedChannelIds.map(id => {
          actions.onChannelRelationInputChange('isNew', id, false)
        })
      }
    }
      if(!this.state.redirectLoad && ((this.props.searchIsFetching != nextProps.searchIsFetching) || (this.props.searchAreDataValid != nextProps.searchAreDataValid)) &&
          (!nextProps.searchIsFetching && nextProps.searchAreDataValid) && nextProps.searchedServices && this.props.editedEntity){
              const {actions, countOfConnectedServices} = this.props;
              if(countOfConnectedServices == 0){
                  actions.setServiceSearchExpanded(false);
                  actions.setChannelSearchExpanded(true);
                  actions.setRelationsReadOnlyMode(false);
                  this.setState({redirectLoad:true})
                  nextProps.searchedServices.forEach(service => actions.onConnectedEntityAdd(
                      { connectedServices: [ { uiId: shortId.generate(), service: service.toJS(), channelRelations: service.get('channelRelations').toJS() }] })
                  );
              }
      }
  }

  clearList = () => {
      this.props.actions.setRelationsConfirmation(confirmationTypes.CLEAR_PAGE, true);
  }

  onClearServiceAndChannelsAccept = () =>{
      this.props.actions.onServiceAndChannelsListRemove();
      this.props.actions.onChannelRelationsEntityClearAll();
      this.props.actions.clearRelationsConfirmation();
  }

  render() {
      const { formatMessage } = this.props.intl;
      const { keyToState, readOnly, hideRightSearchPannel, countOfConnectedServices, language } = this.props;
      const className = (readOnly || hideRightSearchPannel) ? 'col-lg-12' : 'col-lg-8'
      return (
              <div className='box box-white'>
                <div className='form-wrap'>
                  { this.props.location.state && (this.props.location.state.serviceId  || this.props.location.state.channelId)?
                  <PTVLabel>
                      <Link onClick = {this.goBack}>
                          <PTVIcon name='icon-angle-left' className='brand-fill'/>
                          {formatMessage(messages.linkGoBack)}
                      </Link>
                  </PTVLabel>: null}
                  <NotificationContainer
                      keyToState = { keyToState }
                      validationMessages={ [] }
                  />
                  { !readOnly ?
                      <div className='button-group'>
                          <ButtonClearList onClick={this.clearList} disabled = {this.props.isAnyRelation}/>
                      </div>
                  : null }
                  <ServiceAndChannelConfirmOverlay
                          isVisible = {this.props.confirmationData.has(confirmationTypes.CLEAR_PAGE)}
                          description = {formatMessage(confirmationMessages.clearAllServiceAndChannelsTitle)}
                          acceptCallback = { () => this.onClearServiceAndChannelsAccept(this.props.confirmationData.get(confirmationTypes.CLEAR_PAGE)) }
                      />
                  <PTVHeaderSection
                      titleClass = 'card-title'
                      title= {formatMessage(messages.pageHeaderTitle)}
                      descriptionClass = 'lead'
                      description = {formatMessage(messages.pageHeaderDescription)}
                      showButton = { false }
                  />
                  <div className='row'>
                      <div className = {className}>
                          <div className='box box-white'>
                              <div className='box-inner'>
                                  <ServiceAndChannelInfoSection
                                      keyToState = { keyToState }
                                      readOnly = { readOnly }
                                      language = { language }
                                      isChannelSearchButtonDisabled = { this.props.currentTabIndex === 1 }
                                  />
                                  <ServiceAndChannelButtons
                                      keyToState = { keyToState }
                                      readOnly = { readOnly }
                                      language = { language }
                                  />
                                  <AccordionGrid
                                      componentClass = 'accordion-grid'
                                      keyToState = { keyToState }
                                      readOnly = { readOnly }
                                      language = { language }
                                  />
                                  <ServiceAndChannelButtons
                                      keyToState = { keyToState }
                                      readOnly = { readOnly }
                                      language = { language }
                                      buttonSaveUp
                                  />
                              </div>
                          </div>
                      </div>
                      {(readOnly || hideRightSearchPannel) ? null
                          :<div className = 'col-lg-4'>
                              <div style={{position: 'relative'}}>
                                  <Tabs
                                      index={this.props.currentTabIndex}
                                      onChange={this.handleTabChange}
                                      framed
                                  >
                                      <Tab label={formatMessage(messages.tabItemServices)}>
                                          <div>
                                              <ServiceSearch
                                                  language = { language }
                                              />
                                          </div>
                                      </Tab>
                                      <Tab label={formatMessage(messages.tabItemChannels)}>
                                          <div>
                                              {countOfConnectedServices > 0 ? <ChannelSearch language = { language } /> : formatMessage(messages.tabItemNoServicesSelected)}
                                          </div>
                                      </Tab>
                                  </Tabs>
                              </div>
                          </div>
                      }
                  </div>
                </div>
              </div>
      );
  }
}

Relations.propTypes = {
        actions: PropTypes.object,
        intl: intlShape.isRequired
    };

Relations.defaultProps = {
        keyToState: 'serviceAndChannel',
    };

Relations.childContextTypes = {
    ValidationManager: React.PropTypes.object
};

export default Relations
