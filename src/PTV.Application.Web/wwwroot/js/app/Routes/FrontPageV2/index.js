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
import React, { PureComponent, PropTypes } from 'react'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { loadTranslatableLanguages } from 'Containers/Common/Actions'
import { onRecordSelect, loadFrontPageData } from './actions'
import {
  Button,
  ButtonMenu,
  PageHeader,
  Tabs,
  Tab
} from 'sema-ui-components'
import { SecurityCreate, SecurityCreateAny } from 'appComponents/Security'
import Divider from 'Components/Divider/Divider'
import { Link, browserHistory } from 'react-router'
import { defineMessages, injectIntl } from 'react-intl'
import { getAreFrontPageDataLoaded } from 'Routes/FrontPageV2/selectors'

const messages = defineMessages({
  pageTitle: {
    id: 'FrontPage.Search.Title',
    defaultMessage: 'Palvelutietovaranto'
  },
  pageDescription: {
    id: 'FrontPage.Search.Description',
    defaultMessage: 'Palvelu-ja pohjakuvaukset, asiontikanavat, organisaatiokuvaukset ja rekisterikuvaukset'
  },
  navigationSearch: {
    id: 'FrontPage.Navigation.Search.Title',
    defaultMessage: 'Kuvaukset'
  },
  navigationRelations: {
    id: 'FrontPage.Navigation.Relations.Title',
    defaultMessage: 'Liitokset'
  },
  addNewContentTitle: {
    id: 'FrontPage.Buttons.AddNewContent.Title',
    defaultMessage: 'Luo uusi'
  },
  eChannelLinkTitle: {
    id: 'FrontPage.AddNewMenu.ElectronicChannel.LinkTitle',
    defaultMessage: 'Verkkoasiointi'
  },
  webPageLinkTitle: {
    id: 'FrontPage.AddNewMenu.WebPage.LinkTitle',
    defaultMessage: 'Verkkosivu'
  },
  printableFormLinkTitle: {
    id: 'FrontPage.AddNewMenu.PrintableForm.LinkTitle',
    defaultMessage: 'Tulostettava lomake'
  },
  phoneLinkTitle: {
    id: 'FrontPage.AddNewMenu.Phone.LinkTitle',
    defaultMessage: 'Puhelinasiointi'
  },
  serviceLocationLinkTitle: {
    id: 'FrontPage.AddNewMenu.ServiceLocation.LinkTitle',
    defaultMessage: 'Palvelupiste'
  },
  services: {
    id: 'FrontPage.AddNewMenu.Services.LinkTitle',
    defaultMessage: 'Palvelukuvaus'
  },
  channels: {
    id: 'FrontPage.AddNewMenu.Channels.LinkTitle',
    defaultMessage: 'Asiontikanava'
  },
  generalDescriptions: {
    id: 'FrontPage.AddNewMenu.GeneralDescription.LinkTitle',
    defaultMessage: 'Pohjakuvaukset'
  },
  organizations: {
    id: 'FrontPage.AddNewMenu.Organizations.LinkTitle',
    defaultMessage: 'Organisaatiokuvaukset'
  }
})

class FrontPageRoute extends PureComponent {
  // TODO: move loading logic to one server call //
  componentWillMount () {
    if (!this.props.isLoadNeeded) {
      this.props.loadFrontPageData()
      this.props.loadTranslatableLanguages()
      this._setActiveIndexByRouteName(
        this.props.location.pathname.replace('/frontpage', '')
      )
    }
  }
  _setActiveIndexByRouteName = routeName => {
    const activeIndex = {
      '/search': 0,
      '/relations': 1
    }[routeName] || 0
    this.setState({ activeIndex })
  }
  state = { activeIndex: 0 }
  _createTabOnClickHandler = routeName => () => {
    this._setActiveIndexByRouteName(routeName)
    browserHistory.push(`/frontpage${routeName}`)
  }
  _newLinkOnClickHandler = keyToState => () => {
    if (this.props.onRecordSelect) {
      this.props.onRecordSelect(null, keyToState)
    }
  }

