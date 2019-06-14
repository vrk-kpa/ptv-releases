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
import { compose } from 'redux'
import { connect } from 'react-redux'
import { defineMessages, injectIntl } from 'util/react-intl'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { getMassToolType } from './selectors'
import SearchFilterLanguage from '../SearchFilterLanguage'
import { Organization, Name } from 'util/redux-form/fields'
import { massToolTypes } from 'enums'
import styles from './styles.scss'

const messages = defineMessages({
  massCopyOrganizationPlaceholder: {
    id: 'MassTool.SelectionForm.CustomContent.MassCopy.Organization.Placeholder',
    defaultMessage: 'Anna tehtävälle nimi'
  },
  massCopyOrganizationLabel: {
    id: 'MassTool.SelectionForm.CustomContent.MassCopy.Organization.Title',
    defaultMessage: 'Organization'
  },
  taskListNamePlaceholder: {
    id: 'MassTool.SelectionForm.CustomContent.CreateTaskList.Name.Placeholder',
    defaultMessage: 'Anna tehtävälle nimi'
  },
  massPublishLanguageFilterTitle: {
    id: 'MassTool.SelectionForm.CustomContent.MassPublish.SearchFilterLanguage.Title',
    defaultMessage: 'Kielet'
  }
})

// Language filter is not used now, do not remove functionality
export const MassPublishCustomContent = compose(
  injectIntl,
  injectFormName,
  connect((state, { formName }) => ({
    massToolType: getMassToolType(state, { formName })
  }))
)(({
  massToolType,
  componentClass,
  intl: { formatMessage }
}) => {
  return massToolType === massToolTypes.PUBLISH && <div className={componentClass}>
    <SearchFilterLanguage
      filterName='reviewLanguages'
      title={formatMessage(messages.massPublishLanguageFilterTitle)}
      className={styles.massLanguageFilter}
    />
  </div>
})

export const MassCopyCustomContent = compose(
  injectIntl,
  injectFormName,
  connect((state, { formName }) => ({
    massToolType: getMassToolType(state, { formName })
  }))
)(({
  massToolType,
  componentClass,
  intl: { formatMessage }
}) => {
  return massToolType === massToolTypes.COPY && <div className={componentClass}>
    <Organization
      label={formatMessage(messages.massCopyOrganizationLabel)}
      labelPosition='inside'
      placeholder={formatMessage(messages.massCopyOrganizationPlaceholder)}
      searchOnlyDraftAndPublished
      skipValidation
    />
  </div>
})

export const CreateTaskListCustomContent = compose(
  injectIntl,
  injectFormName,
  connect((state, { formName }) => ({
    massToolType: getMassToolType(state, { formName })
  }))
)(({
  massToolType,
  componentClass,
  intl: { formatMessage }
}) => {
  return massToolType === massToolTypes.CREATETASKLIST && <div className={componentClass}>
    <Name
      label={null}
      placeholder={formatMessage(messages.taskListNamePlaceholder)}
      skipValidation
    />
  </div>
})
