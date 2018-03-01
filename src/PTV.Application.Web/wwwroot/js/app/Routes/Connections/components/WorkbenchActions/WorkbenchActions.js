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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { reset, isDirty, change, submit, isSubmitting, getFormInitialValues } from 'redux-form/immutable'
import { List } from 'immutable'
import { Select, Button, Spinner } from 'sema-ui-components'
import { API_CALL_CLEAN } from 'Containers/Common/Actions'
import { mergeInUIState } from 'reducers/ui'
import UnsavedConnectionsDialog from 'appComponents/UnsavedConnectionsDialog'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import {
  setConnectionsEntity,
  setConnectionsMainEntity,
  clearConnectionsActiveEntity,
  setConnectionsView,
  setShouldAddChildToAllEntities
} from 'reducers/selections'
import { getConnectionsMainEntity, isConnectionsPreview } from 'selectors/selections'
import styles from './styles.scss'

const messages = defineMessages({
  actionSelectPlaceholder: {
    id: 'Routes.Connections.components.WorkbenchActions.ActionSelectTitle.Placeholder',
    defaultMessage: 'Toiminnot'
  },
  actionUndo: {
    id: 'Routes.Connections.components.WorkbenchActions.ActionUndo.Text',
    defaultMessage: 'Peru viimeisin'
  },
  actionClear: {
    id: 'Routes.Connections.components.WorkbenchActions.ActionClear.Text',
    defaultMessage: 'Tyhjennä työpöytä'
  },
  actionReset: {
    id: 'Routes.Connections.components.WorkbenchActions.ActionReset.Text',
    defaultMessage: 'Aloita alusta'
  },
  viewConnectionsSummary: {
    id: 'Routes.Connections.components.WorkbenchActions.SummaryButton.Title',
    defaultMessage: 'Katso yhteenveto'
  },
  saveConnectionsButton: {
    id: 'Routes.Connections.components.WorkbenchActions.SaveButton.Title',
    defaultMessage: 'Tallenna liitokset'
  }
})

const clearChannelSearchResults = () => ({
  type: API_CALL_CLEAN,
  keys: ['connections', 'channelSearch']
})
const clearServiceSearchResults = () => ({
  type: API_CALL_CLEAN,
  keys: ['connections', 'serviceSearch']
})

class WorkbenchActions extends Component {
  handleOnUndo = this.props.undo
  handleOnClearWorkbench = this.props.clear
  handleOnReset = this.props.reset
  handleOnChange = ({ value }) => value()
  handleOnPreview = this.props.preview

  render () {
    const {
      isDirty,
      mainEntity,
      saveConnections,
      isSubmiting,
      isPreview,
      resetMainType,
      mergeInUIState,
      intl: { formatMessage }
    } = this.props
    const options = [
      {
        value: () => this.props.undo(resetMainType),
        label: formatMessage(messages.actionUndo),
        disabled: !mainEntity || !isDirty
      },
      {
        value: this.props.clear,
        label: formatMessage(messages.actionClear),
        disabled: !mainEntity
      },
      {
        value: (!mainEntity || !isDirty) ? this.props.reset : () => mergeInUIState({
          key: 'UnsavedConnectionsDialog',
          value: {
            isOpen: true
          }
        }),
        label: formatMessage(messages.actionReset)
      }
    ]
    return (
      <div className={styles.toolbar}>
        <UnsavedConnectionsDialog successAction={this.props.reset} />
        <div className={styles.functions}>
          <Select
            options={options}
            onChange={this.handleOnChange}
            placeholder={formatMessage(messages.actionSelectPlaceholder)}
            disabled={isSubmiting}
          />
        </div>
        <div className={styles.buttonGroup}>
          {mainEntity &&
            <Button
              children={formatMessage(messages.viewConnectionsSummary)}
              onClick={() => this.handleOnPreview(!isPreview)}
              medium
              secondary
              disabled={isSubmiting}
            />
          }
          <Button
            children={isSubmiting && <Spinner /> || formatMessage(messages.saveConnectionsButton)}
            onClick={saveConnections}
            medium
            disabled={!mainEntity || !isDirty}
            secondary={isSubmiting}
          />
        </div>
      </div>
    )
  }
}
WorkbenchActions.propTypes = {
  undo: PropTypes.func.isRequired,
  clear: PropTypes.func.isRequired,
  reset: PropTypes.func.isRequired,
  preview: PropTypes.func.isRequired,
  isDirty: PropTypes.bool.isRequired,
  isSubmiting: PropTypes.bool.isRequired,
  isPreview: PropTypes.bool.isRequired,
  mainEntity: PropTypes.oneOf(['channels', 'services']),
  saveConnections: PropTypes.func.isRequired,
  intl: intlShape,
  mergeInUIState: PropTypes.func,
  resetMainType: PropTypes.bool
}

export default compose(
  injectIntl,
  connect(state => {
    const initalWorkBench = getFormInitialValues('connectionsWorkbench')(state)
    return {
      isDirty: isDirty('connectionsWorkbench')(state),
      mainEntity: getConnectionsMainEntity(state),
      isSubmiting: isSubmitting('connectionsWorkbench')(state),
      isPreview: isConnectionsPreview(state),
      resetMainType: initalWorkBench.get('connections').size === 0
    }
  }, {
    saveConnections: () => submit('connectionsWorkbench'),
    undo: (resetMainType) => ({ dispatch }) => {
      dispatch(reset('connectionsWorkbench'))
      resetMainType && dispatch(setConnectionsMainEntity(null))
    },
    clear: () => ({ dispatch }) => {
      [
        change('connectionsWorkbench', 'connections', List()),
        change('connectionsWorkbench', 'mainEntityType', null),
        setConnectionsMainEntity(null)
      ].forEach(dispatch)
    },
    preview: (isPreview) => setConnectionsView(isPreview),
    reset: () => ({ dispatch }) => {
      [
        change('connectionsWorkbench', 'connections', List()),
        change('connectionsWorkbench', 'mainEntityType', null),
        setConnectionsMainEntity(null),
        setConnectionsEntity(null),
        clearConnectionsActiveEntity(),
        setShouldAddChildToAllEntities(true),
        clearChannelSearchResults(),
        clearServiceSearchResults()
      ].forEach(dispatch)
    },
    mergeInUIState
  })
)(WorkbenchActions)
