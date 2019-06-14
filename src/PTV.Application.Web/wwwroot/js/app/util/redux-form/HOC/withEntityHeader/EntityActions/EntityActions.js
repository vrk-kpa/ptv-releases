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
import { Map } from 'immutable'

import { compose } from 'redux'
import { connect } from 'react-redux'
import { injectIntl, intlShape } from 'util/react-intl'
import { withRouter } from 'react-router'

import injectFormName from 'util/redux-form/HOC/injectFormName'

import { reset } from 'redux-form/immutable'
import { mergeInUIState } from 'reducers/ui'
import { getShowErrorsAction } from 'reducers/notifications'
import { apiCall3 } from 'actions'
import { setContentLanguage } from 'reducers/selections'
import {
  getSelectedEntityId,
  getEntity } from 'selectors/entities/entities'
import { EntitySelectors } from 'selectors'
import { getFormValue } from 'selectors/base'
import { getContentLanguageId } from 'selectors/selections'
import {
  getArchiveEntityVisibilitySelector,
  getWithdrawEntityVisibilitySelector,
  getRestoreEntityVisibilitySelector,
  getArchiveLanguageVisibilitySelector,
  getWithdrawLanguageVisibilitySelector,
  getRestoreLanguageVisibilitySelector,
  getTranslateVisibilitySelector,
  getCopyVisibilitySelector,
  getSelectedLanguageCode,
  getIsEntityActionsComboVisible,
  getCreateServiceVisibilitySelector,
  getRemoveEntityVisibilitySelector
} from '../selectors'

import {
  formEntityTypes,
  formTypesEnum,
  formActions,
  formActionsTypesEnum,
  formPaths,
  formApiPaths,
  entityActionTypesEnum,
  entityCoverageTypesEnum,
  permisionTypes,
  securityOrganizationCheckTypes
} from 'enums'
import { capitalizeFirstLetter } from 'util/helpers'
import { EntitySchemas } from 'schemas'
// import moment from 'moment'

import messages, { dialogMessages } from '../messages'

import { Select } from 'sema-ui-components'
import { localizeProps, languageTranslationTypes } from 'appComponents/Localize'
import CopyDialog from 'appComponents/CopyDialog/CopyDialog'
import TranslationOrderDialog from 'appComponents/TranslationOrderDialog'
import CancelEntityActionDialog from 'appComponents/CancelEntityActionDialog'
import EntityStatusChangeDialog from './EntityStatusChangeDialog'
import cx from 'classnames'
import styles from '../styles.scss'
import { getGeneralDescription } from 'Routes/GeneralDescription/actions'

const ActionOption = compose(
  injectIntl,
  injectFormName,
  connect((state, {
    visibilitySelector,
    optionsSelector,
    additionalMessageDefinition,
    formName,
    isAccessible,
    domain
  }) => {
    const entityId = getSelectedEntityId(state)
    const securityDomain = domain || formEntityTypes[formName]
    const checkOrganization = securityOrganizationCheckTypes.byOrganization
    const permisionType = entityId ? permisionTypes.update : permisionTypes.create
    const languageCode = getSelectedLanguageCode(state, { formName })
    return {
      visible: visibilitySelector(state, {
        formName: formName || 'not defined',
        languageCode,
        domain: securityDomain,
        checkOrganization,
        permisionType
      }),
      options: optionsSelector && optionsSelector(state, { formName }),
      additionalMessage: additionalMessageDefinition &&
        additionalMessageDefinition.selector && typeof additionalMessageDefinition.selector === 'function' &&
        additionalMessageDefinition.formatter && typeof additionalMessageDefinition.formatter === 'function' &&
        additionalMessageDefinition.formatter(additionalMessageDefinition.selector(state))
    }
  })
)(({ message, options, visible, additionalMessage, additionalMessageDefinition, intl: { formatMessage } }) => {
  const additionalMessageClassName = additionalMessageDefinition && additionalMessageDefinition.className
  const additionalClass = cx(styles.additionalMessage, additionalMessageClassName)
  return visible && (
    options
      ? <span>{formatMessage(message, options)}</span>
      : <span>
        {formatMessage(message)}
        {additionalMessage && <span className={additionalClass}>{formatMessage(additionalMessage)}</span>}
      </span>
  ) || null
})

