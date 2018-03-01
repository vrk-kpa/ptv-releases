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
import { submit, isSubmitting } from 'redux-form/immutable'
import { Button, Spinner } from 'sema-ui-components'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import {
  setConnectionsView
} from 'reducers/selections'
import { isConnectionsPreview } from 'selectors/selections'
import styles from './styles.scss'

const messages = defineMessages({
  editConnectionsSummary: {
    id: 'Routes.Connections.components.PreviewActions.EditSummaryButton.Title',
    defaultMessage: 'Muokkaa liitoksia'
  },
  saveConnectionsButton: {
    id: 'Routes.Connections.components.WorkbenchActions.SaveButton.Title',
    defaultMessage: 'Tallenna liitokset'
  }
})

class PreviewActions extends Component {
  handleOnPreview = this.props.preview
  render () {
    const {
      saveConnections,
      isPreview,
      isSubmiting,
      intl: { formatMessage }
    } = this.props
    return (
      <div className={styles.toolbar}>
        <div className={styles.buttonGroup}>
          <Button
            children={formatMessage(messages.editConnectionsSummary)}
            onClick={() => this.handleOnPreview(!isPreview)}
            medium
            secondary
            disabled={isSubmiting}
          />
          <Button
            children={isSubmiting && <Spinner /> || formatMessage(messages.saveConnectionsButton)}
            onClick={() => saveConnections('connectionsWorkbench')}
            medium
            secondary={isSubmiting}
          />
        </div>
      </div>
    )
  }
}
PreviewActions.propTypes = {
  preview: PropTypes.func.isRequired,
  isPreview: PropTypes.bool.isRequired,
  isSubmiting: PropTypes.bool.isRequired,
  saveConnections: PropTypes.func.isRequired,
  intl: intlShape
}

export default compose(
  injectIntl,
  connect(state => ({
    isSubmiting: isSubmitting('connectionsWorkbench')(state),
    isPreview: isConnectionsPreview(state)
  }), {
    saveConnections: submit,
    preview: (isPreview) => setConnectionsView(isPreview)
  })
)(PreviewActions)
