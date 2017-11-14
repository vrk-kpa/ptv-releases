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
import { defineMessages, injectIntl } from 'react-intl'
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps'

// Actions
import * as serviceSearchActions from '../../ServiceSearch/Actions'
import * as commonServiceAndChannelActions from '../../Common/Actions'
import { setLanguageTo } from 'Containers/Common/Actions'

// components
import Organizations from '../../../../Common/organizations'
import { LocalizedComboBox, LocalizedAsyncComboBox } from '../../../../Common/localizedData'
import LanguageSelect from '../../../../Common/languageSelect'

// selectors
import * as ServiceSearchSelectors from '../../ServiceSearch/Selectors'
import * as CommonServiceSelectors from '../../../../Services/Common/Selectors'
import * as CommonServiceAndChannelsSelectors from '../../Common/Selectors'
import * as IntlSelectors from '../../../../../Intl/Selectors'

const languageMessages = defineMessages({
  tooltip: {
    id: 'Containers.ServiceAndChannelRelations.LanguageSelect.Tooltip',
    defaultMessage: 'Rajaa hakua kieliversion mukaan.'
  }
})

export const ServiceSearchCriteria = ({
        messages, organizationId, serviceTypeId, serviceTypes, serviceClassId, serviceClasses, ontologyWord,
        language, isAnyRelation, searchingLanguage,
        keyToState, intl, actions }) => {
  const { formatMessage } = intl

  const onInputChange = (input, isSet = false) => value => {
    actions.onServiceSearchInputChange(input, value, isSet)
  }
  const onOntologyInputChange = input => (value, object) => {
    actions.onServiceSearchEntityAdd(input, object)
  }
  const onListChange = (input) => (value, isAdd) => {
    actions.onServiceSearchListChange(input, value, isAdd)
  }
  const onLanguageSelectChange = (id) => {
    actions.onServiceSearchListRemove()
    actions.onChannelSearchListRemove()
    actions.setLanguageTo('serviceAndChannelServiceSearch', id)
    actions.setLanguageTo('serviceAndChannelChannelSearch', id)
  }

  return (
    <div>

      <div className='form-group'>
        <LanguageSelect
          keyToState={'serviceAndChannel'}
          language={language}
          disabled={!isAnyRelation}
          changeCallback={onLanguageSelectChange}
          labelClass={'col-xs-12'}
          autoClass={'col-xs-12'}
          tooltip={formatMessage(languageMessages.tooltip)}
                    />
      </div>

      <div className='form-group'>
        <Organizations
          value={organizationId}
          id='4'
          label={formatMessage(messages.organizationComboTitle)}
          tooltip={formatMessage(messages.organizationComboTooltip)}
          name='organizationId'
          changeCallback={onInputChange('organizationId')}
          virtualized
          className='limited w320'
          inputProps={{ 'maxLength': '100' }} />
      </div>
      <div className='form-group'>
        <LocalizedComboBox
          value={serviceTypeId}
          id='3'
          label={formatMessage(messages.serviceTypeComboTitle)}
          tooltip={formatMessage(messages.serviceTypeComboTooltip)}
          name='serviceType'
          values={serviceTypes}
          changeCallback={onInputChange('serviceTypeId')}
          className='limited w320'
                            />
      </div>

      <div className='form-group'>
        <LocalizedComboBox
          value={serviceClassId}
          id='3'
          label={formatMessage(messages.serviceClassComboTitle)}
          tooltip={formatMessage(messages.serviceClassComboTooltip)}
          name='serviceClassId'
          values={serviceClasses}
          changeCallback={onInputChange('serviceClassId')}
          className='limited w320' />
      </div>
      <div className='form-group'>
        <LocalizedAsyncComboBox
          id='2'
          placeholder={formatMessage(messages.ontologyKeysPlaceholder)}
          label={formatMessage(messages.ontologyKeysTitle)}
          tooltip={formatMessage(messages.ontologyKeysTooltip)}
          name='ontologyWord'
          endpoint='service/GetFilteredList'
          getCallData={input => ({
            searchValue: input,
            treeType: 'OntologyTerm',
            result: 'List',
            language: searchingLanguage
          })}
          value={ontologyWord}
          onChange={onOntologyInputChange('ontologyWord')}
                  />
      </div>
    </div>
  )
}

ServiceSearchCriteria.propTypes = {

}

function mapStateToProps (state, ownProps) {
  return {
    organizationId: ServiceSearchSelectors.getOrganizationId(state),
    serviceTypeId: ServiceSearchSelectors.getServiceTypeId(state),
    serviceTypes: CommonServiceSelectors.getServiceTypesObjectArray(state),
    serviceClassId: ServiceSearchSelectors.getServiceClassId(state),
    serviceClasses: CommonServiceSelectors.getServiceClassesObjectArray(state),
    ontologyWord: ServiceSearchSelectors.getOntologyWord(state),
    language: CommonServiceAndChannelsSelectors.getLanguageToForServiceAndChannel(state),
    isAnyRelation: CommonServiceAndChannelsSelectors.getIsAnyRelation(state, ownProps),
    searchingLanguage: IntlSelectors.getSelectedLanguage(state)
  }
}

const actions = [
  serviceSearchActions,
  commonServiceAndChannelActions,
  { setLanguageTo }
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceSearchCriteria))
