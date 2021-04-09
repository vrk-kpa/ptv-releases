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
import { ReduxAccordion } from 'appComponents/Accordion'
import LanguageProvider from 'appComponents/LanguageProvider'
import ConnectionsStep from 'appComponents/ConnectionsStep'
import ASTIConnections from 'appComponents/ConnectionsStep/ASTIConnectionsForm'
import {
  getSelectedEntityId,
  getSelectedEntityType
} from 'selectors/entities/entities'
import {
  getEntityConnectionCount,
  getServiceCollectionConnectionCount,
  getSelectedASTIConnectionsCount,
  getSelectedFlatASTIConnectionsCount
} from './selectors'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { injectIntl, intlShape } from 'util/react-intl'
import { entityTypesEnum, formTypesEnum } from 'enums'
import cx from 'classnames'
import styles from './styles.scss'
import { messages } from './messages'

const tooltips = {
  [entityTypesEnum.GENERALDESCRIPTIONS]: messages.gdTooltip,
  [entityTypesEnum.SERVICES]: messages.serviceTooltip,
  [entityTypesEnum.SERVICECOLLECTIONS]: messages.serviceCollectionsTooltip,
  [entityTypesEnum.CHANNELS]: messages.serviceChannelsTooltip
}

const withConnectionStep = WrappedComponent => {
  const InnerComponent = props => {
    const {
      entityId,
      entityConnectionCount,
      ASTIConnectionCount,
      intl: { formatMessage },
      entityType,
      ...rest
    } = props
    // When we are on service we are searching channels and the other way around //
    const shouldShowConnections = entityId && entityType
    const ASTIClass = cx(
      styles.form,
      {
        [styles.hidden]: !ASTIConnectionCount
      }
    )

    return (
      <div>
        <WrappedComponent {...rest} />
        {shouldShowConnections &&
          <LanguageProvider languageKey='connections'>
            <div className={styles.form}>
              <ReduxAccordion reduxKey={'connectionAccordion'} isExclusive={false} formName={formTypesEnum.CONNECTIONS}>
                <ReduxAccordion.Title
                  title={formatMessage(messages[entityType], { connectionCount: entityConnectionCount || '0' })}
                  tooltip={tooltips[entityType] && formatMessage(tooltips[entityType])} />
                <ReduxAccordion.Content>
                  <ConnectionsStep searchMode={entityType} notificationForm={props.formName} />
                </ReduxAccordion.Content>
              </ReduxAccordion>
            </div>
            {entityType !== entityTypesEnum.GENERALDESCRIPTIONS &&
            entityType !== entityTypesEnum.SERVICECOLLECTIONS &&
            <div className={ASTIClass}>
              <ReduxAccordion
                activeIndex={[]}
                reduxKey={'astiConnectionAccordion'}
                isExclusive={false}
                formName={formTypesEnum.ASTICONNECTIONS}
              >
                <ReduxAccordion.Title
                  title={formatMessage(messages[`${entityType}ASTI`], { connectionCount: ASTIConnectionCount || '0' })}
                  helpText={formatMessage(messages[`${entityType}ASTIHelp`])}
                />
                <ReduxAccordion.Content>
                  <ASTIConnections notificationForm={props.formName} />
                </ReduxAccordion.Content>
              </ReduxAccordion>
            </div>
            }
          </LanguageProvider>
        }
      </div>
    )
  }

  InnerComponent.propTypes = {
    title: PropTypes.string,
    searchMode: PropTypes.string,
    entityId: PropTypes.string,
    entityType: PropTypes.string,
    entityConnectionCount: PropTypes.number,
    formName: PropTypes.string,
    intl: intlShape,
    ASTIConnectionCount: PropTypes.number
  }
  return compose(
    injectFormName,
    injectIntl,
    connect((state, { formName }) => {
      const entityType = getSelectedEntityType(state)
      const ASTIConnectionCount = {
        [entityTypesEnum.CHANNELS]: getSelectedASTIConnectionsCount(state),
        [entityTypesEnum.SERVICES]: getSelectedFlatASTIConnectionsCount(state)
      }[entityType]
      return {
        entityId: getSelectedEntityId(state),
        entityConnectionCount: entityType === entityTypesEnum.SERVICECOLLECTIONS
          ? getServiceCollectionConnectionCount(state)
          : getEntityConnectionCount(state),
        entityType,
        ASTIConnectionCount
      }
    }),
    withFormStates
  )(InnerComponent)
}

export default withConnectionStep
