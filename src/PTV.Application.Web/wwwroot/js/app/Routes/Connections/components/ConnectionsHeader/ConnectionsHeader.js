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
import Spacer from 'appComponents/Spacer'
import CellHeaders from 'appComponents/Cells/CellHeaders'
import ConnectionsTitle from './ConnectionsTitle'
import { getConnectionsMainEntity, getConnectionsAddToAllEntities } from 'selectors/selections'
import { getMainTranslatedEntities } from '../../selectors'
import cx from 'classnames'
import styles from './styles.scss'
import { injectIntl, intlShape } from 'util/react-intl'
import ColumnHeaderName from 'appComponents/Cells/ColumnHeaderName'
import withState from 'util/withState'
import { mergeInUIState } from 'reducers/ui'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { getSelectedLanguage } from 'Intl/Selectors'
import { change, isDirty } from 'redux-form/immutable'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { getSelections } from 'selectors/base'
import { setConnectionsActiveEntity } from 'reducers/selections'
import { makeCurrentFormStateInitial } from 'Routes/Connections/actions'
import { sortDirectionTypesEnum } from 'enums'

const ConnectionsHeader = ({
  mainEntity,
  isBatchAdding,
  intl: { formatMessage },
  sorting,
  dispatch,
  change,
  formName,
  isDirty,
  makeCurrentFormStateInitial
}) => {
  const headingsClass = cx(
    {
      [styles.indented]: !isBatchAdding
    }
  )

  const renumberActiveConnections = (work, sorted, selections) => {
    const ws = work.map((val, index) => selections.has(index) ? val.getIn(['mainEntity', 'id']) : null).filter(x => x)
    const ss = sorted.map((val, index) => ws.includes(val.getIn(['mainEntity', 'id'])) ? index : null).filter(x => x !== null)
    return ss.toSet()
  }

  const onSort = (column) => {
    const currentSortDirection = sorting.getIn(['mainConnections', 'column']) === column.property
      ? sorting.getIn(['mainConnections', 'sortDirection'])
      : null
    const sortDirection = currentSortDirection
      ? currentSortDirection === sortDirectionTypesEnum.DESC
        ? sortDirectionTypesEnum.ASC
        : sortDirectionTypesEnum.DESC
      : sortDirectionTypesEnum.ASC
    const newSorting = sorting.mergeIn(['mainConnections'],
      { column: column.property,
        sortDirection: sortDirection })
    dispatch(mergeInUIState({ key: 'uiData', value: { sorting: newSorting } }))

    let sortedWork
    dispatch(({ getState }) => {
      const state = getState()
      const workBench = getMainTranslatedEntities(state)
      const lang = getSelectedLanguage(state)
      const activeConnections = getSelections(state).get('connectionsActiveEntities')
      sortedWork = workBench.sort((a, b) => {
        const first = a.getIn(['mainEntity', column.property]).get(lang) || a.getIn(['mainEntity', column.property]).first()
        const second = b.getIn(['mainEntity', column.property]).get(lang) || b.getIn(['mainEntity', column.property]).first()
        return sortDirection === sortDirectionTypesEnum.ASC
          ? first.toLowerCase().localeCompare(second.toLowerCase(), lang)
          : second.toLowerCase().localeCompare(first.toLowerCase(), lang)
      })
      dispatch(setConnectionsActiveEntity(renumberActiveConnections(workBench, sortedWork, activeConnections)))
    })
    sortedWork = sortedWork.map(x => x.deleteIn(['mainEntity', 'contentType']))
    dispatch(change(formName, 'connections', sortedWork))
    if (!isDirty) {
      makeCurrentFormStateInitial()
    }
  }
  return (
    <div>
      <ConnectionsTitle />
      {mainEntity &&
        <div>
          <Spacer marginSize='m10' />
          <div className={headingsClass}>
            <div className='row'>
              <div className='col-lg-12'>
                <ColumnHeaderName
                  name={formatMessage(CellHeaders.name)}
                  column={{ property:'name' }}
                  contentType={'mainConnections'}
                  onSort={onSort}
                />
              </div>
              <div className='col-lg-4'>
                <ColumnHeaderName
                  name={formatMessage(CellHeaders.contentType)}
                  column={{ property: 'contentType' }}
                  contentType={'mainConnections'}
                  onSort={onSort}
                />
              </div>
            </div>
          </div>
        </div>
      }
    </div>
  )
}
ConnectionsHeader.propTypes = {
  mainEntity: PropTypes.oneOf(['channels', 'services']),
  isBatchAdding: PropTypes.bool,
  intl: intlShape,
  formName: PropTypes.string.isRequired,
  sorting: ImmutablePropTypes.map.isRequired,
  dispatch: PropTypes.func,
  change: PropTypes.func.isRequired,
  isDirty: PropTypes.bool.isRequired,
  makeCurrentFormStateInitial: PropTypes.func.isRequired
}

export default compose(
  injectIntl,
  injectFormName,
  withState({
    redux: true,
    key: 'uiData',
    keepImmutable: true,
    initialState: {
      sorting: Map()
    }
  }),
  connect((state, ownProps) => ({
    mainEntity: getConnectionsMainEntity(state),
    isBatchAdding: getConnectionsAddToAllEntities(state),
    isDirty: isDirty(ownProps.formName)(state)
  }), {
    mergeInUIState,
    change,
    makeCurrentFormStateInitial
  }),
  withFormStates
)(ConnectionsHeader)
