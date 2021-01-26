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
import React, { Component, Fragment } from 'react'
import PropTypes from 'prop-types'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { Field, arrayRemove, formValueSelector, getFormSyncErrors, isDirty } from 'redux-form/immutable'
import { getConnectionsMainEntity } from 'selectors/selections'
import {
  getLanguageCode,
  getHasAnyConnectedChildValue,
  getIsOrganizingActiveForConnection
} from 'Routes/Connections/selectors'
import AdditionalInformation from 'appComponents/ConnectionsStep/AdditionalInformation'
import LanguageProvider from 'appComponents/LanguageProvider'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { injectIntl, intlShape } from 'util/react-intl'
import { messages } from 'appComponents/ConnectionsStep/messages'
import EntityDescription from 'appComponents/EntityDescription'
import ChannelTypeCell from 'appComponents/Cells/ChannelTypeCell'
import ServiceTypeCell from 'appComponents/Cells/ServiceTypeCell'
import ValueCell from 'appComponents/Cells/ValueCell'
import { PTVIcon } from 'Components'
import cx from 'classnames'
import styles from './styles.scss'
import { setContentLanguage } from 'reducers/selections'
import { resetConnectionReadOnly } from 'reducers/formStates'
import { List, Map } from 'immutable'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import {
  formTypesEnum,
  entityTypesEnum,
  securityOrganizationCheckTypes,
  permisionTypes
} from 'enums'
import { Security } from 'appComponents/Security'
import { isChannelConnectionCommon } from '../../selectors'
import { getFormSyncErrorWithPath } from 'selectors/base'
import { mergeInUIState } from 'reducers/ui'
import { EditIcon } from 'appComponents/Icons'
import ModifiedAtCell from 'appComponents/Cells/ModifiedAtCell'
import ModifiedByCell from 'appComponents/Cells/ModifiedByCell'

class Child extends Component {
  handleOnRemove = () => {
    this.props.removeChild(this.props.connectionIndex, this.props.childIndex)
  }
  get defaultLanguage () {
    const defLang = this.props.mainLanguages.find(
      lang => !!this.props.childLanguages.find(
        childLang => childLang.get('languageId') === lang.get('languageId')
      )
    )
    return defLang && defLang.get('languageId') || null
  }

  onEditClick = (parentIndex, childIndex, isAsti, security) => {
    this.props.mergeInUIState({
      key: 'editConnectionDialog',
      value: {
        isOpen: true,
        parentIndex: parentIndex,
        childIndex: childIndex,
        isAsti: isAsti,
        isEdit: false,
        security: security
      }
    })
    this.props.setContentLanguage(this.defaultLanguage, 'connectionDialogPreview')
  }

