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
import {
  ProvisionType,
  ShortDescription,
  Organization,
  SelfProducers
} from 'util/redux-form/fields'
import asSection from 'util/redux-form/HOC/asSection'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { getIsAlreadySelfProducedForIndex,
  getIsAnySelectedForIndex,
  getIsMoreThanOneOrganizationForForm,
  getIsOtherWithOrganizationForIndex } from './selectors'
import { connect } from 'react-redux'
import { injectIntl, defineMessages, intlShape } from 'util/react-intl'

export const messages = defineMessages({
  label: {
    id : 'Containers.Services.AddService.Step3.ServiceProducer.SearchProducer.Title',
    defaultMessage: 'Tuottaja'
  },
  placeholder: {
    id: 'Components.AutoCombobox.Placeholder',
    defaultMessage: '- valitse -'
  },
  producerExternalDescriptionLabel: {
    id : 'Containers.Services.AddService.Step3.ServiceProducer.ExternalDescription.Title',
    defaultMessage: 'Jos tuottaja ei löydy Palvelutietovarannosta, voit lisätä ulkopuolisen kuvauksen.'
  },
  producerFreeDescriptionPlaceholder: {
    id : 'Containers.Services.AddService.Step3.ServiceProducer.FreeDescription.Placeholder',
    defaultMessage: 'Kirjoita vapaa kuvaus'
  }
})

const Producer = ({
  isSelfProduced,
  isOtherWithOrganization,
  isAnySelected,
  index,
  isMoreThanOne,
  isCompareMode,
  splitView,
  intl: { formatMessage }
}) => {
  const basicClass = isCompareMode || splitView ? 'col-lg-24' : 'col-lg-12'

  const renderSelfProducers = () => {
    return isMoreThanOne &&
      <div className='form-row'>
        <div className='row'>
          <div className={basicClass}>
            <SelfProducers index={index} required />
          </div>
        </div>
      </div> ||
      null
  }

  const renderOthersProducers = () => {
    return (
      <div>
        <div className='form-row'>
          <div className='row'>
            <div className={basicClass}>
              <Organization
                label={formatMessage(messages.label)}
                tooltip={null}
                showAll
                required
                skipValidation
              />
            </div>
          </div>
        </div>
        {!isOtherWithOrganization &&
        <div className='form-row'>
          <div className='row'>
            <div className={basicClass}>
              <ShortDescription
                name={'additionalInformation'}
                label={formatMessage(messages.producerExternalDescriptionLabel)}
                placeholder={formatMessage(messages.producerFreeDescriptionPlaceholder)}
                counter
                multiline
                rows={3}
                maxLength={150}
                noTranslationLock
              />
            </div>
          </div>
        </div>}
      </div>
    )
  }

  const renderProducers = () => {
    return isSelfProduced ? renderSelfProducers() : renderOthersProducers()
  }

  return (
    <div>
      <div className='collection-form-row'>
        <div className='row'>
          <div className={basicClass}>
            <ProvisionType index={index} required />
          </div>
        </div>
      </div>
      {isAnySelected && renderProducers()}
    </div>
  )
}

Producer.propTypes = {
  isSelfProduced: PropTypes.bool.isRequired,
  intl: intlShape.isRequired,
  isMoreThanOne: PropTypes.bool.isRequired,
  isOtherWithOrganization: PropTypes.bool.isRequired,
  isAnySelected: PropTypes.bool.isRequired,
  index: PropTypes.number,
  isCompareMode: PropTypes.bool,
  splitView: PropTypes.bool
}

export default compose(
  injectFormName,
  injectIntl,
  withFormStates,
  connect((state, ownProps) => ({
    isOtherWithOrganization: getIsOtherWithOrganizationForIndex(state, ownProps),
    isSelfProduced: getIsAlreadySelfProducedForIndex(state, ownProps),
    isAnySelected: getIsAnySelectedForIndex(state, ownProps),
    isMoreThanOne: getIsMoreThanOneOrganizationForForm(state, ownProps)
  })),
  asSection('serviceProducers')
)(Producer)
