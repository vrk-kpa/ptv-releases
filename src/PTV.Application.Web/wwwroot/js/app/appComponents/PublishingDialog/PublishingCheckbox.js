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
import { Checkbox } from 'sema-ui-components'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { PTVIcon } from 'Components'
import Popup from 'appComponents/Popup'
import ItemList from 'appComponents/ItemList'
import PublishErrorsTitle from 'appComponents/PublishErrorsTitle'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import {
  getPublishingStatusDraftId,
  getPublishingStatusPublishedId,
  getPublishingStatusModifiedId,
  getPublishingStatusDeletedId
} from 'selectors/common'
import {
  getLanguageStatusId,
  getLanguageCode
} from './selectors'
import { getValidationMessages } from 'util/redux-form/syncValidation/publishing/selectors'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import PropTypes from 'prop-types'
import ImmutablePropTypes from 'react-immutable-proptypes'
import styles from './styles.scss'
import { fieldPropTypes } from 'redux-form/immutable'

const messages = defineMessages({
  statusPublished: {
    id: 'Containers.Common.PublishingStatus.Status.Published',
    defaultMessage: 'Julkaistu'
  }
})

const PublishingCheckbox = ({
  formPublishingStatusId,
  entityPublishingStatusId,
  input,
  canBePublished,
  errors,
  deletedStatusId,
  intl: { formatMessage },
  publishedStatusId,
  modifiedStatusId,
  draftStatusId,
  rowIndex,
  language
}) => {
  const checked = formPublishingStatusId === publishedStatusId
  const disabled = entityPublishingStatusId === publishedStatusId
  const handleOnChange = () => {
    let newValue = input.value.setIn([rowIndex, 'newStatusId'], checked
      ? (input.value.getIn([rowIndex, 'statusId']) || draftStatusId)
      : publishedStatusId)
    input.onChange(newValue)
  }
  // console.log(errors && errors.toJS())
  return <div className={styles.publishingCheckbox}>{
    !canBePublished &&
    <Popup
      position={'top left'}
      trigger={
        <div className={styles.publishError}>
          <PTVIcon name='icon-exclamation-circle' width={30} height={30} />
        </div>
      }
      maxWidth='mW500'
      hideOnScroll
    >
      <ItemList
        type='error'
        title={<PublishErrorsTitle />}
        items={errors}
        showForLanguage={language}
      />
    </Popup> ||
    <Checkbox
      checked={checked}
      onChange={handleOnChange}
      disabled={disabled}
    />
  }</div>
}

PublishingCheckbox.propTypes = {
  language: PropTypes.string,
  formPublishingStatusId: PropTypes.string.isRequired,
  entityPublishingStatusId: PropTypes.string.isRequired,
  publishedStatusId: PropTypes.string.isRequired,
  draftStatusId: PropTypes.string.isRequired,
  deletedStatusId: PropTypes.string.isRequired,
  modifiedStatusId: PropTypes.string.isRequired,
  canBePublished: PropTypes.bool.isRequired,
  rowIndex: PropTypes.number.isRequired,
  intl: intlShape,
  input: fieldPropTypes.input,
  errors: ImmutablePropTypes.list.isRequired
}

export default compose(
  injectFormName,
  connect((state, { newStatusId: formPublishingStatusId, rowIndex, formName, onClick }) => {
    const publishedStatusId = getPublishingStatusPublishedId(state)
    const draftStatusId = getPublishingStatusDraftId(state)
    const deletedStatusId = getPublishingStatusDeletedId(state)
    const entityPublishingStatusId = getLanguageStatusId(state, { rowIndex })
    return {
      formPublishingStatusId,
      entityPublishingStatusId,
      publishedStatusId,
      deletedStatusId,
      modifiedStatusId: getPublishingStatusModifiedId(state),
      draftStatusId,
      errors: getValidationMessages(state, { formName, onClick }),
      language: getLanguageCode(state, { rowIndex })
    }
  }),
  injectIntl
)(PublishingCheckbox)
