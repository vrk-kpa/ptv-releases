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
import React, { PropTypes } from 'react'
import { connect } from 'react-redux'
import { injectIntl, FormattedMessage } from 'react-intl'
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps'
import shortId from 'shortid'

// Actions
import * as commonActions from '../Actions'
import * as organizationActions from '../../Organization/Actions'

// components
import * as PTVValidatorTypes from '../../../../../Components/PTVValidators'
import { PTVTextInputNotEmpty, PTVCheckBox, PTVTextEditorNotEmpty } from '../../../../../Components'
import Business from '../../../../Common/Business/Business'
import OrganizationAreaInformation from './OrganizationAreaInformation'

// selectors
import * as CommonOrganizationSelectors from '../Selectors'

const OrganizationDescriptionContainer = props => {
  const validatorsName = [PTVValidatorTypes.IS_REQUIRED]

  const onAddBusiness = (entity) => {
    props.actions.onLocalizedOrganizationEntityAdd('business', entity, entityId, props.language)
  }

  const onInputChange = (input, isSet = false) => value => {
    props.actions.onLocalizedOrganizationInputChange(input, entityId, value, isSet, props.language)
  }

  const onInputCheckboxChange = (input, isSet = false) => value => {
    props.actions.onLocalizedOrganizationInputChange(input, entityId, value.target.checked, isSet, props.language)
  }

  const { formatMessage } = props.intl
  const { messages, readOnly, language, translationMode, splitContainer, description,
          name, alternateName, isAlternateNameUsedAsDisplayName, businessId, organizationId, entityId, areaInformationVisible } = props
  const translatableAreaRO = readOnly && translationMode == 'none'
  const sharedProps = { readOnly, language, translationMode }
  const splitClass = splitContainer ? 'col-xs-12' : 'col-lg-6'

  return (
    <div>
      {areaInformationVisible &&
      <OrganizationAreaInformation {...sharedProps}
        messages={messages}
                />}
      <div className='row form-group'>
        <PTVTextInputNotEmpty
          componentClass='col-lg-6'
          label={formatMessage(messages.organizationNameTitle)}
          validatedField={messages.organizationNameTitle}
          placeholder={formatMessage(messages.organizationNamePlaceholder)}
          value={name}
          blurCallback={onInputChange('organizationName')}
          name='organizationName'
          validators={validatorsName}
          readOnly={readOnly && translationMode === 'none'}
          disabled={translationMode === 'view'}
          maxLength={100}
                    />
        <div className='col-xs-12 col-lg-6'>
          <div className='row'>
            <PTVTextInputNotEmpty
              componentClass='col-xs-12'
              label={formatMessage(messages.organizationAlternativeNameTitle)}
              validatedField={messages.organizationAlternativeNameTitle}
              tooltip={formatMessage(messages.organizationAlternativeNumberTooltip)}
              placeholder={formatMessage(messages.organizationAlternativeNamePlaceholder)}
              value={alternateName}
              blurCallback={onInputChange('organizationAlternateName')}
              name='organizationAlternative'
              readOnly={readOnly && translationMode === 'none'}
              disabled={translationMode === 'view'}
              validators={isAlternateNameUsedAsDisplayName && validatorsName}
              maxLength={100}
                            />
            { readOnly ? alternateName
            ? <div className='col-xs-12'>
              <PTVCheckBox
                className='strong'
                id={'chckUseAsDisplayName'}
                isSelectedSelector={CommonOrganizationSelectors.isAlternateNameUsedAsDisplayNameSelected}
                onClick={onInputCheckboxChange('isAlternateNameUsedAsDisplayName')}
                isDisabled={readOnly}
                showCheck
                language={language} >
                <FormattedMessage {...messages.isAlternateNameUsedAsDisplayName} />
              </PTVCheckBox>

            </div> : null
            : <div className='col-xs-12'>
              <PTVCheckBox
                className='strong'
                id={'chckUseAsDisplayName'}
                isSelectedSelector={CommonOrganizationSelectors.isAlternateNameUsedAsDisplayNameSelected}
                onClick={onInputCheckboxChange('isAlternateNameUsedAsDisplayName')}
                isDisabled={readOnly}
                showCheck
                language={language} >
                <FormattedMessage {...messages.isAlternateNameUsedAsDisplayName} />
              </PTVCheckBox>

            </div>}

          </div>
        </div>
      </div>
      <div className='row form-group'>
        <Business {...sharedProps}
          componentClass={splitClass}
          messages={messages}
          businessId={businessId || shortId.generate()}
          isNew={businessId === null}
          onAddBusiness={onAddBusiness}
        />

        <PTVTextInputNotEmpty
          componentClass={splitClass}
          label={formatMessage(messages.organizationIdTitle)}
          tooltip={formatMessage(messages.organizationIdTooltip)}
          placeholder={formatMessage(messages.organizationIdPlaceholder)}
          blurCallback={onInputChange('organizationId')}
          value={organizationId}
          name='organizationId'
          maxLength={100}
          readOnly={readOnly || translationMode === 'view' || translationMode === 'edit'}
                    />
      </div>

      <div className='row form-group'>
        <PTVTextEditorNotEmpty
          componentClass='col-lg-12'
          maxLength={2500}
          label={formatMessage(messages.organizationDescriptionTitle)}
          validatedField={messages.organizationDescriptionTitle}
          placeholder={formatMessage(messages.organizationDescriptionPlaceholder)}
          tooltip={formatMessage(messages.organizationDescriptionTooltip)}
          value={description}
          name='orgDescription'
          blurCallback={onInputChange('description')}
          disabled={translationMode === 'view'}
          readOnly={translatableAreaRO}
                  />
      </div>
    </div>
  )
}

OrganizationDescriptionContainer.propTypes = {
  description: PropTypes.string.isRequired,
  readOnly: PropTypes.bool,
  areaInformationVisible: PropTypes.bool
}

function mapStateToProps (state, ownProps) {
  const areaType = CommonOrganizationSelectors.getOrganizationType(state, ownProps)
  const municipalityAreTypeId = CommonOrganizationSelectors.getOrganizationTypeForCode(state, 'municipality')
  return {
    name: CommonOrganizationSelectors.getName(state, ownProps),
    alternateName: CommonOrganizationSelectors.getAlternateName(state, ownProps),
    isAlternateNameUsedAsDisplayName:
        CommonOrganizationSelectors.isAlternateNameUsedAsDisplayNameSelected(state, ownProps),
    businessId: CommonOrganizationSelectors.getBusinessId(state, ownProps),
    description: CommonOrganizationSelectors.getDescription(state, ownProps),
    organizationId: CommonOrganizationSelectors.getOrganizationStringId(state, ownProps),
    entityId: CommonOrganizationSelectors.getOrganizationId(state, ownProps),
    areaInformationVisible: areaType !== null && areaType !== municipalityAreTypeId
  }
}

const actions = [
  commonActions,
  organizationActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(OrganizationDescriptionContainer))
