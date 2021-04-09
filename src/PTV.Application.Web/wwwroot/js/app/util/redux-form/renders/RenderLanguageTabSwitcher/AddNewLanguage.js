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
import { connect } from 'react-redux'
import { setContentLanguage, clearComparisionLanguage, clearContentLanguage } from 'reducers/selections'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { setIsAddingNewLanguage, disableForm, setReadOnly, setCompareMode, enableForm } from 'reducers/formStates'
import { getIsAddingNewLanguage } from 'selectors/formStates'
import { Button } from 'sema-ui-components'
import styles from './styles.scss'
import { EnumsSelectors } from 'selectors'
import { getShowReviewBar } from 'util/redux-form/HOC/withMassToolForm/selectors'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { getPublishingStatusPublishedId } from 'selectors/common'
import ImmutablePropTypes from 'react-immutable-proptypes'
import {
  getEntity,
  getPreviousInfoVersion
} from 'selectors/entities/entities'
import { getIsSoteOrganizationType } from 'util/redux-form/fields/OrganizationType/selectors'
import { change } from 'redux-form/immutable'
import { List } from 'immutable'
import { getFormValue } from 'selectors/base'

const messages = defineMessages({
  addNewLanguageTitle: {
    id: 'Util.ReduxForm.Renders.RenderLanguageTabSwitcher.AddNewLanguage.Title',
    defaultMessage: '+ Lisää kieliversio'
  },
  cancelAddingNewLanguage: {
    id: 'NewLanguageSelection.Cancel.Title',
    defaultMessage: '- Poista uusi kieliversio'
  }
})

const AddNewLanguage = ({
  input,
  intl: { formatMessage },
  newLanguage,
  setContentLanguage,
  setIsAddingNewLanguage,
  setReadOnly,
  disableForm,
  enableForm,
  formName,
  change,
  canAddLanguage,
  hasError,
  clearComparisionLanguage,
  clearContentLanguage,
  formLanguages,
  setCompareMode,
  canCancelLanguage
}) => {
  const handleAddNewLanguageOnClick = () => {
    setIsAddingNewLanguage({
      form: formName,
      value: true
    })
    disableForm(formName)
    // this cause set of values in draft js to default
    setReadOnly({
      form: formName,
      value: false
    })
    input.onChange(input.value.push(newLanguage))
    setCompareMode({
      form: formName,
      value: false
    })
    clearComparisionLanguage()
    setContentLanguage({ id: newLanguage.get('languageId'), code: newLanguage.get('code') })
  }

  const handleOnCancelAddingNewLanguage = () => {
    enableForm(formName)
    clearContentLanguage()
    setIsAddingNewLanguage({
      form: formName,
      value: false
    })
    if (formLanguages) {
      change(formName, 'languagesAvailabilities', formLanguages.pop())
    }
  }

  return (
    <div>
      {canAddLanguage && (
        <div className={styles.addNewButton}>
          <Button link onClick={handleAddNewLanguageOnClick} disabled={hasError}>{
            formatMessage(messages.addNewLanguageTitle)
          }</Button>
        </div>
      )}
      {canCancelLanguage && (
        <div className={styles.addNewButton}>
          <Button
            link
            onClick={handleOnCancelAddingNewLanguage}
            className={styles.cancelAddingNewLanguage}
          >
            <span className={styles.cancelAddNewLanguageText}>&ndash; {formatMessage(messages.cancelAddingNewLanguage)}</span>
          </Button>
        </div>
      )}
    </div>
  )
}
AddNewLanguage.propTypes = {
  input: PropTypes.any,
  newLanguage: ImmutablePropTypes.map.isRequired,
  setContentLanguage: PropTypes.func,
  setIsAddingNewLanguage: PropTypes.func,
  setReadOnly: PropTypes.func,
  intl: intlShape,
  disableForm: PropTypes.func,
  formName: PropTypes.string,
  canAddLanguage: PropTypes.bool,
  hasError: PropTypes.bool,
  clearComparisionLanguage: PropTypes.func,
  setCompareMode: PropTypes.func,
  enableForm: PropTypes.func.isRequired,
  clearContentLanguage: PropTypes.func.isRequired,
  change: PropTypes.func.isRequired,
  formLanguages: ImmutablePropTypes.list,
  canCancelLanguage: PropTypes.bool
}

export default compose(
  injectIntl,
  injectFormName,
  connect(
    (state, ownProps) => {
      const isAddingNewLanguage = getIsAddingNewLanguage(ownProps.formName)(state)
      const entity = getEntity(state)
      const entityPS = entity.get('publishingStatus')
      const previousInfo = getPreviousInfoVersion(state)
      const publishedStatusId = getPublishingStatusPublishedId(state)
      const modifiedExist = !!(publishedStatusId === entityPS && previousInfo.get('lastModifiedId'))
      const isInReview = getShowReviewBar(state)
      const isSoteOrganization = getIsSoteOrganizationType(state)
      return {
        canAddLanguage: !isInReview && !modifiedExist && !isAddingNewLanguage && !isSoteOrganization &&
          ownProps.input.value.size < EnumsSelectors.translationLanguages.getEnums(state).size,
        formLanguages: getFormValue('languagesAvailabilities')(state,
          { formName: ownProps.formName }) || List(),
        canCancelLanguage: isAddingNewLanguage && ownProps.input.value.size > 0
      }
    }, {
      setContentLanguage,
      setIsAddingNewLanguage,
      setReadOnly,
      disableForm,
      enableForm,
      change,
      clearComparisionLanguage,
      clearContentLanguage,
      setCompareMode
    }
  )
)(AddNewLanguage)
