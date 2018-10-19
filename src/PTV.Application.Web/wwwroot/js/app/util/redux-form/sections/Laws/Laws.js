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
import { withProps } from 'recompose'
import {
  Name,
  UrlChecker
} from 'util/redux-form/fields'
import asGroup from 'util/redux-form/HOC/asGroup'
import asCollection from 'util/redux-form/HOC/asCollection'
import PromptRemoveButton from 'util/redux-form/HOC/asCollection/PromptRemoveButton'
import asSection from 'util/redux-form/HOC/asSection'
import { defineMessages, injectIntl, FormattedMessage, intlShape } from 'util/react-intl'
import LawTitle from './LawTitle'

export const lawsMessages = defineMessages({
  title: {
    id: 'Containers.Services.AddService.Step1.Laws.Title',
    defaultMessage: 'Linkki lakitietoihin'
  },
  lawNameTitle: {
    id: 'Containers.Services.AddService.Step1.Laws.Name.Title',
    defaultMessage: 'Nimi'
  },
  lawNamePlaceholder: {
    id: 'Containers.Services.AddService.Step1.Laws.Name.Placeholder',
    defaultMessage: 'Kirjoita lain nimi',
    description: {
      en: 'Enter the name of the law'
    }
    // Enter the name of the law
  },
  nameTooltip: {
    id: 'Containers.Services.AddService.Step1.Laws.Name.Tooltip',
    defaultMessage: 'Anna palvelupisteen verkkosivuille havainnollinen nimi.'
  },
  addBtnTitle: {
    id : 'Util.ReduxForm.Sections.Laws.AddButton.Title',
    defaultMessage: '+ Uusi linkki'
  },
  promptForRemoval: {
    id : 'Util.ReduxForm.Sections.Laws.CrossIcon.PromptTitle',
    defaultMessage: 'Haluatko poistaa elementin? Poistaminen vaikuttaa muihin kieliversioihin.'
  },
  lawUrlLabel: {
    id: 'Containers.Services.AddService.Step1.Laws.Url.Title',
    defaultMessage: 'Verkko-osoite',
    description: 'Containers.Channels.AddElectronicChannel.Step1.UrlChecker.Title'
  },
  lawUrlTooltip: {
    id: 'Containers.Services.AddService.Step1.Laws.Url.Tooltip',
    defaultMessage: 'Verkko-osoite',
    description: 'Containers.Channels.AddElectronicChannel.Step1.AttachmentUrl.Tooltip'
  },
  lawUrlPlaceholder: {
    id: 'Containers.Services.AddService.Step1.Laws.Url.Placeholder',
    defaultMessage: 'Verkko-osoite',
    description: 'Containers.Channels.AddElectronicChannel.Step1.AttachmentUrl.Placeholder'
  }
})

const LawTitleComponent = compose(
  injectIntl
)(LawTitle)

const Laws = ({ intl: { formatMessage } }) => {
  return (
    <div>
      <div className='collection-form-row'>
        <Name
          name='name'
          placeholder={formatMessage(lawsMessages.lawNamePlaceholder)}
          title={formatMessage(lawsMessages.lawNameTitle)}
          skipValidation
          tooltip={formatMessage(lawsMessages.nameTooltip)}
          maxLength={100}
          noTranslationLock
        />
      </div>
      <div className='collection-form-row'>
        <UrlChecker
          label={formatMessage(lawsMessages.lawUrlLabel)}
          tooltip={formatMessage(lawsMessages.lawUrlTooltip)}
          placeholder={formatMessage(lawsMessages.lawUrlPlaceholder)}
          />
      </div>
    </div>
  )
}

Laws.propTypes = {
  intl: intlShape.isRequired
}

export default compose(
  asGroup({ title: lawsMessages.title }),
  withProps(props => ({
    comparable: true
  })),
  asCollection({
    name: 'law',
    addBtnTitle: <FormattedMessage {...lawsMessages.addBtnTitle} />,
    dragAndDrop: true,
    RemoveButton: PromptRemoveButton,
    simple: true,
    stacked: true,
    Title: LawTitleComponent
  }),
  asSection('laws'),
  injectIntl,
)(Laws)
