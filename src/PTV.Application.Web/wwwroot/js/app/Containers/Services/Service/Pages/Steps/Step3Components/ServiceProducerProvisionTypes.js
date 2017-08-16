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
import { connect } from 'react-redux'

import * as mainActions from '../../../Actions'
import mapDispatchToProps from '../../../../../../Configuration/MapDispatchToProps'
import * as PTVValidatorTypes from '../../../../../../Components/PTVValidators'
import { PTVTextInput, PTVTextAreaNotEmpty } from '../../../../../../Components'
import * as ServiceSelectors from '../../../Selectors'
import * as OrganizationSelectors from '../../../../../Manage/Organizations/Common/Selectors'
import Organizations from '../../../../../Common/organizations'
import OrganizationsTagSelect from '../../../../../Common/organizationsTagSelect'

const messages = defineMessages({
  producerSelectionTitle: {
    id : 'Containers.Services.AddService.Step3.ServiceProducer.SearchProducer.Title',
    defaultMessage: 'Tuottaja'
  },
  producerSearchBoxPlaceholder: {
    id : 'Containers.Services.AddService.Step3.ServiceProducer.SearchProducer.Placeholder',
    defaultMessage: 'Hae organisaatioista'
  },
  producerSearchBoxTooltip: {
    id : 'Containers.Services.AddService.Step3.ServiceProducer.SearchProducer.Tooltip',
    defaultMessage: 'Tuottaja'
  },
  producerLinkTitle: {
    id : 'Containers.Services.AddService.Step3.ServiceProducer.ProducerLink.Title',
    defaultMessage: 'Linkki palvelun tuottajien tietoihin'
  },
  producerLinkPlaceholder: {
    id : 'Containers.Services.AddService.Step3.ServiceProducer.ProducerLink.Placeholder',
    defaultMessage: 'Linkki palvelun tuottajien tietoihin'
  },
  producerLinkTooltip: {
    id : 'Containers.Services.AddService.Step3.ServiceProducer.ProducerLink.Tooltip',
    defaultMessage: 'Linkki palvelun tuottajien tietoihin'
  },
  producerExternalDescriptionTitle: {
    id : 'Containers.Services.AddService.Step3.ServiceProducer.ExternalDescription.Title',
    defaultMessage: 'Jos tuottaja ei löydy Palvelutietovarannosta, voit lisätä ulkopuolisen kuvauksen.'
  },
  producerAdditionalInformationTitle: {
    id : 'Containers.Services.AddService.Step3.ServiceProducer.AdditionalInformation.Title',
    defaultMessage: 'Jos tuottaja ei löydy Palvelutietovarannosta, voit lisätä ulkopuolisen kuvauksen.'
  },
  producerExternalDescriptionTooltip: {
    id : 'Containers.Services.AddService.Step3.ServiceProducer.ExternalDescription.Tooltip',
    defaultMessage: 'Tuottaja ei löydy Palvelutietovarannosta, voit lisätä ulkopuolisen kuvauksen.'
  },
  producerFreeDescriptionPlaceholder: {
    id : 'Containers.Services.AddService.Step3.ServiceProducer.FreeDescription.Placeholder',
    defaultMessage: 'Kirjoita vapaa kuvaus'
  },
  producerLinkTitleView: {
    id : 'Containers.Services.ViewService.Step3.ServiceProducer.Title',
    defaultMessage: 'Palvelun tuottaja:'
  }
})

function onInputChange (props, type, input, isSet = false) {
   return (value) => {
    props.actions.onProducerObjectChange(props.producerId, { [type] : { [input]: value } }, props.language, isSet)
  }
}

const onInputChange2 = (props, input, isSet) => value => {
  props.actions.onProducerInputChange(props.producerId, input, value, props.language, isSet)
}
class SelfProducer extends Component {
  constructor (props) {
    super(props)
  }

  validators = [PTVValidatorTypes.IS_REQUIRED];

  static propTypes = {
    intl: intlShape.isRequired,
    actions: PropTypes.object.isRequired,
    producerId: PropTypes.string.isRequired,
    details: PropTypes.object.isRequired
  };

  render () {
    const { details, readOnly, translationMode, language, producerId } = this.props
    const { formatMessage } = this.props.intl
    return (
      <div className='row form-group'>
        <OrganizationsTagSelect
          componentClass='col-xs-12'
          id={producerId}
          label={messages.producerSelectionTitle}
          tooltip={messages.producerSearchBoxTooltip}
          changeCallback={onInputChange2(this.props, 'organizers', true)}
          validators={this.validators}
          order={40}
          sourceSelector={ServiceSelectors.getSelectedOrganizersItemsWithMainOrganizationJS}
          selector={ServiceSelectors.getSelectedServiceProducerOrganizersItemsJS}
          keyToState={'any'}
          language={language}
          readOnly={readOnly || translationMode === 'view' || translationMode === 'edit'} />
      </div>)
  }
}

// / PurchaseServices component - Ostopalvelut
class PurchaseServices extends Component {
  constructor (props) {
    super(props)
  }

  static propTypes = {
    intl: intlShape.isRequired,
    actions: PropTypes.object.isRequired,
    producerId: PropTypes.string.isRequired,
    details: PropTypes.object.isRequired
  };

