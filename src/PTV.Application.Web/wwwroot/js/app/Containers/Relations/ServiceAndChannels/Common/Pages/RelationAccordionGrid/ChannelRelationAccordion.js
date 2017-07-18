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
import mapDispatchToProps from '../../../../../../Configuration/MapDispatchToProps'
import { connect } from 'react-redux'
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl'

// components
import AdditionalInformation from '../../../../ServiceAndChannels/Common/Pages/RelationAccordionGrid/ChannelRelationAdditionalInformation'
import { PTVIcon, PTVLabel, PTVPreloader } from '../../../../../../Components'
import { LocalizedText } from '../../../../../Common/localizedData'
import { ButtonAdd } from '../../../../../../Containers/Common/Buttons'
import { ColumnFormatter } from '../../../../../Common/PublishStatusContainer'
import cx from 'classnames'
import TableNameFormatter from '../../../../../Common/tableNameFormatter'

// actions
import * as commonServiceAndChannelActions from '../../../Common/Actions'
import * as channelActions from '../../../ChannelSearch/Actions'
import * as serviceAndChannelActions from '../../../ServiceAndChannel/Actions'

// selectors
import * as CommonServiceAndChannelSelectors from '../../Selectors'
import * as CommonSelectors from '../../../../../Common/Selectors'

// Messages
import * as Messages from '../../../ServiceAndChannel/Messages'

export const ChannelRelationAccordion = (props) => {
  const { formatMessage } = props.intl
  const { id, actions,
          channelRelation, showingAdditional, infoExists, showingPublishAdditionalDetail,
          channelId, serviceId, serviceUiId, chargeType, description, chargeTypeAdditionalInformation, isNew,
          channelRootId, channelType, channelName, publishingStatusId, componentClass, chargeTypeName,
          relationDetailIsFetching, relationDetailAreDataValid, relationDetailIsLoaded,
          readOnly, language, channelCode, onDetailClick
        } = props

  const onToggleAdditional = (channelRelationId, value, input) => {
    if (!relationDetailIsLoaded && value) {
      actions.getRelationDetail(serviceId, channelRootId, channelRelationId, language)
    }

    actions.onChannelRelationInputChange(input, channelRelationId, value)
  }

  const onRemove = (serviceUiId, channelRelationId) => {
    actions.onConnectedChannelListChange(serviceUiId, channelRelationId)
       // update channel search
    if (!isNew) {
      actions.loadChannelSearchResults(false, language)
    }
  }

  const formatPublishingStatus = (cell, row) => {
    return <ColumnFormatter cell={cell} row={row} />
  }

  return (
    <div className={cx(componentClass, { 'highlighted': isNew })}>

      <div className={cx('accordion-subitem', { 'extra': !!readOnly })}>

        { !readOnly
                  ? <div className='flex-container flex-centered justified-between padded'>
                    <div className='w75'>
                      {formatPublishingStatus(publishingStatusId, null)}
                      <PTVIcon
                        onClick={() => onDetailClick(channelId, channelCode)}
                        name='icon-info-circle'
                      />
                    </div>
                    <div className='w200 ellipsis nowrap'>
                      <TableNameFormatter
                        content={channelName}
                        language={language}
                      />
                      <LocalizedText id={channelType.get('id')} name={channelType.get('name')} />
                    </div>
                    <div className='w180 flex-container flex-centered justified-end'>
                      <div>
                        <ButtonAdd
                          onClick={() => onToggleAdditional(id, !showingAdditional, 'showingAdditional')}
                          className='button-link left-aligned wrap-label'>
                          {!infoExists ? formatMessage(Messages.channelRelationAccordionMessages.addAdditionalInformation) : formatMessage(Messages.channelRelationAccordionMessages.editAdditionalInformation) }
                          <PTVIcon name={showingAdditional ? 'icon-angle-up' : 'icon-angle-down'} />
                        </ButtonAdd>
                      </div>
                      <div>
                        <PTVIcon
                          name={'icon-trash'}
                          onClick={() => onRemove(serviceUiId, id)}
                            />
                      </div>
                    </div>
                  </div>
              : <div>
                <div className='flex-container flex-centered padded channel-accordion-item'>
                  <div className='w80'>
                    {formatPublishingStatus(publishingStatusId, null)}
                    <PTVIcon
                      onClick={() => onDetailClick(channelId, channelCode)}
                      name='icon-info-circle'
                    />
                  </div>
                  <div className='full'>
                    <div className='row valign'>
                      <div className='col-lg-6 ellipsis nowrap'>
                        <TableNameFormatter
                          content={channelName}
                          language={language}
                        />
                        <LocalizedText id={channelType.get('id')} name={channelType.get('name')} />
                      </div>
                      <div className='col-lg-6'>
                        <ButtonAdd
                          onClick={() => onToggleAdditional(id, !showingPublishAdditionalDetail, 'showingPublishAdditionalDetail')}
                          className='button-link left-aligned wrap-label'>
                          { showingPublishAdditionalDetail ? formatMessage(Messages.channelRelationAccordionMessages.hidePublishAdditionalDetail) : formatMessage(Messages.channelRelationAccordionMessages.showPublishAdditionalDetail) }
                          <PTVIcon name={showingPublishAdditionalDetail ? 'icon-angle-up' : 'icon-angle-down'} />
                        </ButtonAdd>
                      </div>
                    </div>
                  </div>
                </div>

                { showingPublishAdditionalDetail
                ? relationDetailIsFetching ? <PTVPreloader />
                : <div className='accordion-subitem-details'>
                  <div className='flex-container'>
                    <PTVLabel labelClass='separator'> {formatMessage(Messages.channelRelationAdditionalInformationMessages.descriptionTitle)} </PTVLabel>
                    <PTVLabel> { description } </PTVLabel>
                  </div>
                  <div className='row'>
                    <div className='col-lg-5'>
                      <div className='flex-container'>
                        <PTVLabel labelClass='separator'> {formatMessage(Messages.channelRelationAdditionalInformationMessages.chargeTypeTitle)} </PTVLabel>
                        <PTVLabel> <LocalizedText id={chargeType} name={chargeTypeName} /> </PTVLabel>
                      </div>
                    </div>
                    <div className='col-lg-7'>
                      <div className='flex-container'>
                        <PTVLabel labelClass='separator'> {formatMessage(Messages.channelRelationAdditionalInformationMessages.chargeTypeAdditionalInfoTitle)} </PTVLabel>
                        <PTVLabel> { chargeTypeAdditionalInformation } </PTVLabel>
                      </div>
                    </div>
                  </div>
                </div>
                : null }

              </div>
        }

      </div>

      { showingAdditional && !readOnly
            ? relationDetailIsFetching ? <PTVPreloader />
                : <AdditionalInformation
                  id={id}
                  componentClass='channel-relation-info'
                  keyToState={id}
                  language={language}
                 />
            : null
        }

    </div>
  )
}

