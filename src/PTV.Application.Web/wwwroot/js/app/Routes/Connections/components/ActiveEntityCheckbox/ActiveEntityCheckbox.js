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
import { Checkbox } from 'sema-ui-components'
import { compose, bindActionCreators } from 'redux'
import { connect } from 'react-redux'
import { getSelections } from 'selectors/base'
import { createSelector } from 'reselect'
import {
  addConnectionsActiveEntity,
  removeConnectionsActiveEntity
} from 'reducers/selections'
import { getConnectionsAddToAllEntities } from 'selectors/selections'

class ActiveEntityCheckbox extends Component {
  handleOnChange = () => {
    const {
      checked,
      connectionIndex,
      removeConnectionsActiveEntity,
      addConnectionsActiveEntity
    } = this.props
    checked
      ? removeConnectionsActiveEntity(connectionIndex)
      : addConnectionsActiveEntity(connectionIndex)
  }
  render () {
    const { shouldShow } = this.props
    return shouldShow && (
      <Checkbox
        checked={this.props.checked}
        onChange={this.handleOnChange}
      />
    )
  }
}
ActiveEntityCheckbox.propTypes = {
  shouldShow: PropTypes.bool.isRequired,
  checked: PropTypes.bool.isRequired,
  addConnectionsActiveEntity: PropTypes.func.isRequired,
  removeConnectionsActiveEntity: PropTypes.func.isRequired,
  connectionIndex: PropTypes.number.isRequired
}

const getConnectionIndex = (_, { connectionIndex }) => connectionIndex
const makeGetIsChecked = () => createSelector(
  [getSelections, getConnectionIndex],
  (selections, connectionIndex) => selections
    .get('connectionsActiveEntities')
    .has(connectionIndex)
)
const makeMapStateToProps = () => {
  const getIsChecked = makeGetIsChecked()
  return (state, props) => ({
    checked: getIsChecked(state, props),
    shouldShow: !getConnectionsAddToAllEntities(state)
  })
}

const mapDispatchToProps = dispatch => bindActionCreators({
  addConnectionsActiveEntity,
  removeConnectionsActiveEntity
}, dispatch)

export default compose(
  connect(makeMapStateToProps, mapDispatchToProps)
)(ActiveEntityCheckbox)
