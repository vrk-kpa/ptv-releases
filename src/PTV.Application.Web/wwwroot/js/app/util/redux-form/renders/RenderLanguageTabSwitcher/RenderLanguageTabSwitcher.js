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
import { connect } from 'react-redux'
import { setContentLanguage, setComparisionLanguage } from 'reducers/selections'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { setIsAddingNewLanguage, disableForm, setReadOnly } from 'reducers/formStates'
import { getIsAddingNewLanguage, getIsCompareMode } from 'selectors/formStates'
import { Tabs, Tab, Button } from 'sema-ui-components'
import Popup from 'appComponents/Popup'
import styles from './styles.scss'
import cx from 'classnames'
import { localizeList, languageTranslationTypes } from 'appComponents/Localize'
import LanguageStatusSquare from 'appComponents/LanguageStatusSquare'
import { EnumsSelectors } from 'selectors'
import { getContentLanguageId, getSelectedComparisionLanguage } from 'selectors/selections'
import { getShowReviewBar } from 'util/redux-form/HOC/withMassToolForm/selectors'
import { getFormValue } from 'selectors/base'
import { Map, List } from 'immutable'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import * as commonMessages from 'Routes/messages'
import { createSelector } from 'reselect'
import {
  getPublishingStatusDraftCode,
  getPublishingStatusPublishedId
} from 'selectors/common'
import ImmutablePropTypes from 'react-immutable-proptypes'
import {
  getEntity,
  getTranslationAvailability,
  getPreviousInfoVersion
} from 'selectors/entities/entities'
import { mergeInUIState } from 'reducers/ui'
import { formTypesEnum } from 'enums'
import { getFormValues } from 'redux-form/immutable'
import { getIsSoteOrganizationType } from 'util/redux-form/fields/OrganizationType/selectors'

const messages = defineMessages({
  addNewLanguageTitle: {
    id: 'Util.ReduxForm.Renders.RenderLanguageTabSwitcher.AddNewLanguage.Title',
    defaultMessage: '+ Lisää kieliversio'
  },
  translationArrived: {
    id: 'Util.ReduxForm.Renders.RenderLanguageTabSwitcher.TranslationArrived.Tooltip',
    defaultMessage: 'Käännös saapunut'
  },
  translationSent: {
    id: 'Util.ReduxForm.Renders.RenderLanguageTabSwitcher.TranslationSent.Tooltip',
    defaultMessage: 'Käännös tilattu'
  }
})

const getLanguageAvailabilities = createSelector(
  getFormValue('languagesAvailabilities'),
  languages => languages || List()
)

const getReviewedRecords = createSelector(
  getFormValues(formTypesEnum.MASSTOOLFORM),
  formValues => {
    return formValues && formValues.get('review') || List()
  }
)

const getReviewedEntitiesApprovedLanguages = createSelector(
  getReviewedRecords,
  records => {
    const approvedRecords = records.filter(record => record.get('approved'))
    return approvedRecords.reduce((acc, curr) => {
      const key = curr.get('id')
      const value = curr.get('language')
      return acc.update(key, (x = List()) => x.push(value))
    }, Map())
  }
)

const getCurrentEntityApprovedLanguages = createSelector(
  getReviewedEntitiesApprovedLanguages,
  getEntity,
  (all, selected) => all.get(selected.get('id')) || List()
)

