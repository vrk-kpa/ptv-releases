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
import { reset, isDirty, change, submit, isSubmitting, getFormInitialValues, getFormSyncErrors } from 'redux-form/immutable'
import { List } from 'immutable'
import { Select, Button, Spinner } from 'sema-ui-components'
import { mergeInUIState } from 'reducers/ui'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import UnsavedConnectionsDialog from 'appComponents/UnsavedConnectionsDialog'
import Tooltip from 'appComponents/Tooltip'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import {
  setConnectionsMainEntity,
  setConnectionsView
} from 'reducers/selections'
import { resetConnectionsWorkbench } from './actions'
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
  },
  functionsTooltip: {
    id: 'Routes.Connections.components.WorkbenchActions.Functions.Tooltip',
    defaultMessage: 'Peru viimeisin-valinta peruuttaa viimeisimmän tekemäsi muutoksen. Tyhjennä työpöytä -valinta tyhjentää työpöydän valitsemistasi palveluista ja kanavista, mutta tekemäsi haut jäävät muistiin. Tee uusi haku-valinta tyhjentää työpöydän ja poistaa kaikki hakutulokset.' // eslint-disable-line
  }
})

class WorkbenchActions extends Component {
  handleOnUndo = this.props.undo
  handleOnClearWorkbench = this.props.clear
  handleOnReset = this.props.reset
  handleOnChange = ({ value }) => value()
  handleOnPreview = this.props.preview

  render () {
    const {
      mainEntity,
      saveConnections,
      isSubmiting,
      isPreview,
      resetMainType,
      mergeInUIState,
      hasformError,
      isDirty,
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
          <Tooltip tooltip={formatMessage(messages.functionsTooltip)} position='top center' />
        </div>
        <div className={styles.buttonGroup}>
          {mainEntity &&
            <Button
              children={formatMessage(messages.viewConnectionsSummary)}
              onClick={() => this.handleOnPreview(!isPreview)}
              medium
              secondary
              disabled={isSubmiting || hasformError}
            />
          }
          <Button
            children={isSubmiting && <Spinner /> || formatMessage(messages.saveConnectionsButton)}
            onClick={saveConnections}
            medium
            disabled={!mainEntity || !isDirty || hasformError}
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
  isSubmiting: PropTypes.bool.isRequired,
  isPreview: PropTypes.bool.isRequired,
  isDirty: PropTypes.bool.isRequired,
  mainEntity: PropTypes.oneOf(['channels', 'services']),
  saveConnections: PropTypes.func.isRequired,
  intl: intlShape,
  mergeInUIState: PropTypes.func,
  resetMainType: PropTypes.bool,
  hasformError: PropTypes.bool
}

export default compose(
  injectIntl,
  injectFormName,
  connect((state, ownProps) => {
    const initalWorkBench = getFormInitialValues('connectionsWorkbench')(state)
    const formSyncErrors = getFormSyncErrors(ownProps.formName)(state) || null
    return {
      mainEntity: getConnectionsMainEntity(state),
      isSubmiting: isSubmitting('connectionsWorkbench')(state),
      isPreview: isConnectionsPreview(state),
      resetMainType: initalWorkBench.get('connections').size === 0,
      isDirty: isDirty('connectionsWorkbench')(state),
      hasformError: formSyncErrors && formSyncErrors.hasOwnProperty('connections')
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
    reset: resetConnectionsWorkbench,
    mergeInUIState
  })
)(WorkbenchActions)
