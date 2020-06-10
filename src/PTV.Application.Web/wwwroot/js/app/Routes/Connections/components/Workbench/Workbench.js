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
import PropTypes from 'prop-types'
import { EntitySchemas } from 'schemas'
import Connections from 'Routes/Connections/components/Connections'
import ConnectionsHeader from 'Routes/Connections/components/ConnectionsHeader'
import WorkbenchActions from 'Routes/Connections/components/WorkbenchActions'
import ConnectionsPreview from 'Routes/Connections/components/ConnectionsPreview'
import { reduxForm, initialize, change } from 'redux-form/immutable'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { fromJS, List } from 'immutable'
import {
  getConnectionsEntity,
  getConnectionsMainEntity,
  getIsConnectionsPreview
} from 'selectors/selections'
import {
  getMainEntityConnections,
  getMainEntity,
  getConnectionsWorkbenchInitialValues,
  getPresetConnectionsEntityInfo,
  getConnectionServiceSearchAbort,
  getIsWorkbenchLoading
} from 'Routes/Connections/selectors'
import WorkbenchPlaceholder from './WorkbenchPlaceholder'
import Placeholder from 'appComponents/Placeholder'
import PlaceholderLabel from 'appComponents/PlaceholderLabel'
import ConnectionsTitle from '../ConnectionsHeader/ConnectionsTitle'
import { Spinner } from 'sema-ui-components'
import { schema } from 'normalizr'
import { formTypesEnum } from 'enums'
import { resetConnectionReadOnly, setReadOnly } from 'reducers/formStates'
import { handleOnSubmit } from 'util/redux-form/util'
import { StopSearchButton } from 'appComponents/Buttons'
import {
  mainEntityTransformer,
  childsTransformer,
  astiChildsTransformer,
  connectionsTransformer,
  phoneNumbersTransformer,
  faxNumbersTransformer,
  collectionsTransformer,
  openingHoursTransformer
} from './transformers'
import cx from 'classnames'
import styles from './styles.scss'
import messages from 'Routes/Connections/messages'
import { injectIntl } from 'util/react-intl'
import { API_CALL_CLEAN } from 'actions'
import {
  toggleConnectionOrdering,
  makeCurrentFormStateInitial,
  clearAfterStopSearch,
  removeSorting
} from 'Routes/Connections/actions'
import { withRouter } from 'react-router'
import withEditConnectionDialog from 'util/redux-form/HOC/withEditConnectionDialog'
import OrganizationConnections from 'Routes/Connections/components/OrganizationConnections'
import { mergeInUIState } from 'reducers/ui'
import {
  setConnectionsMainEntity
} from 'reducers/selections'
import withState from 'util/withState'

class Workbench extends PureComponent {
  componentWillUnmount () {
    this.props.clear()
  }
  onStop = e => {
    e.preventDefault()
    this.props.abort && this.props.abort()
    this.props.clearAfterStopSearch()
  }
  render () {
    const {
      selectedEntity,
      mainEntity,
      isPreview,
      isOrganizationPreview,
      isLoading
    } = this.props
    const placeholderClass = cx(
      styles.placeholder,
      {
        [styles.divided]: selectedEntity
      }
    )
    if (isPreview) {
      return <ConnectionsPreview />
    }
    if (isOrganizationPreview) {
      return <OrganizationConnections />
    }
    if (!selectedEntity) {
      return <div className={styles.placeholderWrap}>
        <div className={placeholderClass}>
          <div className={styles.title}>
            <ConnectionsTitle />
          </div>
          <div className={styles.content}>
            <WorkbenchPlaceholder />
          </div>
        </div>
      </div>
    }
    if (!mainEntity) {
      return <div className={styles.placeholderWrap}>
        <div className={placeholderClass}>
          <WorkbenchActions />
          <div className={styles.title}>
            <ConnectionsTitle />
          </div>
          <div className={styles.content}>
            <WorkbenchPlaceholder />
          </div>
        </div>
      </div>
    }
    return (
      <div className={styles.workbench}>
        <WorkbenchActions />
        <div className={styles.headerWrap}>
          <ConnectionsHeader />
        </div>
        {isLoading &&
          <Placeholder>
            <Spinner />
            <PlaceholderLabel placeholder={messages.loadingWorkbenchData} />
            <StopSearchButton
              onClick={this.onStop}
              disabled={!isLoading}
            />
          </Placeholder> ||
          <Connections />
        }
      </div>
    )
  }
}
Workbench.propTypes = {
  selectedEntity: PropTypes.oneOf(['channels', 'services']),
  mainEntity: PropTypes.oneOf(['channels', 'services']),
  isPreview: PropTypes.bool,
  isOrganizationPreview: PropTypes.any,
  clear: PropTypes.func.isRequired,
  isLoading: PropTypes.bool,
  abort: PropTypes.func,
  clearAfterStopSearch: PropTypes.func
}

