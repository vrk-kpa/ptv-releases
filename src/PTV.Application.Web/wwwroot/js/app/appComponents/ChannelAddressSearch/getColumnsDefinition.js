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
import Cell from 'appComponents/Cells/Cell'
import CellHeaders from 'appComponents/Cells/CellHeaders'
import NameCell from 'appComponents/Cells/NameCell'
import AddressCell from 'appComponents/Cells/AddressCell'
import OrganizationCell from 'appComponents/Cells/OrganizationCell'
import ColumnHeaderName from 'appComponents/Cells/ColumnHeaderName'
import LanguageBarCell from 'appComponents/Cells/LanguageBarCell'

const getColumnsDefinition = ({ formatMessage, previewOnClick, formName }) => (onSort) => [
  {
    property: 'languageAvailabilities',
    header: {
      label: formatMessage(CellHeaders.languages)
    },
    cell: {
      formatters: [
        (languagesAvailabilities, { rowData }) =>
          <Cell dWidth='dw260' ldWidth='ldw350' inline>
            <LanguageBarCell {...rowData} />
          </Cell>
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
              name={formatMessage(CellHeaders.channelName)}
              column={column}
              contentType={formName}
              onSort={onSort}
            />
          </div>
        )
      ]
    },
    cell: {
      formatters: [
        (name, { rowData }) => (
          <Cell dWidth='dw180' ldWidth='ldw240' inline>
            <NameCell
              viewIcon
              viewOnClick={previewOnClick}
              {...rowData}
              // Used in previewOnClick ^ callback //
              id={rowData.serviceChannelId}
              languageCode
            />
          </Cell>
        )
      ]
    }
  }, {
    property: 'organization',
    header: {
      formatters: [
        (_, { column }) => (
          <div>
            <ColumnHeaderName
              name={formatMessage(CellHeaders.channelSearchBoxOrganizationTitle)}
              column={column}
              contentType={formName}
              onSort={onSort}
            />
          </div>
        )
      ]
    },
    cell: {
      formatters: [
        (name, { rowData }) => (
          <OrganizationCell organizationId={rowData.organization} />
        )
      ]
    }
  }, {
    property: 'address',
    header: {
      formatters: [
        (_, { column }) => (
          <div>
            <ColumnHeaderName
              name={formatMessage(CellHeaders.address)}
              column={column}
              contentType={formName}
              onSort={onSort}
            />
          </div>
        )
      ]
    },
    cell: {
      formatters: [
        (name, { rowData }) => (
          <AddressCell
            street={rowData.street}
            streetNumber={rowData.streetNumber}
            postalCode={rowData.postalCode}
          />
        )
      ]
    }
  }
]

export default getColumnsDefinition
