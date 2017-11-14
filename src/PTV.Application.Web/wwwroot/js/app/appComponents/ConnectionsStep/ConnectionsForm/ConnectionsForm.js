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
import { compose } from 'redux'
import { Button, Spinner, Label } from 'sema-ui-components'
import { connect } from 'react-redux'
import { reduxForm, FieldArray, reset } from 'redux-form/immutable'
import styles from './styles.scss'
import { handleOnSubmit } from 'util/redux-form/util'
import RenderChannelConnections from './RenderChannelConnections'
import {
  getConnectionsFormInitialValues,
  getIsAnyEntityConnected
} from './selectors'
import RenderServiceConnections from './RenderServiceConnections'
import { EntitySchemas } from 'schemas'
import { formTypesEnum, entityTypesEnum } from 'enums'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import {
  withFormStates
} from 'util/redux-form/HOC'
import { setReadOnly, resetConnectionReadOnly } from 'reducers/formStates'
import { SecurityCreate } from 'appComponents/Security'

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

const ConnectionsForm = ({
  searchMode,
  handleSubmit,
  dispatch,
  submitting,
  resetForm,
  resetConnectionReadOnly,
  setReadOnly,
  isAnyEntityConnected,
  label,
  intl: { formatMessage },
  isReadOnly,
  formName
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
    <div>
      <Label
        labelPosition='top'
        labelText={formatMessage(label)}
      />
      {isAnyEntityConnected &&
      <FieldArray
        name='selectedConnections'
        component={
          searchMode === entityTypesEnum.CHANNELS
            ? RenderServiceConnections
            : RenderChannelConnections
        }
        props={{ dispatch, isReadOnly }}
      />}

      <div className={styles.buttonGroup}>
        {!isReadOnly && <Button
          onClick={handleCancel}
          children={formatMessage(messages.cancelUpdateButton)}
          small
          secondary
        />}
        {isReadOnly && searchMode !== entityTypesEnum.GENERALDESCRIPTIONS && <Button
          onClick={handleUpdate}
          type='button'
          children={isAnyEntityConnected ? formatMessage(messages.updateButton) : formatMessage(messages.addButton)}
          small
          secondary
        />}
        {isReadOnly && searchMode === entityTypesEnum.GENERALDESCRIPTIONS &&
          <SecurityCreate domain={entityTypesEnum.GENERALDESCRIPTIONS} >
            <Button
              onClick={handleUpdate}
              type='button'
              children={isAnyEntityConnected ? formatMessage(messages.updateButton) : formatMessage(messages.addButton)}
              small
              secondary
            />
          </SecurityCreate>
        }
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
ConnectionsForm.propTypes = {
  searchMode: PropTypes.oneOf(['channels', 'services', 'generalDescriptions']),
  handleSubmit: PropTypes.func,
  dispatch: PropTypes.func,
  submitting: PropTypes.bool.isRequired,
  intl: intlShape,
  label: PropTypes.object,
  isAnyEntityConnected: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  resetForm: PropTypes.func,
  formName: PropTypes.string.isRequired,
  setReadOnly: PropTypes.func
}

const onSubmit = async (...args) => {
  const { searchMode, notificationForm } = args[2] // ...args === (formValues, dispatch, ownProps)
  // Creating appropriate submit fucntion based on searchMode //
  let submit = null
  switch (searchMode) {
    case 'services':
      submit = handleOnSubmit({
        url: 'service/SaveRelations',
        schema: EntitySchemas.SERVICE,
        notificationForm
      })
      break
    case 'channels':
      submit = handleOnSubmit({
        url: 'channel/SaveRelations',
        schema: EntitySchemas.CHANNEL,
        notificationForm
      })
      break
    case 'generalDescriptions':
      submit = handleOnSubmit({
        url: 'generalDescription/SaveRelations',
        schema: EntitySchemas.GENERAL_DESCRIPTION,
        notificationForm
      })
      break
  }
  await submit(...args)
}

const onSubmitSuccess = (props, dispatch) => {
  [
    setReadOnly({
      form: formTypesEnum.CONNECTIONS,
      value: true
    }),
    resetConnectionReadOnly({
      form: formTypesEnum.CONNECTIONS
    })
  ].forEach(dispatch)
}

export default compose(
  injectIntl,
  connect((state, searchMode) => ({
    initialValues: getConnectionsFormInitialValues(state),
    isAnyEntityConnected: getIsAnyEntityConnected(state)
  }), {
    setReadOnly,
    resetConnectionReadOnly,
    resetForm: reset
  }),
  reduxForm({
    form: formTypesEnum.CONNECTIONS,
    onSubmit,
    onSubmitSuccess,
    enableReinitialize: true
  }),
  withFormStates
)(ConnectionsForm)
