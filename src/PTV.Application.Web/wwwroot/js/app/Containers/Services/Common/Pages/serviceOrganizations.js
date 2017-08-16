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
import { defineMessages, injectIntl, intlShape } from 'react-intl'

// components
import Organizations from '../../../Common/organizationsTagSelect'
import Organization from '../../../Common/organizations'

// actions
import * as serviceActions from '../../Service/Actions'
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps'

// selectors
import * as ServiceSelectors from '../../Service/Selectors'
import { getOrganizationsObjectArray } from '../../../Manage/Organizations/Common/Selectors'
import { getAllTranslationExists } from 'Intl/Selectors'

// Validators
import * as PTVValidatorTypes from '../../../../Components/PTVValidators'

const serviceMessages = defineMessages({
  organizationTranslationWarning: {
    id: 'Service.Organization.TranslationMissing',
    defaultMessage: 'Organisaation tulee olla kuvattu palvelukuvauksen kielellä.'
  },
  organizationProducerWarning:{
    id: 'Service.Organization.ProducerWarning',
    defaultMessage: 'Olet tehnyt muutoksia vastuuorganisaatiolistaan, jolla voi olla vaikutuksia myös uottajatietoihin. Ole hyvä ja tarkista ne alempana lomakkeella.'
  }
})

const ServiceOrganizations = ({
    messages,
    readOnly,
    language,
    translationMode,
    actions,
    entityId,
    translationWarning,
    producerWarning,
    intl:{ formatMessage },
    mainOrganizationId
  }) => {
  const onInputChange = (input, isSet = false) => value => {
    actions.onServiceInputChange(input, value, language, isSet)
  }
  const onMainOrganizationChange = (input, isSet = false) => value => {
    actions.onServiceInputChange(input, value, language, isSet)
    actions.onRemoveOrganizer(value, language)
    actions.getServiceAreaInformation(language)
  }
  const validators = [PTVValidatorTypes.IS_REQUIRED]

  return (
    <div className='row form-group break-word'>
      <Organization
        value={mainOrganizationId}
        label={formatMessage(messages.mainOrganisationTitle)}
        name='mainOrganization'
        changeCallback={onMainOrganizationChange('organizationId')}
        componentClass='col-md-6'
        inputProps={{ 'maxLength': '50' }}
        validators={validators}
        readOnly={readOnly}
        translationMode={translationMode}
        validatedField={messages.mainOrganisationTitle} />
      <Organizations
        componentClass='col-md-6'
        id='organizations'
        label={messages.responsibleOrganisationTitle}
        changeCallback={onInputChange('organizers', true)}
        order={40}
        selector={ServiceSelectors.getSelectedOrganizersItemsJS}
        sourceSelector={getOrganizationsObjectArray}
        withoutOrganizationId={mainOrganizationId}
        keyToState={'any'}
        language={language}
        translationWarning={translationWarning && serviceMessages.organizationTranslationWarning || null}
        producerWarning={producerWarning && serviceMessages.organizationProducerWarning || null}
        readOnly={readOnly || translationMode === 'view' || translationMode === 'edit'}
        />
    </div>

  )
}

ServiceOrganizations.propTypes = {
  messages: PropTypes.any.isRequired,
  readOnly: PropTypes.bool.isRequired,
  language: PropTypes.string.isRequired,
  translationMode: PropTypes.string.isRequired,
  actions: PropTypes.object.isRequired,
  entityId: PropTypes.string,
  translationWarning: PropTypes.bool.isRequired,
  producerWarning: PropTypes.bool,
  mainOrganizationId: PropTypes.any,
  intl: intlShape.isRequired
}

function mapStateToProps (state, ownProps) {
  const ids = ServiceSelectors.getSelectedOrganizersWithMainOrganization(state, ownProps)
  const translationWarning = !getAllTranslationExists(state, { ids, language: ownProps.language })
  const selfOrganizers = ServiceSelectors.getServiceSelfProducerOrganizers(state, ownProps)
  const producerWarning = ownProps.entityId && selfOrganizers.some(self => !ids.contains(self))
  return {
    translationWarning,
    producerWarning,
    mainOrganizationId: ServiceSelectors.getMainOrganizationId(state, ownProps)
  }
}

const actions = [
  serviceActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceOrganizations))

