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
import { FormSection } from 'redux-form/immutable'
import { compose } from 'redux'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import {
  BusinessId,
  OrganizationId,
  Name,
  OrganizationAlternativeName,
  OrganizationType,
  Description,
  IsAlternateNameUsed,
  MunicipalityCode,
  ShortDescription
} from 'util/redux-form/fields'
import {
  AreaInformationExpanded
} from 'util/redux-form/sections'
import {
   asSection,
   injectFormName,
   withFormStates
} from 'util/redux-form/HOC'
import OrganizationWithGroupLevel from '../OrganizationWithGroupLevel'
import {
  isOrganizationTypeMunicipalitySelected,
  isOrganizationTypeRegionalSelected,
  isAreaInformationVisible
} from './selectors'
import { injectIntl, intlShape, defineMessages } from 'react-intl'

export const organizationMessages = defineMessages({
  shortDescriptionTitle: {
    id: 'Routes.Organization.Components.OrganizationFormBasic.ShortDescription.Title',
    defaultMessage: 'Lyhyt kuvaus'
  },
  shortDescriptionPlaceholder: {
    id: 'Routes.Organization.Components.OrganizationFormBasic.ShortDescription.Placeholder',
    defaultMessage: 'Kirjoita lyhyt tiivistelmä hakukoneita varten.'
  },
  shortDescriptionTooltip: {
    id: 'Routes.Organization.Components.OrganizationFormBasic.ShortDescription.Tooltip',
    defaultMessage: 'Kirjoita lyhyt kuvaus eli tiivistelmä palvelun keskeisestä sisällöstä. Lyhyt kuvaus esitetään hakutuloksissa, ja se auttaa hakukoneita löytämään palvelun.'
  },
  nameTitle: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Organization.Name.Title',
    defaultMessage: 'Organisaation nimi'
  },
  namePlaceholder: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Organization.Name.Placeholder',
    defaultMessage: 'Kirjoita organisaation tai alaorganisaation nimi'
  },
  areaInformationTitle: {
    id: 'Containers.Manage.Organizations.Manage.Step1.OrganizationArea.Title',
    defaultMessage: 'Alue, joilla organisaatio pääsääntöisesti toimii'
  },
  areaInformationTooltip: {
    id: 'Containers.Manage.Organizations.Manage.Step1.OrganizationArea.Tooltip',
    defaultMessage: 'Alue, joilla organisaatio pääsääntöisesti toimii'
  },
  organizationDescriptionTooltip: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Organization.Description.Tooltip',
    defaultMessage: 'Kirjoita organisaatiolle lyhyt, käyttäjäystävällinen kuvaus. Mikä tämä (ala)organisaatio on ja mitä se tekee organisaation palveluita käyttävän asiakkaan näkökulmasta? Alä mainosta vaan kuvaa neutraalisti organisaation ydintehtävä. Kuvaus saa olla korkeintaan 500 merkkiä pitkä.'
  },
  organizationDescriptionTitle: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Organization.Description.Title',
    defaultMessage: 'Kuvaus'
  },
  organizationDescriptionPlaceholder: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Organization.Description.Placeholder',
    defaultMessage: 'Anna palvelulle kuvaus.'
  },
  businessLabel: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Organization.BusinessId.Title',
    defaultMessage: 'Y-tunnus'
  },
  businessTooltip: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Organization.BusinessId.Tooltip',
    defaultMessage: 'Kirjota kenttään organisaatiosi Y-tunnus. Jos et muista sitä, voit hakea Y-tunnuksen Yritys- ja yhteisötietojärjestelmästä (YTJ) [https://www.ytj.fi/yrityshaku.aspx?path=1547;1631;1678&kielikoodi=1]. | Tarkista oman organisaatiosi Y-tunnuksen käytön käytäntö: Joillain organisaatioilla on vain yksi yhteinen Y-tunnus, toisilla myös alaorganisaatioilla on omat Y-tunnuksensa. Varmista, että annat alaorganisaatiolle oikean Y-tunnuksen.'
  },
  businessPlaceholder: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Organization.BusinessId.Placeholder',
    defaultMessage: 'paikkamerkkiteksti'
  }
})

