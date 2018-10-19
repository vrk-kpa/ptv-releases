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
import { Field, getFormValues } from 'redux-form/immutable'
import { RenderRadioButtonTable } from 'util/redux-form/renders'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { updateUI } from 'util/redux-ui/action-reducer'
import { injectIntl } from 'util/react-intl'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withState from 'util/withState'
import { Map } from 'immutable'

const sortDirectionTypes = {
  ASC: 'asc',
  DESC: 'desc'
}

const getColumnsDefinition = ({
  columnsDefinition,
  formName,
  sorting,
  updateUI,
  sortOnClick }) => {
  const onSort = (column) => {
    const sortDirection = sorting.getIn([formName, 'column']) === column.property
      ? sorting.getIn([formName, 'sortDirection']) || sortDirectionTypes.DESC : sortDirectionTypes.DESC
    const newSorting = sorting.mergeIn([formName],
      { column: column.property,
        sortDirection: sortDirection === sortDirectionTypes.DESC && sortDirectionTypes.ASC || sortDirectionTypes.DESC })
    updateUI('sorting', newSorting)
    sortOnClick(getFormValues(formName))
  }
  return columnsDefinition(onSort)
}

const DataTable = ({
  columnsDefinition,
  formName,
  sorting,
  updateUI,
  setInUIState,
  mergeInUIState,
  uiState,
  stateExist,
  sortOnClick,
  ...rest
}) => (console.log(rest) ||
  <Field
    name={rest.name}
    component={RenderRadioButtonTable}
    columns={getColumnsDefinition({
      columnsDefinition,
      formName,
      sorting,
      updateUI,
      sortOnClick
    })}
    {...rest}
  />
)
DataTable.propTypes = {
  name: PropTypes.string.isRequired
}

export default compose(
  injectIntl,
  injectFormName,
  connect(null, { updateUI }),
  asDisableable,
  withState({
    redux: true,
    key: 'uiData',
    keepImmutable: true,
    initialState: {
      sorting: Map()
    }
  })
)(DataTable)
