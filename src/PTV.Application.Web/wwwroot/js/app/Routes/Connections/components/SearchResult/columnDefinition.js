import React from 'react'
import { EntityDescription } from 'appComponents'
import { AddConnectionItemButton } from 'Routes/Connections/components'

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

export const serviceColumnDefinition = [
  {
    property: 'name',
    header: {
      label: 'Nimi'
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

export const channelColumnDefinition = [
  {
    property: 'name',
    header: {
      label: 'Nimi'
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