const Business = compose(
  asSection('business')
)(props => <BusinessId {...props} name='code' />)
class OrganizationFormBasic extends Component {
  static defaultProps = {
    name: 'organizationBasic'
  }
  render () {
    const {
      isMunicipalitySelected,
      isAreaInformationVisible,
      isRegionalSelected,
      isCompareMode,
      intl: { formatMessage }
     } = this.props

    const basicCompareModeClass = isCompareMode ? 'col-lg-24' : 'col-lg-12'
    const indentCompareModeClass = isCompareMode ? 'col-lg-24 mb-4' : 'col-lg-12 mb-4 mb-lg-0'
    return (
      <div>
        <div className='form-row'>
          <OrganizationWithGroupLevel />
        </div>
        <div className='form-row'>
          <div className='row'>
            <div className={indentCompareModeClass}>
              <OrganizationType />
            </div>
            <div className={basicCompareModeClass}>
              {isMunicipalitySelected && <MunicipalityCode />}
            </div>
          </div>
        </div>

        <div className='form-row'>
          { isAreaInformationVisible &&
            <AreaInformationExpanded
              hideType={isRegionalSelected}
              singleSelect
              title={formatMessage(organizationMessages.areaInformationTitle)}
              tooltip={formatMessage(organizationMessages.areaInformationTooltip)} />
          }
        </div>

        <div className='form-row'>
          <div className='row'>
            <div className={indentCompareModeClass}>
              <Name label={formatMessage(organizationMessages.nameTitle)}
                placeholder={formatMessage(organizationMessages.namePlaceholder)}
                required
              />
            </div>
            <div className={basicCompareModeClass}>
              <OrganizationAlternativeName />
              <div className='mt-2'>
                <IsAlternateNameUsed />
              </div>
            </div>
          </div>
        </div>
        <div className='form-row'>
          <div className='row'>
            <div className={indentCompareModeClass}>
              <Business
                label={formatMessage(organizationMessages.businessLabel)}
                tooltip={formatMessage(organizationMessages.businessTooltip)}
                placeholder={formatMessage(organizationMessages.businessPlaceholder)} />
            </div>
            <div className={basicCompareModeClass}>
              <OrganizationId />
            </div>
          </div>
        </div>

        <div className='form-row'>
          <div className='row'>
            <div className={basicCompareModeClass}>
              <ShortDescription label={formatMessage(organizationMessages.shortDescriptionTitle)}
                placeholder={formatMessage(organizationMessages.shortDescriptionPlaceholder)}
                tooltip={formatMessage(organizationMessages.shortDescriptionTooltip)}
                multiline
                rows={3}
                counter
                maxLength={150}
                required />
            </div>
          </div>
        </div>

        <div className='form-row'>
          <Description
            label={formatMessage(organizationMessages.organizationDescriptionTitle)}
            tooltip={formatMessage(organizationMessages.organizationDescriptionTooltip)}
            placeholder={formatMessage(organizationMessages.organizationDescriptionPlaceholder)} />
        </div>
      </div>
    )
  }
}

OrganizationFormBasic.propTypes = {
  isMunicipalitySelected: PropTypes.bool.isRequired,
  isAreaInformationVisible: PropTypes.bool.isRequired,
  isRegionalSelected: PropTypes.bool.isRequired,
  isCompareMode: PropTypes.bool,
  intl: intlShape
}

export default compose(
  injectFormName,
  withFormStates,
  connect((state, ownProps) => ({
    isMunicipalitySelected: isOrganizationTypeMunicipalitySelected(state, ownProps),
    isAreaInformationVisible: isAreaInformationVisible(state, ownProps),
    isRegionalSelected: isOrganizationTypeRegionalSelected(state, ownProps)
  })),
  injectIntl)(OrganizationFormBasic)
