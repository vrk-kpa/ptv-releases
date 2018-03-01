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
import { getLocalizedOrganizationsJS, getTranslationExists } from 'selectors/common'
import { injectIntl, intlShape, defineMessages } from 'react-intl'
import { compose } from 'redux'
import { RenderSelect, RenderSelectDisplay } from 'util/redux-form/renders'
import { Field } from 'redux-form/immutable'
import { asDisableable, asComparable, withValidation } from 'util/redux-form/HOC'
import { isRequired } from 'util/redux-form/validators'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import { Label } from 'sema-ui-components'
import { getContentLanguageCode,
  getSelectedComparisionLanguageCode
} from 'selectors/selections'
import { getFormOrganization } from 'selectors/form'
import styles from './styles.scss'

const messages = defineMessages({
  label: {
    id: 'FrontPage.SelectOrganization.Title',
    defaultMessage: 'Organisaatio'
  },
  tooltip: {
    id: 'FrontPage.SelectOrganization.Tooltip',
    defaultMessage: 'Valitse pudotusvalikosta haluamasi organisaatio tai organisaatiotaso.'
  },
  organizationTranslationWarning: {
    id: 'FrontPage.SelectOrganization.Warning',
    defaultMessage: 'Organisaation tulee olla kuvattu asiointikanavakuvauksen kielellä.'
  }
})

const Organization = ({
  intl: { formatMessage },
  isOrganizationWithWarning,
  organizationTranslationExists,
  isReadOnly,
  disabled,
  ...rest
}) => {
  return (
    <div>
      <Field
        label={formatMessage(messages.label)}
        tooltip={formatMessage(messages.tooltip)}
        component={RenderSelect}
        searchable
        name='organization'
        disabled={disabled}
        {...rest}
    />

      { isOrganizationWithWarning && !isReadOnly && !disabled && !organizationTranslationExists &&
        <div className={styles.labelWarning}>
          <Label labelText={formatMessage(messages.organizationTranslationWarning)} />
        </div>
    }
    </div>
  )
}

Organization.propTypes = {
  intl: intlShape,
  validate: PropTypes.func,
  isOrganizationWithWarning: PropTypes.bool,
  organizationTranslationExists: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  disabled: PropTypes.bool
}

export default compose(
    injectIntl,
    asComparable({ DisplayRender: RenderSelectDisplay }),
    connect((state, ownProps) => {
      const options = getLocalizedOrganizationsJS(state, {
        showAll: ownProps.isReadOnly || ownProps.showAll, compare: ownProps.compare
      })
      if (ownProps.isOrganizationWithWarning) {
        const languageCode = getContentLanguageCode(state)
        const comparisionLanguageCode = getSelectedComparisionLanguageCode(state)
        const language = ownProps.compare && comparisionLanguageCode || languageCode
        const organizationId = getFormOrganization(state, ownProps)
        return {
          options,
          organizationTranslationExists: organizationId
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
