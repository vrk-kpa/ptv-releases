import React from 'react'
import Cell from 'appComponents/Cells/Cell'
import CellHeaders from 'appComponents/Cells/CellHeaders'
import LanguageBarCell from 'appComponents/Cells/LanguageBarCell'
import ModifiedAtCell from 'appComponents/Cells/ModifiedAtCell'
import ModifiedByCell from 'appComponents/Cells/ModifiedByCell'
import NameCellWithNavigation from 'appComponents/Cells/NameCellWithNavigation'

const getColumnsDefinition = ({ formatMessage, previewOnClick }) => [
  {
    property: 'languagesAvailabilities',
    header: {
      label: formatMessage(CellHeaders.languages)
    },
    cell: {
      formatters: [
        (languagesAvailabilities, { rowData }) => (
          <Cell dWidth='dw120' ldWidth='ldw160'>
            <LanguageBarCell
              clickable
              {...rowData}
              id={rowData.versionedId}
            />
          </Cell>
        )
      ]
    }
  },
  {
    property: 'name',
    header: {
      label: formatMessage(CellHeaders.name)
    },
    cell: {
      formatters: [
        (name, { rowData }) => (
          <Cell dWidth='dw260' ldWidth='ldw350' inline>
            <NameCellWithNavigation
              viewIcon
              viewOnClick={previewOnClick}
              {...rowData}
              // Used in previewOnClick ^ callback //
              id={rowData.versionedId}
              languageCode
            />
          </Cell>
        )
      ]
    }
  },
  {
    property: 'createdBy',
    header: {
      label: formatMessage(CellHeaders.modified)
    },
    cell: {
      formatters: [
        modifiedBy => (
          <Cell dWidth='dw160' ldWidth='ldw220'>
            <ModifiedByCell componentClass='small' modifiedBy={modifiedBy} />
          </Cell>
        )
      ]
    }
  },
  {
    property: 'created',
    header: {
      label: formatMessage(CellHeaders.modified)
    },
    cell: {
      formatters: [
        createdAt => (
          <Cell dWidth='dw90' ldWidth='ldw120'>
            <ModifiedAtCell modifiedAt={createdAt} />
          </Cell>
        )
      ]
    }
  }
]

export default getColumnsDefinition
