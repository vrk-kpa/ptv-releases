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
import ModalDialog from 'appComponents/ModalDialog'
import styles from '../styles.scss'
import { messages } from '../messages'
import { Checkbox } from 'sema-ui-components'
import Spacer from 'appComponents/Spacer'
import { injectIntl, intlShape } from 'util/react-intl'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { connect } from 'react-redux'
import { PublishingStatus } from 'util/redux-form/fields'
import { getPublishingStatuses } from 'util/redux-form/fields/PublishingStatus/selectors'
import { Map } from 'immutable'
import { change } from 'redux-form/immutable'

export const publishingStatusDialogName = 'publishingStatusDialog'

const PublishingStatusDialog = props => {
  const {
    intl: { formatMessage },
    dispatch,
    formName,
    availablePublishingStatuses,
    isAllSelected,
    nothingSelected
  } = props

  const handleAllChange = () => {
    const newValue = Map(availablePublishingStatuses
      .map(value => [value.get('id'), !isAllSelected]))

    dispatch(change(formName, 'selectedPublishingStatuses', newValue))
  }

  const getNothingError = () => {
    const title = formatMessage(messages.publishingStatusFilterTitle) || ''
    const prefix = formatMessage(messages.nothingSelectedPrefix)

    return `${prefix} ${title.toLowerCase()}.`
  }

  return (
    <ModalDialog
      className={styles.searchFilterDialog}
      name={publishingStatusDialogName}
      title={`${formatMessage(messages.commonSearchFilterTitle)}: ${formatMessage(messages.publishingStatusFilterTitle)}`}
      preventClosing={nothingSelected}
    >
      <div className={styles.dialogBody}>
        {nothingSelected &&
          <div className={styles.validationError}>
            {getNothingError()}
          </div>}
        <Checkbox
          label={formatMessage(messages.allSelectedLabel)}
          onChange={handleAllChange}
          checked={isAllSelected}
          className={styles.node}
        />
        <Spacer />
        <PublishingStatus
          name='selectedPublishingStatuses'
          wrapClass={styles.publishingStatusWrap}
        />
      </div>
    </ModalDialog>
  )
}

PublishingStatusDialog.propTypes = {
  intl: intlShape,
  dispatch: PropTypes.func,
  formName: PropTypes.string,
  isAllSelected: PropTypes.bool,
  availablePublishingStatuses: PropTypes.any,
  nothingSelected: PropTypes.bool
}

export default compose(
  injectIntl,
  injectFormName,
  connect((state, ownProps) => {
    return {
      availablePublishingStatuses: getPublishingStatuses(state, ownProps)
    }
  })
)(PublishingStatusDialog)

