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
import { reduxForm, initialize, isSubmitting, reset } from 'redux-form/immutable'
import { handleOnSubmit } from 'util/redux-form/util'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { injectIntl, defineMessages, intlShape } from 'util/react-intl'
import { loadInstructions } from 'Routes/CurrentIssues/actions'
import { getInstructionsIsFetching,
  getCurrentIssue
} from 'Routes/CurrentIssues/selectors'
import LastInstruction from 'Routes/CurrentIssues/components/LastInstruction'
import PreviousInstruction from 'Routes/CurrentIssues/components/PreviousInstruction'
import { SecurityUpdate } from 'appComponents/Security'
import { Spinner, Button } from 'sema-ui-components'
import { EntitySchemas } from 'schemas'
import withState from 'util/withState'
import { currentIsssueTransformer } from '../transformers'
import { List } from 'immutable'
import GoBackLink from 'appComponents/GoBackLink'

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

const CurrentIssuesForm = (
  { handleSubmit,
    isLoading,
    intl: { formatMessage },
    isEditable,
    updateUI,
    history,
    isSubmiting,
    resetForm,
    ...rest
  }) => {
  const handleEdit = () => {
    updateUI('isEditable', true)
  }

  const handleCancel = () => {
    updateUI('isEditable', false)
    resetForm('currentIssuesForm')
  }

  return <form onSubmit={handleSubmit}>
    <GoBackLink />
    <div className='step-container-actions clearfix'>
      <h3>{formatMessage(messages.currentIssuesInfo)}</h3>
    </div>
    <div className='box box-white'>
      <div className='form-wrap'>
        { isLoading
          ? <Spinner />
          : <div className='step-form'>
            <div className='row'>
              <LastInstruction
                isEditable={isEditable}
              />
            </div>
            <div className='row'>
              <PreviousInstruction />
            </div>
            <div className='row'>
              <div className='col-xs-6 button-group'>
                <SecurityUpdate domain='currentIssues'>
                  { isEditable
                    ? <div>
                      <Button
                        small
                        type='submit'
                        children={isSubmiting && <Spinner /> || formatMessage(messages.saveButton)}
                        disabled={isSubmiting}
                      />
                      <Button
                        small
                        type='button'
                        secondary
                        onClick={handleCancel}
                        children={formatMessage(messages.cancelButton)}
                        disabled={isSubmiting}
                      />
                    </div>
                    : <Button
                      small
                      type='button'
                      children={formatMessage(messages.editButton)}
                      onClick={handleEdit}
                      disabled={isSubmiting}
                    />
                  }
                </SecurityUpdate>
              </div>
            </div>
          </div>
        }
      </div>
    </div>
  </form>
}

const onSubmit = handleOnSubmit({
  url: 'common/SaveEnvironmentInstructions',
  transformers: [
    currentIsssueTransformer
  ],
  schema:  EntitySchemas.CURRENT_ISSUE_ARRAY
})

const onSubmitSuccess = (response, dispatch, props) => {
  props.updateUI('isEditable', false)
  // Initialize with updated state from save response //
  dispatch(({ getState, dispatch }) => {
    const state = getState()
    const result = List(response.map(x => x.id))
    const formValues = getCurrentIssue(state, { result })
    dispatch(initialize('currentIssuesForm', formValues))
  }
  )
}

CurrentIssuesForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  isLoading: PropTypes.bool.isRequired,
  loadInstructions: PropTypes.func.isRequired,
  intl: intlShape.isRequired,
  isEditable: PropTypes.bool.isRequired,
  updateUI: PropTypes.func.isRequired,
  history: PropTypes.object,
  isSubmiting: PropTypes.bool.isRequired,
  resetForm: PropTypes.func
}

export default compose(
  connect((state, ownProps) => ({
    isLoading: getInstructionsIsFetching(state, ownProps),
    initialValues: getCurrentIssue(state, ownProps),
    isSubmiting: isSubmitting('currentIssuesForm')(state)
  }),
  { loadInstructions, resetForm: reset }),
  withState({
    key: 'currentIssue',
    initialState: {
      isEditable: false
    }
  }),
  reduxForm({
    form: 'currentIssuesForm',
    enableReinitialize: true,
    onSubmit,
    onSubmitSuccess
  }),
  injectIntl
)(CurrentIssuesForm)
