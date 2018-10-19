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
import { Field, arrayRemove, formValueSelector } from 'redux-form/immutable'
import { getConnectionsMainEntity, isConnectionOpen } from 'selectors/selections'
import { getLanguageCode } from 'Routes/Connections/selectors'
import AdditionalInformation from 'appComponents/ConnectionsStep/AdditionalInformation'
import LanguageProvider from 'appComponents/LanguageProvider'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { injectIntl } from 'util/react-intl'
import EntityDescription from 'appComponents/EntityDescription'
import TableRowDetailButton from 'appComponents/TableRowDetailButton'

import ChannelTypeCell from 'appComponents/Cells/ChannelTypeCell'
import ServiceTypeCell from 'appComponents/Cells/ServiceTypeCell'
import { PTVIcon } from 'Components'
import styles from './styles.scss'
import {
  setConnectionOpenIndex, setContentLanguage
} from 'reducers/selections'
import { resetConnectionReadOnly } from 'reducers/formStates'
import { List } from 'immutable'
// import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'
import { formTypesEnum,
  entityTypesEnum,
  securityOrganizationCheckTypes,
  permisionTypes } from 'enums'
import { Security } from 'appComponents/Security'
import { isChannelConnectionCommon } from '../../selectors'

class Child extends Component {
  handleDetailButtonOnClick = () => {
    this.props.isOpen
    ? this.props.openConnection(-1, -1, -1)
    : this.props.isAsti
      ? this.props.openConnection(this.props.connectionIndex, -1, this.props.childIndex)
      : this.props.openConnection(this.props.connectionIndex, this.props.childIndex, -1)
    this.props.resetConnectionReadOnly()
    this.props.setContentLanguage(this.defaultLanguage, this.languageKey)
  }
  handleOnRemove = () => {
    this.props.removeChild(this.props.connectionIndex, this.props.childIndex)
    if (this.props.isOpen) {
      this.props.openConnection(-1, -1, -1)
    }
  }
  get defaultLanguage () {
    const defLang = this.props.mainLanguages.find(
      lang => !!this.props.childLanguages.find(
        childLang => childLang.get('languageId') === lang.get('languageId')
      )
    )
    return defLang && defLang.get('languageId') || null
  }
  get languageKey () {
    return `connection[${this.props.connectionIndex}].child[${this.props.childIndex}]`
  }
  childRender = ({ input }) => {
    const {
      mainEntity,
      isOpen,
      isAsti,
      childIndex,
      dragHandleProps,
      mainEntityChannelType,
      field,
      connectionIndex,
      closeConnection,
      isCommon
    } = this.props
    if (!input.value) return null
    const security = {
      entityId: input.value.get('id'),
      domain: mainEntity === 'services' ? entityTypesEnum.CHANNELS : entityTypesEnum.SERVICES,
      organizationId: input.value.get('organizationId'),
      checkOrganization: isCommon ? securityOrganizationCheckTypes.ownOrganization : securityOrganizationCheckTypes.byOrganization,
      permisionTypes: permisionTypes.update
    }
    return (
      <div className={styles.subEntity}>
        <div className={styles.subEntityMainData}>
          <div className='row align-items-center'>
            <div className='col-lg-12'>
              <EntityDescription
                name={input.value.get('name')}
                entityId={input.value.get('id')}
                organizationId={input.value.get('organizationId')}
                entityConcreteType={mainEntity === 'services'
                  ? input.value.get('channelType')
                  : 'service'}
                isNewConnection={!input.value.get('modifiedBy')}
                unificRootId={input.value.get('unificRootId')}
                connectionIndex={connectionIndex}
              />
            </div>
            <div className='col-lg-4'>
              {mainEntity === 'services'
                ? <ChannelTypeCell
                  channelTypeId={input.value.get('channelTypeId')}
                />
                : <ServiceTypeCell
                  serviceTypeId={input.value.get('serviceType')}
                />}
            </div>
            <div className='col-lg-8'>
              <div className={styles.actions}>
                {!isAsti &&
                <Security
                  id={security.entityId}
                  domain={security.domain}
                  checkOrganization={security.checkOrganization}
                  permisionType={security.permisionTypes}
                  organization={security.organizationId}
              >
                  <PTVIcon onClick={this.handleOnRemove} name={'icon-trash'} />
                </Security>}
                {!isAsti &&
                  <div
                    {...dragHandleProps}
                    onMouseDown={(...args) => {
                      closeConnection()
                      dragHandleProps.onMouseDown(...args)
                    }}
                  >
                    <PTVIcon
                      name={'icon-move'}
                      componentClass={styles.dragHandle}
                    />
                  </div>}
                <TableRowDetailButton
                  isOpen={isOpen}
                  onClick={this.handleDetailButtonOnClick}
                />
              </div>
            </div>
          </div>
        </div>
        {isOpen &&
          <LanguageProvider languageKey={`connection[${connectionIndex}].child[${childIndex}]`} >
            <AdditionalInformation
              index={childIndex}
              field={field}
              isAsti={isAsti}
              parentType={this.props.mainEntity}
              parentId={this.props.mainEntityId}
              entityConcreteType={mainEntity === 'services'
              ? input.value.get('channelType')
              : mainEntityChannelType}
              security={security}
          />
          </LanguageProvider>
        }
      </div>
    )
  }
  render () {
    const {
      field,
      isOpen,
      childIndex,
      isAsti
    } = this.props
    return this.props.mainEntity && (
      <div>
        <Field
          name={field}
          component={this.childRender}
          isOpen={isOpen}
          childIndex={childIndex}
          isAsti={isAsti}
          field={field}
          {...this.props}
        />
      </div>
    )
  }
}
Child.propTypes = {
  childIndex: PropTypes.number.isRequired,
  connectionIndex: PropTypes.number.isRequired,
  field: PropTypes.string.isRequired,
  mainEntity: PropTypes.oneOf(['channels', 'services']),
  mainEntityChannelType: PropTypes.string,
  updateUI: PropTypes.func,
  closeConnection: PropTypes.func,
  setContentLanguage: PropTypes.func.isRequired,
  mainEntityId: PropTypes.string,
  removeChild: PropTypes.func,
  openConnection: PropTypes.func,
  resetConnectionReadOnly: PropTypes.func,
  isOpen: PropTypes.bool,
  isAsti: PropTypes.bool,
  isCommon: PropTypes.bool,
  dragHandleProps: PropTypes.object,
  languageKey: PropTypes.string
}

