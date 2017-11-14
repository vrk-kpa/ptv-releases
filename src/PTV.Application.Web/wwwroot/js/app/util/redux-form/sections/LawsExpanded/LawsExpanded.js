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
  Name,
  UrlChecker
} from 'util/redux-form/fields'
import { asSection, asCollection, asGroup } from 'util/redux-form/HOC'
import { defineMessages, injectIntl, FormattedMessage } from 'react-intl'

export const lawsMessages = defineMessages({
  title: {
    id: 'Containers.Services.AddService.Step1.Laws.Title',
    defaultMessage: 'Linkki lakitietoihin'
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
  }
})

const Laws = ({ intl: { formatMessage } }) => {
  return (
    <div>
      <div className='form-row'>
        <Name
          name='name'
          placeholder=''
          skipValidation
          tooltip={formatMessage(lawsMessages.nameTooltip)} />
      </div>
      <div className='form-row'>
        <UrlChecker />
      </div>
    </div>
  )
}

Laws.propTypes = {
  intl: PropTypes.object.isRequired
}

export default compose(
  asGroup({ title: lawsMessages.title }),
  asCollection({
    name: 'law',
    simple: true,
    addBtnTitle: <FormattedMessage {...lawsMessages.addBtnTitle} />,
    confirmDeleteMessage: lawsMessages.promptForRemoval
  }),
  asSection('laws'),
  injectIntl,
)(Laws)
