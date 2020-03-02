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
import { defineMessages } from 'util/react-intl'
import EntityDescription from 'appComponents/EntityDescription'
import AddConnectionItemButton from 'Routes/Connections/components/AddConnectionItemButton'
import styles from './styles.scss'

const messages = defineMessages({
  name: {
    id: 'FrontPage.Shared.Search.Header.Name',
    defaultMessage: 'Nimi'
  },
  addAll: {
    id: 'Routes.Connections.Components.SearchResult.Header.AddAll',
    defaultMessage: 'Liitä kaikki'
  },
  searchResultText: {
    id: 'Routes.Connections.Components.SearchFormFooter.SearchResult.Text',
    defaultMessage: 'Hakutuloksia: {count}'
  }
})

const addConnectionColumn = {
  property: 'actions',
  header: {
    formatters: [
      () => <AddConnectionItemButton all />
    ]
  },
  cell: {
    formatters: [
      (name, { rowData }) => <AddConnectionItemButton item={rowData} />
    ]
  }
}

export const serviceColumnDefinition = (formatMessage, size) => [
  addConnectionColumn,
  {
    property: 'name',
    header: {
      label: <div className={styles.resultsCount}>
        <div className={styles.addAll}>{formatMessage(messages.addAll)}</div>
        <div>{formatMessage(messages.searchResultText, { count: size || 0 })}</div>
      </div>
    },
    cell: {
      formatters: [
        (name, { rowData: { id, organizationId, unificRootId, ...rest } }) => {
          return <EntityDescription
            name={name}
            entityId={id}
            unificRootId={unificRootId}
            organizationId={organizationId}
            entityConcreteType='service'
          />
        }
      ]
    }
  }
]

export const channelColumnDefinition = (formatMessage, size) => [
  addConnectionColumn,
  {
    property: 'name',
    header: {
      label: <div className={styles.resultsCount}>
        <div className={styles.addAll}>{formatMessage(messages.addAll)}</div>
        <div>{formatMessage(messages.searchResultText, { count: size || 0 })}</div>
      </div>
    },
    cell: {
      formatters: [
        (name, { rowData: { id, organizationId, channelType, unificRootId, ...rest } }) => {
          return <EntityDescription
            name={name}
            entityId={id}
            unificRootId={unificRootId}
            organizationId={organizationId}
            entityConcreteType={channelType && channelType.toLowerCase()}
            {...rest}
          />
        }
      ]
    }
  }
]
