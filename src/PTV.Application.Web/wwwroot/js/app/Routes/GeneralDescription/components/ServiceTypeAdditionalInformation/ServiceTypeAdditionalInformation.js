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
import React from 'react'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import asGroup from 'util/redux-form/HOC/asGroup'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import {
  EditorDescription
} from 'util/redux-form/fields'
import { getGdTypeCodeSelected } from './selectors'

export const messages = defineMessages({
  obligationTitle:{
    id: 'Containers.GeneralDescription.ServiceType.AdditionalInfo.Title',
    defaultMessage: 'Lupaan tai velvoitteeseen liittyvät lisätiedot'
  },
  qualificationTitle:{
    id: 'Containers.GeneralDescription.ServiceType.ProfessionalAdditionalInfo.Title',
    defaultMessage: 'Ammattipätevyyteen liittyvät lisätiedot'
  },
  deadlineTitle:{
    id: 'Containers.GeneralDescription.ServiceType.AdditionalInfo.DeadLine.Title',
    defaultMessage: 'Määräaika'
  },
  deadlinePlaceholder:{
    id: 'Containers.GeneralDescription.ServiceType.AdditionalInfo.DeadLine.Placeholder',
    defaultMessage: 'Kirjoita määräaikaan liittyvät tiedot.'
  },
  deadlineTootltip:{
    id: 'Containers.GeneralDescription.ServiceType.AdditionalInfo.DeadLine.Tooltip',
    defaultMessage: 'Kuvaa lyhyesti, jos lupaa on haettava / ilmoitus tai rekisteröinti on tehtävä tiettyyn määräaikaan mennessä. Esim. Ilmoitus on tehtävä viimeistään neljä (4) viikkoa ennen toiminnan aloittamista tai olennaista muuttamista.'
  },
  processingTimeTitle:{
    id: 'Containers.GeneralDescription.ServiceType.AdditionalInfo.ProcessingTime.Title',
    defaultMessage: 'Käsittelyaika'
  },
  processingTimePlaceholder:{
    id: 'Containers.GeneralDescription.ServiceType.AdditionalInfo.ProcessingTime.Placeholder',
    defaultMessage: 'Kirjoita käsittelyaikaan liittyvät tiedot.'
  },
  processingTimeTootltip:{
    id: 'Containers.GeneralDescription.ServiceType.AdditionalInfo.ProcessingTime.Tootltip',
    defaultMessage: 'Kuvaa lyhyesti, miten kauan asian käsittely viranomaistasolla kestää. Esim. Lupa käsitellään kuuden (6) kuukauden kuluessa hakemuksen vastaanottamisesta tai, jos hakemus on puutteellinen, siitä kun hakija on antanut asian ratkaisemista varten tarvittavat asiakirjat ja selvitykset.'
  },
  validityTimeTitle:{
    id: 'Containers.GeneralDescription.ServiceType.AdditionalInfo.ValidityTime.Title',
    defaultMessage: 'Voimassaoloaika'
  },
  validityTimePlaceholder:{
    id: 'Containers.GeneralDescription.ServiceType.AdditionalInfo.ValidityTime.Placeholder',
    defaultMessage: 'Kirjoita voimassaoloaikaan liittyvät tiedot.'
  },
  validityTimeTootltip:{
    id: 'Containers.GeneralDescription.ServiceType.AdditionalInfo.ValidityTime.Tootltip',
    defaultMessage: 'Kuvaa lyhyesti tieto siitä, miten kauan lupa/ilmoitus/rekisteröinti on voimassa. Esim. Lupa on voimassa kolme (3) vuotta. / Lupa on voimassa toistaiseksi.'
  }
})

const ServiceTypeAdditionalInformation = ({
  intl: { formatMessage }
}) => {
  return (
    <div>
      <EditorDescription
        name='deadLineInformation'
        label={formatMessage(messages.deadlineTitle)}
        placeholder={formatMessage(messages.deadlinePlaceholder)}
        tooltip={formatMessage(messages.deadlineTootltip)}
      />
      <EditorDescription
        name='processingTimeInformation'
        label={formatMessage(messages.processingTimeTitle)}
        placeholder={formatMessage(messages.processingTimePlaceholder)}
        tooltip={formatMessage(messages.processingTimeTootltip)}
      />
      <EditorDescription
        name='validityTimeInformation'
        label={formatMessage(messages.validityTimeTitle)}
        placeholder={formatMessage(messages.validityTimePlaceholder)}
        tooltip={formatMessage(messages.validityTimeTootltip)}
      />
    </div>
  )
}

ServiceTypeAdditionalInformation.propTypes = {
  intl: intlShape
}

export default compose(
  injectFormName,
  injectIntl,
  connect((state, ownProps) => {
    const gdTypeCode = getGdTypeCodeSelected(state, ownProps)
    return {
      title: gdTypeCode === 'obligation' && messages.obligationTitle ||
        gdTypeCode === 'qualification' && messages.qualificationTitle || ''
    }
  }),
  asGroup({
    title: messages.obligationTitle
  })
)(ServiceTypeAdditionalInformation)
