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
import { connect } from 'react-redux'
import { compose } from 'redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import PropTypes from 'prop-types'
import styles from './styles.scss'
import { fieldPropTypes, getFormValues, change, getFormInitialValues } from 'redux-form/immutable'
import moment from 'moment'
import ValueCell from 'appComponents/Cells/ValueCell'
import ComposedCell from 'appComponents/Cells/ComposedCell'
import Cell from 'appComponents/Cells/Cell'
import withState from 'util/withState'
import { Button, Spinner } from 'sema-ui-components'
import {
  DatePicker
} from 'util/redux-form/fields'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'

const messages = defineMessages({
  updateButton: {
    id: 'Components.Buttons.UpdateButton',
    defaultMessage: 'Muokkaa'
  },
  linkDelete: {
    id: 'Components.DateTimeInputCell.Link.Delete',
    defaultMessage: 'Poista ajastus'
  },
  linkAdd: {
    id: 'Components.DateTimeInputCell.Link.Add',
    defaultMessage: 'Aseta ajastu'
  },
  noDate: {
    id: 'Components.DateTimeInputCell.NoDate.Title',
    defaultMessage: 'Ei julkaistu'
  },
  saveButton: {
    id: 'Components.Buttons.SaveButton',
    defaultMessage: 'Tallenna'
  },
  closeButtonTitle: {
    id: 'Components.Buttons.Cancel',
    defaultMessage: 'Peruuta'
  }
})

const renderDisplayComponent = (text) => (
  <Cell dWidth='dw80' ldWidth='ldw100'>
    <ValueCell value={text} />
  </Cell>
)
const renderEditComponent = (text, dateFrom, edit, cancel, formatMessage, disableLink) => (
  <ComposedCell
    inTable
    componentClass={styles.date}
    main={<Cell dWidth='dw180' ldWidth='ldw260'><ValueCell value={text} /></Cell>}
    sub={
      dateFrom
        ? <Cell dWidth='dw180' ldWidth='ldw260'>
          <div className={styles.buttonGroup}>
            <Button link onClick={edit} disabled={disableLink}>{formatMessage(messages.updateButton)}</Button>
            <Button link onClick={cancel} disabled={disableLink}>{formatMessage(messages.linkDelete)}</Button>
          </div>
        </Cell>
        : <Cell dWidth='dw180' ldWidth='ldw260'>
          <Button link onClick={edit} disabled={disableLink}>{formatMessage(messages.linkAdd)}</Button>
        </Cell>
    }
  />
)

const DateTimeInputCell = ({
  dateFrom,
  initialDateFrom,
  input,
  rowIndex,
  readOnly,
  updateUI,
  formName,
  active,
  setFormValue,
  name,
  intl: { formatMessage },
  onSubmit,
  filterDate,
  isSaving,
  ...rest
}) => {
  const handleOnEdit = () => {
    updateUI('active', name)
  }
  const handleOnSave = () => {
    updateUI('active', false)
    if (typeof onSubmit === 'function') {
      onSubmit()
    }
  }
  const handleOnClear = () => {
    updateUI('active', false)
    setFormValue(formName, name, null)
    if (typeof onSubmit === 'function') {
      onSubmit()
    }
  }

  const handleOnCancel = () => {
    updateUI('active', false)
    setFormValue(formName, name, initialDateFrom)
  }
  let displayText = formatMessage(messages.noDate)
  if (dateFrom) {
    displayText = moment(dateFrom).format('DD.MM.YYYY')
  }
  if (readOnly) {
    return renderDisplayComponent(displayText)
  }
  if ((active && active === name && isSaving)) {
    return <Spinner />
  }
  if (!active || (active && active !== name)) {
    return renderEditComponent(
      displayText,
      dateFrom,
      handleOnEdit,
      handleOnClear,
      formatMessage,
      isSaving || (active && active !== name) || (active && active === name && isSaving)
    )
  }

  return (
    <div className={styles.publishDialogDatepicker}>
      <DatePicker
        name={name}
        isReadOnly={false}
        filterDate={filterDate}
        futureDays
        {...rest}
      />
      <div className={styles.buttonGroup}>
        <Button onClick={handleOnSave} secondary small disabled={!dateFrom}>{formatMessage(messages.saveButton)}</Button>
        <Button link onClick={handleOnCancel}>{formatMessage(messages.closeButtonTitle)}</Button>
      </div>
    </div>
  )
}

DateTimeInputCell.propTypes = {
  dateFrom: PropTypes.number,
  initialDateFrom: PropTypes.any,
  rowIndex: PropTypes.number,
  input: fieldPropTypes.input,
  updateUI: PropTypes.func,
  readOnly: PropTypes.any,
  formName: PropTypes.string.isRequired,
  active: PropTypes.any.isRequired,
  setFormValue: PropTypes.func.isRequired,
  name: PropTypes.string.isRequired,
  intl: intlShape.isRequired,
  onSubmit: PropTypes.any,
  filterDate: PropTypes.any,
  isSaving: PropTypes.bool
}

export default compose(
  injectFormName,
  withState({
    key: ({ formName }) => formName + 'InputCell',
    initialState: {
      active: false
    },
    redux: true
  }),
  connect((state, { valuePath, formName, readOnly, filterDate, filterDatePath, isSavingSelector, showInitialDate }) => {
    const values = getFormValues(formName)(state)
    const initial = getFormInitialValues(formName)(state)
    const newFilterDate = filterDate || (filterDatePath && values && values.getIn(filterDatePath)) || null
    const initialDateFrom = initial && initial.getIn(valuePath) || null
    return {
      initialDateFrom,
      dateFrom: showInitialDate ? initialDateFrom : values && values.getIn(valuePath) || null,
      readOnly,
      formName,
      filterDate: newFilterDate,
      isSaving: isSavingSelector && isSavingSelector(state) || false
    }
  }, {
    setFormValue: change
  }),
  injectIntl
)(DateTimeInputCell)
