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
import { taskTypesEnum, notificationTypesEnum } from 'enums'
import ColumnHeaderName from 'appComponents/Cells/ColumnHeaderName'
import Cell from 'appComponents/Cells/Cell'
import LanguageBarCell from 'appComponents/Cells/LanguageBarCell'
import NameCellWithNavigation from 'appComponents/Cells/NameCellWithNavigation'
import ModifiedAtCell from 'appComponents/Cells/ModifiedAtCell'
import ModifiedByCell from 'appComponents/Cells/ModifiedByCell'
import LanguagesCell from 'appComponents/Cells/LanguagesCell'
import CellHeaders from 'appComponents/Cells/CellHeaders'
import ContentTypeCell from 'appComponents/Cells/ContentTypeCell'
import OperationTypeCell from 'appComponents/Cells/OperationTypeCell'
import IconEye from 'appComponents/IconEye'
import Language from 'appComponents/Language'
import ComposedCell from 'appComponents/Cells/ComposedCell'
import MassToolCheckbox from 'Routes/Search/components/MassTool/MassToolCheckbox'
import styles from '../NotificationTable/styles.scss'
import DateTimeCell from 'appComponents/Cells/DateTimeCell'

const languagesAvailabilitiesDef = (formatMessage) => ({
  property: 'languagesAvailabilities',
  header: {
    label: formatMessage(CellHeaders.languages)
  },
  cell: {
    formatters: [
      (languagesAvailabilities, { rowData }) => <Cell dWidth='dw120' ldWidth='ldw160'>
        <LanguageBarCell clickable {...rowData} />
      </Cell>
    ]
  }
})

const languageDef = (formatMessage) => ({
  property: 'language',
  header: {
    label: formatMessage(CellHeaders.languages)
  },
  cell: {
    formatters: [
      (language, { rowData }) => <Language {...rowData} />
    ]
  }
})