  handleSearchOnClick = this._createTabOnClickHandler('/search')
  handleRelationsOnClick = this._createTabOnClickHandler('/relations')
  handleNewEChannelClick = this._newLinkOnClickHandler('eChannel')
  handleNewWebPageClick = this._newLinkOnClickHandler('webPage')
  handleNewPrintableFormClick = this._newLinkOnClickHandler('printableForm')
  handleNewServiceLocationClick = this._newLinkOnClickHandler('serviceLocation')
  handleNewPhoneClick = this._newLinkOnClickHandler('phone')
  handleNewServiceClick = this._newLinkOnClickHandler('service')
  handleNewOrganizationClick = this._newLinkOnClickHandler('organization')
  handleNewGeneralDescriptionClick = this._newLinkOnClickHandler('generalDescription')

  render () {
    const {
      intl: { formatMessage },
      children
    } = this.props
    return (
      <div>
        <PageHeader
          title={formatMessage(messages.pageTitle)}
          description={formatMessage(messages.pageDescription)}
        />
        <div style={{ position: 'relative' }}>
          <div style={{
            position: 'absolute',
            top: 0,
            right: 0,
            zIndex: 10
          }}>
            <SecurityCreateAny isNotAccessibleComponent={null}>
              <ButtonMenu
                buttonLabel={formatMessage(messages.addNewContentTitle)}
                align='right'
                elements={[
                  {
                    header: null,
                    content: (
                      <ul>
                        <SecurityCreate keyToState='service' >
                          <li>
                            <Link to='/service/manageService' onClick={this.handleNewServiceClick}>
                              {formatMessage(messages.services)}
                            </Link>
                            <Divider componentClass='symmetric-margin' />
                          </li>
                        </SecurityCreate>
                        <SecurityCreate domain='channels' >
                          <li>
                            <div><strong>{formatMessage(messages.channels)}</strong></div>
                          </li>
                          <li>
                            <Link to='/channels/manage/eChannel' onClick={this.handleNewEChannelClick}>
                              {formatMessage(messages.eChannelLinkTitle)}
                            </Link>
                          </li>
                          <li>
                            <Link to='/channels/manage/webPage' onClick={this.handleNewWebPageClick}>
                              {formatMessage(messages.webPageLinkTitle)}
                            </Link>
                          </li>
                          <li>
                            <Link to='/channels/manage/printableForm' onClick={this.handleNewPrintableFormClick}>
                              {formatMessage(messages.printableFormLinkTitle)}
                            </Link>
                          </li>
                          <li>
                            <Link to='/channels/manage/phone' onClick={this.handleNewPhoneClick}>
                              {formatMessage(messages.phoneLinkTitle)}
                            </Link>
                          </li>
                          <li>
                            <Link to='/channels/manage/serviceLocation' onClick={this.handleNewServiceLocationClick}>
                              {formatMessage(messages.serviceLocationLinkTitle)}
                            </Link>
                            <Divider componentClass='symmetric-margin' />
                          </li>
                        </SecurityCreate>
                        <SecurityCreate keyToState='generalDescription' >
                          <li>
                            <Link to='/manage/manage/generalDescription' onClick={this.handleNewGeneralDescriptionClick}>
                              {formatMessage(messages.generalDescriptions)}
                            </Link>
                            <Divider componentClass='symmetric-margin' />
                          </li>
                        </SecurityCreate>
                        <SecurityCreate keyToState='organization' >
                          <li>
                            <Link to='/manage/manage/organization' onClick={this.handleNewOrganizationClick} >
                              {formatMessage(messages.organizations)}
                            </Link>
                          </li>
                        </SecurityCreate>
                      </ul>
                    )
                  }
                ]}
              />
            </SecurityCreateAny>
          </div>
          <Tabs
            index={this.state.activeIndex}
            className='indented'
          >
            <Tab onClick={this.handleSearchOnClick} label={formatMessage(messages.navigationSearch)} />
            <Tab onClick={this.handleRelationsOnClick} label={formatMessage(messages.navigationRelations)} />
          </Tabs>
          <div className='search-table'>
            {children}
          </div>
        </div>
      </div>
    )
  }
}
FrontPageRoute.propTypes = {
  intl: PropTypes.object,
  children: PropTypes.any,
  location: PropTypes.object,
  loadFrontPageData: PropTypes.func,
  loadTranslatableLanguages: PropTypes.func,
  onRecordSelect: PropTypes.func
}

export default compose(
  connect(state => ({
    isLoadNeeded: getAreFrontPageDataLoaded(state)
  }),
    {
      loadTranslatableLanguages,
      onRecordSelect,
      loadFrontPageData
    }
  ),
  injectIntl
)(FrontPageRoute)
