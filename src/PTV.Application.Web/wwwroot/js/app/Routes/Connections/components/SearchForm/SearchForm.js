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
import React, { PureComponent } from 'react'
import { connect } from 'react-redux'
import { compose } from 'redux'
import PropTypes from 'prop-types'
import { Tabs, Tab } from 'sema-ui-components'
import {
  SearchServicesForm,
  SearchChannelsForm
} from 'Routes/Connections/components'
import ChannelTabTitle from './ChannelTabTitle'
import ServiceTabTitle from './ServiceTabTitle'
import {
  getHasResults,
  getShouldSearchProposedChannels,
  getNotConnectedProposedChannelIds
} from 'Routes/Connections/selectors'
import { setConnectionsEntity } from 'reducers/selections'
import { getConnectionsEntity } from 'selectors/selections'
import { apiCall3 } from 'actions'
import { entityTypesEnum } from 'enums'
import { EntitySchemas } from 'schemas'
import cx from 'classnames'
import styles from './styles.scss'

class SearchForm extends PureComponent {
  handleOnSearchTypeChange = index => {
    this.props.searchProposedChannelsIfNeccessary()
    this.props.setEntity({
      1: entityTypesEnum.CHANNELS,
      0: entityTypesEnum.SERVICES
    }[index])
  }
  get activeIndex () {
    return {
      channels: 1,
      services: 0
    }[this.props.selectedEntity]
  }
  render () {
    const { hasResults } = this.props
    const tabsClass = cx({ [styles.isConnected]: hasResults })
    return (
      <div>
        <Tabs
          index={this.activeIndex}
          onChange={this.handleOnSearchTypeChange}
          asBookmark
          tabAlignment='between'
          className={tabsClass}
        >
          <Tab label={<ServiceTabTitle />}>
            <SearchServicesForm />
          </Tab>
          <Tab label={<ChannelTabTitle />}>
            <SearchChannelsForm />
          </Tab>
        </Tabs>
      </div>
    )
  }
}
SearchForm.propTypes = {
  setEntity: PropTypes.func.isRequired,
  selectedEntity: PropTypes.oneOf([entityTypesEnum.SERVICES, entityTypesEnum.CHANNELS]),
  hasResults: PropTypes.bool,
  searchProposedChannelsIfNeccessary: PropTypes.func
}

const searchProposedChannelsIfNeccessary = () => ({ dispatch, getState }) => {
  const state = getState()
  const shouldSearchProposedChannels = getShouldSearchProposedChannels(state)
  if (shouldSearchProposedChannels) {
    const channelIds = getNotConnectedProposedChannelIds(state)
    dispatch(
      apiCall3({
        keys: [
          'connections',
          'channelSearch'
        ],
        payload: {
          endpoint: 'channel/GetConnectionsChannels',
          data: { channelIds }
        },
        saveRequestData: true,
        schemas: EntitySchemas.GET_SEARCH(EntitySchemas.CHANNEL)
      })
    )
  }
}
export default compose(
  connect(state => ({
    selectedEntity: getConnectionsEntity(state),
    hasResults: getHasResults(state),
    proposedChannelsIds: getNotConnectedProposedChannelIds(state)
  }), {
    setEntity: setConnectionsEntity,
    searchProposedChannelsIfNeccessary
  })
)(SearchForm)
