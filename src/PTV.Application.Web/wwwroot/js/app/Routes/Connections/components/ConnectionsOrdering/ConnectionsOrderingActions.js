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
import React, { Fragment } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { Button } from 'sema-ui-components'
import { CancelButton } from 'appComponents/Buttons'
import Tooltip from 'appComponents/Tooltip'
import {
  removeSorting,
  toggleConnectionOrdering,
  restoreInitialConnectionOrder
} from 'Routes/Connections/actions'
import { getHasOrderChanged } from 'Routes/Connections/selectors'
import { mergeInUIState } from 'reducers/ui'
import withState from 'util/withState'
import { List } from 'immutable'
import cx from 'classnames'
import styles from './styles.scss'

const messages = defineMessages({
  saveConnectionOrder: {
    id: 'ConnectionsOrderingActions.Save.Title',
    defaultMessage: 'Tallenna järjestys'
  },
  organizeButton: {
    id: 'ConnectionsOrderingActions.Organize.Title',
    defaultMessage: 'Järjestä'
  },
  organizeTooltip: {
    id: 'ConnectionsOrderingActions.Organize.Tooltip',
    defaultMessage: 'Organize action tooltip'
  }
})

const ConnectionsOrderingActions = ({
  intl: { formatMessage },
  activeIndex,
  connectionIndex,
  mergeInUIState,
  removeSorting,
  toggleConnectionOrdering,
  hasOrderChanged,
  restoreInitialConnectionOrder
}) => {
  const handleToggleOrdering = connectionIndex => {
    toggleConnectionOrdering(connectionIndex)
  }
  const handleSaveOrdering = () => {
    toggleConnectionOrdering(null)
  }
  const handleCancelOrdering = connectionIndex => {
    restoreInitialConnectionOrder(connectionIndex)
    toggleConnectionOrdering(null)
  }
  const isActive = activeIndex === connectionIndex
  const connectionsOrderingActionsClass = cx(
    styles.connectionsOrderingActions,
    {
      [styles.active]: isActive
    }
  )
  return (
    <div className={connectionsOrderingActionsClass}>
      {activeIndex === connectionIndex
        ? <Fragment>
          <Button
            small
            children={formatMessage(messages.saveConnectionOrder)}
            onClick={handleSaveOrdering}
            disabled={!hasOrderChanged}
          />
          <CancelButton
            onClick={() => handleCancelOrdering(connectionIndex)}
            className={styles.cancelButton}
          />
        </Fragment>
        : <Fragment>
          <Button
            link
            children={formatMessage(messages.organizeButton)}
            onClick={() => handleToggleOrdering(connectionIndex)}
            className={styles.organizeLink}
          />
          <Tooltip tooltip={formatMessage(messages.organizeTooltip)} indent='i0' />
        </Fragment>
      }
    </div>
  )
}
ConnectionsOrderingActions.propTypes = {
  intl: intlShape,
  activeIndex: PropTypes.bool,
  connectionIndex: PropTypes.number,
  mergeInUIState: PropTypes.func,
  removeSorting: PropTypes.func,
  toggleConnectionOrdering: PropTypes.func,
  hasOrderChanged: PropTypes.bool,
  restoreInitialConnectionOrder: PropTypes.func
}

export default compose(
  injectIntl,
  withState({
    redux: true,
    key: 'activeConnectionUiData',
    initialState: {
      activeIndex: null,
      order: List()
    }
  }),
  connect((state, { connectionIndex }) => ({
    hasOrderChanged: getHasOrderChanged(state, { connectionIndex })
  }), {
    mergeInUIState,
    removeSorting,
    toggleConnectionOrdering,
    restoreInitialConnectionOrder
  })
)(ConnectionsOrderingActions)
