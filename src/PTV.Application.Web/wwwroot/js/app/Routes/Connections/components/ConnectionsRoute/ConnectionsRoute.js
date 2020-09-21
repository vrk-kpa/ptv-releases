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
import withPreviewDialog from 'util/redux-form/HOC/withPreviewDialog'
import SearchArea from 'Routes/Connections/components/SearchArea'
import Workbench from 'Routes/Connections/components/Workbench'
import styles from './styles.scss'
import cx from 'classnames'
import { connect } from 'react-redux'
import { getIsConnectionsPreview } from 'selectors/selections'
import { Tabs, Tab, Label } from 'sema-ui-components'
import Tooltip from 'appComponents/Tooltip'
import withState from 'util/withState'
import { injectIntl, intlShape } from 'util/react-intl'
import messages from '../../messages'
import { API_CALL_CLEAN, apiCall3 } from 'actions'
import { EntitySchemas } from 'schemas'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { getSearchOrganizationValues } from './selectors'
import { change, isDirty } from 'redux-form/immutable'
import {
  setConnectionsMainEntity,
  setConnectionsView
} from 'reducers/selections'
import { makeCurrentFormStateInitial } from 'Routes/Connections/actions'
import { addItemToWorkBench, resetConnectionsWorkbench } from 'Routes/Connections/components/WorkbenchActions/actions'
import { mergeInUIState } from 'reducers/ui'
import { Map, List } from 'immutable'
import { formTypesEnum } from 'enums'

const ConnectionsRoute = ({
  intl: { formatMessage },
  isPreview,
  activeConnectionsTabIndex,
  updateUI,
  dispatch,
  searchValues,
  mergeInUIState,
  reset,
  isDirty
}) => {
  const connectionsPageClass = cx(
    styles.form,
    styles.connectionPage
  )
  const switchToOrganizationAction = (index) => {
    updateUI('activeConnectionsTabIndex', index)
    dispatch(setConnectionsView(false))
    dispatch(change('connectionsWorkbench', 'connections', List()))
    dispatch({
      type: API_CALL_CLEAN,
      keys: ['connections', 'serviceSearch']
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
          dispatch(setConnectionsMainEntity('services'))
          addItemToWorkBench(0, dispatch)
          dispatch(makeCurrentFormStateInitial())
        }
      })
    )
  }
  const handleTabOnChange = index => {
    mergeInUIState({
      key: 'uiData',
      value: {
        sorting: Map(),
        currentPage: 0,
        showAll: false
      }
    })
    if (index) {
      if (isDirty) {
        mergeInUIState({
          key: 'navigationDialog',
          value: {
            isOpen: true,
            onlyDefaultAction: true,
            defaultAction: () => switchToOrganizationAction(index)
          }
        })
      } else {
        switchToOrganizationAction(index)
      }
    }
    if (!index) {
      updateUI('activeConnectionsTabIndex', index)
      reset()
    }
  }

  return (
    <div className={connectionsPageClass}>
      <div className={styles.connectionsPageLabel}>
        <Label labelText={formatMessage(messages.pageTitle)} labelPosition='top'>
          <Tooltip tooltip={formatMessage(messages.pageTooltip)} />
        </Label>
      </div>
      <div>
        <Tabs index={activeConnectionsTabIndex} onChange={handleTabOnChange} className={styles.connectionTabs}>
          <Tab label={formatMessage(messages.editConnectionsTabName)}>
            <div className='row'>
              {!isPreview &&
                <div className='col-lg-8'>
                  <SearchArea />
                </div>}
              <div className={!isPreview ? 'col-lg-16' : 'col-lg-24'}>
                <Workbench />
              </div>
            </div>
          </Tab>
          <Tab label={formatMessage(messages.organizationConnectionsTabName)}>
            <div className={'col-lg-24'}>
              <Workbench isOrganizationPreview />
            </div>
          </Tab>
        </Tabs>
      </div>
    </div>
  )
}

ConnectionsRoute.propTypes = {
  intl: intlShape,
  isPreview: PropTypes.bool,
  activeConnectionsTabIndex: PropTypes.number,
  updateUI: PropTypes.func,
  dispatch: PropTypes.func,
  mergeInUIState: PropTypes.func,
  reset: PropTypes.func,
  searchValues: PropTypes.any,
  isDirty: PropTypes.bool
}

export default compose(
  injectIntl,
  withFormStates,
  connect(state => ({
    isPreview: getIsConnectionsPreview(state),
    searchValues: getSearchOrganizationValues(state),
    isDirty: isDirty(formTypesEnum.CONNECTIONSWORKBENCH)(state)
  }), {
    mergeInUIState,
    reset: resetConnectionsWorkbench
  }),
  withPreviewDialog,
  withState({
    redux: true,
    key: 'connectionsTabIndex',
    initialState: {
      activeConnectionsTabIndex: 0
    }
  })
)(ConnectionsRoute)
