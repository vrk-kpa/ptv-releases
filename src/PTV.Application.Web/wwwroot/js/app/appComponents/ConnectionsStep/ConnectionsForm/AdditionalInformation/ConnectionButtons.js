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
import { connect } from 'react-redux'
import { compose } from 'redux'
import { setConnectionReadOnly } from 'reducers/formStates'
import { injectFormName } from 'util/redux-form/HOC'
import { isConnectionRowReadOnly, isConnectionsReadOnly } from 'appComponents/ConnectionsStep/ConnectionsForm/selectors'
import { Button, Spinner } from 'sema-ui-components'
import { getFormInitialValues, arrayInsert, arrayRemove, submit, isSubmitting, change } from 'redux-form/immutable'
import { injectIntl, intlShape, defineMessages } from 'react-intl'
import { formTypesEnum } from 'enums'
import { Map, List } from 'immutable'
import styles from './styles.scss'

const messages = defineMessages({
  updateButton: {
    id: 'Components.Buttons.UpdateButton',
    defaultMessage: 'Muokkaa'
  },
  cancelUpdateButton: {
    id: 'Components.Buttons.CancelUpdateButton',
    defaultMessage: 'Keskeytä'
  },
  saveButton: {
    id: 'Components.Buttons.SaveButton',
    defaultMessage: 'Tallenna'
  }
})
const ConnectionButtons = ({
  index,
  field,
  intl: { formatMessage },
  isReadOnly,
  setConnectionReadOnly,
  isConnectionsReadOnly,
  submit,
  change,
  formName,
  isSaving,
  initialValues
}) => {
  const basicInformationFieldName = `${field}.basicInformation`
  const handleOnEnable = () => {
    setConnectionReadOnly({
      form: formTypesEnum.CONNECTIONS,
      index,
      value: false
    })
  }
  const handleOnDisable = () => {
    const basicInformation = initialValues.get('basicInformation') || Map()
    const digitalAuthorization = initialValues.getIn(['digitalAuthorization', 'digitalAuthorizations']) || List()
    change(formName, `${basicInformationFieldName}.description`, basicInformation.get('description') || Map())
    change(formName, `${basicInformationFieldName}.chargeType`, basicInformation.get('chargeType') || null)
    change(formName, `${basicInformationFieldName}.additionalInformation`,
      basicInformation.get('additionalInformation') || Map())
    change(formName, `${field}.digitalAuthorization.digitalAuthorizations`, digitalAuthorization)
    setConnectionReadOnly({
      form: formTypesEnum.CONNECTIONS,
      index,
      value: true
    })
  }
  const handleOnSave = () => {
    submit(formName)
    setConnectionReadOnly({
      form: formTypesEnum.CONNECTIONS,
      index,
      value: true
    })
  }
  return (
    <div>
      {isReadOnly
        ? <Button
          onClick={handleOnEnable}
          children={isSaving && <Spinner /> || formatMessage(messages.updateButton)}
          small
          disabled={isSaving}
          />
        : <div className={styles.buttonGroup}>
          <Button
            onClick={handleOnDisable}
            children={isSaving && <Spinner /> || formatMessage(messages.cancelUpdateButton)}
            secondary
            small
            disabled={isSaving}
          />
          <Button
            onClick={handleOnSave}
            children={isSaving && <Spinner /> || formatMessage(messages.saveButton)}
            small
            disabled={isSaving}
          />
        </div>}
    </div>
  )
}

export default compose(
  injectFormName,
  injectIntl,
  connect((state, { index, formName }) => ({
    isReadOnly: isConnectionRowReadOnly(index)(state),
    isConnectionsReadOnly: isConnectionsReadOnly(state),
    initialValues: getFormInitialValues(formName)(state).getIn(['selectedConnections', index]),
    isSaving: isSubmitting(formName)(state)
  }), {
    setConnectionReadOnly,
    arrayInsert,
    arrayRemove,
    submit,
    change
  })
)(ConnectionButtons)
