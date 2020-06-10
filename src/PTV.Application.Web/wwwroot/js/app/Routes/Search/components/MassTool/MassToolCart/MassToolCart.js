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
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { PTVIcon } from 'Components'
import { Button } from 'sema-ui-components'
import MassToolSelectionCounter from '../MassToolSelectionCounter'
import MassToolCartDialog from './MassToolCartDialog'
import MassToolCartEmptyDialog from './MassToolCartEmptyDialog'
import MassToolCartIcon from './MassToolCartIcon'
import MassToolCartLink from './MassToolCartLink'
import {
  getSelectedCount,
  getSelectedLanguageVersionsCount,
  getSelectedEntitiesForMassOperation
} from '../selectors'
import { mergeInUIState } from 'reducers/ui'
import { MASSTOOL_SELECTION_LIMIT } from 'enums'
import cx from 'classnames'
import styles from '../styles.scss'

const messages = defineMessages({
  cartTitle: {
    id: 'MassTool.Cart.Title',
    defaultMessage: 'Sisältökärry'
  },
  clearCartButton: {
    id: 'MassTool.Cart.Clear',
    defaultMessage: 'Tyhjennä kori'
  },
  limitReached: {
    id: 'MassTool.ConfirmButton.LimitReached.Text',
    defaultMessage: 'You have selected more than {limit} language versions.'
  },
  limitReached2: {
    id: 'MassTool.ConfirmButton.LimitReached2.Text',
    defaultMessage: 'Please, unselect exceeding records.'
  }
})

const MassToolCart = ({
  intl: { formatMessage },
  count,
  totalCount,
  mergeInUIState
}) => {
  const handleClear = () => {
    mergeInUIState({
      key: 'massToolCartEmptyDialog',
      value: {
        isOpen: true
      }
    })
  }
  const hasError = totalCount > MASSTOOL_SELECTION_LIMIT
  const massToolCartClass = cx(
    styles.massToolCart,
    {
      [styles.hasError]: hasError
    }
  )
  return (
    <div className={massToolCartClass}>
      <div className={styles.massToolCartBody}>
        <MassToolCartIcon />
        <MassToolCartLink />
        <MassToolSelectionCounter />
        <Button
          small
          secondary
          disabled={count === 0}
          onClick={handleClear}
          children={formatMessage(messages.clearCartButton)}
          className={styles.clearButton}
        />
      </div>
      <div className={styles.massToolCartFooter}>
        {hasError && (
          <div className={styles.error}>
            <PTVIcon name='icon-exclamation-circle' width={24} height={24} componentClass={styles.cartErrorIcon} />
            <span>
              {formatMessage(messages.limitReached, { limit: MASSTOOL_SELECTION_LIMIT })}
              <br />
              {formatMessage(messages.limitReached2)}
            </span>
          </div>
        )}
      </div>
      <MassToolCartDialog />
      <MassToolCartEmptyDialog />
    </div>
  )
}

MassToolCart.propTypes = {
  intl: intlShape,
  count: PropTypes.number,
  totalCount: PropTypes.number,
  mergeInUIState: PropTypes.func
}

export default compose(
  injectIntl,
  injectFormName,
  connect(
    (state, { formName }) => {
      const selected = getSelectedEntitiesForMassOperation(state, { formName })
      return {
        count: getSelectedCount(state, { selected, formName }),
        totalCount: getSelectedLanguageVersionsCount(state, { selected, formName })
      }
    }, {
      mergeInUIState
    }
  )
)(MassToolCart)