const nameDef = (formatMessage, previewOnClick, formName, onSort, customProperty) => ({
  property: 'name',
  header: {
    formatters: [
      (_, { column }) => (
        <div>
          <ColumnHeaderName
            name={formatMessage(CellHeaders.name)}
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
      (name, { rowData }) => {
        const id = customProperty && rowData[customProperty] || rowData.id
        return <Cell dWidth='dw260' ldWidth='ldw350' inline>
          <NameCellWithNavigation viewIcon viewOnClick={previewOnClick}{...rowData} id={id} languageCode />
        </Cell>
      }
    ]
  }
})

const modifyByDef = (formatMessage, formName, onSort, customProperty) => ({
  property: customProperty || 'modifiedBy',
  header: {
    formatters: [
      (_, { column }) => (
        <div>
          <ColumnHeaderName
            name={formatMessage(CellHeaders.modified)}
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
      modifiedBy => <Cell dWidth='dw160' ldWidth='ldw220'>
        <ModifiedByCell componentClass='small' modifiedBy={modifiedBy} />
      </Cell>
    ]
  }
})

const modifiedDef = (formatMessage, formName, onSort) => ({
  property: 'created',
  header: {
    formatters: [
      (_, { column }) => (
        <div>
          <ColumnHeaderName
            name={formatMessage(CellHeaders.edited)}
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
      createdAt => (
        <Cell dWidth='dw90' ldWidth='ldw120'>
          <ModifiedAtCell modifiedAt={createdAt} timeParens />
        </Cell>
      )
    ]
  }
})

const expireOnDef = (formatMessage, formName, onSort) => ({
  property: 'modified',
  header: {
    formatters: [
      (_, { column }) => (
        <div>
          <ColumnHeaderName
            name={formatMessage(CellHeaders.expireOn)}
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
      (_, { rowData: { expireOn } }) => <Cell dWidth='dw90' ldWidth='ldw120'>
        <ModifiedAtCell modifiedAt={expireOn} />
      </Cell>
    ]
  }
})

const lastFailedPublishDateDef = (formatMessage, formName, onSort) => ({
  property: 'lastFailedPublishDate',
  header: {
    formatters: [
      (_, { column }) => (
        <div>
          <ColumnHeaderName
            name={formatMessage(CellHeaders.lastFailedPublishDate)}
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
      lastFailedPublishDate => <Cell dWidth='dw90' ldWidth='ldw120'>
        <DateTimeCell value={lastFailedPublishDate} />
      </Cell>
    ]
  }
})

const missingLanguagesDef = (formatMessage) => ({
  property: 'missingLanguages',
  header: {
    label: formatMessage(CellHeaders.missingLanguages)
  },
  cell: {
    formatters: [
      missingLanguages => <Cell dWidth='dw120' ldWidth='ldw160'>
        <LanguagesCell languageIds={missingLanguages} />
      </Cell>
    ]
  }
})

const nameWithTypeDef = (formatMessage, previewOnClick, formName, onSort, customProperty) => ({
  property: 'name',
  header: {
    formatters: [
      (_, { column }) => (
        <div>
          <ColumnHeaderName
            name={formatMessage(CellHeaders.name)}
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
      (name, { rowData }) => {
        const id = customProperty && rowData[customProperty] || rowData.id
        return <ComposedCell
          inTable
          icon={
            <IconEye
              viewOnClick={previewOnClick}
              {...rowData}
              id={id}
            />
          }
          main={
            <Cell dWidth={'dw150'} ldWidth={'ldw150'}>
              <NameCellWithNavigation
                {...rowData}
                id={id}
                languageCode
              />
            </Cell>
          }
          sub={
            <ContentTypeCell
              subEntityType={rowData['subEntityType']}
              formatMessage={formatMessage}
            />
          }
        />
      }
    ]
  }
})

const connectionDef = (formatMessage, previewOnClick, formName, onSort, customProperty) => ({
  property: 'operationType',
  header: {
    formatters: [
      (_, { column }) => (
        <div>
          <ColumnHeaderName
            name={formatMessage(CellHeaders.additionalInformation)}
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
      (name, { rowData }) => {
        const id = customProperty && rowData[customProperty] || rowData.id
        return <ComposedCell
          inTable
          icon={
            <IconEye
              viewOnClick={previewOnClick}
              id={id}
              mainEntityType={rowData['connectedMainEntityType']}
              subEntityType={rowData['connectedSubEntityType']} />
          }
          main={
            <Cell>
              <OperationTypeCell
                opetarionType={rowData['operationType']}
                formatMessage={formatMessage}
              />
            </Cell>
          }
          sub={
            <Cell className={styles.alignInline}>
              <NameCellWithNavigation
                id={id}
                name={rowData['connectedName']}
                mainEntityType={rowData['connectedMainEntityType']}
                subEntityType={rowData['connectedSubEntityType']}
                languageCode
                componentClass={styles.mainEntityType}
              />
              <ContentTypeCell
                subEntityType={rowData['connectedSubEntityType']}
                formatMessage={formatMessage}
                subParens
              />
            </Cell>
          }
        />
      }
    ]
  }
})

const massToolCheckboxDef = (formatMessage) => ({
  header: {
    label: formatMessage(CellHeaders.rowSelection)
  },
  cell: {
    formatters: [
      (languagesAvailabilities, { rowData }) => <Cell dWidth='dw30' ldWidth='ldw30'>
        <MassToolCheckbox
          id={rowData.id}
          className={styles.massToolCheckbox}
          entityType={rowData.mainEntityType}
          {...rowData}
        />
      </Cell>
    ]
  }
})

export const getTNColumnsDefinition = (type, isMassToolActive) => ({
  previewOnClick,
  formName,
  formatMessage }) => (onSort) => {
  switch (type) {
    case taskTypesEnum.OUTDATEDDRAFTSERVICES:
    case taskTypesEnum.OUTDATEDPUBLISHEDSERVICES:
    case taskTypesEnum.OUTDATEDDRAFTCHANNELS:
    case taskTypesEnum.OUTDATEDPUBLISHEDCHANNELS:
      const columns = [
        languagesAvailabilitiesDef(formatMessage),
        nameDef(formatMessage, previewOnClick, formName, onSort),
        modifyByDef(formatMessage, formName, onSort),
        expireOnDef(formatMessage, formName, onSort)
      ]
      return isMassToolActive ? [massToolCheckboxDef(formatMessage)].concat(columns) : columns
    case taskTypesEnum.CHANNELSWITHOUTSERVICES:
    case taskTypesEnum.SERVICESWITHOUTCHANNELS:
      return [
        languagesAvailabilitiesDef(formatMessage),
        nameDef(formatMessage, previewOnClick, formName, onSort, 'versionedId'),
        modifyByDef(formatMessage, formName, onSort)]
    case taskTypesEnum.TRANSLATIONARRIVED:
      return [
        languagesAvailabilitiesDef(formatMessage),
        nameDef(formatMessage, previewOnClick, formName, null, 'versionedId'),
        modifyByDef(formatMessage, formName, null, 'createdBy')]
    case taskTypesEnum.MISSINGLANGUAGEORGANIZATIONS:
      return [
        languagesAvailabilitiesDef(formatMessage),
        nameDef(formatMessage, previewOnClick, formName, onSort),
        missingLanguagesDef(formatMessage),
        modifyByDef(formatMessage, formName, onSort)]
    case taskTypesEnum.TIMEDPUBLISHFAILED:
      return [
        languageDef(formatMessage),
        nameDef(formatMessage, previewOnClick, formName, null, 'versionedId'),
        modifyByDef(formatMessage, formName, null, 'createdBy'),
        lastFailedPublishDateDef(formatMessage, formName, null)
      ]
    case notificationTypesEnum.SERVICECHANNELINCOMMONUSE:
    case notificationTypesEnum.CONTENTUPDATED:
    case notificationTypesEnum.CONTENTARCHIVED:
    case notificationTypesEnum.GENERALDESCRIPTIONCREATED:
    case notificationTypesEnum.GENERALDESCRIPTIONUPDATED:
      return [
        languagesAvailabilitiesDef(formatMessage),
        nameDef(formatMessage, previewOnClick, formName, onSort, 'versionedId'),
        modifyByDef(formatMessage, formName, onSort, 'createdBy'),
        modifiedDef(formatMessage, formName, onSort)]
    case notificationTypesEnum.CONNECTIONCHANGES:
      return [
        languagesAvailabilitiesDef(formatMessage),
        nameWithTypeDef(formatMessage, previewOnClick, formName, onSort, 'versionedId'),
        connectionDef(formatMessage, previewOnClick, formName, onSort, 'connectedVersionedId'),
        modifyByDef(formatMessage, formName, onSort, 'createdBy'),
        modifiedDef(formatMessage, formName, onSort)
      ]
  }
}
