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
// import PropTypes from 'prop-types'
import { compose } from 'redux'
import {
  ServiceType,
  Organization,
  ServiceClass,
  OntologyTerm,
  AreaInformationType,
  TargetGroupSelect,
  LifeEvent,
  IndustrialClass
} from 'util/redux-form/fields'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import styles from './styles.scss'

const messages = defineMessages({
  areaInformationTypeTitle: {
    id: 'AppComponents.Filters.ServiceFilters.AreaInformationType.Title',
    defaultMessage: 'Alue'
  }
})

const ServiceFilters = ({
  intl: { formatMessage, locale }
}) => {
  return (
    <div className={styles.filterWrap}>
      <ServiceType
        name='serviceTypeId'
        locale={locale}
        labelPosition='inside'
      />
      <Organization
        name='organizationId'
        labelPosition='inside'
        skipValidation
      />
      <ServiceClass
        name='serviceClasses'
        locale={locale}
        labelPosition='inside'
      />
      <OntologyTerm
        name='ontologyTerms'
        locale={locale}
        labelPosition='inside'
      />
      <AreaInformationType
        name='areaInformationTypes'
        locale={locale}
        labelPosition='inside'
        label={formatMessage(messages.areaInformationTypeTitle)}
      />
      <TargetGroupSelect
        name='targetGroups'
        locale={locale}
        labelPosition='inside'
      />
      <LifeEvent
        name='lifeEvents'
        locale={locale}
        labelPosition='inside'
      />
      <IndustrialClass
        name='industrialClasses'
        locale={locale}
        labelPosition='inside'
      />
    </div>
  )
}

ServiceFilters.propTypes = {
  intl: intlShape
}

export default compose(
  injectIntl
)(ServiceFilters)
