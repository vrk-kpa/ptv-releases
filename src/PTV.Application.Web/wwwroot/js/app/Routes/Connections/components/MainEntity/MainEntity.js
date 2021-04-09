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
import { Field, arrayRemove, isDirty } from 'redux-form/immutable'
import { getConnectionsMainEntity } from 'selectors/selections'
import {
  getMainEntitiesCount,
  getIsConnectionsOrganizingActive
} from 'Routes/Connections/selectors'
import { compose } from 'redux'
import { connect } from 'react-redux'
import EntityDescription from 'appComponents/EntityDescription'
import ChannelTypeCell from 'appComponents/Cells/ChannelTypeCell'
import ServiceTypeCell from 'appComponents/Cells/ServiceTypeCell'
import { PTVIcon } from 'Components'
import cx from 'classnames'
import styles from './styles.scss'
import {
  setConnectionsMainEntity
} from 'reducers/selections'
import { makeCurrentFormStateInitial } from 'Routes/Connections/actions'

class MainEntity extends Component {
  handleOnRemove = () => {
    this.props.removeEntity(this.props.connectionIndex)
    if (!this.props.isDirty) {
      this.props.makeCurrentFormStateInitial()
    }
    if (this.props.mainEntitiesCount === 1) {
      this.props.resetMainEntity()
      this.props.makeCurrentFormStateInitial()
    }
  }
  childRender = ({ input }) => {
    const { mainEntity, modalMode, isOrganizingActive } = this.props
    const contentClass = cx(
      styles.mainEntityDescriptionCell,
      {
        [styles.inModal]: modalMode
      }
    )
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
              : (input.value.get('channelType') || input.value.get('subEntityType'))}
            contentClass={contentClass}
            preview={!modalMode}
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
        {!modalMode && !isOrganizingActive && (
          <div className='col-lg-8'>
            <div className={styles.actions}>
              <PTVIcon onClick={this.handleOnRemove} name={'icon-trash'} />
            </div>
          </div>
        )}
      </div>
    )
  }
  render () {
    const { mainEntity } = this.props
    return mainEntity && (
      <Field
        name={'mainEntity'}
        component={this.childRender}
        {...this.props}
      />
    )
  }
}
MainEntity.propTypes = {
  mainEntity: PropTypes.oneOf(['channels', 'services']),
  removeEntity: PropTypes.func.isRequired,
  resetMainEntity: PropTypes.func.isRequired,
  connectionIndex: PropTypes.number.isRequired,
  mainEntitiesCount: PropTypes.number.isRequired,
  isDirty: PropTypes.bool.isRequired,
  makeCurrentFormStateInitial: PropTypes.func.isRequired,
  modalMode: PropTypes.bool,
  isOrganizingActive: PropTypes.bool
}

export default compose(
  connect((state, ownProps) => ({
    mainEntity: getConnectionsMainEntity(state),
    mainEntitiesCount: getMainEntitiesCount(state),
    isDirty: isDirty('connectionsWorkbench')(state),
    isOrganizingActive: getIsConnectionsOrganizingActive(state)
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
    resetMainEntity:() => setConnectionsMainEntity(null),
    makeCurrentFormStateInitial
  })
)(MainEntity)
