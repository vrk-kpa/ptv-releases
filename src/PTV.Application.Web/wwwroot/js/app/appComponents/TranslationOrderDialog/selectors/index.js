import { EntitySelectors, EnumsSelectors } from 'selectors'
import { createSelector } from 'reselect'
import { getContentLanguageId } from 'selectors/selections'
import { getUserName, getUserEmail } from 'selectors/userInfo'
import { getSelectedEntityId } from 'selectors/entities/entities'
import { fromJS, Set } from 'immutable'
import { getFormValues } from 'redux-form/immutable'
import { formTypesEnum } from 'enums'

export const isRequiredLanguageSelected = createSelector(
  getFormValues(formTypesEnum.TRANSLATIONORDERFORM),
  formValues => formValues && formValues.get('requiredLanguages').size > 0 || false
)

export const getTranslationCompanyId = createSelector(
  EnumsSelectors.translationCompanies.getEnums,
  companies => companies.first() || null
)

export const getTranslationCompany = createSelector(
  EntitySelectors.translationCompanies.getEntities,
  companies => companies.first() || null
)

export const getTranslationOrder = createSelector(
  [
    getSelectedEntityId,
    getUserName,
    getUserEmail,
    getContentLanguageId
  ], (
    entityId,
    senderName,
    senderEmail,
    sourceLanguage
  ) => fromJS({
    entityId,
    senderName,
    senderEmail,
    sourceLanguage,
    requiredLanguages: Set()
  })
)