const LanguageTabSwitcher = ({
  input,
  intl: { formatMessage },
  languages,
  publishingStatuses,
  activeIndex,
  setContentLanguage,
  setIsAddingNewLanguage,
  setReadOnly,
  disableForm,
  isAddingNewLanguage,
  formName,
  publishingStatusDraftCode,
  canAddLanguage,
  isCompareMode,
  isArchived,
  selectedComparisionLanguage,
  entityLanguageAvailabilities,
  setComparisionLanguage,
  isReadOnly,
  translationAvailability,
  dispatch,
  hasError,
  approvedLanguages,
  isInReview,
  isSoteOrganization
}) => {
  const newLanguage = Map({
    code: '?',
    name: formatMessage(commonMessages.messages.languageVersionTitleNew).toUpperCase(),
    languageId: 'temp_lang_id'
  })

  const handleAddNewLanguageOnClick = () => {
    setIsAddingNewLanguage({
      form: formName,
      value: true// !isAddingNewLanguage
    })
    disableForm(formName)
    // this cause set of values in draft js to default
    setReadOnly({
      form: formName,
      value: false
    })
    input.onChange(input.value.push(newLanguage))
  }

  const collapseAccordions = (inTranslation) => {
    dispatch(mergeInUIState({
      key: `translationAccordion`,
      value: {
        activeIndex: inTranslation ? -1 : 0
      }
    }))
  }

  return (
    <div className={styles.languageTabSwitcher}>
      {input.value.size > 0 &&
        <Tabs index={activeIndex}>
          {input.value.map((language, index) => {
            const languageEntity = languages.get(language.get('languageId')) || language
            const statusId = language.get('statusId')
            const validFrom = language.get('validFrom')
            // const statusCode = (publishingStatuses.get(statusId) || Map()).get('code') ||
            //   publishingStatusDraftCode
            const langCode = languageEntity.get('code')
            const langId = languageEntity.get('id') || languageEntity.get('languageId')
            const languageTranslationAvailability = translationAvailability.get(langCode)
            const isTranslationDelivered = languageTranslationAvailability &&
              languageTranslationAvailability.get('isTranslationDelivered')
            const isInTranslation = languageTranslationAvailability &&
              languageTranslationAvailability.get('isInTranslation')
            const isApprovedLanguage = isInReview && approvedLanguages.includes(langCode)
            const handleOnClick = () => {
              const comparisionLanguageSameAsEntityLanguage =
                selectedComparisionLanguage.get('code') === langCode ||
                selectedComparisionLanguage.get('id') === langId
              if (
                isCompareMode &&
                comparisionLanguageSameAsEntityLanguage &&
                entityLanguageAvailabilities.size > 1
              ) {
                const newComparisionLanguageId = entityLanguageAvailabilities
                  .find(language => language.get('languageId') !== selectedComparisionLanguage.get('id'))
                  .get('languageId')
                const newComparisionLanguage = languages.get(newComparisionLanguageId)
                setComparisionLanguage({
                  id: newComparisionLanguage.get('id'),
                  code: newComparisionLanguage.get('code')
                })
              }
              setContentLanguage({ id: langId, code: langCode })
              collapseAccordions(isInTranslation)
            }

            const languageNameClass = cx(
              styles.languageName, { [styles.isApproved]: isApprovedLanguage }
            )
            const tabDisabled = !isReadOnly && hasError || (isInReview && activeIndex !== index)
            return (
              <Tab
                disabled={tabDisabled}
                key={index}
                label={
                  <span className={styles.languageTab}>
                    <LanguageStatusSquare
                      code={langCode}
                      languageId={langId}
                      statusId={statusId}
                      isArchived={isArchived}
                      validFrom={validFrom}
                    />
                    <span className={languageNameClass}>{languageEntity.get('name') || langCode}</span>
                    {(isTranslationDelivered || isInTranslation) &&
                      <Popup
                        trigger={<span className={styles.notificationIcon}>!</span>}
                        position='top center'
                        on='hover'
                        dark
                        iconClose={false}
                      >
                        {isTranslationDelivered
                          ? formatMessage(messages.translationArrived)
                          : formatMessage(messages.translationSent) }
                      </Popup>
                    }
                  </span>
                }
                onClick={handleOnClick}
              />
            )
          })}
        </Tabs> ||
        <Tabs index={0}><Tab
          key={0}
          label={
            <span className={styles.languageTab}>
              <span className={cx(styles.languageCode, styles[publishingStatusDraftCode.toLowerCase()])}>
                {newLanguage.get('code')}
              </span>
              <span className={styles.languageName}>{newLanguage.get('name') || newLanguage.get('code')}</span>
            </span>
          }
        /></Tabs>
      }
      {!isReadOnly && !isAddingNewLanguage && canAddLanguage && !isSoteOrganization &&
        <div className={styles.addNewButton}>
          <Button link onClick={handleAddNewLanguageOnClick} disabled={hasError}>{
            formatMessage(messages.addNewLanguageTitle)
          }</Button>
        </div>
      }
    </div>
  )
}
LanguageTabSwitcher.propTypes = {
  input: PropTypes.any,
  languages: ImmutablePropTypes.map.isRequired,
  publishingStatuses: ImmutablePropTypes.map.isRequired,
  setContentLanguage: PropTypes.func,
  setIsAddingNewLanguage: PropTypes.func,
  setReadOnly: PropTypes.func,
  isAddingNewLanguage: PropTypes.bool,
  intl: intlShape,
  activeIndex: PropTypes.number.isRequired,
  disableForm: PropTypes.func,
  formName: PropTypes.string,
  publishingStatusDraftCode: PropTypes.string,
  canAddLanguage: PropTypes.bool,
  translationAvailability: ImmutablePropTypes.map.isRequired,
  hasError: PropTypes.bool,
  approvedLanguages: ImmutablePropTypes.list,
  isInReview: PropTypes.bool,
  dispatch: PropTypes.func,
  isCompareMode: PropTypes.bool,
  isArchived: PropTypes.bool,
  selectedComparisionLanguage: ImmutablePropTypes.map,
  entityLanguageAvailabilities: ImmutablePropTypes.list,
  setComparisionLanguage: PropTypes.func,
  isReadOnly: PropTypes.bool,
  isSoteOrganization: PropTypes.bool
}

