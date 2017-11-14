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
import { Accordion } from 'appComponents/Accordion'
import { ConnectionsStep } from 'appComponents'
import {
  getSelectedEntityId,
  getSelectedEntityType
} from 'selectors/entities/entities'
import { getEntityConnectionCount } from './selectors'
import { injectFormName } from 'util/redux-form/HOC'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import { entityTypesEnum } from 'enums'
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
  gdTooltip: {
    id: 'withConnectionStep.gd.tooltip',
    defaultMessage: 'Suggested channels tooltip'
  }
})

const withConnectionStep = WrappedComponent => {
  const InnerComponent = props => {
    const {
      entityId,
      entityConnectionCount,
      intl: { formatMessage },
      entityType,
      ...rest
    } = props
    // When we are on service we are searching channels and the other way around //
    const shouldShowConnections = entityId && entityType
    return (
      <div>
        <WrappedComponent {...rest} />
        {shouldShowConnections &&
          <div className={styles.form}>
            <Accordion>
              <Accordion.Title
                title={formatMessage(messages[entityType], { connectionCount: entityConnectionCount || '0' })}
                tooltip={entityType === entityTypesEnum.GENERALDESCRIPTIONS && formatMessage(messages.gdTooltip)} />
              <Accordion.Content>
                <ConnectionsStep searchMode={entityType} notificationForm={props.formName} />
              </Accordion.Content>
            </Accordion>
          </div>
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
    intl: intlShape
  }
  return compose(
    injectFormName,
    injectIntl,
    connect((state, { formName }) => ({
      entityId: getSelectedEntityId(state),
      entityType: getSelectedEntityType(state),
      entityConnectionCount: getEntityConnectionCount(state)
    }))
  )(InnerComponent)
}

export default withConnectionStep
