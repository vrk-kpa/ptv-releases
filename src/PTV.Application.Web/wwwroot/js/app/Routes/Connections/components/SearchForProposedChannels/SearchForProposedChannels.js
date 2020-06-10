/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { compose } from 'redux'
import {
  getIsProposedChannelsButtonVisible,
  getNotConnectedProposedChannelsIds
} from 'Routes/Connections/selectors'
import { apiCall3 } from 'actions'
import { EntitySchemas } from 'schemas'

class SearchForProposedChannels extends Component {
  render () {
    const {
      isVisible,
      search
    } = this.props
    return isVisible && (
      <div onClick={search}>
        SearchForProposedChannels
      </div>
    )
  }
}
SearchForProposedChannels.propTypes = {
  isVisible: PropTypes.bool,
  search: PropTypes.func
}

const searchProposedChannels = () => ({ dispatch, getState }) => {
  const state = getState()
  const channelIds = getNotConnectedProposedChannelsIds(state)
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
export default compose(
  connect(state => ({
    isVisible: getIsProposedChannelsButtonVisible(state)
  }), {
    search: searchProposedChannels
  })
)(SearchForProposedChannels)
