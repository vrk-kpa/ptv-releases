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
import { defineMessages } from 'util/react-intl'

export default defineMessages({
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
    defaultMessage: 'Kirjoita lyhyt kuvaus eli tiivistelmä palvelun keskeisestä sisällöstä. Lyhyt kuvaus esitetään hakutuloksissa, ja se auttaa hakukoneita löytämään palvelun.' // eslint-disable-line
  },
  nameTitle: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Organization.Name.Title',
    defaultMessage: 'Organisaation nimi'
  },
  namePlaceholder: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Organization.Name.Placeholder',
    defaultMessage: 'Kirjoita organisaation tai alaorganisaation nimi'
  },
  nameTooltip: {
    id: 'Organization.Name.Tooltip',
    defaultMessage: 'Organization name tooltip placeholder'
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
    defaultMessage: 'Kirjoita organisaatiolle lyhyt, käyttäjäystävällinen kuvaus. Mikä tämä (ala)organisaatio on ja mitä se tekee organisaation palveluita käyttävän asiakkaan näkökulmasta? Alä mainosta vaan kuvaa neutraalisti organisaation ydintehtävä. Kuvaus saa olla korkeintaan 500 merkkiä pitkä.' // eslint-disable-line
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
    defaultMessage: 'Kirjota kenttään organisaatiosi Y-tunnus. Jos et muista sitä, voit hakea Y-tunnuksen Yritys- ja yhteisötietojärjestelmästä (YTJ) [https://www.ytj.fi/yrityshaku.aspx?path=1547;1631;1678&kielikoodi=1]. | Tarkista oman organisaatiosi Y-tunnuksen käytön käytäntö: Joillain organisaatioilla on vain yksi yhteinen Y-tunnus, toisilla myös alaorganisaatioilla on omat Y-tunnuksensa. Varmista, että annat alaorganisaatiolle oikean Y-tunnuksen.'  // eslint-disable-line
  },
  businessPlaceholder: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Organization.BusinessId.Placeholder',
    defaultMessage: 'paikkamerkkiteksti'
  },
  alternateNameTooltip: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Organization.AlternativeNumber.Tooltip',
    defaultMessage: 'esim. Kela'
  }
})