const renderEntityActionOption = option => <ActionOption {...option} />

const options = [
  {
    value: () => {},
    action: entityActionTypesEnum.CREATESERVICE,
    message: messages.createServiceButton,
    visibilitySelector: getCreateServiceVisibilitySelector
  }, {
    value: () => {},
    action: formActionsTypesEnum.RESTOREENTITY,
    coverage: entityCoverageTypesEnum.ENTITY,
    message: messages.restoreEntityButton,
    visibilitySelector: getRestoreEntityVisibilitySelector
  }, {
    value: () => {},
    action: formActionsTypesEnum.RESTORELANGUAGE,
    coverage: entityCoverageTypesEnum.LANGUAGE,
    message: messages.restoreLanguageButton,
    visibilitySelector: getRestoreLanguageVisibilitySelector
  }, {
    value: () => {},
    action: formActionsTypesEnum.WITHDRAWENTITY,
    coverage: entityCoverageTypesEnum.ENTITY,
    message: messages.withdrawEntityButton,
    visibilitySelector: getWithdrawEntityVisibilitySelector
  }, {
    value: () => {},
    action: formActionsTypesEnum.WITHDRAWLANGUAGE,
    coverage: entityCoverageTypesEnum.LANGUAGE,
    message: messages.withdrawLanguageButton,
    visibilitySelector: getWithdrawLanguageVisibilitySelector
  }, {
    value: () => {},
    action: entityActionTypesEnum.TRANSLATE,
    message: messages.translateButton,
    visibilitySelector: getTranslateVisibilitySelector
  }, {
    value: () => {},
    action: formActionsTypesEnum.ARCHIVEENTITY,
    coverage: entityCoverageTypesEnum.ENTITY,
    message: messages.archiveEntityButton,
    visibilitySelector: getArchiveEntityVisibilitySelector
  }, {
    value: () => {},
    action: formActionsTypesEnum.ARCHIVELANGUAGE,
    coverage: entityCoverageTypesEnum.LANGUAGE,
    message: messages.archiveLanguageButton,
    visibilitySelector: getArchiveLanguageVisibilitySelector
  }, {
    value: () => {},
    action: formActionsTypesEnum.REMOVEENTITY,
    coverage: entityCoverageTypesEnum.ENTITY,
    message: messages.removeEntityButton,
    visibilitySelector: getRemoveEntityVisibilitySelector
  }, {
    value: () => {},
    action: entityActionTypesEnum.COPY,
    message: messages.copyButton,
    visibilitySelector: getCopyVisibilitySelector
  }
]

const successRedirectToNewService = (history, id) => {
  history.push({ pathname : `${formPaths[formTypesEnum.SERVICEFORM]}`, state:{ gd:id } })
}

const handleEntityActionCancel = (action, handleActionCancel, callback) => store => {
  handleActionCancel && handleActionCancel(action, callback, store)
}

class EntityActions extends Component {
  handleOnChange = ({ action }) => {
    const { handleActionCancel, handleEntityActionCancel } = this.props
    if (handleActionCancel) {
      handleEntityActionCancel(action, handleActionCancel, this.handleOnChangeAction)
      return
    }
    this.handleOnChangeAction(action)
  }

  handleOnChangeAction = (action) => {
    switch (action) {
      case formActionsTypesEnum.ARCHIVEENTITY:
        return this.handleArchiveEntityClick()
      case formActionsTypesEnum.ARCHIVELANGUAGE:
        return this.handleArchiveLanguageClick()
      case formActionsTypesEnum.WITHDRAWENTITY:
        return this.handleWithdrawEntityClick()
      case formActionsTypesEnum.WITHDRAWLANGUAGE:
        return this.handleWithdrawLanguageClick()
      case formActionsTypesEnum.RESTOREENTITY:
        return this.handleRestoreEntityClick()
      case formActionsTypesEnum.RESTORELANGUAGE:
        return this.handleRestoreLanguageClick()
      case entityActionTypesEnum.TRANSLATE:
        return this.handleTranslateClick()
      case entityActionTypesEnum.COPY:
        return this.handleCopyClick()
      case entityActionTypesEnum.CREATESERVICE:
        return this.handleCreateServiceClick()
      case formActionsTypesEnum.REMOVEENTITY:
        return this.handleRemoveEntityClick()
      default:
        return console.warn('Undefined entity action: ', action)
    }
  }

