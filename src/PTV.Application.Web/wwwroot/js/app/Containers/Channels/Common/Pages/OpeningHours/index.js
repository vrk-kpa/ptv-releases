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
import { connect } from 'react-redux'
import { Sticky, StickyContainer } from 'react-sticky'
import {defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl';

// Components
import PTVAddItem from '../../../../../Components/PTVAddItem'
import PTVLabel from '../../../../../Components/PTVLabel'
import OpeningHoursList from './OpeningHoursList'
import OpeningHoursPreviewList from './OpeningHoursPreviewList'
import cx from 'classnames'
import * as PTVValidatorTypes from '../../../../../Components/PTVValidators'

// selectors
import * as CommonChannelSelectors from '../../Selectors'

// enums
import { openingHoursTypes } from '../../../../Common/Enums'

// messages
import { openingHoursMessages } from '../../Messages'

export const Opening = ({
  intl: { formatMessage },
  messages,
  readOnly,
  translationMode,
  keyToState,
  language,
  normalIds,
  specialIds,
  exceptionalIds,
  activeHours,
  validators
}) => {
  const readOnlyHours = readOnly || translationMode === 'view' || translationMode === 'edit'
  const previewClass = readOnlyHours ? 'col-xs-12' : 'col-lg-4'
  const sharedProps = { keyToState, language, readOnly:readOnlyHours }
  const isSomeOpeningHoursFilled = normalIds.size !== 0 || specialIds.size !== 0 || exceptionalIds.size !== 0
  const renderOpeningHours = () => {
      return (
          <StickyContainer>
        <div className='row'>
          { !readOnlyHours &&
            <div className='col-lg-8'>
              <div className={cx('opening-hours-container', { readonly: readOnlyHours })}>
                <OpeningHoursList.OpeningHoursNormal
                  {...sharedProps}
                  messages={messages.openingHoursMessages}
                  showList={activeHours === openingHoursTypes.normal}
                  openingHoursType={openingHoursTypes.normal}
                  label={formatMessage(messages.openingHoursMessages.mainLabelNormal)}
                  validatedField={messages.openingHoursMessages.mainLabelNormal}
                  validators={validators}
                              />
                              <OpeningHoursList.OpeningHoursSpecial
                                  {...sharedProps}
                                  messages = { messages.openingHoursMessages }
                  showList={activeHours === openingHoursTypes.special}
                  openingHoursType={openingHoursTypes.special}
                  label={formatMessage(messages.openingHoursMessages.mainLabelSpecial)}
                  validatedField={messages.openingHoursMessages.mainLabelSpecial}
                  validators={validators}
                              />
                              <OpeningHoursList.OpeningHoursExceptional
                                  {...sharedProps}
                                  messages = { messages.openingHoursMessages }
                  showList={activeHours === openingHoursTypes.exceptional}
                  openingHoursType={openingHoursTypes.exceptional}
                  label={formatMessage(messages.openingHoursMessages.mainLabelExceptional)}
                  validatedField={messages.openingHoursMessages.mainLabelExceptional}
                  validators={validators}
                />
              </div>
            </div>
          }
          <div className={previewClass}>
            <Sticky stickyClassName='ptv-sticky'>
              <div className='opening-hours-preview'>
                { !readOnlyHours &&
                  <div className='opening-hours-preview-title'>
                    <PTVLabel tooltip={formatMessage(openingHoursMessages.previewTooltip)} >
                      { formatMessage(openingHoursMessages.previewTitle) }
                    </PTVLabel>
                  </div>
                }
                <OpeningHoursPreviewList
                  {...sharedProps}
                  componentSectionClass='opening-hours-preview-section'
                  componentSectionItemClass='opening-hours-preview-section-item'
                  ids={normalIds}
                  openingHoursType={openingHoursTypes.normal}
                  activeHours={activeHours}
                />
                <OpeningHoursPreviewList
                  {...sharedProps}
                  componentSectionClass='opening-hours-preview-section'
                  componentSectionItemClass='opening-hours-preview-section-item'
                  ids={specialIds}
                  openingHoursType={openingHoursTypes.special}
                  activeHours={activeHours}
                />
                <OpeningHoursPreviewList
                  {...sharedProps}
                  componentSectionClass='opening-hours-preview-section'
                  componentSectionItemClass='opening-hours-preview-section-item'
                  ids={exceptionalIds}
                  openingHoursType={openingHoursTypes.exceptional}
                  activeHours={activeHours}
                />
              </div>
            </Sticky>
          </div>
        </div>
      </StickyContainer>
    )
  }
  const body = (
    <PTVAddItem
      readOnly={readOnlyHours}
      containsData={isSomeOpeningHoursFilled}
      renderItemContent={renderOpeningHours}
      messages={{ 'label': formatMessage(messages.openingHoursMessages.showOpeningHours) }}
      componentClass='outer'
    />
  )
  return <div className='step-opening-hours'>
    {((readOnly && isSomeOpeningHoursFilled) || (!readOnly && translationMode !== 'view')) && body}
  </div>
}

Opening.propTypes = {
  messages: PropTypes.object.isRequired,
  keyToState: PropTypes.string.isRequired,
  readOnly: PropTypes.bool.isRequired,
  activeHours: PropTypes.number,
  intl: intlShape.isRequired
}

function mapStateToProps (state, ownProps) {
  return {
    normalIds: CommonChannelSelectors.getOpeningHoursNormal(state, ownProps),
    specialIds: CommonChannelSelectors.getOpeningHoursSpecial(state, ownProps),
    exceptionalIds: CommonChannelSelectors.getOpeningHoursExceptional(state, ownProps),
    activeHours: CommonChannelSelectors.getActiveOpeningHours(state, ownProps),
    validators: [{
      ...PTVValidatorTypes.IS_REQUIRED,
      errorMessage: ownProps.messages.openingHoursMessages.containsInvalidInfo
    }]
  }
}

export default connect(mapStateToProps)(injectIntl(Opening))