export default compose(
  injectIntl,
  injectFormName,
  connect(
    (state, ownProps) => {
      const contentLanguageId = getContentLanguageId(state, ownProps)
      const aIndex = (ownProps.input.value || List()).findIndex(language =>
        language.get('languageId') === contentLanguageId)

      const entity = getEntity(state)
      const entityPS = entity.get('publishingStatus')
      const previousInfo = getPreviousInfoVersion(state)
      const publishedStatusId = getPublishingStatusPublishedId(state)
      const modifiedExist = !!(publishedStatusId === entityPS && previousInfo.get('lastModifiedId'))
      const hasError = ownProps.getHasErrorSelector && ownProps.getHasErrorSelector(state, ownProps)
      const approvedLanguages = getCurrentEntityApprovedLanguages(state)
      const isInReview = getShowReviewBar(state)
      return {
        publishingStatusDraftCode: getPublishingStatusDraftCode(state),
        languages: EnumsSelectors.translationLanguages.getEntitiesMap(state),
        publishingStatuses: EnumsSelectors.publishingStatuses.getEntitiesMap(state),
        isAddingNewLanguage: getIsAddingNewLanguage(ownProps.formName)(state),
        canAddLanguage: !isInReview && !modifiedExist &&
          ownProps.input.value.size < EnumsSelectors.translationLanguages.getEnums(state).size,
        activeIndex: aIndex > 0 ? aIndex : (contentLanguageId == null && ownProps.input.value.size || 0),
        isCompareMode: getIsCompareMode(ownProps.formName)(state),
        selectedComparisionLanguage: getSelectedComparisionLanguage(state),
        entityLanguageAvailabilities: getLanguageAvailabilities(state, { formName: ownProps.formName }),
        translationAvailability: getTranslationAvailability(state),
        hasError,
        approvedLanguages,
        isInReview,
        isSoteOrganization: getIsSoteOrganizationType(state)
      }
    }, {
      setContentLanguage,
      setIsAddingNewLanguage,
      setComparisionLanguage,
      setReadOnly,
      disableForm
    }
  ),
  localizeList({
    input: 'languages',
    languageTranslationType: languageTranslationTypes.locale
  }),
  withFormStates
)(LanguageTabSwitcher)
