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
import React, { Component } from 'react'
import { PropTypes } from 'prop-types'
import { AddressSwitch } from 'util/redux-form/sections'
import { addressUseCasesEnum } from 'enums'
import { compose } from 'redux'
import { withProps } from 'recompose'
import {
  injectIntl,
  intlShape,
  defineMessages,
  FormattedMessage
} from 'util/react-intl'
import asCollection from 'util/redux-form/HOC/asCollection'
import asContainer from 'util/redux-form/HOC/asContainer'
import asSection from 'util/redux-form/HOC/asSection'
import PostalAddressTitle from './PostalAddressTitle'
import commonMessages from 'util/redux-form/messages'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'
import withPath from 'util/redux-form/HOC/withPath'
import injectFormName from 'util/redux-form/HOC/injectFormName'

const messages = defineMessages({
  addBtnTitle: {
    id : 'Routes.Channels.ServiceLocation.PostalAddressCollection.AddButton.Title',
    defaultMessage: '+ Uusi postiosoite'
  },
  addNewBtnTitle: {
    id : 'Routes.Channels.ServiceLocation.PostalAddressCollection.AddNewButton.Title',
    defaultMessage: 'Lisää osoite'
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

class Address extends Component {
  additionalInformationProps = {
    title: this.props.intl.formatMessage(messages.additionalInformationTitle),
    placeholder: this.props.intl.formatMessage(messages.additionalInformationPlaceholder)
  }
  render () {
    const {
      key,
      index,
      ...rest
    } = this.props
    return (
      <div key={key}>
        <AddressSwitch
          index={index}
          addressUseCase={addressUseCasesEnum.POSTAL}
          additionalInformationProps={this.additionalInformationProps}
          mapDisabled
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
  setActiveIndex: PropTypes.func,
  entityConcreteType: PropTypes.string
}

const PostalAddressTitleComponent = compose(
  injectIntl,
)(PostalAddressTitle)

const PostalAddressCollection = compose(
  withFormStates,
  injectFormName,
  asContainer({
    title: commonMessages.postalAddresses,
    dataPaths: 'postalAddresses'
  }),
  withProps(props => ({
    comparable: true,
    defaultStreetTitle: true
  })),
  asCollection({
    name: 'postalAddress',
    pluralName: 'postalAddresses',
    simple: true,
    stacked: true,
    dragAndDrop: true,
    addBtnTitle: <FormattedMessage {...messages.addBtnTitle} />,
    addNewBtnTitle: <FormattedMessage {...messages.addNewBtnTitle} />,
    Title: PostalAddressTitleComponent
  }),
  asSection(),
  withPath,
  withLanguageKey,
  injectIntl
)(Address)

export default PostalAddressCollection