ChannelRelationAccordion.propTypes = {

}

ChannelRelationAccordion.defaultProps = {

}

function mapStateToProps (state, ownProps) {
  const channelRelation = CommonServiceAndChannelSelectors.getChannelRelationWithServiceChannel(state, ownProps)
  const chargeTypeId = channelRelation.get('chargeType')
  const description = CommonServiceAndChannelSelectors.getChannelRelationDescription(state, ownProps)
  const chargeTypeAdditionalInformation = CommonServiceAndChannelSelectors.getChannelRelationChargeTypeAdditionalInformation(state, ownProps)
  const isSomeDescription = CommonServiceAndChannelSelectors.getChannelRelationIsSomeDescription(state, ownProps)
  const isSomeChargeTypeAdditionalInformation = CommonServiceAndChannelSelectors.getChannelRelationIsSomeChargeTypeAdditionalInformation(state, ownProps)
  const infoExists = chargeTypeId != null || isSomeDescription || isSomeChargeTypeAdditionalInformation
  const channelType = CommonSelectors.getChannelType(state, { id: channelRelation.get('channelTypeId') })
  return {
    channelRelation,
      // Channel relations
    channelId: channelRelation.get('connectedChannel'),
    isNew: channelRelation.get('isNew'),
    serviceId: channelRelation.get('service'),
    chargeType: chargeTypeId,
    description: description,
    showingAdditional: channelRelation.get('showingAdditional') === true,
    showingPublishAdditionalDetail: channelRelation.get('showingPublishAdditionalDetail') === true,
    chargeTypeAdditionalInformation:chargeTypeAdditionalInformation,
    chargeTypeName: CommonSelectors.getChargeTypesNameForId(state, { id:chargeTypeId }),
      // Channel
    channelRootId: channelRelation.get('channelRootId'),
    channelType,
    channelCode: channelType.get('code'),
    channelName: channelRelation.get('channelName'),
    publishingStatusId: channelRelation.get('publishingStatus'),
    infoExists,
      // RelationDetail
    relationDetailIsFetching: CommonServiceAndChannelSelectors.getRelationDetailIsFetching(state, ownProps),
    relationDetailAreDataValid: CommonServiceAndChannelSelectors.getRelationDetailAreDataValid(state, ownProps),
    relationDetailIsLoaded: CommonServiceAndChannelSelectors.getRelationDetailIsLoaded(state, ownProps)
  }
}

const actions = [
  commonServiceAndChannelActions,
  serviceAndChannelActions,
  channelActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ChannelRelationAccordion))
