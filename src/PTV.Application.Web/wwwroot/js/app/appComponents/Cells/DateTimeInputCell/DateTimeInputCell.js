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
import Cell from 'appComponents/Cells/Cell'
import withState from 'util/withState'
import {
  DatePicker
} from 'util/redux-form/fields'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { getIsInTranslation } from 'selectors/entities/entities'

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
  <Cell dWidth='dw80' ldWidth='ldw100' >
    <ValueCell value={text} />
  </Cell>
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
  let displayText = formatMessage(messages.noDate)
  if (dateFrom) {
    displayText = moment(dateFrom).format('DD.MM.YYYY')
  }
  if (readOnly) {
    return renderDisplayComponent(displayText)
  }
  return (
   
    <div className={styles.publishDialogDatepicker}>
      <DatePicker
        name={name}
        filterDate={filterDate}
        {...rest}
        isReadOnly={false}
        inputClass={styles.dateTooltip}
      />
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
  connect((state, { valuePath, formName, readOnly, filterDate, filterDatePath, isSavingSelector, showInitialDate, languageCode }) => {
    const isInTranslation = getIsInTranslation(state, { languageCode })
    const values = getFormValues(formName)(state)
    const initial = getFormInitialValues(formName)(state)
    const valueFilterDatePath = (filterDatePath && values && values.getIn(filterDatePath)) || null
    const isDates = filterDate && valueFilterDatePath
    const newFilterDate = isDates
      ? filterDate < valueFilterDatePath
        ? filterDate
        : valueFilterDatePath
      : filterDate || valueFilterDatePath || null
    const initialDateFrom = initial && initial.getIn(valuePath) || null
    return {
      initialDateFrom,
      dateFrom: showInitialDate ? initialDateFrom : values && values.getIn(valuePath) || null,
      readOnly: readOnly && !isInTranslation,
      formName,
      filterDate: newFilterDate,
      isSaving: isSavingSelector && isSavingSelector(state) || false
    }
  }, {
    setFormValue: change
  }),
  injectIntl
)(DateTimeInputCell)
