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
import React, { Fragment } from 'react'
import { compose } from 'redux'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import styles from './styles.scss'
import { Button, Label, Spinner } from 'sema-ui-components'
import cx from 'classnames'
import Pagination from 'appComponents/Pagination'
import Paragraph from 'appComponents/Paragraph'
import Placeholder from 'appComponents/Placeholder'
import PreviewConnections from 'Routes/Connections/components/PreviewConnections'
import withState from 'util/withState'
import PropTypes from 'prop-types'
import {
  getServiceIsFetching,
  getServiceResultCount,
  GetServiceResultJS
} from 'Routes/Connections/selectors'
import { connect } from 'react-redux'
import { arrayPush, change } from 'redux-form/immutable'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { addItemToWorkBench, addAllItemsToWorkBench } from 'Routes/Connections/components/WorkbenchActions/actions'
import { makeCurrentFormStateInitial } from 'Routes/Connections/actions'
import { mergeInUIState } from 'reducers/ui'
import { Map, List } from 'immutable'
import {
  setConnectionsMainEntity,
  setConnectionsEntity,
  setConnectionsView
} from 'reducers/selections'
import { sortDirectionTypesEnum, EDIT_CONNECTIONS_SERVICE_COUNT_TRESHOLD } from 'enums'
import { getSearchOrganizationValues, getAllSearchOrganizationValues } from '../ConnectionsRoute/selectors'
import { apiCall3 } from 'actions'
import { EntitySchemas } from 'schemas'
import EditOrganizationConnectionDialog from '../EditOrganizationConnectionDialog'

const messages = defineMessages({
  pageTitle: {
    id: 'Routes.Connections.OrganizationConnections.Title.Text',
    defaultMessage: 'Yhteenveto: Palveluihin liitetyt asiointikanavat'
  },
  pageSubtitle: {
    id: 'Routes.Connections.OrganizationConnections.Subtitle.Text',
    // eslint-disable-next-line max-len
    defaultMessage: 'Alla on yhteenveto organisaatiosi palvelujen ja asiointikanavien liitoksista. Voit tarkastella liitoksia ja muokata liitosten lisätietoja. Voit siirtyä muokkaamaan nykyisiä tai lisäämään uusia liitoksia Muokkaa liitoksia -napista. '
  },
  editMyConnections: {
    id: 'Routes.Connections.OrganizationConnections.Edit.Title',
    defaultMessage: 'Muokkaa organisaatiosi liitoksia'
  },
  serviceCount: {
    id: 'Routes.Connections.OrganizationConnections.Services.Count',
    defaultMessage: 'Palveluita {count}'
  },
  seeAll: {
    id: 'Routes.Connections.OrganizationConnections.Link.SeeAll',
    defaultMessage: 'Näytä kaikki'
  },
  emptyPlaceholder: {
    id: 'OrganizationConnections.Empty.Placeholder',
    defaultMessage: 'There are no connections in the organization yet.'
  }
})

