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
  getSelectedASTIConnectionsCount,
  getSelectedFlatASTIConnectionsCount
} from './selectors'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { entityTypesEnum } from 'enums'
import cx from 'classnames'
import styles from './styles.scss'

const messages = defineMessages({
  services: {
    id: 'withConnectionStep.channel.title',
    defaultMessage: 'Liitetyt palvelut ({connectionCount})'
  },
  generalDescriptions: {
    id: 'withConnectionStep.gd.title',
    defaultMessage: 'Ehdotetut asiointikanavat ({connectionCount})'
  },
  channels: {
    id: 'withConnectionStep.service.title',
    defaultMessage: 'Liitetyt asiontikanavat ({connectionCount})'
  },
  serviceCollections: {
    id: 'withConnectionStep.serviceCollection.title',
    defaultMessage: 'Liitetyt palvelut ({connectionCount})'
  },
  gdTooltip: {
    id: 'withConnectionStep.gd.tooltip',
    defaultMessage: 'Suggested channels tooltip'
  },
  serviceTooltip: {
    id: 'Connection.Step.Service.Tooltip',
    defaultMessage: 'Suggested channels tooltip'
  },
  serviceChannelsTooltip: {
    id: 'Connection.Step.ServiceChannel.Tooltip',
    defaultMessage: 'Suggested services tooltip'
  },
  serviceCollectionsTooltip: {
    id: 'Connection.Step.ServiceCollections.Tooltip',
    defaultMessage: 'Suggested services tooltip'
  },
  servicesASTI: {
    id: 'withConnectionStep.channelASTI.title',
    defaultMessage: 'ASTI-järjestelmässä liitetyt palvelupisteet ({connectionCount})'
  },
  channelsASTI: {
    id: 'withConnectionStep.serviceASTI.title',
    defaultMessage: 'ASTI-järjestelmässä liitetyt palvelut ({connectionCount})'
  },
  servicesASTIHelp: {
    id: 'withConnectionStep.channelASTIHelp.text',
    defaultMessage: 'Asiointipisteiden sopimusjärjestelmässä (ASTI) on tähän palveluun liitetty seuraavat palvelupisteet. Näitä liitoksia ei voi poistaa Palvelutietovarannossa.'
  },
  channelsASTIHelp: {
    id: 'withConnectionStep.serviceASTIHelp.text',
    defaultMessage: 'Asiointipisteiden sopimusjärjestelmässä (ASTI) on tähän palvelupisteeseen liitetty seuraavat palvelut. Näitä liitoksia ei voi poistaa Palvelutietovarannossa.'
  }
})

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
      inTranslation,
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
              <ReduxAccordion reduxKey={inTranslation ? 'translationAccordionConnection' : 'connectionAccordion'}>
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
               <ReduxAccordion activeIndex={-1} reduxKey={inTranslation ? 'translationAccordionConnectionASTI' : 'astiConnectionAccordion'}>
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
    ASTIConnectionCount: PropTypes.number,
    inTranslation: PropTypes.bool
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
        entityConnectionCount: getEntityConnectionCount(state),
        entityType,
        ASTIConnectionCount
      }
    }),
    withFormStates
  )(InnerComponent)
}

export default withConnectionStep
