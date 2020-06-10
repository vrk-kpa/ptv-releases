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
import React, { PureComponent, Fragment } from 'react'
import { compose } from 'redux'
import { connect } from 'react-redux'
import PropTypes from 'prop-types'
import ImmutablePropTypes from 'react-immutable-proptypes'
import cx from 'classnames'
import styles from './styles.scss'
import {
  getMainEntityFormConnections,
  getMainEntityFormAstiConnections
} from 'Routes/Connections/selectors'
import { isDirty } from 'redux-form/immutable'
import { getConnectionsMainEntity } from 'selectors/selections'
import EntityDescription from 'appComponents/EntityDescription'
import LanguageBarCell from 'appComponents/Cells/LanguageBarCell'
import ModifiedAtCell from 'appComponents/Cells/ModifiedAtCell'
import ModifiedByCell from 'appComponents/Cells/ModifiedByCell'
import ChannelTypeCell from 'appComponents/Cells/ChannelTypeCell'
import ServiceTypeCell from 'appComponents/Cells/ServiceTypeCell'
import ConnectionDescriptionCell from 'appComponents/ConnectionDescriptionCell'
import PlaceholderLabel from 'appComponents/PlaceholderLabel'
import { Accordion } from 'appComponents/Accordion'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import LanguageProvider from 'appComponents/LanguageProvider'
import { EditIcon } from 'appComponents/Icons'
import { mergeInUIState } from 'reducers/ui'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import ConnectionsSubHeader from '../ConnectionsSubHeader'
import { getSortingOrder } from 'Routes/Connections/components/Childs/selectors'
import { setContentLanguageForPreviewConection } from 'Routes/Connections/actions'
import OrganizationCell from 'appComponents/Cells/OrganizationCell'
import {
  entityTypesEnum,
  securityOrganizationCheckTypes,
  permisionTypes
} from 'enums'
import { getConnectionTypeCommonForAllId } from 'appComponents/ChannelConnectionType/selectors'

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
  },
  withoutConnectedservices: {
    id: 'PreviewConnections.WithoutConnectedServices.Title',
    defaultMessage: 'No services have been connected to the channel yet'
  },
  withoutConnectedchannels: {
    id: 'PreviewConnections.WithoutConnectedChannels.Title',
    defaultMessage: 'No channels have been connected to the service yet'
  }
})

