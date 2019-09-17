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
import { compose } from 'redux'
import { getOrganizationsByIds } from '../../selectors'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import withShowMore from 'util/redux-form/HOC/withShowMore'
import cx from 'classnames'
import styles from './styles.scss'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { localizeList } from 'appComponents/Localize'

const messages = defineMessages({
  responsibleOrganizationsTitle: {
    id: 'Routes.FrontPage.Search.Components.ResponsibleOrganizationsTitle',
    defaultMessage: 'Muut vastuuorganisaatiot'
  },
  mainResponsibleOrganizationTitle: {
    id: 'Routes.FrontPage.Search.Components.MainResponsibleOrganizationTitle',
    defaultMessage: 'Päävastuuorganisaatio'
  },
  producerOrganizationsTitle: {
    id: 'Routes.FrontPage.Search.Components.ProducerOrganizationsTitle',
    defaultMessage: 'Tuottajaorganisaatiot'
  }
})

const OrganizationsMoreContent = compose(
  injectIntl
)(({ items, visibleItems, intl: { formatMessage }, ...rest }) => {
  const responsibleOrgs = items.filter(item => item.type === 'responsible')
  const producerOrgs = items.filter(item => item.type === 'producer')
  return (
    <div className={styles.organizationListWrap}>
      <div className={styles.organizationList}>
        <strong>{formatMessage(messages.mainResponsibleOrganizationTitle)}</strong>
        <ul>
          {visibleItems.map(item => {
            const org = item.org
            return org && (
              <li key={org.get('id')}>{org.get('displayName') || ''}</li>
            )
          })}
        </ul>
      </div>
      {responsibleOrgs.size > 0 &&
        <div className={styles.organizationList}>
          <strong>{formatMessage(messages.responsibleOrganizationsTitle)}</strong>
          <ul>
            {responsibleOrgs.map(item => {
              const org = item.org
              return org && (
                <li key={org.get('id')}>{org.get('displayName') || ''}</li>
              )
            })}
          </ul>
        </div>
      }
      {producerOrgs.size > 0 &&
        <div className={styles.organizationList}>
          <strong>{formatMessage(messages.producerOrganizationsTitle)}</strong>
          <ul>
            {producerOrgs.map(item => {
              const org = item.org
              return org && (
                <li key={org.get('id')}>{org.get('displayName') || ''}</li>
              )
            })}
          </ul>
        </div>
      }
    </div>
  )
})

OrganizationsMoreContent.propTypes = {
  items: ImmutablePropTypes.list.isRequired,
  visibleItems: ImmutablePropTypes.list.isRequired,
  intl: intlShape
}

const RelatedOrganizations = ({
  items,
  componentClass
}) => {
  const organizationClass = cx(
    componentClass,
    styles.relatedOrganizations
  )
  return (
    <div className={organizationClass}>
      {items.map(item => {
        const org = item.org
        return org && (
          <div className={styles.item} key={org.get('id')}>
            {org.get('displayName') || ''}
          </div>
        )
      })}
    </div>
  )
}
RelatedOrganizations.propTypes = {
  items: ImmutablePropTypes.list.isRequired,
  componentClass: PropTypes.string
}
export default compose(
  connect((state, { organizationIds, producerIds }) => {
    let organizations = getOrganizationsByIds(organizationIds)(state)
    const orgItems = organizations.map(org => ({ org: org, type: 'responsible' }))
    let producers = getOrganizationsByIds(producerIds)(state)
    const producerItems = producers.map(org => ({ org: org, type: 'producer' }))
    const items = orgItems.concat(producerItems)
    const itemsToLocalize = organizations.concat(producers)
    return {
      items,
      itemsToLocalize
    }
  }),
  withShowMore({
    countOfVisible: 1,
    MoreContent: props => <OrganizationsMoreContent {...props} />,
    className: styles.moreComponent,
    triggerCSS: styles.trigger
  }),
  localizeList({
    input: 'itemsToLocalize',
    output: 'organizations'
  }),
)(RelatedOrganizations)
