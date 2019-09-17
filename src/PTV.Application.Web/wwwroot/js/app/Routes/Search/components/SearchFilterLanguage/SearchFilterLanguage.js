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
import { compose } from 'redux'
import { Field } from 'redux-form/immutable'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { LanguageSearchTree } from 'util/redux-form/fields'
import { RenderDropdownFilter } from 'util/redux-form/renders'
import { injectIntl, intlShape } from 'util/react-intl'
import { getLanguageIdsByCode } from 'selectors/selections'
import { localizeProps, languageTranslationTypes } from 'appComponents/Localize'

const renderContent = (all, props) => (
  <LanguageSearchTree disabled={all} filterTree {...props} />
)

const ShowValue = compose(
  connect((state, { firstValue, displayValue }) => ({
    id : firstValue && getLanguageIdsByCode(state).get(firstValue) || null,
    name: displayValue
  })),
  localizeProps({
    languageTranslationType: languageTranslationTypes.locale
  })
)(({ name }) => name)

const SearchFilterLanguage = ({
  intl: { formatMessage },
  selectedValues,
  filterName,
  title,
  ...rest
}) => {
  return (
    <Field
      name='languages'
      component={RenderDropdownFilter}
      content={renderContent}
      title={title}
      selectedValues={selectedValues}
      filterName={filterName}
      ShowValueComponent={ShowValue}
      selectAllIfEmpty
      {...rest}
    />
  )
}

SearchFilterLanguage.propTypes = {
  formName: PropTypes.string.isRequired,
  selectedValues: PropTypes.any,
  filterName: PropTypes.string.isRequired,
  title: PropTypes.string,
  intl: intlShape
}

export default compose(
  injectIntl,
  injectFormName
)(SearchFilterLanguage)