const OrganizationConnections = props => {
  const {
    intl: { formatMessage },
    currentPage,
    count,
    dispatch,
    isLoading,
    makeCurrentFormStateInitial,
    sorting,
    mergeInUIState,
    setConnectionsMainEntity,
    setConnectionsEntity,
    setConnectionsView,
    showAll,
    change
  } = props

  const onSort = (column) => {
    const sortDirection = sorting.getIn(['summaryMainConnection', 'column']) === column.property
      ? sorting.getIn(['summaryMainConnection', 'sortDirection']) || sortDirectionTypesEnum.DESC : sortDirectionTypesEnum.DESC
    const newSorting = sorting.mergeIn(['summaryMainConnection'],
      { column: column.property,
        sortDirection: sortDirection === sortDirectionTypesEnum.DESC && sortDirectionTypesEnum.ASC || sortDirectionTypesEnum.DESC })
    mergeInUIState({ key: 'uiData', value: { sorting: newSorting } })
    !showAll && search(currentPage)
  }

  const handleOnPageChange = active => {
    search(active)
  }

  const search = (active) => {
    let searchValues
    dispatch(({ getState }) => {
      const state = getState()
      searchValues = getSearchOrganizationValues(state, { pageNumber: active, contentType: 'summaryMainConnection' })
    })
    dispatch(
      apiCall3({
        keys: [
          'connections',
          'serviceSearch'
        ],
        payload: {
          endpoint: 'service/GetOrganizationConnections',
          data: searchValues
        },
        saveRequestData: true,
        schemas: EntitySchemas.GET_SEARCH(EntitySchemas.SERVICE),
        requestProps: { isWorkbench: true },
        successNextAction: () => {
          mergeInUIState({ key: 'uiData', value: { currentPage: active } })
          addItemToWorkBench(0, dispatch)
          makeCurrentFormStateInitial()
        }
      })
    )
  }

  const onEditOrganizationDialog = () => {
    mergeInUIState({ key: 'editOrganizationConnectionDialog', value: { isOpen: true } })
  }

  const onEditOrganization = (previewMode) => {
    !previewMode && mergeInUIState({ key: 'connectionsTabIndex', value: { activeConnectionsTabIndex: 0 } })
    setConnectionsMainEntity('services')
    setConnectionsEntity('services')
    change('connectionsWorkbench', 'connections', List())
    makeCurrentFormStateInitial()
    let searchValues
    dispatch(({ getState }) => {
      const state = getState()
      searchValues = getAllSearchOrganizationValues(state, { count: count })
    })
    dispatch(
      apiCall3({
        keys: [
          'connections',
          'serviceSearch'
        ],
        payload: {
          endpoint: 'service/GetOrganizationConnections',
          data: searchValues
        },
        saveRequestData: true,
        schemas: EntitySchemas.GET_SEARCH(EntitySchemas.SERVICE),
        requestProps: { isWorkbench: true },
        successNextAction: () => {
          addAllItemsToWorkBench(dispatch)
          makeCurrentFormStateInitial()
          previewMode && mergeInUIState({ key: 'uiData', value: { showAll: true } })
        }
      })
    )
  }

  const topPaginationClass = cx(
    styles.pagination,
    styles.top
  )

  return (
    <div className={styles.myConnections}>
      <EditOrganizationConnectionDialog
        onCancelSuccess={() => onEditOrganization(false)}
        count={count}
      />
      <div className={styles.organizationConnectionsHead}>
        <Label labelText={formatMessage(messages.pageTitle)} />
        <div className='row'>
          <div className='col-lg-18'>
            <Paragraph className={styles.subTitle}>
              {formatMessage(messages.pageSubtitle)}
            </Paragraph>
          </div>
          <div className='col-lg-6 mt-3 mt-lg-0 text-right'>
            <Button
              children={formatMessage(messages.editMyConnections)}
              onClick={count > EDIT_CONNECTIONS_SERVICE_COUNT_TRESHOLD
                ? onEditOrganizationDialog
                : () => onEditOrganization(false)}
              medium
              disabled={isLoading}
            />
          </div>
        </div>
      </div>
      {count > 0
        ? <Fragment>
          <div className='d-flex'>
            <Label
              labelText={formatMessage(messages.serviceCount, { count: count })}
              className={styles.resultCount}
            />
            {!showAll && <Pagination
              count={count}
              active={currentPage}
              onChange={handleOnPageChange}
              disabled={isLoading}
              className={topPaginationClass}
            />}
            {(count <= EDIT_CONNECTIONS_SERVICE_COUNT_TRESHOLD && !showAll) && <Button
              children={formatMessage(messages.seeAll)}
              className={styles.seeAll}
              onClick={() => onEditOrganization(true)}
              small
              link
            />}
          </div>
          {isLoading && <Placeholder><Spinner /></Placeholder> || (
            <PreviewConnections
              contentType={'summaryMainConnection'}
              onSort={onSort}
            />
          )}
          <div className='d-flex'>
            {!showAll && <Pagination
              count={count}
              active={currentPage}
              onChange={handleOnPageChange}
              disabled={isLoading}
              className={styles.pagination}
            />}
          </div>
        </Fragment>
        : <Placeholder>
          {isLoading && <Spinner /> || <Label infoLabel>{formatMessage(messages.emptyPlaceholder)}</Label>}
        </Placeholder>
      }
    </div>
  )
}
OrganizationConnections.propTypes = {
  intl: intlShape,
  currentPage: PropTypes.number,
  mergeInUIState: PropTypes.func,
  isLoading: PropTypes.bool,
  dispatch: PropTypes.func,
  makeCurrentFormStateInitial: PropTypes.func,
  count: PropTypes.number,
  sorting: PropTypes.any,
  setConnectionsMainEntity: PropTypes.func,
  setConnectionsEntity: PropTypes.func,
  change: PropTypes.func.isRequired
}

export default compose(
  injectIntl,
  withFormStates,
  withState({
    redux: true,
    key: 'uiData',
    keepImmutable: true,
    initialState: {
      sorting: Map(),
      currentPage: 0,
      showAll: false
    }
  }),
  connect((state, { currentPage }) => ({
    isLoading: getServiceIsFetching(state),
    count: getServiceResultCount(state),
    data: GetServiceResultJS(state, { index: currentPage, contentType: 'summaryMainConnection' })
  }), {
    change,
    arrayPush,
    makeCurrentFormStateInitial,
    mergeInUIState,
    setConnectionsMainEntity,
    setConnectionsEntity,
    setConnectionsView
  })
)(OrganizationConnections)