export default compose(
  injectIntl,
  connect((state, ownProps) => {
    const getFormValues = formValueSelector('connectionsWorkbench')
    const mainLanguages = getFormValues(state, `connections[${ownProps.connectionIndex}].mainEntity.languagesAvailabilities`) || List()
    const childLanguages = getFormValues(state, `connections[${ownProps.connectionIndex}].childs[${ownProps.childIndex}].languagesAvailabilities`) || List()
    const connectionType = getFormValues(state, `connections[${ownProps.connectionIndex}].childs[${ownProps.childIndex}].connectionType`)
    return {
      mainEntityId: getFormValues(state, `connections[${ownProps.connectionIndex}].mainEntity.id`) || null,
      mainEntityChannelType: getFormValues(state, `connections[${ownProps.connectionIndex}].mainEntity.channelType`) || null,
      mainEntity: getConnectionsMainEntity(state),
      isOpen: isConnectionOpen(state, { connectionIndex: ownProps.connectionIndex, childIndex: ownProps.childIndex, isAsti: ownProps.isAsti }),
      mainLanguages,
      childLanguages,
      isCommon: isChannelConnectionCommon(state, { connectionType })
    }
  }, {
    removeChild: (connectionIndex, childIndex) => ({ dispatch }) => {
      dispatch(
        arrayRemove(
          'connectionsWorkbench',
          `connections[${connectionIndex}].childs`,
          childIndex
        )
      )
    },
    openConnection:(connectionIndex, childIndex, astiIndex) => setConnectionOpenIndex([connectionIndex, childIndex, astiIndex]),
    resetConnectionReadOnly: () => resetConnectionReadOnly({ form: formTypesEnum.CONNECTIONS }),
    closeConnection:() => setConnectionOpenIndex([-1, -1, -1]),
    setContentLanguage: (id, languageKey) => ({ getState, dispatch }) => {
      dispatch(setContentLanguage({
        id,
        code: getLanguageCode(getState(), { id }),
        languageKey
      }))
    }
  }
  )
)(Child)
