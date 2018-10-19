import React from 'react'
import { defineMessages } from 'util/react-intl'
import EntityDescription from 'appComponents/EntityDescription'
import AddConnectionItemButton from 'Routes/Connections/components/AddConnectionItemButton'

const messages = defineMessages({
  name: {
    id: 'FrontPage.Shared.Search.Header.Name',
    defaultMessage: 'Nimi'
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

export const serviceColumnDefinition = (formatMessage) => [
  {
    property: 'name',
    header: {
      label: formatMessage(messages.name)
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
  },
  addConnectionColumn
]

export const channelColumnDefinition = (formatMessage) => [
  {
    property: 'name',
    header: {
      label: formatMessage(messages.name)
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
          />
        }
      ]
    }
  },
  addConnectionColumn
]
