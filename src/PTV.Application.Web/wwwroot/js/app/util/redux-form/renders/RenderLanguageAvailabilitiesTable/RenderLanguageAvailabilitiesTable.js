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
import { compose } from 'redux'
import { SimpleTable } from 'sema-ui-components'
import NameCell from './NameCell'
import Language from 'appComponents/Language'
import moment from 'moment'
import { injectIntl, intlShape } from 'util/react-intl'
import ComposedCell from 'appComponents/Cells/ComposedCell'
import CellHeaders from 'appComponents/Cells/CellHeaders'
import TranslationTags from './TranslationTags'
import PublishingTags from './PublishingTags'
import { Map } from 'immutable'
import { connect } from 'react-redux'
import { getPublishingStatusDeletedId } from 'selectors/common'

const Modified = ({ modified }) => {
  return modified && (
    <div>
      <div>{moment.utc(modified).local().format('DD.MM.YYYY')}</div>
      <div>({moment.utc(modified).local().format('HH:mm')})</div>
    </div>
  ) || <p />
}

Modified.propTypes = {
  modified: PropTypes.string
}
const HeaderFormatter = compose(injectIntl)(({
  intl: { formatMessage },
  label }) => (<div>{formatMessage(label)}</div>))

HeaderFormatter.propTypes = {
  intl: intlShape,
  label: PropTypes.object.isRequired
}

const columns = {
  language: {
    property: 'language',
    header: {
      formatters: [props => <HeaderFormatter label={CellHeaders.languages} />]
    },
    cell: {
      formatters: [
        (language, { rowData }) => <Language {...rowData} />
      ]
    }
  },
  name: {
    property: 'name',
    header: {
      formatters: [props => <HeaderFormatter label={CellHeaders.name} />]
    },
    cell: {
      formatters: [
        (name, { rowData }) => <ComposedCell main={<NameCell {...rowData} />} inTable>
          <TranslationTags {...rowData} />
          <PublishingTags {...rowData} />
        </ComposedCell>
      ]
    }
  },
  modifiedBy: {
    property: 'modifiedBy',
    header: {
      formatters: [props => <HeaderFormatter label={CellHeaders.modified} />]
    }
  },
  modified: {
    property: 'modified',
    header: {
      formatters: [props => <HeaderFormatter label={CellHeaders.edited} />]
    },
    cell: {
      formatters: [
        modified => <Modified modified={modified} />
      ]
    }
  },
  comment: {
    property: 'comment',
    header: {
      formatters: [props => <HeaderFormatter label={CellHeaders.publishingComment} />]
    }
  },
  numberOfConnections: {
    property: 'numberOfConnections',
    header: {
      formatters: [props => <HeaderFormatter label={CellHeaders.numberOfConnections} />]
    }
  }
}

const columnsWithConnections = {
  services: {}, channels: {}, generalDescriptions:{}
}

const getColumnsDefinition = ({
  entityType,
  appendColumns,
  prependColumns,
  customSetup,
  noAuditing,
  noConnections
}) => {
  let tableColumns = [
    columns.language,
    columns.name
  ]

  if (columnsWithConnections[entityType] && !customSetup && !noConnections) {
    tableColumns.push(columns.numberOfConnections)
  }
  if (!noAuditing) {
    tableColumns = tableColumns.concat([
      columns.modifiedBy,
      columns.modified
    // TODO: not required for release 1.7
    // columns.comment
    ])
  }
  if (appendColumns) {
    tableColumns = [...tableColumns, ...appendColumns]
  }
  if (prependColumns) {
    tableColumns = [...prependColumns, ...tableColumns]
  }

  return tableColumns
}

const LanguageAvailabilities = props => (
  <SimpleTable
    columns={getColumnsDefinition(props)}
    borderless={props.borderless}
    columnWidths={props.columnWidths}
    tight={props.tight}
    className={props.componentClass}>
    <SimpleTable.Header />
    <SimpleTable.Body
      rowKey='languageId'
      rows={props.rows}
    />
  </SimpleTable>
)
LanguageAvailabilities.propTypes = {
  rows: PropTypes.array,
  borderless: PropTypes.bool,
  columnWidths: PropTypes.any,
  tight: PropTypes.bool,
  componentClass: PropTypes.string
}

export default compose(
  connect((state, { input, entity, formName, isArchived }) => {
    const deletedId = getPublishingStatusDeletedId(state)
    const rows = input.value.size > 0 &&
      input.value.map(lA => ({
        languageId: lA.get('languageId'),
        statusId: isArchived && deletedId || lA.get('statusId'),
        validFrom: lA.get('validFrom'),
        newStatusId: isArchived && deletedId || lA.get('newStatusId') || lA.get('statusId'),
        modified: lA.get('modified'),
        modifiedBy: lA.get('modifiedBy'),
        comment: lA.get('comment'),
        canBePublished: lA.get('canBePublished'),
        validatedFields: lA.get('validatedFields'),
        name: entity.get('name') || Map(),
        version: entity.getIn(['version', 'value']) || null,
        numberOfConnections: entity.get('numberOfConnections'),
        formName: formName,
        isArchived,
        useAlter: entity.get('isAlternateNameUsedAsDisplayName') || Map(),
        alterName: entity.get('alternateName') || Map(),
        translationAvailability: entity.get('translationAvailability') || Map(),
        input
      })).toJS() || []
    return {
      rows
    }
  }),
)(LanguageAvailabilities)
