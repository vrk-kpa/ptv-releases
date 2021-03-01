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
import React from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { Button, Spinner, Label } from 'sema-ui-components'
import { connect } from 'react-redux'
import { reduxForm, reset } from 'redux-form/immutable'
import styles from 'appComponents/ConnectionsStep/styles.scss'
import { handleOnSubmit } from 'util/redux-form/util'
import {
  getServiceCollectionConnectionsInitialValues,
  getIsAnyServiceConnected,
  getIsAnyChannelConnected
} from 'appComponents/ConnectionsStep/selectors'
import { EntitySchemas } from 'schemas'
import {
  formActions,
  formActionsTypesEnum,
  formTypesEnum,
  securityOrganizationCheckTypes,
  permisionTypes
} from 'enums'
import { injectIntl, intlShape } from 'util/react-intl'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import withEditConnectionDialog from 'util/redux-form/HOC/withEditConnectionDialog'
import { setReadOnly, resetConnectionReadOnly } from 'reducers/formStates'
import { Security } from 'appComponents/Security'
import ChannelConnections from '../ChannelConnections'
import ServiceConnections from '../ServiceConnections'
import {
  sortServicesTransformer,
  sortChannelsTransformer,
  serviceCollectionConnectionsTransformer
} from './transformers'
import {
  getSelectedEntityId
} from 'selectors/entities/entities'
import {
  apiCall3,
  API_CALL_CLEAN
} from 'actions'
import { mergeInUIState } from 'reducers/ui'
import { push } from 'connected-react-router'
import { messages } from '../messages'
import { isConnectableLoading } from './ConnectionsForm'

const ServiceCollectionConnectionsForm = ({
  handleSubmit,
  dispatch,
  submitting,
  resetForm,
  resetConnectionReadOnly,
  setReadOnly,
  isAnyServiceConnected,
  isAnyChannelConnected,
  intl: { formatMessage },
  isReadOnly,
  formName,
  notificationForm,
  entityId,
  isLoading,
  inTranslation
}) => {
  const handleOnEdit = () => {
    dispatch(
      apiCall3({
        keys: [formName, 'isConnectable'],
        payload: {
          endpoint: formActions[formActionsTypesEnum.ISCONNECTABLE][notificationForm],
          data: { id: entityId }
        },
        formName: notificationForm,
        successNextAction: handleUpdate
      })
    )
  }

  const handleUpdate = () => {
    setReadOnly({
      form: formName,
      value: false
    })
  }

  const handleCancel = () => {
    resetForm(formName)
    resetConnectionReadOnly({
      form: formName
    })
    setReadOnly({
      form: formName,
      value: true
    })
  }

  const isAnyEntityConnected = isAnyChannelConnected || isAnyServiceConnected

  return (
    <div>
      {!isAnyEntityConnected && (
        <Label
          infoLabel
          labelText={formatMessage(messages.noConnectedContent)}
        />
      )}
      {isAnyServiceConnected && (
        <div>
          <Label
            labelPosition='top'
            labelText={formatMessage(messages.connectedServices)}
          />
          <div>
            <ServiceConnections name='selectedServices' />
          </div>
        </div>
      )}
      {isAnyChannelConnected && (
        <div>
          <Label
            labelPosition='top'
            labelText={formatMessage(messages.connectedChannels)}
          />
          <div>
            <ChannelConnections name='selectedChannels' />
          </div>
        </div>
      )}
      <div className={styles.buttonGroup}>
        {!isReadOnly && <Button
          onClick={handleCancel}
          children={formatMessage(messages.cancelUpdateButton)}
          small
          secondary
        />}
        {isReadOnly &&
        <Security
          formName={notificationForm}
          checkOrganization={securityOrganizationCheckTypes.byOrganization}
          permisionType={permisionTypes.update}
        >
          <Button
            onClick={handleOnEdit}
            type='button'
            children={isLoading && <Spinner /> ||
              (isAnyEntityConnected ? formatMessage(messages.updateButton) : formatMessage(messages.addButton))}
            small
            secondary
            disabled={isLoading || inTranslation}
          />
        </Security>}
        {!isReadOnly && <Button
          children={submitting && <Spinner /> || formatMessage(messages.saveButton)}
          onClick={handleSubmit}
          type='submit'
          small
          secondary={submitting || inTranslation}
        />}
      </div>
    </div>
  )
}
ServiceCollectionConnectionsForm.propTypes = {
  handleSubmit: PropTypes.func,
  dispatch: PropTypes.func,
  submitting: PropTypes.bool.isRequired,
  intl: intlShape,
  isAnyServiceConnected: PropTypes.bool,
  isAnyChannelConnected: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  resetForm: PropTypes.func,
  formName: PropTypes.string.isRequired,
  setReadOnly: PropTypes.func,
  resetConnectionReadOnly: PropTypes.func,
  notificationForm: PropTypes.string.isRequired,
  entityId: PropTypes.string,
  isLoading: PropTypes.bool,
  inTranslation: PropTypes.bool
}
 
const onSubmit = async (...args) => {
  const { notificationForm } = args[2]
  let submit = handleOnSubmit({
    url: 'serviceCollection/SaveRelations',
    schema: EntitySchemas.SERVICECOLLECTION,
    notificationForm,
    transformers: [
      sortServicesTransformer,
      sortChannelsTransformer,
      serviceCollectionConnectionsTransformer
    ]
  })
  return await submit(...args)
}

const onSubmitSuccess = (response, dispatch, { initialize }) => {
  [
    // Initialize with updated state from save response //
    ({ getState }) => initialize(getServiceCollectionConnectionsInitialValues(getState())),
    setReadOnly({
      form: formTypesEnum.CONNECTIONS,
      value: true
    }),
    resetConnectionReadOnly({
      form: formTypesEnum.CONNECTIONS
    }),
    mergeInUIState({
      key: 'languageVersionsVisible',
      value: { areLanguageVersionsVisible:false }
    }),
    {
      type: API_CALL_CLEAN,
      keys: ['connectionHistory']
    }
  ].forEach(dispatch)
  dispatch(mergeInUIState({
    key: 'editSmallConnectionDialog',
    value: {
      isEdit: false
    }
  }))
  dispatch(setReadOnly({
    form: formTypesEnum.CONNECTIONS,
    value: true
  }))
}

export default compose(
  injectIntl,
  connect((state) => ({
    initialValues: getServiceCollectionConnectionsInitialValues(state),
    isAnyChannelConnected: getIsAnyChannelConnected(state),
    isAnyServiceConnected: getIsAnyServiceConnected(state),
    entityId: getSelectedEntityId(state),
    isLoading: isConnectableLoading(state)
  }), {
    setReadOnly,
    resetConnectionReadOnly,
    resetForm: reset,
    push
  }),
  reduxForm({
    form: formTypesEnum.CONNECTIONS,
    onSubmit,
    onSubmitSuccess,
    enableReinitialize: true
  }),
  withFormStates,
  withEditConnectionDialog({
    reduxKey: 'editSmallConnectionDialog'
  })
)(ServiceCollectionConnectionsForm)
