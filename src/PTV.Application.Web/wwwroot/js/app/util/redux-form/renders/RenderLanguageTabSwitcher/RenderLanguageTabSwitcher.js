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
import { injectFormName } from 'util/redux-form/HOC'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { setIsAddingNewLanguage, disableForm, setReadOnly } from 'reducers/formStates'
import { getIsAddingNewLanguage, getIsCompareMode } from 'selectors/formStates'
import { Tabs, Tab, Button } from 'sema-ui-components'
import styles from './styles.scss'
import cx from 'classnames'
import { localizeList } from 'appComponents/Localize'
import { EnumsSelectors } from 'selectors'
import { getContentLanguageId, getSelectedComparisionLanguage } from 'selectors/selections'
import { getFormValue } from 'selectors/base'
import { Map, List } from 'immutable'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import * as commonMessages from 'Routes/messages'
import { getEntityLanguageAvailabilities } from 'Routes/selectors'
import { createSelector } from 'reselect'
import { getPublishingStatusDraftCode, getPublishingStatusPublishedId } from 'selectors/common'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { getEntity } from 'selectors/entities/entities'

const messages = defineMessages({
  addNewLanguageTitle: {
    id: 'Util.ReduxForm.Renders.RenderLanguageTabSwitcher.AddNewLanguage.Title',
    defaultMessage: '+ Lisää kieliversio'
  }
})

const getLanguageAvailabilities = createSelector(
  getFormValue('languagesAvailabilities'),
  languages => languages || List()
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
  isReadOnly
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

  return (
    <div className={styles.languageTabSwitcher}>
      {input.value.size > 0 &&
        <Tabs index={activeIndex}>
          {input.value.map((language, index) => {
            const languageEntity = languages.get(language.get('languageId')) || language
            const statusCode = (publishingStatuses.get(language.get('statusId')) || Map()).get('code') ||
              publishingStatusDraftCode
            const langCode = languageEntity.get('code')
            const langId = languageEntity.get('id') || languageEntity.get('languageId')
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
            }

            const languageCodeClass = cx(
              styles.languageCode, styles[isArchived ? 'deleted' : statusCode.toLowerCase()]
            )
            return (
              <Tab
                key={index}
                label={
                  <span><span className={languageCodeClass}>{langCode}</span>
                    {languageEntity.get('name') || langCode}</span>}
                onClick={handleOnClick}
              />
            )
          })}
        </Tabs> ||
        <Tabs index={0}><Tab
          key={0}
          label={
            <span><span className={cx(styles.languageCode, styles[publishingStatusDraftCode.toLowerCase()]
            )}>{newLanguage.get('code')}</span>
              {newLanguage.get('name') || newLanguage.get('code')}</span>}
      /></Tabs>
      }
      {!isReadOnly && !isAddingNewLanguage && canAddLanguage &&
        <div className={styles.addNewButton}>
          <Button link onClick={handleAddNewLanguageOnClick}>{
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
  canAddLanguage: PropTypes.bool
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
      const previousInfo = entity.get('previousInfo') || Map()
      const publishedStatusId = getPublishingStatusPublishedId(state)
      const modifiedExist = !!(publishedStatusId === entityPS && previousInfo.get('lastModifiedId'))

      return {
        publishingStatusDraftCode: getPublishingStatusDraftCode(state),
        languages: EnumsSelectors.translationLanguages.getEntitiesMap(state),
        publishingStatuses: EnumsSelectors.publishingStatuses.getEntitiesMap(state),
        isAddingNewLanguage: getIsAddingNewLanguage(ownProps.formName)(state),
        canAddLanguage: !modifiedExist &&
          ownProps.input.value.size < EnumsSelectors.translationLanguages.getEnums(state).size,
        activeIndex: aIndex > 0 ? aIndex : (contentLanguageId == null && ownProps.input.value.size || 0),
        isCompareMode: getIsCompareMode(ownProps.formName)(state),
        selectedComparisionLanguage: getSelectedComparisionLanguage(state),
        entityLanguageAvailabilities: getLanguageAvailabilities(state, { formName: ownProps.formName })
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
    translateLocaleOnly: true
  }),
  withFormStates
)(LanguageTabSwitcher)
