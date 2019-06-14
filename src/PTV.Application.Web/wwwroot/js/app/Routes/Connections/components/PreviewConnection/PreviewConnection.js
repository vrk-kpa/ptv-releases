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
import React, { PureComponent } from 'react'
import { compose } from 'redux'
import { connect } from 'react-redux'
import PropTypes from 'prop-types'
import styles from './styles.scss'
import {
  getMainEntityFormConnections,
  getMainEntity,
  getMainEntityFormAstiConnections
} from 'Routes/Connections/selectors'
import { getConnectionsMainEntity } from 'selectors/selections'
import EntityDescription from 'appComponents/EntityDescription'
import Spacer from 'appComponents/Spacer'
import LanguageBarCell from 'appComponents/Cells/LanguageBarCell'
import ModifiedAtCell from 'appComponents/Cells/ModifiedAtCell'
import ModifiedByCell from 'appComponents/Cells/ModifiedByCell'
import ChannelTypeCell from 'appComponents/Cells/ChannelTypeCell'
import ServiceTypeCell from 'appComponents/Cells/ServiceTypeCell'
import ConnectionDescriptionCell from 'appComponents/ConnectionDescriptionCell'
import { Accordion } from 'appComponents/Accordion'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'

const messages = defineMessages({
  services: {
    id: 'withConnectionStep.channel.title',
    defaultMessage: 'Liitetyt palvelut ({connectionCount})'
  },
  channels: {
    id: 'withConnectionStep.service.title',
    defaultMessage: 'Liitetyt asiontikanavat ({connectionCount})'
  },
  servicesASTI: {
    id: 'withConnectionStep.channelASTI.title',
    defaultMessage: 'ASTI-järjestelmässä liitetyt palvelupisteet ({connectionCount})'
  },
  channelsASTI: {
    id: 'withConnectionStep.serviceASTI.title',
    defaultMessage: 'ASTI-järjestelmässä liitetyt palvelut ({connectionCount})'
  }
})

class PreviewConnection extends PureComponent {
  render () {
    const {
      mainEntity,
      parent,
      childs,
      asti,
      intl: { formatMessage }
    } = this.props

    const renderChild = (child, mainEntity, parent) => {
      return (
        <div className={styles.subEntity} key={child.get('id')}>
          <Spacer marginSize='m0' />
          <div className={styles.previewRow}>
            <div className='row align-items-center'>
              <div className='col-lg-2'>
                <LanguageBarCell
                  showMissing
                  id={parent.get('id')}
                  type={mainEntity}
                  languagesAvailabilities={child.get('languagesAvailabilities').toJS()} />
              </div>
              <div className='col-lg-6'>
                <EntityDescription
                  name={child.get('name')}
                  entityId={child.get('id')}
                  organizationId={child.get('organizationId')}
                  entityConcreteType={mainEntity === 'services'
                    ? child.get('channelType')
                    : 'service'}
                  isNewConnection={child.get('isNew')}
                  preview={false}
                />
              </div>
              <div className='col-lg-4'>
                {mainEntity === 'services'
                  ? <ChannelTypeCell
                    channelTypeId={child.get('channelTypeId')}
                  />
                  : <ServiceTypeCell
                    serviceTypeId={child.get('serviceTypeId')}
                  />}
              </div>
              <div className='col-lg-4'>
                <ModifiedAtCell modifiedAt={child.get('isNew') ? null : child.get('modified')} inline />
                <ModifiedByCell modifiedBy={child.get('isNew') ? null : child.get('modifiedBy')} compact />
              </div>
              <div className='col-lg-8'>
                <ConnectionDescriptionCell
                  connection={child}
                />
              </div>
            </div>
          </div>
        </div>
      )
    }
    return (
      <div className={styles.mainEntityWrap}>
        <div className={styles.mainEntity}>
          <div className={styles.previewRow}>
            <div className='row align-items-center'>
              <div className='col-lg-2'>
                <LanguageBarCell
                  languagesAvailabilities={parent.get('languagesAvailabilities').toJS()} />
              </div>
              <div className='col-lg-6'>
                <EntityDescription
                  name={parent.get('name')}
                  entityId={parent.get('id')}
                  organizationId={parent.get('organizationId')}
                  entityConcreteType={mainEntity === 'services'
                    ? 'service'
                    : parent.get('channelType')}
                  preview={false}
                />
              </div>
              <div className='col-lg-6'>
                {mainEntity === 'services'
                  ? <ServiceTypeCell
                    serviceTypeId={parent.get('serviceTypeId')}
                  />
                  : <ChannelTypeCell
                    channelTypeId={parent.get('channelTypeId')}
                  />}
              </div>
            </div>
          </div>
          <Spacer marginSize='m0' />
          <div className='row align-items-center'>
            <div className='col-lg-24'>
              <Accordion light>
                <Accordion.Title
                  className={childs.size === 0 ? styles.disabled : ''}
                  validate={false}
                  title={formatMessage(
                    messages[mainEntity],
                    { connectionCount: childs.size }
                  )}
                />
                <Accordion.Content>
                  {childs.map(child => renderChild(child, mainEntity, parent))}
                </Accordion.Content>
              </Accordion>
            </div>
          </div>
          {asti.size > 0 &&
            <div>
              <Spacer marginSize='m0' />
              <div className='row align-items-center'>
                <div className='col-lg-24'>
                  <Accordion light>
                    <Accordion.Title
                      className={asti.size === 0 ? styles.disabled : ''}
                      validate={false}
                      title={formatMessage(
                        messages[`${mainEntity}ASTI`],
                        { connectionCount: asti.size }
                      )}
                    />
                    <Accordion.Content>
                      {asti.map(child => renderChild(child, mainEntity, parent))}
                    </Accordion.Content>
                  </Accordion>
                </div>
              </div>
            </div>}
        </div>
      </div>
    )
  }
}
PreviewConnection.propTypes = {
  mainEntity: PropTypes.any,
  parent: PropTypes.any,
  childs: PropTypes.any,
  asti: PropTypes.any,
  intl: intlShape
}

export default compose(
  injectIntl,
  connect((state, ownProps) => ({
    mainEntity: getConnectionsMainEntity(state, ownProps),
    parent: getMainEntity(state, ownProps),
    childs: getMainEntityFormConnections(state, ownProps),
    asti: getMainEntityFormAstiConnections(state, ownProps)
  }))
)(PreviewConnection)
