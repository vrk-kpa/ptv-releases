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
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { Map } from 'immutable'
import Tooltip from 'appComponents/Tooltip'
import CellHeaders from 'appComponents/Cells/CellHeaders'
import { getConnectionsMainEntity, getIsConnectionsPreview } from 'selectors/selections'
import cx from 'classnames'
import styles from './styles.scss'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import ColumnHeaderName from 'appComponents/Cells/ColumnHeaderName'
import { mergeInUIState } from 'reducers/ui'
import ImmutablePropTypes from 'react-immutable-proptypes'
import {
  getConnectionsUISorting,
  getIsOrganizingActiveForConnection,
  getOrganizationConnectionsActiveTab
} from 'Routes/Connections/selectors'
import { sortDirectionTypesEnum, entityTypesEnum } from 'enums'

const messages = defineMessages({
  detailTitle: {
    id: 'ConnectionsSubHeader.Detail.Title',
    defaultMessage: 'Liitostiedot'
  },
  detailTooltip: {
    id: 'ConnectionsSubHeader.Detail.Tooltip',
    defaultMessage: 'Liitostiedot Tooltip'
  },
  detailTooltipASTI: {
    id: 'ConnectionsSubHeader.DetailASTI.Tooltip',
    defaultMessage: 'Liitostiedot ASTI Tooltip'
  }
})

const ConnectionsSubHeader = ({
  mainEntity,
  intl: { formatMessage },
  uiSorting,
  connectionIndex,
  mergeInUIState,
  isOrganizingActive,
  isAsti,
  isPreview,
  className,
  connectionsActiveTab
}) => {
  const isOnOrganizationConnections = connectionsActiveTab === 1
  const onSort = column => {
    const sortMapKey = isAsti && `connectionASTI${connectionIndex}` || `connection${connectionIndex}`
    const sortDirection = uiSorting.getIn([sortMapKey, 'column']) === column.property
      ? uiSorting.getIn([sortMapKey, 'sortDirection'])
      : null
    const newSortDirection = {
      [sortDirectionTypesEnum.ASC]: sortDirectionTypesEnum.DESC,
      [sortDirectionTypesEnum.DESC]: sortDirectionTypesEnum.ASC
    }[sortDirection]
    const sortingMap = Map({
      column: column.property,
      sortDirection: newSortDirection || column.defaultOrder || sortDirectionTypesEnum.ASC,
      isLocalized: column.isLocalized
    })
    const newUISorting = uiSorting.update(sortMapKey, (value = Map()) => sortingMap)

    mergeInUIState({
      key: 'uiData',
      value: {
        'sorting': newUISorting
      }
    })
  }
  const contentType = isAsti && `connectionASTI${connectionIndex}` || `connection${connectionIndex}`
  const contentTypeIdProperty = isOnOrganizationConnections && 'channelTypeId' || {
    [entityTypesEnum.CHANNELS]: 'serviceTypeId',
    [entityTypesEnum.SERVICES]: 'channelTypeId'
  }[mainEntity]
  const detailTooltip = isAsti && formatMessage(messages.detailTooltipASTI) || formatMessage(messages.detailTooltip)
  const hideSortingIcon = isOrganizingActive
  const connectionsSubHeaderClass = cx(
    styles.connectionsSubHeader,
    className
  )
  const isOnPreview = isPreview || isOnOrganizationConnections
  return (
    <div className={connectionsSubHeaderClass}>
      <div className='row align-items-center'>
        {isOnPreview && (
          <div className='col-lg-2'>
            <ColumnHeaderName
              name={formatMessage(CellHeaders.languages)}
              contentType={contentType}
              column={{ property:'languages' }}
            />
          </div>
        )}
        <div className={isOnPreview ? 'col-lg-6' : 'col-lg-10'}>
          <ColumnHeaderName
            name={formatMessage(CellHeaders.name)}
            column={{ property: 'name', isLocalized: true }}
            contentType={contentType}
            onSort={onSort}
            hideSortingIcon={hideSortingIcon}
          />
        </div>
        <div className={isOnPreview ? 'col-lg-4' : 'col-lg-4'}>
          <ColumnHeaderName
            name={formatMessage(CellHeaders.contentType)}
            column={{ property: contentTypeIdProperty }}
            contentType={contentType}
            onSort={onSort}
            hideSortingIcon={hideSortingIcon}
          />
        </div>
        <div className={isOnPreview ? 'col-lg-4' : 'col-lg-5'}>
          <ColumnHeaderName
            name={formatMessage(CellHeaders.connection)}
            column={{ property: 'modified', defaultOrder: sortDirectionTypesEnum.DESC }}
            contentType={contentType}
            onSort={onSort}
            hideSortingIcon={hideSortingIcon}
          />
        </div>
        {!isOnPreview && (
          <div className='col-lg-5'>
            <div className='d-flex align-items-center'>
              <span>{formatMessage(messages.detailTitle)}</span>
              <Tooltip tooltip={detailTooltip} />
            </div>
          </div>
        )}
        {isOnPreview && (
          <div className='col-lg-8'>
            <ColumnHeaderName
              name={formatMessage(CellHeaders.connectionInfo)}
              contentType={contentType}
              column={{ property:'connectionInfo' }}
            />
          </div>
        )}
      </div>
    </div>
  )
}
ConnectionsSubHeader.propTypes = {
  mainEntity: PropTypes.oneOf([entityTypesEnum.CHANNELS, entityTypesEnum.SERVICES]),
  intl: intlShape,
  uiSorting: ImmutablePropTypes.map.isRequired,
  connectionIndex: PropTypes.number,
  mergeInUIState: PropTypes.func,
  isOrganizingActive: PropTypes.bool,
  isAsti: PropTypes.bool,
  isPreview: PropTypes.bool,
  className: PropTypes.string,
  connectionsActiveTab: PropTypes.number
}

export default compose(
  injectIntl,
  connect((state, { connectionIndex }) => ({
    uiSorting: getConnectionsUISorting(state),
    mainEntity: getConnectionsMainEntity(state),
    isOrganizingActive: getIsOrganizingActiveForConnection(state, { connectionIndex }),
    isPreview: getIsConnectionsPreview(state),
    connectionsActiveTab: getOrganizationConnectionsActiveTab(state)
  }), {
    mergeInUIState
  })
)(ConnectionsSubHeader)
