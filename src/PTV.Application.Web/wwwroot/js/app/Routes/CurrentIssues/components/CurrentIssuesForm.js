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

import React, { PureComponent } from 'react'
import PropTypes from 'prop-types'
import { Field, reduxForm } from 'redux-form/immutable'
import { connect } from 'react-redux'
import { Link } from 'react-router-dom'
import { compose } from 'redux'
import { toggleEdit } from 'reducers/currentIssues'
import { injectIntl, defineMessages, intlShape } from 'util/react-intl'
import { loadInstructions, saveInstructions } from 'Routes/CurrentIssues/actions'
import { getInstructionsIsFetching,
  getIsEditEnabled,
  getLastInstruction,
  getPreviousInstruction
} from 'Routes/CurrentIssues/selectors'
import { PTVIcon } from 'Components'
import renderLastInstruction from 'Routes/CurrentIssues/components/renderLastInstruction'
import renderPreviousInstruction from 'Routes/CurrentIssues/components/renderPreviousInstruction'
import { SecurityUpdate } from 'appComponents/Security'
import { Spinner, Label, Button } from 'sema-ui-components'

const messages = defineMessages({
  currentIssuesInfo: {
    id: 'CurrentIssues.Header.CurrentIssues',
    defaultMessage: 'Tiedotteet'
  },
  linkGoBack: {
    id: 'CurrentIssues.Link.Back',
    defaultMessage: 'Takaisin'
  },
  saveButton: {
    id: 'CurrentIssues.Buttons.SaveButton',
    defaultMessage: 'Tallenna'
  },
  editButton: {
    id: 'CurrentIssues.Buttons.EditButton',
    defaultMessage: 'Muokkaa'
  },
  cancelButton: {
    id: 'CurrentIssues.Buttons.CancelButton',
    defaultMessage: 'Keskeytä'
  }
})

class CurrentIssuesForm extends PureComponent {
  handleToggle = (e) => {
    e.preventDefault()
    this.props.toggleEdit()
  }

  handleCancel = (e) => {
    e.preventDefault()
    this.props.toggleEdit()
    this.props.loadInstructions()
  }

  goBack = () => {
    this.props.history.goBack()
  }

  render () {
    const {
      isLoading,
      handleSubmit,
      isEditEnabled,
      intl: { formatMessage }
    } = this.props
    return (
      <form onSubmit={handleSubmit}>
        <Label>
          <Link to='/frontpage'
            onClick={this.goBack}>
            <PTVIcon name='icon-angle-left' className='brand-fill' componentClass='top-align' />
            {formatMessage(messages.linkGoBack)}
          </Link>
        </Label>
        <div className='step-container-actions clearfix'>
          <h3>{formatMessage(messages.currentIssuesInfo)}</h3>
        </div>
        <div className='box box-white'>
          <div className='form-wrap'>
            { isLoading ? <Spinner /> : <div className='step-form'><div className='row'>
              <Field
                name='lastInstruction'
                component={renderLastInstruction}
              />
              <Field
                name='previousInstruction'
                component={renderPreviousInstruction}
              />
            </div>
              <div className='row'>
              <div className='col-xs-6 button-group'>
                  <SecurityUpdate domain='currentIssues'>
                  {isEditEnabled ? <div>
                      <Button
                      small
                      type='submit'
                      children={formatMessage(messages.saveButton)}
                    />
                      <Button
                      small
                      type='button'
                      secondary
                      onClick={this.handleCancel}
                      children={formatMessage(messages.cancelButton)}
                    />

                    </div>
                    : <Button
                      small
                      type='button'
                      children={formatMessage(messages.editButton)}
                      onClick={this.handleToggle}
                    />
                  }
                </SecurityUpdate>
                </div>
            </div>
            </div> }
          </div>
        </div>
      </form>
    )
  }
}

const onSubmit = (formValues, dispatch, ownProps) => {
  return new Promise((resolve, reject) => {
    // TODO: redux form move
    ownProps.saveInstructions({
      environmentInstructions: formValues.get('lastInstruction').get('environmentInstructions')
    })
    ownProps.toggleEdit()
    resolve()
  })
}

CurrentIssuesForm.propTypes = {
  isLoading: PropTypes.bool.isRequired,
  isEditEnabled: PropTypes.bool.isRequired,
  handleSubmit: PropTypes.func.isRequired,
  toggleEdit: PropTypes.func.isRequired,
  loadInstructions: PropTypes.func.isRequired,
  intl: intlShape.isRequired
}

export default compose(
  connect((state, ownProps) => ({
    isLoading: getInstructionsIsFetching(state, ownProps),
    isEditEnabled: getIsEditEnabled(state, ownProps),
    initialValues: { lastInstruction: getLastInstruction(state, ownProps),
      previousInstruction: getPreviousInstruction(state, ownProps) }
  }),
  { loadInstructions, saveInstructions, toggleEdit }),
  reduxForm({
    form: 'currentIssuesForm',
    enableReinitialize: true,
    onSubmit
  }),
  injectIntl
)(CurrentIssuesForm)
