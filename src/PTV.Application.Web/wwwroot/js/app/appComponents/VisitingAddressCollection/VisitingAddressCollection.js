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
import React, { Component } from 'react'
import { PropTypes } from 'prop-types'
import { AddressSwitch } from 'util/redux-form/sections'
import { addressUseCasesEnum, entityConcreteTypesEnum } from 'enums'
import { compose } from 'redux'
import {
  injectIntl,
  intlShape,
  defineMessages,
  FormattedMessage
} from 'util/react-intl'
import AccessibilityRemoveButton from 'util/redux-form/HOC/asCollection/AccessibilityRemoveButton'
import asCollection from 'util/redux-form/HOC/asCollection'
import asGroup from 'util/redux-form/HOC/asGroup'
import asContainer from 'util/redux-form/HOC/asContainer'
import asSection from 'util/redux-form/HOC/asSection'
import VisitingAddressTitle from './VisitingAddressTitle'
import commonMessages from 'util/redux-form/messages'
import withErrorDisplay from 'util/redux-form/HOC/withErrorDisplay'
import withMapComponent from 'util/redux-form/HOC/withMapComponent'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'
import withPath from 'util/redux-form/HOC/withPath'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { withProps, branch } from 'recompose'
import { formValueSelector } from 'redux-form/immutable'
import { connect } from 'react-redux'
import { getSelectedEntityConcreteType } from 'selectors/entities/entities'
import withAccessibilityInformation from 'util/redux-form/HOC/withAccessibilityInformation'
import withMissingCoordinatesLabel from 'util/redux-form/HOC/withMissingCoordinatesLabel'
import { getIsAccessibilityRegisterSet } from 'util/redux-form/fields/Accessibility/selectors'

const messages = defineMessages({
  addBtnTitle: {
    id : 'Routes.Channels.ServiceLocation.VisitingAddressCollection.AddButton.Title',
    defaultMessage: '+ Uusi käyntiosoite'
  },
  addNewBtnTitle: {
    id : 'Routes.Channels.ServiceLocation.VisitingAddressCollection.AddNewButton.Title',
    defaultMessage: 'Lisää osoite'
  },
  visitingAddressDisclaimer: {
    id : 'Routes.Channels.ServiceLocation.VisitingAddressCollection.DisclaimerMessage.Title',
    defaultMessage: 'Huomioi, että tämä lisäämäsi osoite liittyy tähän samaan palvelupaikkaan. Esimerkiksi toinen sisäänkäynti samaan kiinteistöön.' // eslint-disable-line
  },
  additionalInformationTitle: {
    id: 'Containers.Channels.Address.AdditionalInformation.Title',
    defaultMessage: 'Osoitteen lisätiedot'
  },
  additionalInformationPlaceholder: {
    id: 'Containers.Channels.Address.AdditionalInformation.Placeholder',
    defaultMessage: 'paikkamerkkiteksti'
  }
})

const additionalInformationProps = {
  title: messages.additionalInformationTitle,
  placeholder: messages.additionalInformationPlaceholder
}

class Address extends Component {
  render () {
    const {
      key,
      disableTypes,
      ...rest
    } = this.props
    return (
      <div key={key}>
        <AddressSwitch
          addressUseCase={addressUseCasesEnum.VISITING}
          additionalInformationProps={additionalInformationProps}
          disableTypes={disableTypes}
          {...rest}
        />
      </div>
    )
  }
}
Address.propTypes = {
  intl: intlShape,
  key: PropTypes.string,
  index: PropTypes.number,
  isReadOnly: PropTypes.bool,
  disableTypes: PropTypes.bool,
  entityConcreteType: PropTypes.string
}

const VisitingAddressTitleComponent = compose(
  injectIntl,
  connect(state => ({
    entityConcreteType: getSelectedEntityConcreteType(state)
  })),
  withFormStates,
  branch(({ entityConcreteType, isReadOnly, ...rest }) =>
    isReadOnly && entityConcreteType === entityConcreteTypesEnum.SERVICELOCATIONCHANNEL, withAccessibilityInformation),
  branch(({ visitingAddresses, isReadOnly, ...rest }) => {
    return isReadOnly && visitingAddresses && visitingAddresses.some(
      (address) => address && (address.get('streetType') || '').toLowerCase() !== 'foreign')
  }, withMissingCoordinatesLabel),
)(VisitingAddressTitle)

const VisitingAddressCollectionBase = compose(
  withFormStates,
  injectFormName,
  connect((state, { formName, ...rest }) => ({
    visitingAddresses: formValueSelector(formName)(state, 'visitingAddresses'),
    defaultStreetTitle: true
  })),
  branch(({ visitingAddresses, ...rest }) => {
    return visitingAddresses && visitingAddresses.some(
      (address) => address && (address.get('streetType') || '').toLowerCase() !== 'foreign')
  }, withMapComponent),
  withProps(props => ({
    comparable: true
  })),
  asCollection({
    name: 'visitingAddress',
    pluralName: 'visitingAddresses',
    simple: true,
    stacked: true,
    dragAndDrop: true,
    addBtnTitle: <FormattedMessage {...messages.addBtnTitle} />,
    addNewBtnTitle: <FormattedMessage {...messages.addNewBtnTitle} />,
    RemoveButton: AccessibilityRemoveButton,
    Title: VisitingAddressTitleComponent
  }),
  asSection(),
  withPath,
  withLanguageKey,
  injectIntl
)(Address)

export const VisitingAddressCollectionSL = compose(
  withErrorDisplay('visitingAddresses'),
  withProps(props => ({
    disableTypes: true,
    disclaimerMessage: <FormattedMessage {...messages.visitingAddressDisclaimer} />
  })),
  asGroup({
    title: commonMessages.visitingAddresses,
    required: true
  }),
  connect(state => {
    const isAccessibilityRegisterSet = getIsAccessibilityRegisterSet(state)
    return {
      draggable: !isAccessibilityRegisterSet
    }
  })
)(VisitingAddressCollectionBase)

export const VisitingAddressCollectionORG = compose(
  asContainer({
    title: commonMessages.visitingAddresses,
    dataPaths: 'visitingAddresses'
  })
)(VisitingAddressCollectionBase)