  validators = [PTVValidatorTypes.IS_REQUIRED]

  renderReadOnlyPurchase (organization, description) {
    const { formatMessage } = this.props.intl
    return (
      <div>
        <div className='row' >
          <PTVTextInput componentClass='col-lg-12'
            value={organization && organization.name}
            label={formatMessage(messages.producerLinkTitleView)}
            name='readOnlyPurchase'
            readOnly />
        </div>
        {description !== '' &&
        <div className='row' >
          <PTVTextInput componentClass='col-lg-12'
            value={description}
            label={formatMessage(messages.producerLinkTitleView)}
            name='readOnlyPurchase'
            readOnly />
        </div>}
      </div>
    )
  }

  render () {
    const { formatMessage } = this.props.intl
    const { allOrganizations, organizationId, additionalInformation, readOnly, translationMode, language } = this.props
    return readOnly && translationMode === 'none'
        ? this.renderReadOnlyPurchase(allOrganizations.find(x => x.id === organizationId), additionalInformation)
         : (
           <div>
             <div className='row'>
               <Organizations
                 value={organizationId}
                 label={formatMessage(messages.producerSelectionTitle)}
                 tooltip={formatMessage(messages.producerSearchBoxPlaceholder)}
                 componentClass='col-xs-12'
                 name='producer'
                 changeCallback={onInputChange2(this.props, 'organizationId')}
                 virtualized
                 className='limited w480'
                 readOnly={translationMode === 'view' || translationMode === 'edit'}
                 inputProps={{ 'maxLength': '100' }}
                 language={language}
                 validators={additionalInformation === '' && this.validators}
                 visibleAllOrganizations
                    />
             </div>
             {
              organizationId === '' &&
                <div className='row'>
                  <PTVTextAreaNotEmpty componentClass='col-lg-12'
                    label={formatMessage(messages.producerExternalDescriptionTitle)}
                    counterClass='counter'
                    placeholder={formatMessage(messages.producerFreeDescriptionPlaceholder)}
                    minRows={6} maxLength={150}
                    value={additionalInformation}
                    blurCallback={onInputChange2(this.props, 'additionalInformation')}
                    name='producerTextArea'
                    disabled={translationMode === 'view'}
                    validators={this.validators}
                    />
                </div>
            }
           </div>
        )
  }
}

// / Other component - Muu
class OtherProducer extends Component {
  constructor (props) {
    super(props)
  }

  static propTypes = {
    intl: intlShape.isRequired,
    actions: PropTypes.object.isRequired,
    producerId: PropTypes.string.isRequired,
    details: PropTypes.object.isRequired
  };

  validators = [PTVValidatorTypes.IS_REQUIRED]

  renderReadOnlyOtherProducer = (organization, description) => {
    const { formatMessage } = this.props.intl
    return (
      <div className='row' >
        <PTVTextInput componentClass='col-xs-12'
          value={organization ? organization.name : description}
          label={formatMessage(messages.producerLinkTitleView)}
          name='readOnlyOtherProducer'
          readOnly />
      </div>
    )
  }

  render () {
    const { formatMessage } = this.props.intl
    const { allOrganizations, organizationId, additionalInformation, readOnly, translationMode, language } = this.props
    return readOnly && translationMode === 'none'
       ? this.renderReadOnlyOtherProducer(allOrganizations.find(x => x.id === organizationId), additionalInformation)
        : (
          <div>
            <div className='row'>
              <Organizations
                value={organizationId}
                label={formatMessage(messages.producerSelectionTitle)}
                tooltip={formatMessage(messages.producerSearchBoxPlaceholder)}
                componentClass='col-xs-12'
                name='otherProducer'
                changeCallback={onInputChange2(this.props, 'organizationId')}
                virtualized
                className='limited w480'
                readOnly={translationMode === 'view' || translationMode === 'edit'}
                inputProps={{ 'maxLength': '100' }}
                language={language}
                validators={additionalInformation === '' && this.validators}
                visibleAllOrganizations
                />
            </div>
            {
            organizationId === '' &&
            <div className='row'>
              <PTVTextAreaNotEmpty componentClass='col-lg-12'
                label={translationMode === 'none' ? formatMessage(messages.producerExternalDescriptionTitle) : formatMessage(messages.producerLinkTitleView)}
                counterClass='counter'
                placeholder={formatMessage(messages.producerFreeDescriptionPlaceholder)}
                minRows={6} maxLength={150}
                value={additionalInformation}
                blurCallback={onInputChange2(this.props, 'additionalInformation')}
                name='producerOtherTextArea'
                disabled={translationMode === 'view'}
                validators={this.validators}
                />
            </div>}
          </div>
        )
  }
}

// / maps state to props
function mapStateToProps (state, ownProps) {
  return {
    selectedOrganizers : ServiceSelectors.getSelectedOrganizersMap(state, ownProps),
    allOrganizations: OrganizationSelectors.getOrganizationsObjectArray(state)
  }
}

const actions = [
  mainActions
]

export default {
  SelfProducer: connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(SelfProducer)),
 // VoucherServices: connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(VoucherServices)),
  PurchaseServices: connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(PurchaseServices)),
  OtherProducer: connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(OtherProducer))
}

