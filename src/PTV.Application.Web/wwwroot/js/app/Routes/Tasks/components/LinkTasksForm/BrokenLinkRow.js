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
import { compose } from 'redux'
import { connect } from 'react-redux'
import { branch, renderNothing } from 'recompose'
import { injectIntl } from 'util/react-intl'
import { mergeInUIState } from 'reducers/ui'
import {
  getBrokenLinkInitialValues,
  getIsBrokenLinkVisible,
  getBrokenLinkContentCount
} from 'Routes/Tasks/selectors'
import { handleOnSubmit } from 'util/redux-form/util'
import PropTypes from 'prop-types'
import styles from './styles.scss'
import { messages } from './messages'
import {
  UrlChecker,
  AdditionalInformation,
  ToggleSwitch
} from 'util/redux-form/fields'
import {
  // getFormSyncErrors,
  isSubmitting,
  isDirty,
  submit,
  reduxForm,
  reset
} from 'redux-form/immutable'
import { getUrlCheckingAbort } from 'util/redux-form/HOC/withEntityButtons/selectors'
import { Label } from 'sema-ui-components'
import { SaveButton, CancelButton } from 'appComponents/Buttons'
import Paragraph from 'appComponents/Paragraph'
import BrokenLinkDetails from './BrokenLinkDetails'
import Spacer from 'appComponents/Spacer'
import { taskTypesEnum, formTypesEnum } from 'enums'
import cx from 'classnames'
import { EntitySchemas } from 'schemas'
import { TasksSchemas } from 'schemas/tasks'
import { createLoadTaskEntities } from 'Routes/Tasks/actions'

const onSubmit = handleOnSubmit({
  url: 'url/updateBrokenLink',
  schema: EntitySchemas.BROKEN_LINK
})

const onSubmitSuccess = (result, dispatch, props) => {
  dispatch(({ dispatch, getState }) => {
    dispatch(createLoadTaskEntities(
      taskTypesEnum.EXCEPTIONLINKS,
      TasksSchemas.GET_ENTITIES(EntitySchemas.BROKEN_LINK)
    )(getState(), dispatch, { form: `taskEntitiesForm${taskTypesEnum.EXCEPTIONLINKS}` }))
    dispatch(createLoadTaskEntities(
      taskTypesEnum.UNSTABLELINKS,
      TasksSchemas.GET_ENTITIES(EntitySchemas.BROKEN_LINK)
    )(getState(), dispatch, { form: `taskEntitiesForm${taskTypesEnum.UNSTABLELINKS}` }))
  })
  if (!result) {
    dispatch(reset(props.form))
  }
}

const BrokenLinkRow = compose(
  connect((state, { formName, taskType, id }) => ({
    dirty: isDirty(formName)(state),
    submitting: isSubmitting(formName)(state),
    abort: getUrlCheckingAbort(state),
    count: getBrokenLinkContentCount(state, { id })
  }), { submit, mergeInUIState }),
)(({
  intl: { formatMessage },
  isVisible,
  isException,
  url,
  id,
  content,
  taskType,
  formName,
  dirty,
  submitting,
  handleSubmit,
  submit,
  mergeInUIState,
  count,
  abort
}) => {
  const handleOnSave = () => {
    abort && abort()
    submit(formName)
  }
  const handleOnCancel = () => {
    mergeInUIState({
      key: 'clearFormDialog',
      value: {
        isOpen: true,
        confirmAction: null,
        forms: [formName]
      }
    })
  }
  const urlLabel = isException ? formatMessage(messages.urlExceptionLabel) : formatMessage(messages.urlUnstableLabel)
  return isVisible && (
    <div className={styles.brokenLinkRowWrapper}>
      <div>
        {!isException && <Label labelPosition='top' labelText={formatMessage(messages.urlUnstableTitle)} />}
        <Paragraph>{urlLabel}</Paragraph>
        {isException
          ? <Paragraph className={styles.link}>{url}</Paragraph>
          : <UrlChecker
            name='url'
            label={null}
            tooltip={null}
            isLocalized={false}
            customValidation
          />
        }
      </div>
      <div className='form-group'>
        <ToggleSwitch
          name='isException'
          round
          label={formatMessage(messages.exceptionToggleTitle)}
          showTooltip
          tooltip={formatMessage(messages.exceptionToggleTooltip)}
        />
        {formatMessage(messages.exceptionToggleDescription)}
      </div>
      <div>
        <div className='row'>
          <div className='col-lg-15'>
            <AdditionalInformation
              name='exceptionComment'
              label={formatMessage(messages.exceptionCommentTitle)}
              tooltip={isException ? formatMessage(messages.exceptionCommentTooltip) : null}
              placeholder={isException
                ? formatMessage(messages.exceptionCommentPlaceholder)
                : formatMessage(messages.unstableCommentPlaceholder)
              }
              isLocalized={false}
              rows={4}
              maxLength={500}
            />
          </div>
        </div>
      </div>
      <div className={styles.buttonGroup}>
        <SaveButton
          onClick={handleOnSave}
          submitting={submitting}
          disabled={!dirty}
        />
        <CancelButton
          onClick={handleOnCancel}
          disabled={!dirty || submitting}
        />
      </div>
      <Spacer />
      <div>
        <Label
          className={styles.linkedContentLabel}
          labelText={formatMessage(messages.contentTableTitle,
            { count: content.length === count ? count : `${content.length}/${count}` })}
        />
        <BrokenLinkDetails
          id={id}
          content={content}
          taskType={taskType}
        />
      </div>
    </div>
  )
})

BrokenLinkRow.propTypes = {
  isVisible: PropTypes.bool,
  isException: PropTypes.bool,
  abort: PropTypes.func
}

export const UnstableLinkRow = compose(
  injectIntl,
  connect((state, { id }) => {
    const taskType = taskTypesEnum.UNSTABLELINKS
    return {
      isVisible: getIsBrokenLinkVisible(state, { taskType, id }),
      initialValues: getBrokenLinkInitialValues(state, { id, type: 'brokenLinks' }),
      taskType,
      formName: formTypesEnum.UNSTABLELINKFORM
    }
  }, {
    mergeInUIState
  }),
  branch(({ isVisible }) => !isVisible, renderNothing),
  reduxForm({
    form: formTypesEnum.UNSTABLELINKFORM,
    enableReinitialize: true,
    destroyOnUnmount: false,
    onSubmit,
    onSubmitSuccess
  })
)(BrokenLinkRow)

export const ExceptionLinkRow = compose(
  injectIntl,
  connect((state, { id }) => {
    const taskType = taskTypesEnum.EXCEPTIONLINKS
    return {
      isVisible: getIsBrokenLinkVisible(state, { taskType, id }),
      initialValues: getBrokenLinkInitialValues(state, { id, type: 'brokenLinks' }),
      taskType,
      formName: formTypesEnum.EXCEPTIONLINKFORM
    }
  }, {
    mergeInUIState
  }),
  branch(({ isVisible }) => !isVisible, renderNothing),
  reduxForm({
    form: formTypesEnum.EXCEPTIONLINKFORM,
    enableReinitialize: true,
    destroyOnUnmount: false,
    onSubmit,
    onSubmitSuccess
  }),
)(BrokenLinkRow)
