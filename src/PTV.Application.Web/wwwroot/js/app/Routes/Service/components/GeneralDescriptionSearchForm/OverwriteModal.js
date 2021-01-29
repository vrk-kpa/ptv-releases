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
import ImmutablePropTypes from 'react-immutable-proptypes'
import ModalDialog from 'appComponents/ModalDialog'
import { ModalActions, Button } from 'sema-ui-components'
import { compose } from 'redux'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import styles from './styles.scss'
import { connect } from 'react-redux'
import { EntitySelectors } from 'selectors'
import { getContentLanguageCode } from 'selectors/selections'
import { formValueSelector, change, reset } from 'redux-form/immutable'
import { deleteApiCall } from 'actions'
import { mergeInUIState } from 'reducers/ui'
import { formTypesEnum } from 'enums'
import { getNewServiceName } from './selectors'
import { getGDPublishedLanguages } from 'Routes/Service/components/ServiceComponents/selectors'

const messages = defineMessages({
  dialogOverwriteNameText: {
    id: 'Containers.Services.NameOverwriteDialog',
    defaultMessage: 'Do you want overwrite service name by general description name?'
  },
  buttonOk: {
    id: 'Containers.Services.NameOverwriteDialog.Accept',
    defaultMessage: 'Kyllä'
  },
  buttonCancel: {
    id: 'Containers.Services.NameOverwriteDialog.Cancel',
    defaultMessage: 'Jatka muokkausta'
  }
})
const getServiceFormSelector = formValueSelector(formTypesEnum.SERVICEFORM)

const OverwriteModal = ({
  name,
  generalDescriptionId,
  isLoading,
  language,
  serviceName,
  selectedGDName,
  publishedGDLanguages,
  languagesAvailabilities,
  change,
  reset,
  deleteApiCall,
  mergeInUIState,
  newServiceName,
  intl: { formatMessage }
}) => {
  const overwriteName = () => {
    change(formTypesEnum.SERVICEFORM, 'name', newServiceName)
    selectGD()
    mergeInUIState({
      key: 'overwriteNameFromGDDialog',
      value: {
        isOpen: false
      }
    })
  }

  const cancelOverwrite = () => {
    selectGD()
    mergeInUIState({
      key: 'overwriteNameFromGDDialog',
      value: {
        isOpen: false
      }
    })
  }

  const selectGD = () => change(formTypesEnum.SERVICEFORM, 'generalDescriptionId', generalDescriptionId)

  return (
    <ModalDialog name={name}
      title={formatMessage(messages.dialogOverwriteNameText)}>
      <ModalActions>
        <div className={styles.buttonGroup}>
          <Button small onClick={overwriteName}>
            {formatMessage(messages.buttonOk)}</Button>
          <Button small secondary onClick={cancelOverwrite}>
            {formatMessage(messages.buttonCancel)}
          </Button>
        </div>
      </ModalActions>
    </ModalDialog>
  )
}

OverwriteModal.propTypes = {
  name: PropTypes.string,
  generalDescriptionId: PropTypes.string,
  isLoading: PropTypes.bool.isRequired,
  language: PropTypes.string,
  serviceName: ImmutablePropTypes.map,
  selectedGDName: ImmutablePropTypes.map,
  newServiceName: ImmutablePropTypes.map,
  publishedGDLanguages: ImmutablePropTypes.list,
  languagesAvailabilities: ImmutablePropTypes.list,
  change: PropTypes.func,
  reset: PropTypes.func,
  deleteApiCall: PropTypes.func,
  mergeInUIState: PropTypes.func,
  intl: intlShape
}

export default compose(
  injectIntl,
  connect((state, ownProps) => {
    const generalDescriptionId = getServiceFormSelector(state, 'generalDescriptionId')
    const generalDescription = EntitySelectors.generalDescriptions.getEntity(state, { id: generalDescriptionId })
    return {
      generalDescriptionId,
      isLoading: EntitySelectors.generalDescriptions.getEntityIsFetching(state),
      language: getContentLanguageCode(state, { formName: formTypesEnum.SERVICEFORM }),
      serviceName: getServiceFormSelector(state, 'name'),
      selectedGDName: generalDescription && generalDescription.get('name'),
      publishedGDLanguages: getGDPublishedLanguages(state, { formName: formTypesEnum.SERVICEFORM }),
      languagesAvailabilities: getServiceFormSelector(state, 'languagesAvailabilities'),
      newServiceName: getNewServiceName(state, { generalDescriptionId, formName: formTypesEnum.SERVICEFORM })
    }
  }, {
    change,
    reset,
    deleteApiCall,
    mergeInUIState
  })
)(OverwriteModal)
