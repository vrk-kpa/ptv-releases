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
import {
  Button,
  Spinner
} from 'sema-ui-components'
import { mergeInUIState } from 'reducers/ui'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { getReduxFormKey } from './selectors'
import { getUrlCheckingAbort } from 'util/redux-form/HOC/withEntityButtons/selectors'
import { setReadOnly } from 'reducers/formStates'
import {
  getFormSyncErrors,
  submit,
  isDirty,
  isSubmitting
} from 'redux-form/immutable'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { Security } from 'appComponents/Security'
import { getConnectionAdditionalQualityCheckData } from 'Routes/Connections/selectors'
import { directQualityEntityCheck } from 'actions/qualityAgent'
import { setAlternativeId } from 'util/redux-form/HOC/withQualityAgent/actions'
import { allContentTypesEnum, qualityAgentProfileTypesEnum } from 'enums'

const messages = defineMessages({
  saveDialogButtonTitle: {
    id: 'AppComponents.ConnectionStep.EditConnectionDialog.Buttons.Save.Title',
    defaultMessage: 'Tallenna tiedot'
  },
  updateButton: {
    id: 'Components.Buttons.UpdateButton',
    defaultMessage: 'Muokkaa'
  }
})

const qualityCheck = (dispatch, formName, languages) => {
  dispatch((store) => {
    const options = {
      formName,
      entityType: allContentTypesEnum.SERVICE,
      profile: qualityAgentProfileTypesEnum.VRKP,
      languages
    }
    setAlternativeId(store, formName) //need for showing warning
    const data = getConnectionAdditionalQualityCheckData(store.getState(), { formName })
    directQualityEntityCheck(data, store, options)
  })
}

const SaveEditButton = ({
  security,
  reduxKey,
  formName,
  saveConnections,
  hasformError,
  isDirty,
  isSaving,
  isEdit,
  setReadOnly,
  mergeInUIState,
  className,
  abort,
  qualityCheckCall,
  languages,
  intl: { formatMessage }
  }) => {
  const handleOnSave = () => {
    abort && abort()
    saveConnections(formName)
  }

  const handleOnEdit = () => {
    qualityCheckCall(formName, languages)
    setReadOnly({
      form: formName,
      value: false
    })
    mergeInUIState({
      key: reduxKey,
      value: {
        isEdit: true
      }
    })
  }
  return (
    <div className={className}>
      <Security
        id={security && security.entityId}
        domain={security && security.domain}
        checkOrganization={security && security.checkOrganization}
        permisionType={security && security.permisionTypes}
        organization={security && security.organizationId}
      >
        {!isEdit && <Button onClick={handleOnEdit}
          small
          children={formatMessage(messages.updateButton)} /> }
      </Security>
      {isEdit && <Button onClick={handleOnSave}
        small
        disabled={!isDirty || hasformError}
        children={isSaving && <Spinner inverse /> || formatMessage(messages.saveDialogButtonTitle)} />
      }
    </div>
  )
}

SaveEditButton.propTypes = {
  isEdit: PropTypes.bool,
  mergeInUIState: PropTypes.func,
  intl: intlShape,
  isDirty: PropTypes.bool.isRequired,
  formName: PropTypes.string.isRequired,
  reduxKey: PropTypes.string,
  setReadOnly: PropTypes.func,
  saveConnections: PropTypes.func,
  hasformError: PropTypes.bool,
  isSaving: PropTypes.bool,
  security: PropTypes.object,
  className: PropTypes.string,
  abort: PropTypes.func,
  qualityCheckCall: PropTypes.func,
  languages: PropTypes.array.isRequired
}

export default compose(
  injectIntl,
  injectFormName,
  connect((state, { isAsti, formName }) => {
    const reduxFormKey = getReduxFormKey(state, { isAsti })
    const formSyncErrors = getFormSyncErrors(formName)(state) || null
    return {
      isDirty: isDirty(formName)(state),
      hasformError: formSyncErrors && formSyncErrors.hasOwnProperty(reduxFormKey),
      isSaving: isSubmitting(formName)(state),
      abort: getUrlCheckingAbort(state)
    }
  }, {
    mergeInUIState,
    saveConnections: submit,
    setReadOnly,
    qualityCheckCall: (formName, languages) => ({ dispatch }) => qualityCheck(dispatch, formName, languages)
  })
)(SaveEditButton)
