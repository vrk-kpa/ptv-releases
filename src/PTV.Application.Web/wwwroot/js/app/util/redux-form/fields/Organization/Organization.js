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
import { connect } from 'react-redux'
import { getLocalizedOrganizationsJS } from 'selectors/common'
import { getTranslationExists } from 'Intl/Selectors'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import { RenderSelectDisplay, RenderAsyncSelect } from 'util/redux-form/renders'
import { Field } from 'redux-form/immutable'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'
import withValidation from 'util/redux-form/HOC/withValidation'
import { isRequired } from 'util/redux-form/validators'
import { normalize } from 'normalizr'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import { Label } from 'sema-ui-components'
import { EntitySchemas } from 'schemas'
import { getContentLanguageCode,
  getSelectedComparisionLanguageCode
} from 'selectors/selections'
import { getFormOrganization } from 'selectors/form'
import styles from './styles.scss'
import { camelizeKeys } from 'humps'
import { localizeItem } from 'appComponents/Localize'
import OrganizationWithStatusLabel from './OrganizationStatusLabel'
import { getPublishValidationExist } from './selectors'

const messages = defineMessages({
  label: {
    id: 'FrontPage.SelectOrganization.Title',
    defaultMessage: 'Organisaatio'
  },
  tooltip: {
    id: 'FrontPage.SelectOrganization.Tooltip',
    defaultMessage: 'Valitse pudotusvalikosta haluamasi organisaatio tai organisaatiotaso.'
  }
})

const OptionValue = compose(
  localizeItem({
    input: 'organization',
    output: 'organization',
    idAttribute: 'value',
    nameAttribute: 'label',
    getTranslationTexts: organization => organization.entity.translation.Texts || null
  })
)(OrganizationWithStatusLabel)

const formatDisplay = (org) =>
  org && <OrganizationWithStatusLabel organization={org} /> || null

const formatOption = (org) =>
  <OptionValue organization={org} />

const formatResponse = data =>
  data.map((org) => ({
    label: org.name,
    value: org.id,
    publishingStatus: org.publishingStatus,
    entity: org
  }))

const getQueryData = (searchValue, props) => ({
  searchValue,
  searchAll: !!props.showAll
})

const Organization = ({
  intl: { formatMessage },
  missingTranslationWarning,
  organizationTranslationExists,
  isReadOnly,
  disabled,
  // onChangeObject,
  dispatch,
  ...rest
}) => {
  const handleOnChangeObject = (object) => {
    // onChangeObject && onChangeObject(object && object.entity)
    object &&
      dispatch({ type: 'ORGANIZATIONS', response: normalize(camelizeKeys(object.entity), EntitySchemas.ORGANIZATION) })
  }

  return (
    <div>
      <Field
        label={formatMessage(messages.label)}
        tooltip={formatMessage(messages.tooltip)}
        component={RenderAsyncSelect}
        onChangeObject={handleOnChangeObject}
        formatResponse={formatResponse}
        valueRenderer={formatDisplay}
        optionRenderer={formatOption}
        formatDisplay={formatDisplay}
        getQueryData={getQueryData}
        filterOption={() => true}
        url='publicData/GetOrganizationList'
        name='organization'
        disabled={disabled}
        minLength={2}
        {...rest}
      />

      { missingTranslationWarning && !isReadOnly && !disabled && !organizationTranslationExists &&
        <div className={styles.labelWarning}>
          <Label labelText={formatMessage(missingTranslationWarning)} />
        </div>
      }
    </div>
  )
}

Organization.propTypes = {
  intl: intlShape,
  validate: PropTypes.func,
  missingTranslationWarning: PropTypes.object,
  organizationTranslationExists: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  disabled: PropTypes.bool
}

export default compose(
  injectIntl,
  asComparable({
    getDisplayRenderFromProps: (props) => typeof props.getDisplayRenderFromProps === 'function'
      ? props.getDisplayRenderFromProps(props)
      : RenderSelectDisplay
  }),
  connect((state, ownProps) => {
    const options = getLocalizedOrganizationsJS(state, {
      showAll: ownProps.isReadOnly || ownProps.showAll, compare: ownProps.compare
    })
    if (ownProps.missingTranslationWarning) {
      const languageCode = getContentLanguageCode(state)
      const comparisionLanguageCode = getSelectedComparisionLanguageCode(state)
      const language = ownProps.compare && comparisionLanguageCode || languageCode
      const organizationId = getFormOrganization(state, ownProps)
      const publishValidationExists = getPublishValidationExist(state, { ...ownProps, languageCode })
      return {
        options,
        organizationTranslationExists: (organizationId && !publishValidationExists)
          ? getTranslationExists(state, { id: organizationId, language: language })
          : true
      }
    } else {
      return {
        options
      }
    }
  }),
  asDisableable,
  withValidation({
    label: messages.label,
    validate: isRequired
  }),
  injectSelectPlaceholder()
)(Organization)
