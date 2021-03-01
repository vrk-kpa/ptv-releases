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

import React, { PureComponent } from 'react'
import { compose } from 'redux'
import { connect } from 'react-redux'
import PreviewConnection from 'Routes/Connections/components/PreviewConnection'
import { injectIntl, intlShape } from 'util/react-intl'
import { getMainEntitiesList } from 'Routes/Connections/selectors'
import { getSortingOrderForMainEntities } from 'Routes/Connections/components/Childs/selectors'
import { getConnectionsMainEntity } from 'selectors/selections'
import CellHeaders from 'appComponents/Cells/CellHeaders'
import { entityTypesEnum } from 'enums'
import styles from './styles.scss'
import ColumnHeaderName from 'appComponents/Cells/ColumnHeaderName'
import PropTypes from 'prop-types'
import ImmutablePropTypes from 'react-immutable-proptypes'

class PreviewConnections extends PureComponent {
  render () {
    const {
      intl: { formatMessage },
      onSort,
      contentType,
      sortingOrder,
      mainEntities,
      mainEntity
    } = this.props
    const sorted = mainEntities.map((item, index) => ({ item, index, sort: sortingOrder.indexOf(index) }))
      .sort((a, b) => a.sort < b.sort ? -1 : a.sort > b.sort ? 1 : 0)
    const contentTypeIdProperty = {
      [entityTypesEnum.CHANNELS]: 'channelTypeId',
      [entityTypesEnum.SERVICES]: 'serviceType'
    }[mainEntity]
    return (
      <div>
        <div className={styles.previewHeader}>
          <div className='row align-items-center'>
            <div className='col-lg-2'>
              <ColumnHeaderName
                name={formatMessage(CellHeaders.languages)}
                contentType={contentType}
                column={{ property: 'languages' }}
              />
            </div>
            <div className='col-lg-7'>
              <ColumnHeaderName
                name={formatMessage(CellHeaders.name)}
                column={{ property: 'name', isLocalized: true }}
                contentType={contentType}
                onSort={onSort}
              />
            </div>
            <div className='col-lg-4'>
              <ColumnHeaderName
                name={formatMessage(CellHeaders.contentType)}
                column={{ property: contentTypeIdProperty }}
                contentType={contentType}
                onSort={onSort}
              />
            </div>
            <div className='col-lg-11'>
              <ColumnHeaderName
                name={formatMessage(CellHeaders.channelSearchBoxOrganizationTitle)}
                contentType={contentType}
                column={{ property: 'organization' }}
                onSort={onSort}
              />
            </div>
          </div>
        </div>
        {sorted.map((item, index) => {
          const id = item.item.get('id')
          return (
            <PreviewConnection
              key={id}
              id={id}
              parent={item.item}
              parentIndex={item.index}
            />
          )
        })}
      </div>
    )
  }
}
PreviewConnections.propTypes = {
  intl: intlShape,
  onSort: PropTypes.func,
  contentType: PropTypes.string,
  sortingOrder: ImmutablePropTypes.list,
  mainEntities: ImmutablePropTypes.list,
  mainEntity: PropTypes.string
}

export default compose(
  injectIntl,
  connect((state, ownProps) => {
    const mainEntities = getMainEntitiesList(state)
    return {
      sortingOrder: getSortingOrderForMainEntities(state, {
        data: mainEntities,
        contentType: 'summaryMainConnection',
        serviceCustomType: 'serviceType'
      }),
      mainEntity: getConnectionsMainEntity(state),
      mainEntities
    }
  })
)(PreviewConnections)
