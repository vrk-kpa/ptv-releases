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
import React, { PropTypes, Component } from 'react'
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl'
import Immutable, { Map } from 'immutable'
import { connect } from 'react-redux'

import * as PTVValidatorTypes from '../../../../Components/PTVValidators'
import { PTVTextAreaNotEmpty, PTVTextEditorNotEmpty, PTVTextInputNotEmpty, PTVLabel } from '../../../../Components'
import { LocalizedComboBox } from '../../../Common/localizedData'
import '../Styles/ChannelDescriptionContainer.scss'

import * as CommonSelectors from '../../Common/Selectors'
import { getOrganizationsObjectArray } from '../../../Manage/Organizations/Common/Selectors'
import { getTranslationExists } from 'Intl/Selectors'

import * as channelActions from '../Actions'
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps'

const channelMessages = defineMessages({
  organizationTranslationWarning: {
    id: 'Channel.Organization.TranslationMissing',
    defaultMessage: 'Organisaation tulee olla kuvattu asiointikanavakuvauksen kielellä.'
  }
})

const ChannelDescriptionContainer = ({ actions, channelId, language, translationMode, intl: { formatMessage }, description, channelName, organizations, shortDescription, organizationId,
  readOnly, messages, children, organizationTranslationExists }) => {
  const validators = [PTVValidatorTypes.IS_REQUIRED]
  const onInputChange = (input, isSet = false) => value => {
    actions.onChannelObjectChange(channelId, { [input]: value }, isSet, language)
  }

  const translatableAreaRO = readOnly && translationMode == 'none';

  return (
    <div className='channel-description-container'>
      <div className='row form-group'>
        <PTVTextInputNotEmpty
          componentClass='col-md-6'
          label={formatMessage(messages.nameTitle)}
          validatedField={messages.nameTitle}
          value={channelName}
          blurCallback={onInputChange('name')}
          maxLength={100}
          size='full'
          name='channelDescriptionName'
          validators={validators}
          order={10}
          placeholder={formatMessage(messages.namePlaceholder)}
          readOnly={readOnly && translationMode == 'none'}
          disabled={translationMode == 'view'}
        />
        <div className='col-md-6'>
          <LocalizedComboBox
            value={organizationId}
            values={organizations}
            label={formatMessage(messages.organizationLabel)}
            validatedField={messages.organizationLabel}
            componentClass=''
            changeCallback={onInputChange('organizationId')}
            validators={validators}
            order={20}
            name='organizationName'
            tooltip={formatMessage(messages.organizationInfo)}
            autosize={false}
            readOnly={readOnly || translationMode === 'view' || translationMode === 'edit'}
            className='limited w480'
            inputProps={{ 'maxLength': '100' }}
            language={language}
          />
          {!readOnly && !organizationTranslationExists &&
            <PTVLabel labelClass='has-error' >
              {formatMessage(channelMessages.organizationTranslationWarning)}
            </PTVLabel>
          }
        </div>
      </div>

      {children}

      <div className='row form-group'>
        <PTVTextAreaNotEmpty
          componentClass='col-xs-12'
          minRows={2}
          maxLength={150}
          label={formatMessage(messages.shortDescriptionLabel)}
          validatedField={messages.shortDescriptionLabel}
          value={shortDescription}
          blurCallback={onInputChange('shortDescription')}
          validators={validators}
          order={30}
          name='channelShortDescription'
          tooltip={formatMessage(messages.shortDescriptionInfo)}
          placeholder={formatMessage(messages.shortDescriptionPlaceholder)}
          disabled={translationMode == 'view'}
          readOnly={translatableAreaRO}
        />
      </div>
      <div className='row form-group'>
        <PTVTextEditorNotEmpty
          componentClass='col-xs-12'
          maxLength={2500}
          label={formatMessage(messages.descriptionLabel)}
          validatedField={messages.descriptionLabel}
          placeholder={formatMessage(messages.descriptionPlaceholder)}
          tooltip={formatMessage(messages.descriptionInfo)}
          value={description}
          name='channelDescription'
          blurCallback={onInputChange('description')}
          validators={validators}
          order={40}
          disabled={translationMode == 'view'}
          readOnly={translatableAreaRO}
        />
      </div>
    </div>
  )
}

ChannelDescriptionContainer.propTypes = {
  description: PropTypes.string.isRequired,
  readOnly: PropTypes.bool
}

function mapStateToProps (state, ownProps) {
  const organizationId = CommonSelectors.getOrganizationId(state, ownProps)
  return {
    channelName: CommonSelectors.getChannelName(state, ownProps),
    shortDescription: CommonSelectors.getShortDescription(state, ownProps),
    description: CommonSelectors.getDescription(state, ownProps),
    organizationId,
    organizations: getOrganizationsObjectArray(state),
    organizationTranslationExists:
      organizationId
        ? getTranslationExists(state, { id: organizationId, language: ownProps.language })
        : true,
    channelId: CommonSelectors.getChannelId(state, ownProps)
  }
}

const actions = [
  channelActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ChannelDescriptionContainer))