  // CREATE SERVICE
  handleCreateServiceClick = () => {
    const { history, entityUnificRootId, contentLanguageId, dispatch, languageCode } = this.props
    dispatch(setContentLanguage({ id:contentLanguageId, code:languageCode }))
    dispatch(getGeneralDescription({ id: entityUnificRootId, onlyPublished: true },
      { successCallback: () => successRedirectToNewService(history, entityUnificRootId) }))
  }
  // ARCHiVE ENTiTY
  handleArchiveEntityClick = () => {
    const { handleArchiveResponse, dispatch, formName, entityId } = this.props
    handleArchiveResponse
      ? dispatch(
        apiCall3({
          keys: [formEntityTypes[formName], 'lock'],
          payload: {
            endpoint: formActions[formActionsTypesEnum.ARCHIVEENTITY][formName] + '?id=' + entityId
          },
          successNextAction: this.handleArchiveSuccess,
          formName
        })
      )
      : this.handleArchiveSuccess()
  }
  handleArchiveSuccess = (response) => {
    const { handleArchiveResponse, dispatch, formName, intl: { formatMessage } } = this.props
    const description = handleArchiveResponse && handleArchiveResponse(response)
    dispatch(mergeInUIState({
      key: `${formName}${formActionsTypesEnum.ARCHIVEENTITY}Dialog`,
      value: {
        isOpen: true,
        description: description
          ? formatMessage(description)
          : formatMessage(dialogMessages[formName][entityActionTypesEnum.ARCHIVE].text)
      }
    }))
  }

  // ARCHiVE LANGUAGE
  handleArchiveLanguageClick = () => {
    const { dispatch, formName } = this.props
    dispatch(mergeInUIState({
      key: `${formName}${formActionsTypesEnum.ARCHIVELANGUAGE}Dialog`,
      value: {
        isOpen: true
      }
    }))
  }
  errorArchiveLanguageAction = (data) => () => {
    const { dispatch, formName } = this.props
    if (data && data[0] && data[0].code === 'Common.Exception.ArchiveLanguage') {
      dispatch(mergeInUIState({
        key: `${formName}${formActionsTypesEnum.ARCHIVEENTITY}Dialog`,
        value: {
          isOpen: true
        }
      }))
    } else {
      dispatch(getShowErrorsAction(formName, true)(data))
    }
  }

  // REMOVE ENTiTY
  handleRemoveEntityClick = () => {
    const { dispatch, formName } = this.props
    dispatch(mergeInUIState({
      key: `${formName}${formActionsTypesEnum.REMOVEENTITY}Dialog`,
      value: {
        isOpen: true
      }
    }))
  }

  // WiTHDRAW ENTiTY
  handleWithdrawEntityClick = () => {
    const { dispatch, formName } = this.props
    dispatch(mergeInUIState({
      key: `${formName}${formActionsTypesEnum.WITHDRAWENTITY}Dialog`,
      value: {
        isOpen: true
      }
    }))
  }

  // WiTHDRAW LANGUAGE
  handleWithdrawLanguageClick = () => {
    const { dispatch, formName } = this.props
    dispatch(mergeInUIState({
      key: `${formName}${formActionsTypesEnum.WITHDRAWLANGUAGE}Dialog`,
      value: {
        isOpen: true
      }
    }))
  }

  // RESTORE ENTiTY
  handleRestoreEntityClick = () => {
    const { dispatch, formName } = this.props
    dispatch(mergeInUIState({
      key: `${formName}${formActionsTypesEnum.RESTOREENTITY}Dialog`,
      value: {
        isOpen: true
      }
    }))
  }