  childRender = ({ input }) => {
    const {
      mainEntity,
      isAsti,
      childIndex,
      dragHandleProps,
      mainEntityChannelType,
      field,
      connectionIndex,
      isCommon,
      hasChildError,
      hasformError,
      modalMode,
      hasAnyChildValue,
      isDirty,
      isOrganizingActive,
      intl: { formatMessage },
      isReadOnly
    } = this.props
    if (!input.value) return null
    const security = {
      entityId: input.value.get('id'),
      domain: mainEntity === 'services' ? entityTypesEnum.CHANNELS : entityTypesEnum.SERVICES,
      organizationId: input.value.get('organizationId'),
      checkOrganization: isCommon ? securityOrganizationCheckTypes.ownOrganization : securityOrganizationCheckTypes.byOrganization,
      permisionTypes: permisionTypes.update
    }
    const actionsClass = cx(
      styles.actions,
      {
        [styles.isOrganizingActive]: isOrganizingActive
      }
    )
    const subEntityClass = cx(
      styles.subEntity,
      {
        [styles.inModal]: modalMode
      }
    )
    const entityDescriptionClass = modalMode ? 'col-lg-12' : 'col-lg-10'
    return (
      <div className={subEntityClass}>
        <div className={styles.subEntityMainData}>
          <div className='row align-items-center'>
            <div className={entityDescriptionClass}>
              <EntityDescription
                name={input.value.get('name')}
                entityId={input.value.get('id')}
                organizationId={input.value.get('organizationId')}
                entityConcreteType={mainEntity === 'services'
                  ? input.value.get('channelType')
                  : 'service'}
                isNewConnection={input.value.get('isNew')}
                unificRootId={input.value.get('unificRootId')}
                connectionIndex={connectionIndex}
                contentClass={styles.subEntityDescriptionCell}
                preview={!modalMode}
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
            {!modalMode && (
              <Fragment>
                <div className='col-lg-5'>
                  <ModifiedAtCell modifiedAt={input.value.get('modified')} inline />
                  <ModifiedByCell modifiedBy={input.value.get('modifiedBy')} compact />
                </div>
                <div className='col-lg-5'>
                  {isOrganizingActive
                    ? <div className={actionsClass}>
                      {!isAsti && (
                        <div
                          {...dragHandleProps}
                          onMouseDown={(...args) => {
                            dragHandleProps.onMouseDown(...args)
                          }}
                        >
                          {!hasformError && (
                            <PTVIcon
                              name={'icon-move'}
                              width={18}
                              height={18}
                              componentClass={styles.dragHandle}
                            />
                          )}
                        </div>
                      )}
                    </div>
                    : <div className={actionsClass}>
                      {!hasAnyChildValue && (
                        <ValueCell
                          value={formatMessage(messages.connectionsNoDetailInformation)}
                          componentClass={styles.placeholder}
                        />
                      )}
                      <EditIcon
                        onClick={() => this.onEditClick(connectionIndex, childIndex, isAsti, security)}
                        disabled={isDirty}
                        componentClass={styles.editIcon}
                      />
                      {!isAsti &&
                        <Security
                          id={security.entityId}
                          domain={security.domain}
                          checkOrganization={security.checkOrganization}
                          permisionType={security.permisionTypes}
                          organization={security.organizationId}
                        >
                          <PTVIcon
                            onClick={this.handleOnRemove}
                            name={'icon-trash'}
                            componentClass={styles.trashIcon}
                          />
                        </Security>
                      }
                    </div>
                  }
                </div>
              </Fragment>
            )}
          </div>
        </div>
        {modalMode &&
          <LanguageProvider languageKey='connectionDialogPreview' >
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
              hasError={hasChildError}
              isReadOnly={isReadOnly}
            />
          </LanguageProvider>
        }
      </div>
    )
  }
  render () {
    const {
      field,
      childIndex,
      isAsti,
      className
    } = this.props
    return this.props.mainEntity && (
      <div className={className}>
        <Field
          name={field}
          component={this.childRender}
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
  childEntityId: PropTypes.string,
  removeChild: PropTypes.func,
  openConnection: PropTypes.func,
  resetConnectionReadOnly: PropTypes.func,
  isAsti: PropTypes.bool,
  isCommon: PropTypes.bool,
  dragHandleProps: PropTypes.object,
  languageKey: PropTypes.string,
  hasChildError: PropTypes.bool,
  hasformError: PropTypes.bool,
  modalMode: PropTypes.bool,
  mergeInUIState: PropTypes.func.isRequired,
  hasAnyChildValue: PropTypes.bool,
  isDirty: PropTypes.bool.isRequired,
  isOrganizingActive: PropTypes.bool,
  className: PropTypes.string,
  isReadOnly: PropTypes.bool,
  mainLanguages: ImmutablePropTypes.list,
  childLanguages: ImmutablePropTypes.list,
  intl: intlShape
}

const errorSelector = getFormSyncErrorWithPath(({ connectionIndex, childIndex }) => {
  return `connections[${connectionIndex}].childs[${childIndex}]`
})

export default compose(
  injectIntl,
  injectFormName,
  connect((state, ownProps) => {
    const getFormValues = formValueSelector('connectionsWorkbench')
    const connection = getFormValues(state, `connections[${ownProps.connectionIndex}]`) || Map()
    const main = connection.get('mainEntity') || Map()
    const childs = connection.get('childs') || Map()
    const child = childs.get(ownProps.childIndex) || Map()
    const connectionType = child.get('connectionType')
    const mainEntityId = main.get('id') || null
    const childEntityId = child.get('id') || null
    const hasChildError = ownProps.formName && errorSelector(state, ownProps) || false
    const formSyncErrors = getFormSyncErrors(ownProps.formName)(state) || null
    const hasAnyChildValue = getHasAnyConnectedChildValue(state, { connectionIndex: ownProps.connectionIndex, childIndex: ownProps.childIndex, isAsti: ownProps.isAsti }) || false

    return {
      mainEntityId,
      childEntityId,
      mainEntityChannelType: main.get('channelType') || null,
      mainEntity: getConnectionsMainEntity(state),
      mainLanguages: main.get('languagesAvailabilities') || List(),
      childLanguages: child.get('languagesAvailabilities') || List(),
      isCommon: isChannelConnectionCommon(state, { connectionType }),
      hasformError: formSyncErrors && formSyncErrors.hasOwnProperty('connections'),
      hasChildError: hasChildError,
      hasAnyChildValue: hasAnyChildValue,
      isDirty: isDirty(ownProps.formName)(state),
      isOrganizingActive: getIsOrganizingActiveForConnection(state, ownProps)
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
    resetConnectionReadOnly: () => resetConnectionReadOnly({ form: formTypesEnum.CONNECTIONS }),
    setContentLanguage: (id, languageKey) => ({ getState, dispatch }) => {
      dispatch(setContentLanguage({
        id,
        code: getLanguageCode(getState(), { id }),
        languageKey
      }))
    },
    mergeInUIState
  }
  )
)(Child)
