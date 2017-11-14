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
import {
  Modal,
  ModalTitle,
  ModalActions,
  ModalContent,
  Button,
  Spinner
} from 'sema-ui-components'
import { mergeInUIState } from 'reducers/ui'
import { clearPreviewDialogLanguage } from 'reducers/selections'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import { getEntityId } from './selectors'
import PreviewDialogContent from './PreviewDialogContent'
import { formEntityTypes, formAllTypes, entityConcreteTypesEnum } from 'enums'
import { EntitySelectors } from 'selectors'
import { getSelectedPreviewDialogLanguageCode, getContentLanguageCode } from 'selectors/selections'
import { entityTypesMessages } from 'Routes/messages'
import { AvailableLanguages } from 'appComponents'

const messages = defineMessages({
  closeDialogButtonTitle: {
    id: 'AppComponents.PreviewDialog.Buttons.Close.Title',
    defaultMessage: 'Sulje'
  }
})

const PreviewDialog = ({
  subEntityType,
  entityType,
  entityId,
  entityName,
  isOpen,
  isLoading,
  mergeInUIState,
  clearPreviewDialogLanguage,
  languageAvailabilities,
  previewLanguageCode,
  intl: { formatMessage },
  ...rest
}) => {
  const handleOnClose = () => {
    mergeInUIState({
      key: 'entityPreviewDialog',
      value: { isOpen: false },
      sourceForm: ''
    })
    clearPreviewDialogLanguage()
  }

  const concreteTypes = {
    'service': entityConcreteTypesEnum.SERVICE,
    'echannel': entityConcreteTypesEnum.ELECTRONICCHANNEL,
    'webpage': entityConcreteTypesEnum.WEBPAGECHANNEL,
    'printableform': entityConcreteTypesEnum.PRINTABLEFORMCHANNEL,
    'phone': entityConcreteTypesEnum.PHONECHANNEL,
    'servicelocation': entityConcreteTypesEnum.SERVICELOCATIONCHANNEL,
    'generaldescription': entityConcreteTypesEnum.GENERALDESCRIPTION,
    'organization': entityConcreteTypesEnum.ORGANIZATION
  }

  const texts = {
    [entityConcreteTypesEnum.SERVICE]: entityTypesMessages.services,
    [entityConcreteTypesEnum.ELECTRONICCHANNEL]: entityTypesMessages.eChannelLinkTitle,
    [entityConcreteTypesEnum.PHONECHANNEL]: entityTypesMessages.phoneLinkTitle,
    [entityConcreteTypesEnum.PRINTABLEFORMCHANNEL]: entityTypesMessages.printableFormLinkTitle,
    [entityConcreteTypesEnum.SERVICELOCATIONCHANNEL]: entityTypesMessages.serviceLocationLinkTitle,
    [entityConcreteTypesEnum.WEBPAGECHANNEL]: entityTypesMessages.webPageLinkTitle,
    [entityConcreteTypesEnum.GENERALDESCRIPTION]: entityTypesMessages.generalDescriptions,
    [entityConcreteTypesEnum.ORGANIZATION]: entityTypesMessages.organizations
  }

  const concreteType = subEntityType && concreteTypes[subEntityType.toLowerCase()] || ''
  const translatedConcreteType = concreteType && formatMessage(texts[concreteType])
  const titleText = `${entityName} (${translatedConcreteType})`
  return (
    <Modal isOpen={isOpen} onRequestClose={handleOnClose} contentLabel='Entity preview dialog' contentScroll>
      {!isLoading &&
        <ModalTitle title={titleText}>
          <AvailableLanguages availableLanguages={languageAvailabilities} />
        </ModalTitle>
      }
      <ModalContent>
        {isLoading &&
          <Spinner /> ||
          <PreviewDialogContent
            type={entityType}
            id={entityId}
            concreteType={concreteType}
            previewLanguageCode={previewLanguageCode}
          />
        }
      </ModalContent>
      <ModalActions>
        <Button onClick={handleOnClose}
          small
          secondary>
          {formatMessage(messages.closeDialogButtonTitle)}
        </Button>
      </ModalActions>
    </Modal>
  )
}
PreviewDialog.propTypes = {
  subEntityType: PropTypes.string,
  entityType: PropTypes.string,
  entityId: PropTypes.string.isRequired,
  entityName: PropTypes.string,
  isOpen: PropTypes.bool,
  isLoading: PropTypes.bool,
  mergeInUIState: PropTypes.func,
  clearPreviewDialogLanguage: PropTypes.func,
  languageAvailabilities: PropTypes.object,
  previewLanguageCode: PropTypes.string,
  intl: intlShape
}

export default compose(
  injectIntl,
  connect((state, ownProps) => {
    const entityType = formEntityTypes[ownProps.sourceForm] || ''
    const subEntityType = formAllTypes[ownProps.sourceForm] || ''
    const entityId = getEntityId(state, { ...ownProps, entityType })
    const entity = entityType && EntitySelectors[entityType].getEntity(state, { ...ownProps, id: entityId, type: entityType })
    const contentLanguageCode = getContentLanguageCode(state, { ...ownProps, id: entityId, type: entityType })
    const previewLanguageCode = getSelectedPreviewDialogLanguageCode(state) || contentLanguageCode
    const entityNames = entity && entity.get('name') || null
    return {
      entityId,
      isLoading: entityType && EntitySelectors[entityType].getEntityIsFetching(state) || false,
      entityName: entityNames && entityNames.get(previewLanguageCode) || '',
      languageAvailabilities: entity && entity.get('languagesAvailabilities'),
      entityType,
      subEntityType,
      previewLanguageCode
    }
  }, {
    mergeInUIState,
    clearPreviewDialogLanguage
  })
)(PreviewDialog)
