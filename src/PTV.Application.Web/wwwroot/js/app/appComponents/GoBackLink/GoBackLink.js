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
import { injectIntl, defineMessages, intlShape } from 'react-intl'
import { Link, browserHistory } from 'react-router'
import { injectFormName } from 'util/redux-form/HOC'
import { getIsReadOnly } from 'selectors/formStates'
import { setIsAddingNewLanguage, setCompareMode } from 'reducers/formStates'
import { setComparisionLanguage, clearContentLanguage, setSelectedEntity } from 'reducers/selections'
import { Button } from 'sema-ui-components'
import { getSelectedEntityId } from 'selectors/entities/entities'
import { unLockEntity } from 'actions'
import withState from 'util/withState'

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
  setIsAddingNewLanguage,
  setCompareMode,
  setComparisionLanguage,
  clearContentLanguage,
  setSelectedEntity,
  isOpen,
  formName,
  updateUI,
  entityId,
  unLockEntity
}) => {
  const handleOnLinkClick = (value) => updateUI('isOpen', true)
  const handleOnCancelButtonClick = (value) => updateUI('isOpen', false)
  const goBack = (path) => {
    setIsAddingNewLanguage({
      form: formName,
      value: false
    })
    setCompareMode({
      form: formName,
      value: false
    })
    setComparisionLanguage({
      id: null,
      code: null
    })
    clearContentLanguage()
    setSelectedEntity({ id: null })
    browserHistory.push(path)
  }

  const goBackWithUnlockEntity = (path) => {
    unLockEntity(entityId, formName)
    setIsAddingNewLanguage({
      form: formName,
      value: false
    })
    setCompareMode({
      form: formName,
      value: false
    })
    setComparisionLanguage({
      id: null,
      code: null
    })
    clearContentLanguage()
    setSelectedEntity({ id: null })
    browserHistory.push(path)
  }

  return isReadOnly
    ? <Link
      className={styles.goBackLink}
      onClick={() => goBack(path)}>
      {formatMessage(messages.goBackLinkLabel)}
    </Link>
    : !isOpen
      ? <Link
        className={styles.goBackLink}
        onClick={handleOnLinkClick}>
        {formatMessage(messages.goBackLinkLabel)}
      </Link>
      : <div className={styles.messageBox}>
        <h1 className={styles.messageBoxTitle}>{formatMessage(messages.text)}</h1>
        <div className={styles.messageBoxActions}>
          <Button small onClick={() => goBackWithUnlockEntity(path)}>{formatMessage(messages.buttonOk)}</Button>
          <Button small link onClick={handleOnCancelButtonClick}>{formatMessage(messages.buttonCancel)}</Button></div>
      </div>
}

GoBackLink.propTypes = {
  path: PropTypes.string,
  formName: PropTypes.string.isRequired,
  setIsAddingNewLanguage: PropTypes.func.isRequired,
  setCompareMode: PropTypes.func.isRequired,
  setComparisionLanguage: PropTypes.func.isRequired,
  clearContentLanguage: PropTypes.func.isRequired,
  setSelectedEntity: PropTypes.func.isRequired,
  isReadOnly: PropTypes.bool,
  isOpen: PropTypes.bool,
  intl: intlShape,
  updateUI: PropTypes.func.isRequired,
  entityId: PropTypes.string.isRequired,
  unLockEntity: PropTypes.func.isRequired
}

GoBackLink.defaultProps = {
  path: '/frontpage/search'
}

export default compose(
  injectIntl,
  injectFormName,
  connect(
    (state, { formName }) => ({
      isReadOnly: getIsReadOnly(formName)(state),
      formName,
      entityId: getSelectedEntityId(state)
    }), {
      setIsAddingNewLanguage,
      unLockEntity,
      setCompareMode,
      setComparisionLanguage,
      clearContentLanguage,
      setSelectedEntity
    }
  ),
  withState({
    initialState: {
      isOpen: false
    }
  })
)(GoBackLink)
