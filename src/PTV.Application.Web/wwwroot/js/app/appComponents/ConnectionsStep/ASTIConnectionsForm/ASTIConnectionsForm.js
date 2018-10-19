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
import React from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { reduxForm, reset } from 'redux-form/immutable'
import ConnectionsByOrganizations from 'appComponents/ConnectionsStep/ConnectionsByOrganizations'
import ConnectionsFlat from 'appComponents/ConnectionsStep/ConnectionsByOrganizations/ConnectionsFlat'
import {
  getASTIConnectionsFormInitialValuesForChannels,
  getASTIConnectionsFormInitialValuesForServices,
  getIsAnyASTIServiceConnected,
  getIsAnyASTIChannelConnected
} from 'appComponents/ConnectionsStep/selectors'
import { getSelectedEntityType } from 'selectors/entities/entities'
import { entityTypesEnum, formTypesEnum } from 'enums'
import styles from 'appComponents/ConnectionsStep/styles.scss'
import { Button, Spinner } from 'sema-ui-components'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { setReadOnly, resetConnectionReadOnly } from 'reducers/formStates'
import {
  astiConnectionsBasicTransformer,
  astiServiceConnectionsGroupedTransformer
} from './transformers'
import {
  phoneNumbersTransformer,
  openingHoursTransformer,
  collectionsTransformer
} from '../ConnectionsForm/transformers'
import { EntitySchemas } from 'schemas'
import { handleOnSubmit } from 'util/redux-form/util'

const messages = defineMessages({
  saveButton: {
    id: 'Components.Buttons.SaveButton',
    defaultMessage: 'Tallenna'
  },
  updateButton: {
    id: 'Components.Buttons.UpdateButton',
    defaultMessage: 'Muokkaa'
  },
  cancelUpdateButton: {
    id: 'Components.Buttons.CancelUpdateButton',
    defaultMessage: 'Keskeytä'
  },
  addButton: {
    id: 'Components.Buttons.AddButton',
    defaultMessage: 'Lisää'
  }
})

const ASTIConnectionsForm = ({
  entityType,
  intl: { formatMessage },
  resetForm,
  resetConnectionReadOnly,
  setReadOnly,
  handleSubmit,
  dispatch,
  submitting,
  isReadOnly,
  isAnyEntityConnected,
  formName,
  ...rest
  }) => {
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
  return (
    <div className='form-row'>
      {entityType === entityTypesEnum.SERVICES
        ? <ConnectionsFlat {...rest} />
        : <ConnectionsByOrganizations {...rest} />
      }
      <div className={styles.buttonGroup}>
        {!isReadOnly && <Button
          onClick={handleCancel}
          children={formatMessage(messages.cancelUpdateButton)}
          small
          secondary
        />}
        {isReadOnly && isAnyEntityConnected && <Button
          onClick={handleUpdate}
          type='button'
          children={formatMessage(messages.updateButton)}
          small
          secondary
        />}
        {!isReadOnly && <Button
          children={submitting && <Spinner /> || formatMessage(messages.saveButton)}
          onClick={handleSubmit}
          type='submit'
          small
          secondary={submitting}
        />}
      </div>
    </div>
  )
}

ASTIConnectionsForm.propTypes = {
  entityType: PropTypes.string,
  intl: intlShape,
  handleSubmit: PropTypes.func,
  dispatch: PropTypes.func,
  submitting: PropTypes.bool.isRequired,
  isAnyEntityConnected: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  resetForm: PropTypes.func,
  formName: PropTypes.string.isRequired,
  setReadOnly: PropTypes.func
}

const onSubmit = async (...args) => {
  const { entityType, notificationForm } = args[2] // ...args === (formValues, dispatch, ownProps)
  // Creating appropriate submit fucntion based on searchMode //
  let submit = null
  switch (entityType) {
    case entityTypesEnum.SERVICES:
      submit = handleOnSubmit({
        url: 'service/SaveRelations',
        schema: EntitySchemas.SERVICE,
        notificationForm,
        transformers: [
          astiConnectionsBasicTransformer,
          collectionsTransformer,
          phoneNumbersTransformer,
          openingHoursTransformer
        ]
      })
      break
    case entityTypesEnum.CHANNELS:
      submit = handleOnSubmit({
        url: 'channel/SaveRelations',
        schema: EntitySchemas.CHANNEL,
        notificationForm,
        transformers: [
          astiServiceConnectionsGroupedTransformer,
          astiConnectionsBasicTransformer,
          collectionsTransformer,
          phoneNumbersTransformer,
          openingHoursTransformer
        ]
      })
      break
  }
  await submit(...args)
}

const onSubmitSuccess = (result, dispatch, props) => {
  [
    setReadOnly({
      form: formTypesEnum.ASTICONNECTIONS,
      value: true
    }),
    resetConnectionReadOnly({
      form: formTypesEnum.ASTICONNECTIONS
    })
  ].forEach(dispatch)
}

export default compose(
  injectIntl,
  connect(state => {
    const entityType = getSelectedEntityType(state)
    const initialValues = {
      [entityTypesEnum.CHANNELS]: getASTIConnectionsFormInitialValuesForChannels(state),
      [entityTypesEnum.SERVICES]: getASTIConnectionsFormInitialValuesForServices(state)
    }[entityType]
    const isAnyEntityConnected = {
      [entityTypesEnum.CHANNELS]: getIsAnyASTIServiceConnected(state),
      [entityTypesEnum.SERVICES]: getIsAnyASTIChannelConnected(state)
    }[entityType]
    return {
      initialValues,
      entityType,
      isAnyEntityConnected
    }
  }, {
    setReadOnly,
    resetConnectionReadOnly,
    resetForm: reset
  }),
  reduxForm({
    form: formTypesEnum.ASTICONNECTIONS,
    onSubmit,
    onSubmitSuccess,
    enableReinitialize: true
  }),
  withFormStates
)(ASTIConnectionsForm)
