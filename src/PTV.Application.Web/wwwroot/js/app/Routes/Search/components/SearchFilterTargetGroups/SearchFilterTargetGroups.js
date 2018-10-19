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
import { TargetGroupSearchTree } from 'util/redux-form/fields'
import { getTargetGroup } from 'util/redux-form/fields/TargetGroupTree/selectors'
import { RenderDropdownFilter } from 'util/redux-form/renders'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { localizeItem, languageTranslationTypes } from 'appComponents/Localize'

const messages = defineMessages({
  title:{
    id: 'Containers.Services.AddService.Step2.TargetGroup.Title',
    defaultMessage: 'Kohderyhmä'
  }
})

const renderContent = (all, props) => (
  <TargetGroupSearchTree disabled={all} filterTree {...props} />
)

const ShowValue = compose(
  connect((state, { firstValue, displayValue }) => ({
    displayValue : firstValue && getTargetGroup(state, { id: firstValue }).get('name') || displayValue
  })),
  localizeItem({
    input: 'node',
    output: 'node',
    languageTranslationType: languageTranslationTypes.locale
  })
)(({ displayValue, node }) =>
  node && node.get('name') || displayValue
)

const SearchFilterTargetGroups = ({
  intl: { formatMessage },
  ...rest
}) => {
  return (
    <Field
      name='targetGroups'
      component={RenderDropdownFilter}
      content={renderContent}
      title={formatMessage(messages.title)}
      ShowValueComponent={ShowValue}
      filterName='targetGroupSearch'
      {...rest}
    />
  )
}

SearchFilterTargetGroups.propTypes = {
  formName: PropTypes.string.isRequired,
  selectedValues: PropTypes.any,
  intl: intlShape
}

export default compose(
  injectIntl,
  injectFormName
  // connect(
  //   (state, ownProps) => {
  //     const getFormValues = formValueSelector(ownProps.formName)
  //     const ids = getFormValues(state, 'targetGroups') || List()
  //     return {
  //       selectedValues: getTargetGroupsByIds(ids)(state)
  //     }
  //   },
  //   {
  //     reset: (formName) => ({ dispatch }) => dispatch(change(formName, 'targetGroups', OrderedSet()))
  //   })
)(SearchFilterTargetGroups)
