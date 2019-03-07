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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { Field, arrayRemove } from 'redux-form/immutable'
import { getConnectionsMainEntity, hasConnectionOpen } from 'selectors/selections'
import { getMainEntitiesCount } from 'Routes/Connections/selectors'
import { compose } from 'redux'
import { connect } from 'react-redux'
import EntityDescription from 'appComponents/EntityDescription'
import ChannelTypeCell from 'appComponents/Cells/ChannelTypeCell'
import ServiceTypeCell from 'appComponents/Cells/ServiceTypeCell'
import { PTVIcon } from 'Components'
import styles from './styles.scss'
import {
  setConnectionOpenIndex,
  setConnectionsMainEntity
} from 'reducers/selections'

class MainEntity extends Component {
  handleOnRemove = () => {
    this.props.removeEntity(this.props.connectionIndex)
    if (this.props.hasOpenChild) {
      this.props.closeConnection()
    }
    if (this.props.mainEntitiesCount === 1) {
      this.props.resetMainEntity()
    }
  }
  childRender = ({ input }) => {
    const { mainEntity } = this.props
    return (
      <div className='row align-items-center'>
        <div className='col-lg-12'>
          <EntityDescription
            name={input.value.get('name')}
            entityId={input.value.get('id')}
            unificRootId={input.value.get('unificRootId')}
            organizationId={input.value.get('organizationId')}
            entityConcreteType={mainEntity === 'services'
              ? 'service'
              : input.value.get('channelType')}
          />
        </div>
        <div className='col-lg-4'>
          {mainEntity === 'services'
            ? <ServiceTypeCell
              serviceTypeId={input.value.get('serviceType')}
            />
            : <ChannelTypeCell
              channelTypeId={input.value.get('channelTypeId')}
            />}
        </div>
        <div className='col-lg-8'>
          <div className={styles.actions}>
            <PTVIcon onClick={this.handleOnRemove} name={'icon-trash'} />
          </div>
        </div>
      </div>
    )
  }
  render () {
    const { mainEntity } = this.props
    return mainEntity && (
      <Field
        name={'mainEntity'}
        component={this.childRender}
      />
    )
  }
}
MainEntity.propTypes = {
  mainEntity: PropTypes.oneOf(['channels', 'services']),
  removeEntity: PropTypes.func.isRequired,
  resetMainEntity: PropTypes.func.isRequired,
  closeConnection: PropTypes.func.isRequired,
  connectionIndex: PropTypes.number.isRequired,
  mainEntitiesCount: PropTypes.number.isRequired,
  hasOpenChild: PropTypes.bool
}

export default compose(
  connect((state, ownProps) => ({
    mainEntity: getConnectionsMainEntity(state),
    hasOpenChild: hasConnectionOpen(state, { connectionIndex: ownProps.connectionIndex }),
    mainEntitiesCount: getMainEntitiesCount(state)
  }), {
    removeEntity: connectionIndex => ({ dispatch }) => {
      dispatch(
        arrayRemove(
          'connectionsWorkbench',
          `connections`,
          connectionIndex
        )
      )
    },
    closeConnection:() => setConnectionOpenIndex([-1, -1, -1]),
    resetMainEntity:() => setConnectionsMainEntity(null)
  })
)(MainEntity)
