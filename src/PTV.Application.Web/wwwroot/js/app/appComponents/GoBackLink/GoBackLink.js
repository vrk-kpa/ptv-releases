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
import styles from './styles.scss'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { injectIntl, defineMessages, intlShape } from 'util/react-intl'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { getShowReviewBar } from 'util/redux-form/HOC/withMassToolForm/selectors'
import { getIsReadOnly } from 'selectors/formStates'
import { goBack, goBackWithUnlockEntity } from './actions'
import { Button } from 'sema-ui-components'
import { getSelectedEntityId } from 'selectors/entities/entities'
import withState from 'util/withState'
import { withRouter } from 'react-router'

const messages = defineMessages({
  goBackLinkLabel: {
    id: 'Containers.Common.PageContainer.Link.Back',
    defaultMessage: 'Takaisin'
  },
  text: {
    id: 'Containers.Channel.ViewChannel.GoBack.Text',
    defaultMessage: 'Do you want to leave the page and lose all unsaved data?'
  },
  buttonOk: {
    id: 'Containers.Channel.ViewChannel.GoBack.Accept',
    defaultMessage: 'Kyllä'
  },
  buttonCancel: {
    id: 'Containers.Channel.ViewChannel.GoBack.Cancel',
    defaultMessage: 'Jatka muokkausta'
  }
})

const GoBackLink = ({
  path,
  isReadOnly,
  intl: { formatMessage },
  goBack,
  goBackWithUnlockEntity,
  isOpen,
  isInReview,
  formName,
  updateUI,
  entityId,
  history
}) => {
  const handleOnLinkClick = (value) => updateUI('isOpen', true)
  const handleOnCancelButtonClick = (value) => updateUI('isOpen', false)
  const handleGoBack = (path) => {
    goBack()
    history.push(path)
  }

  const handleGoBackWithUnlockEntity = (path) => {
    goBackWithUnlockEntity()
    history.push(path)
  }

  if (isInReview) return null

  return isReadOnly
    ? <Button link
      className={styles.goBackLink}
      onClick={() => handleGoBack(path)}>
      {formatMessage(messages.goBackLinkLabel)}
    </Button>
    : !isOpen
      ? <Button link
        className={styles.goBackLink}
        onClick={handleOnLinkClick}>
        {formatMessage(messages.goBackLinkLabel)}
      </Button>
      : <div className={styles.messageBox}>
        <h1 className={styles.messageBoxTitle}>{formatMessage(messages.text)}</h1>
        <div className={styles.messageBoxActions}>
          <Button small onClick={() => handleGoBackWithUnlockEntity(path)}>{formatMessage(messages.buttonOk)}</Button>
          <Button small link onClick={handleOnCancelButtonClick}>{formatMessage(messages.buttonCancel)}</Button></div>
      </div>
}

GoBackLink.propTypes = {
  path: PropTypes.string,
  formName: PropTypes.string.isRequired,
  goBack: PropTypes.func.isRequired,
  goBackWithUnlockEntity: PropTypes.func.isRequired,
  isReadOnly: PropTypes.bool,
  isOpen: PropTypes.bool,
  isInReview: PropTypes.bool,
  intl: intlShape,
  updateUI: PropTypes.func.isRequired,
  entityId: PropTypes.string.isRequired,
  history: PropTypes.object.isRequired
}

GoBackLink.defaultProps = {
  path: '/frontpage'
}

export default compose(
  injectIntl,
  injectFormName,
  withRouter,
  connect(
    (state, { formName }) => ({
      isReadOnly: getIsReadOnly(formName)(state),
      formName,
      entityId: getSelectedEntityId(state),
      isInReview: getShowReviewBar(state)
    }), {
      goBack,
      goBackWithUnlockEntity
    }
  ),
  withState({
    initialState: {
      isOpen: false
    }
  })
)(GoBackLink)
