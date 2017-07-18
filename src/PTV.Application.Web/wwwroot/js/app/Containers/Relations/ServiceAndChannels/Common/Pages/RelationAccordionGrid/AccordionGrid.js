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
import React, { Component, PropTypes } from 'react'
import { connect } from 'react-redux'
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl'
import mapDispatchToProps from '../../../../../../Configuration/MapDispatchToProps'
import { camelize } from 'humps'

// componnets
import ServiceAccordion from './ServiceAccordion'
import { PTVPreloader, PTVOverlay } from '../../../../../../Components'
import ServiceContainer from '../../../../../Services/Service/Pages/ServiceContainer'
import ElectronicContainer from '../../../../../Channels/Electronic/Pages'
import PhoneContainer from '../../../../../Channels/Phone/Pages'
import PrintableFormContainer from '../../../../../Channels/PrintableForm/Pages'
import ServiceLocationContainer from '../../../../../Channels/ServiceLocation/Pages'
import WebPageContainer from '../../../../../Channels/WebPage/Pages'

// styles
import * as overlayStyles from '../OverlayStyles'

// actions
import * as commonServiceAndChannelActions from '../../../Common/Actions'

// selectors
import * as CommonServiceAndChannelSelectors from '../../Selectors'
import * as CommonSelectors from '../../../../../Common/Selectors'

// messages
import { detailMessages as detailServiceMessages } from '../../../ServiceSearch/Messages'
import { detailMessages as detailChannelMessages } from '../../../ChannelSearch/Messages'

// types
import { channelTypes } from './../../../../../Common/Enums'

export const AccordionGrid = (props) => {
  const { connectedServicesUiIds, componentClass, message, readOnly, isFetching, language,
    intl: { formatMessage }, detailService, detailChannel, keyToState, actions, channelCode } = props

  const handleDetailIconClick = (serviceId, keyToState) => {
    actions.setRelationsDetail(serviceId, keyToState, 'service')
  }

  const onServiceDetailClose = (keyToState) => {
    actions.setRelationsDetail(null, keyToState, 'service')
  }

  const onChannelDetailClose = (channelTypeCode) => {
    actions.setRelationsDetail(null, 'serviceAndChannelChannelSearch', camelize(channelTypeCode))
  }

  const renderAccordion = (id) => (
    <ServiceAccordion
      key={id}
      uiId={id}
      isExpanded
      componentClass='service-accordion'
      readOnly={readOnly}
      language={language}
      onDetailClick={handleDetailIconClick}
    />
  )

  const renderChannelSimpleView = (channelType) => {
    switch (camelize(channelType)) {
      case channelTypes.ELECTRONIC: return <ElectronicContainer simpleView />
      case channelTypes.PHONE: return <PhoneContainer simpleView />
      case channelTypes.PRINTABLE_FORM: return <PrintableFormContainer simpleView />
      case channelTypes.SERVICE_LOCATION: return <ServiceLocationContainer simpleView />
      case channelTypes.WEB_PAGE: return <WebPageContainer simpleView />
      default:return null
    }
  }

  return (
    <div>
      <div className='flex-container'>
        <div className='w80'>
          <FormattedMessage
            id='Containers.ServiceAndChannelRelations.Grid.Header.PublishingStatus'
            defaultMessage='Tila'
          />
        </div>
        <div className='w120'>
          <FormattedMessage
            id='Containers.ServiceAndChannelRelations.Grid.Header.Name'
            defaultMessage='Nimi'
          />
        </div>
      </div>
      <div className={componentClass}>
        { isFetching
        ? <PTVPreloader />
        : connectedServicesUiIds.map((id) =>
            renderAccordion(id)
          ).toArray()
        }
      </div>

      <PTVOverlay
        title={formatMessage(detailServiceMessages.serviceDetailTitle)}
        dialogStyles={overlayStyles.detailDialogStyles}
        overlayStyles={overlayStyles.detailOverlayStyles}
        contentStyles={overlayStyles.detailContentStyles}
        isVisible={detailService.size !== 0}
        onCloseClicked={() => onServiceDetailClose('serviceAndChannelServiceSearch')}
      >
        { detailService.size !== 0 && <ServiceContainer simpleView /> }
      </PTVOverlay>

      <PTVOverlay
        title={formatMessage(detailChannelMessages.channelDetailTitle)}
        dialogStyles={overlayStyles.detailDialogStyles}
        overlayStyles={overlayStyles.detailOverlayStyles}
        contentStyles={overlayStyles.detailContentStyles}
        isVisible={detailChannel.size !== 0}
        onCloseClicked={() => onChannelDetailClose(channelCode)}
      >
        {detailChannel.size !== 0 && renderChannelSimpleView(channelCode)}
      </PTVOverlay>

    </div>
  )
}

AccordionGrid.propTypes = {
}

AccordionGrid.defaultProps = {
}

function mapStateToProps (state, ownProps) {
  const detailLanguage = CommonServiceAndChannelSelectors.getLanguageToCodeForServiceAndChannel(state, { keyToState: 'serviceAndChannel' }) // CommonSelectors.getSearchResultsLanguage(state, ownProps)
  const serviceProps = { keyToState: 'serviceAndChannelServiceSearch', keyToEntities: 'services' }
  const detailService = CommonServiceAndChannelSelectors.getDetailService(state, { ...serviceProps, language: detailLanguage })
  const channelProps = { keyToState: 'serviceAndChannelChannelSearch', keyToEntities: 'channels' }
  const detailChannel = CommonServiceAndChannelSelectors.getDetailChannel(state, { ...channelProps, language: detailLanguage })
  const channelType = CommonSelectors.getChannelType(state, { id: detailChannel.get('typeId') })
  return {
    connectedServicesUiIds: CommonServiceAndChannelSelectors.getRelationConnectedServicesUiIds(state, ownProps),
    isFetching: CommonSelectors.getStepCommonIsFetching(state, ownProps),
    detailService,
    detailChannel,
    channelCode: channelType && channelType.get('code')
  }
}

const actions = [
  commonServiceAndChannelActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(AccordionGrid))
