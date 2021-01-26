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
import CellHeaders from 'appComponents/Cells/CellHeaders'
import NameCell from 'appComponents/Cells/NameCell'
import NameCellWithNavigation from 'appComponents/Cells/NameCellWithNavigation'
import LanguageBarCell from 'appComponents/Cells/LanguageBarCell'
import ColumnHeaderName from 'appComponents/Cells/ColumnHeaderName'
import ValueCell from 'appComponents/Cells/ValueCell'
import { formTypesEnum } from 'enums'

export const getColumnDefinitions = ({ formatMessage, withNavigation }) => onSort => {
  return [
    {
      property: 'languagesAvailabilities',
      header: {
        label: formatMessage(CellHeaders.languages)
      },
      cell: {
        formatters: [
          languagesAvailabilities => {
            return <LanguageBarCell showMissing languagesAvailabilities={languagesAvailabilities} />
          }
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
                name={formatMessage(CellHeaders.name)}
                column={column}
                contentType={formTypesEnum.ELECTRONICCHANNELFORM}
                onSort={onSort}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          (name, { rowData }) => {
            return withNavigation
              ? <NameCellWithNavigation
                name={name}
                id={rowData.id}
                mainEntityType='service'
                subEntityType='service'
                linkProps={{ target: '_blank' }} />
              : <NameCell name={name} />
          }
        ]
      }
    },
    {
      property: 'serviceType',
      header: {
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={formatMessage(CellHeaders.serviceType)}
                column={column}
                contentType={formTypesEnum.ELECTRONICCHANNELFORM}
                onSort={onSort}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          serviceType => {
            return <ValueCell value={serviceType} textHandling='prewrap' />
          }
        ]
      }
    },
    {
      property: 'organization',
      header: {
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={formatMessage(CellHeaders.channelSearchBoxOrganizationTitle)}
                column={column}
                contentType={formTypesEnum.ELECTRONICCHANNELFORM}
                onSort={onSort}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          organization => {
            return <ValueCell value={organization} textHandling='prewrap' />
          }
        ]
      }
    }
  ]
}
