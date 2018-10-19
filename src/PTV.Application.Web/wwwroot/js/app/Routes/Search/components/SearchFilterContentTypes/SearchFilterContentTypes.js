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
import { connect } from 'react-redux'
import { Field, change } from 'redux-form/immutable'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { ContentTypeSearchTree } from 'util/redux-form/fields'
import { RenderDropdownFilter } from 'util/redux-form/renders'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { OrderedSet } from 'immutable'

const messages = defineMessages({
  title: {
    id: 'FrontPage.SearchContentSwitcher.Title',
    defaultMessage: 'Sisältotyyppi'
  },
  service: {
    id: 'FrontPage.SearchContentSwitcher.Services.Title',
    defaultMessage: 'Palvelukuvaus'
  },
  serviceService: {
    id: 'FrontPage.SearchContentSwitcher.Services.Service.Title',
    defaultMessage: 'Palvelu'
  },
  serviceProfessional: {
    id: 'FrontPage.SearchContentSwitcher.Services.ProfessionalQualification.Title',
    defaultMessage: 'Ammattipätevyys'
  },
  servicePermit: {
    id: 'FrontPage.SearchContentSwitcher.Services.PermitOrOtherObligation.Title',
    defaultMessage: 'Lupa tai muu velvoite'
  },
  channel: {
    id: 'FrontPage.SearchContentSwitcher.Channels.Title',
    defaultMessage: 'Asiointikanava'
  },
  generalDescription: {
    id: 'FrontPage.SearchContentSwitcher.GeneralDescriptions.Title',
    defaultMessage: 'Pohjakuvaus'
  },
  organization: {
    id: 'FrontPage.SearchContentSwitcher.Organizations.Title',
    defaultMessage: 'Organisaatiokuvaus'
  },
  eChannel: {
    id: 'FrontPage.SearchContentSwitcher.ElectronicChannel.Title',
    defaultMessage: 'Verkkoasiointi'
  },
  webPage: {
    id: 'FrontPage.SearchContentSwitcher.WebChannel.Title',
    defaultMessage: 'Verkkosivu'
  },
  printableForm: {
    id: 'FrontPage.SearchContentSwitcher.PrintableFormChannel.Title',
    defaultMessage: 'Tulostettava lomake'
  },
  phone: {
    id: 'FrontPage.SearchContentSwitcher.PhoneChannel.Title',
    defaultMessage: 'Puhelinasiointi'
  },
  serviceLocation: {
    id: 'FrontPage.SearchContentSwitcher.ServiceLocationChannel.Title',
    defaultMessage: 'Palvelupiste'
  },
  serviceCollection: {
    id: 'FrontPage.SearchContentSwitcher.ServiceCollection.Title',
    defaultMessage: 'Palvelukokonaisuus'
  }
})

const renderContent = (options) => (all, props) => (
  <ContentTypeSearchTree
    filterName='contentTypeSearch'
    disabled={all}
    filterTree
    options={options}
    messages={messages}
    {...props}
  />
)

const ShowValue = compose(
  injectIntl
)(({ displayValue, firstValue, intl: { formatMessage }}) =>
  firstValue && formatMessage(messages[firstValue]) || displayValue
)

const SearchFilterContentTypes = ({
  intl: { formatMessage },
  setFormValue,
  ...rest
}) => {
  const resetSubFilters = () => {
    setFormValue('frontPageSearch', 'targetGroups', OrderedSet())
    setFormValue('frontPageSearch', 'industrialClasses', OrderedSet())
    setFormValue('frontPageSearch', 'serviceClasses', OrderedSet())
    setFormValue('frontPageSearch', 'lifeEvents', OrderedSet())
    setFormValue('frontPageSearch', 'ontologyTerms', OrderedSet())
  }
  return (
    <Field
      name='contentTypes'
      component={RenderDropdownFilter}
      content={renderContent()}
      title={formatMessage(messages.title)}
      ShowValueComponent={ShowValue}
      filterName={'contentTypeSearch'}
      ariaMultiSelectable
      fieldName='contentTypes'
      selectAllIfEmpty
      customClear={resetSubFilters}
      {...rest}
    />
  )
}

SearchFilterContentTypes.propTypes = {
  formName: PropTypes.string.isRequired,
  setFormValue: PropTypes.func.isRequired,
  intl: intlShape
}

export default compose(
  injectIntl,
  injectFormName,
  connect(null, {
    setFormValue: change
  })
)(SearchFilterContentTypes)