class PreviewConnection extends PureComponent {
  render () {
    const {
      mainEntity,
      parent,
      parentIndex,
      children,
      astiChildren,
      intl: { formatMessage },
      mergeInUIState,
      isDirty,
      sortingOrder,
      astiSortingOrder,
      commonTypeId,
      setContentLanguageForPreviewConection
    } = this.props

    if (!mainEntity) return null

    const onEditClick = (parentIndex, childIndex, isAsti, child, parent) => {
      const security = {
        entityId: child.get('id'),
        domain: mainEntity === 'services' ? entityTypesEnum.CHANNELS : entityTypesEnum.SERVICES,
        organizationId: child.get('organizationId'),
        checkOrganization: child.get('connectionType') === commonTypeId ? securityOrganizationCheckTypes.ownOrganization : securityOrganizationCheckTypes.byOrganization,
        permisionTypes: permisionTypes.update
      }
      mergeInUIState({
        key: 'editConnectionDialog',
        value: {
          isOpen: true,
          parentIndex: parentIndex,
          childIndex: childIndex,
          isAsti: isAsti,
          isEdit: false,
          security: security
        }
      })
      setContentLanguageForPreviewConection(parent.get('languagesAvailabilities'), child.get('languagesAvailabilities'))
    }

    const renderChild = (child, mainEntity, parent, parentIndex, index, isAsti) => {
      const previewRowStyle = cx(
        styles.previewRow,
        styles.subEntity
      )
      return (
        <div className={previewRowStyle} key={child.get('id')}>
          <div className='row align-items-center'>
            <div className='col-lg-2 align-self-start'>
              <LanguageBarCell
                showMissing
                id={parent.get('id')}
                type={mainEntity}
                languagesAvailabilities={child.get('languagesAvailabilities').toJS()} />
            </div>
            <div className='col-lg-6 align-self-start'>
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
            <div className='col-lg-4 align-self-start'>
              {mainEntity === 'services'
                ? <ChannelTypeCell
                  channelTypeId={child.get('channelTypeId')}
                />
                : <ServiceTypeCell
                  serviceTypeId={child.get('serviceType')}
                />}
            </div>
            <div className='col-lg-4 align-self-start'>
              <ModifiedAtCell modifiedAt={child.get('modified')} inline />
              <ModifiedByCell modifiedBy={child.get('modifiedBy')} compact />
            </div>
            <div className='col-lg-8 align-self-start'>
              <div className='d-flex justify-content-between'>
                <LanguageProvider languageKey='connectionPreview' >
                  <ConnectionDescriptionCell
                    connection={child}
                    parentIndex={parentIndex}
                    index={index}
                    isAsti={isAsti}
                  />
                </LanguageProvider>
                <EditIcon
                  onClick={() => onEditClick(parentIndex, index, isAsti, child, parent)}
                  disabled={isDirty}
                />
              </div>
            </div>
          </div>
        </div>
      )
    }
    const sortedChildren = children.map((child, index) => ({ child, index, sort: sortingOrder.indexOf(index) }))
      .sort((a, b) => a.sort < b.sort ? -1 : a.sort > b.sort ? 1 : 0)
    const astiSortedChildren = astiChildren.map((child, index) =>
      ({ child, index, sort: astiSortingOrder.indexOf(index) }))
      .sort((a, b) => a.sort < b.sort ? -1 : a.sort > b.sort ? 1 : 0)

    const connectedEntity = {
      [entityTypesEnum.SERVICES]: entityTypesEnum.CHANNELS,
      [entityTypesEnum.CHANNELS]: entityTypesEnum.SERVICES
    }[mainEntity]

    return (
      <div className={styles.mainEntityWrap}>
        <div className={styles.mainEntity}>
          <div className={styles.previewRow}>
            <div className='row align-items-center'>
              <div className='col-lg-2'>
                <LanguageBarCell
                  languagesAvailabilities={parent.get('languagesAvailabilities').toJS()} />
              </div>
              <div className='col-lg-7'>
                <EntityDescription
                  name={parent.get('name')}
                  entityId={parent.get('id')}
                  entityConcreteType={mainEntity === 'services'
                    ? 'service'
                    : parent.get('channelType')}
                  preview={false}
                />
              </div>
              <div className='col-lg-4'>
                {mainEntity === 'services'
                  ? <ServiceTypeCell
                    serviceTypeId={parent.get('serviceType')}
                  />
                  : <ChannelTypeCell
                    channelTypeId={parent.get('channelTypeId')}
                  />}
              </div>
              <div className='col-lg-11'>
                <OrganizationCell organizationId={parent.get('organizationId')} />
              </div>
            </div>
          </div>
          {children.size > 0 &&
            <div className='row align-items-center'>
              <div className='col-lg-22 offset-lg-2'>
                <Accordion light>
                  <Accordion.Title
                    className={children.size === 0 ? styles.disabled : ''}
                    validate={false}
                    title={formatMessage(
                      messages[mainEntity],
                      { connectionCount: children.size }
                    )}
                  />
                  <Accordion.Content>
                    <Fragment>
                      <ConnectionsSubHeader connectionIndex={parentIndex} className={styles.subHeaderPreview} />
                      <div className={styles.subEntityWrap}>
                        {sortedChildren.map(({ child, index }) =>
                          renderChild(child, mainEntity, parent, parentIndex, index))}
                      </div>
                    </Fragment>
                  </Accordion.Content>
                </Accordion>
              </div>
            </div> || <PlaceholderLabel>
              {formatMessage(messages[`withoutConnected${connectedEntity}`])}
            </PlaceholderLabel>
          }
          {astiChildren.size > 0 &&
            <div>
              <div className='row align-items-center'>
                <div className='col-lg-22 offset-lg-2'>
                  <Accordion light>
                    <Accordion.Title
                      className={astiChildren.size === 0 ? styles.disabled : ''}
                      validate={false}
                      title={formatMessage(
                        messages[`${mainEntity}ASTI`],
                        { connectionCount: astiChildren.size }
                      )}
                    />
                    <Accordion.Content>
                      <Fragment>
                        <ConnectionsSubHeader
                          connectionIndex={parentIndex}
                          className={styles.subHeaderPreview}
                          isAsti
                        />
                        <div className={styles.subEntityWrap}>
                          {astiSortedChildren.map(({ child, index }) =>
                            renderChild(child, mainEntity, parent, parentIndex, index, true))}
                        </div>
                      </Fragment>
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
  children: PropTypes.any,
  astiChildren: PropTypes.any,
  intl: intlShape,
  parentIndex: PropTypes.number,
  mergeInUIState: PropTypes.func,
  isDirty: PropTypes.bool.isRequired,
  sortingOrder: ImmutablePropTypes.list,
  astiSortingOrder: ImmutablePropTypes.list,
  commonTypeId: PropTypes.string,
  setContentLanguageForPreviewConection: PropTypes.func
}

export default compose(
  injectIntl,
  injectFormName,
  connect((state, ownProps) => {
    const children = getMainEntityFormConnections(state, ownProps)
    const astiChildren = getMainEntityFormAstiConnections(state, ownProps)
    return {
      mainEntity: getConnectionsMainEntity(state, ownProps),
      children,
      astiChildren,
      isDirty: isDirty(ownProps.formName)(state),
      sortingOrder: getSortingOrder(state, {
        data: children,
        connectionIndex: ownProps.parentIndex
      }),
      astiSortingOrder: getSortingOrder(state, {
        data: astiChildren,
        connectionIndex: ownProps.parentIndex,
        isAsti: true
      }),
      commonTypeId: getConnectionTypeCommonForAllId(state)
    }
  }, {
    mergeInUIState,
    setContentLanguageForPreviewConection
  })
)(PreviewConnection)