const connectionsSchema = new schema.Object({
  services: EntitySchemas.SERVICE_ARRAY,
  channels: EntitySchemas.CHANNEL_ARRAY
})
const onSubmit = handleOnSubmit({
  url: 'connections/saveRelations',
  schema: connectionsSchema,
  transformers: [
    mainEntityTransformer,
    childsTransformer,
    connectionsTransformer,
    astiChildsTransformer,
    collectionsTransformer,
    phoneNumbersTransformer,
    faxNumbersTransformer,
    openingHoursTransformer
  ]
})

const onSubmitSuccess = (values, dispatch, ownProps) => {
  dispatch(({ getState, dispatch }) => {
    const state = getState()
    const connections = values[ownProps.mainEntity].map(service => {
      const childs = getMainEntityConnections(state, { id: service.id })
      const groupedChilds = childs.groupBy(child =>
        child.getIn(['astiDetails', 'isASTIConnection'])
          ? 'asti'
          : 'nonAsti'
      )
      return {
        mainEntity: getMainEntity(state, { id: service.id }),
        childs: groupedChilds.get('nonAsti') || [],
        astiChilds: groupedChilds.get('asti') || []
      }
    })
    const formValues = fromJS({
      connections,
      mainEntityType: ownProps.mainEntity
    })
    dispatch(initialize('connectionsWorkbench', formValues, false))
    dispatch(resetConnectionReadOnly({ form: formTypesEnum.CONNECTIONS }))
    dispatch({
      type: API_CALL_CLEAN,
      keys: ['connectionHistory']
    })
    dispatch(mergeInUIState({
      key: 'editConnectionDialog',
      value: {
        isEdit: false
      }
    }))
    dispatch(toggleConnectionOrdering(null))
    dispatch(setReadOnly({
      form: formTypesEnum.CONNECTIONSWORKBENCH,
      value: true
    }))
    ownProps.removeSorting && ownProps.removeSorting('sub')
  })
}

export default compose(
  injectIntl,
  withRouter,
  withState({
    redux: true,
    key: 'workbench',
    keepImmutable: true
  }),
  connect((state, { location }) => {
    const entityInfo = getPresetConnectionsEntityInfo(state, { location })
    return {
      selectedEntity: getConnectionsEntity(state),
      mainEntity: getConnectionsMainEntity(state),
      isPreview: getIsConnectionsPreview(state),
      initialValues: getConnectionsWorkbenchInitialValues(state, { ...entityInfo }),
      abort: getConnectionServiceSearchAbort(state),
      isLoading: getIsWorkbenchLoading(state)
    }
  }, {
    clear: () => ({ dispatch }) => {
      dispatch(mergeInUIState({
        key: 'activeConnectionUiData',
        value: {
          activeIndex: null
        }
      }))
      if (location.pathname !== '/frontpage/connections') {
        [
          change('connectionsWorkbench', 'connections', List()),
          setConnectionsMainEntity(null),
          makeCurrentFormStateInitial()
        ].forEach(dispatch)
      }
    },
    clearAfterStopSearch,
    removeSorting
  }),
  reduxForm({
    form: 'connectionsWorkbench',
    onSubmit,
    onSubmitSuccess,
    destroyOnUnmount: false,
    enableReinitialize: true
  }),
  withEditConnectionDialog({
    reduxKey: 'editConnectionDialog'
  })
)(Workbench)