  // RESTORE LANGUAGE
  handleRestoreLanguageClick = () => {
    const { dispatch, formName } = this.props
    dispatch(mergeInUIState({
      key: `${formName}${formActionsTypesEnum.RESTORELANGUAGE}Dialog`,
      value: {
        isOpen: true
      }
    }))
  }

  // COPY
  handleCopyClick = () => {
    const { dispatch, formName } = this.props
    dispatch(reset(formTypesEnum.COPYENTITYFORM))
    dispatch(mergeInUIState({
      key: `${formName}CopyDialog`,
      value: {
        isOpen: true
      }
    }))
  }

  // TRANSLATE
  handleTranslateClick = () => {
    const { dispatch, formName, entityId, contentLanguageId } = this.props
    dispatch(
      apiCall3({
        keys: ['translationOrderForm', 'translationOrderDialog'],
        payload: {
          endpoint: formApiPaths[formName] + '/GetTranslationData',
          data: { entityId: entityId, sourceLanguage: contentLanguageId }
        },
        schemas: EntitySchemas.TRANSLATION,
        successNextAction: this.showTranslationDialog,
        formName
      })
    )
  }
  showTranslationDialog = () => {
    const { dispatch } = this.props
    dispatch(mergeInUIState({
      key: `TranslationOrderDialog`,
      value: {
        isOpen: true
      }
    }))
  }

  successLanguageAction = ({ response: { result: { result } } }) => {
    const { history, formName } = this.props
    history.replace(`${formPaths[formName]}/${result}`)
  }

  successRemoveAction = ({ response: { result: { result } } }) => {
    const { history, formName } = this.props
    history.replace(`${formPaths[formName]}/${result}`)
  }

