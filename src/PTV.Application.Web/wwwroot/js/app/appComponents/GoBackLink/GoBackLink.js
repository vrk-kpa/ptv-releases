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
import { getShowReviewBar } from 'util/redux-form/HOC/withMassToolForm/selectors'
import { goBack, goBackWithUnlockEntity } from './actions'
import { Button } from 'sema-ui-components'
import { getSelectedEntityId } from 'selectors/entities/entities'
import { mergeInUIState } from 'reducers/ui'
import { push } from 'connected-react-router'

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
  isInReview,
  push
}) => {
  const handleOnLinkClick = () => {
    push(path, { shouldClean: true })
  }

  const handleGoBack = (path) => {
    goBack()
    push(path, { shouldClean: true })
  }

  if (isInReview) return null

  return isReadOnly
    ? <Button link
      className={styles.goBackLink}
      onClick={() => handleGoBack(path)}
      type='button'>
      {formatMessage(messages.goBackLinkLabel)}
    </Button>
    : <Button link
      className={styles.goBackLink}
      onClick={handleOnLinkClick}
      type='button'>
      {formatMessage(messages.goBackLinkLabel)}
    </Button>
}

GoBackLink.propTypes = {
  path: PropTypes.string,
  goBack: PropTypes.func.isRequired,
  isReadOnly: PropTypes.bool,
  isInReview: PropTypes.bool,
  intl: intlShape,
  push: PropTypes.func.isRequired
}

GoBackLink.defaultProps = {
  path: '/frontpage'
}

export default compose(
  injectIntl,
  connect(
    (state, { history }) => ({
      path: history && history.location && history.location.state && history.location.state.returnLink,
      entityId: getSelectedEntityId(state),
      isInReview: getShowReviewBar(state)
    }), {
      goBack,
      goBackWithUnlockEntity,
      mergeInUIState,
      push: push
    }
  )
)(GoBackLink)
