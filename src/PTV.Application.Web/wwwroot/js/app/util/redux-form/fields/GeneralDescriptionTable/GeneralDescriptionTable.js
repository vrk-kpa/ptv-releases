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
import { Field, getFormValues } from 'redux-form/immutable'
import { RenderRadioButtonTable } from 'util/redux-form/renders'
import { asDisableable } from 'util/redux-form/HOC'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { updateUI } from 'util/redux-ui/action-reducer'
import { injectIntl } from 'react-intl'
import { RadioButtonCell, NameCell, ShortDescriptionCell, LanguageBarCell, CellHeaders } from 'appComponents'
import { ServiceTypeCell } from 'util/redux-form/components'
import ColumnHeaderName from 'Routes/FrontPage/routes/Search/components/ColumnHeaderName'
import withState from 'util/withState'
import { Map } from 'immutable'

const sortDirectionTypes = {
  ASC: 'asc',
  DESC: 'desc'
}

const getColumnsDefinition = ({
  radioOnChange,
  previewOnClick,
  sorting,
  updateUI,
  sortOnClick,
  ...rest }) => {
  const onSort = (column) => {
    const sortDirection = sorting.getIn(['generalDescriptionSearch', 'column']) === column.property ?
      sorting.getIn(['generalDescriptionSearch', 'sortDirection']) || sortDirectionTypes.DESC : sortDirectionTypes.DESC
    const newSorting = sorting.mergeIn(['generalDescriptionSearch'],
      { column: column.property,
        sortDirection: sortDirection === sortDirectionTypes.DESC && sortDirectionTypes.ASC || sortDirectionTypes.DESC })
    updateUI('sorting', newSorting)
    sortOnClick(getFormValues('generalDescriptionSearchForm'))
  }
  return [
    {
      property: 'id',
      header: {
        label: rest.intl.formatMessage(CellHeaders.select)
      },
      cell: {
        formatters: [
          (id, { rowData }) => <RadioButtonCell {...{ ...{ ...rest, onChange: radioOnChange }, ...rowData }} />
        ]
      }
    },
    {
      property: 'languagesAvailabilities',
      header: {
        label: rest.intl.formatMessage(CellHeaders.languages)
      },
      cell: {
        formatters: [
          (languagesAvailabilities, { rowData }) => <LanguageBarCell showMissing {...rowData} />
        ]
      }
    },
    {
      property: 'name',
      header: {
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={rest.intl.formatMessage(CellHeaders.name)}
                column={column}
                contentType={'generalDescriptionSearch'}
                onSort={onSort}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          (name, { rowData }) => <NameCell viewIcon viewOnClick={previewOnClick}{...rowData} />
        ]
      }
    },
    {
      property: 'serviceTypeId',
      header: {
        label: rest.intl.formatMessage(CellHeaders.generalDescriptionHeaderTitle)
      },
      cell: {
        formatters: [
          (serviceTypeId, { rowData }) => <ServiceTypeCell {...rowData} />
        ]
      }
    },
    {
      property: 'shortDescription',
      header: {
        label: rest.intl.formatMessage(CellHeaders.shortDescription)
      },
      cell: {
        formatters: [
          (shortDescription, { rowData }) => <ShortDescriptionCell {...rowData} />
        ]
      }
    }
  ]
}

const GeneralDescriptionTable = (props) => (
  <Field
    name='generalDescriptionId'
    component={RenderRadioButtonTable}
    columns={getColumnsDefinition(props)}
    {...props}
  />
)
GeneralDescriptionTable.propTypes = {
}

export default compose(
  injectIntl,
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
)(GeneralDescriptionTable)