  render () {
    const {
      language,
      languageId,
      formName,
      isLockingUnlocking,
      history,
      isEntityActionsComboVisible,
      successArchiveAction,
      intl: { formatMessage }
    } = this.props
    return (
      <div>
        {isEntityActionsComboVisible && (
          <Select
            options={options}
            optionRenderer={renderEntityActionOption}
            onChange={this.handleOnChange}
            placeholder={formatMessage(messages.entityActionSelectPlaceholder)}
            disabled={isLockingUnlocking}
            className={styles.entityActions}
          />
        )}
        <EntityStatusChangeDialog
          title={formatMessage(dialogMessages[formName][entityActionTypesEnum.ARCHIVE].title)}
          action={formActionsTypesEnum.ARCHIVEENTITY}
          coverage={entityCoverageTypesEnum.ENTITY}
          successNextAction={response => successArchiveAction(this.props.dispatch, response)}
        />
        <EntityStatusChangeDialog
          title={formatMessage(dialogMessages[formName][entityActionTypesEnum.REMOVE].title)}
          description={formatMessage(dialogMessages[formName][entityActionTypesEnum.REMOVE].text)}
          action={formActionsTypesEnum.REMOVEENTITY}
          coverage={entityCoverageTypesEnum.ENTITY}
          successNextAction={response => this.successRemoveAction(response)}
        />
        <EntityStatusChangeDialog
          title={formatMessage(dialogMessages[formName][entityActionTypesEnum.WITHDRAW].title)}
          description={formatMessage(dialogMessages[formName][entityActionTypesEnum.WITHDRAW].text)}
          action={formActionsTypesEnum.WITHDRAWENTITY}
          coverage={entityCoverageTypesEnum.ENTITY}
          successNextAction={response => successArchiveAction(this.props.dispatch, response)}
        />
        <EntityStatusChangeDialog
          title={formatMessage(dialogMessages[formName][entityActionTypesEnum.RESTORE].title)}
          description={formatMessage(dialogMessages[formName][entityActionTypesEnum.RESTORE].text)}
          action={formActionsTypesEnum.RESTOREENTITY}
          coverage={entityCoverageTypesEnum.ENTITY}
        />
        <EntityStatusChangeDialog
          title={formatMessage(messages.archiveLanguageDialogTitle)}
          description={formatMessage(messages.archiveLanguageDialogText,
            { language: capitalizeFirstLetter(language) })}
          action={formActionsTypesEnum.ARCHIVELANGUAGE}
          coverage={entityCoverageTypesEnum.LANGUAGE}
          successNextAction={this.successLanguageAction}
          onErrorAction={this.errorArchiveLanguageAction}
          languageId={languageId}
        />
        <EntityStatusChangeDialog
          title={formatMessage(messages.withdrawLanguageDialogTitle)}
          description={formatMessage(messages.withdrawLanguageDialogText,
            { language: capitalizeFirstLetter(language) })}
          action={formActionsTypesEnum.WITHDRAWLANGUAGE}
          coverage={entityCoverageTypesEnum.LANGUAGE}
          successNextAction={this.successLanguageAction}
          languageId={languageId}
        />
        <EntityStatusChangeDialog
          title={formatMessage(messages.restoreLanguageDialogTitle)}
          description={formatMessage(messages.restoreLanguageDialogText,
            { language: capitalizeFirstLetter(language) })}
          action={formActionsTypesEnum.RESTORELANGUAGE}
          coverage={entityCoverageTypesEnum.LANGUAGE}
          successNextAction={this.successLanguageAction}
          languageId={languageId}
        />
        <TranslationOrderDialog />
        <CopyDialog history={history} />
        <CancelEntityActionDialog action={formActionsTypesEnum.ARCHIVEENTITY} />
        <CancelEntityActionDialog action={formActionsTypesEnum.ARCHIVELANGUAGE} />
      </div>
    )
  }
}
EntityActions.propTypes = {
  intl: intlShape,
  formName: PropTypes.string.isRequired,
  isLockingUnlocking: PropTypes.bool,
  handleArchiveResponse: PropTypes.func,
  successArchiveAction: PropTypes.func,
  handleActionCancel: PropTypes.func,
  handleEntityActionCancel: PropTypes.func,
  dispatch: PropTypes.func.isRequired,
  entityId: PropTypes.string.isRequired,
  entityUnificRootId: PropTypes.string,
  languageId: PropTypes.string,
  languageCode: PropTypes.string,
  contentLanguageId: PropTypes.string,
  language: PropTypes.string.isRequired,
  history: PropTypes.object.isRequired,
  isEntityActionsComboVisible: PropTypes.bool.isRequired
}

export default compose(
  injectIntl,
  injectFormName,
  withRouter,
  connect((state, { formName, domain }) => {
    const entityId = getSelectedEntityId(state)
    const entity = getEntity(state)
    const entityUnificRootId = entity.get('unificRootId')
    const formLanguages = getFormValue('languagesAvailabilities')(state, { formName })
    const contentLanguageId = getContentLanguageId(state, { formName })
    const selectedLanguage = formLanguages.filter(x => x.get('languageId') === contentLanguageId).first() ||
      formLanguages.first() ||
      Map()
    const languageCode = getSelectedLanguageCode(state, { formName })
    const securityDomain = domain || formEntityTypes[formName]
    const checkOrganization = securityOrganizationCheckTypes.byOrganization
    const permisionType = entityId ? permisionTypes.update : permisionTypes.create
    return {
      isLockingUnlocking: EntitySelectors[formEntityTypes[formName]].getEntityIsLoading(state),
      contentLanguageId,
      entityUnificRootId,
      languageCode,
      languageId: selectedLanguage.get('languageId'),
      entityId,
      formName,
      isEntityActionsComboVisible: getIsEntityActionsComboVisible(state, {
        formName: formName || 'not defined',
        domain: securityDomain,
        checkOrganization,
        permisionType,
        languageCode,
        options
      })
    }
  }, {
    handleEntityActionCancel
  }),
  localizeProps({
    nameAttribute: 'serviceType',
    idAttribute: 'type'
  }),
  localizeProps({
    nameAttribute: 'language',
    idAttribute: 'languageId',
    languageTranslationType: languageTranslationTypes.locale
  })
)(EntityActions)
