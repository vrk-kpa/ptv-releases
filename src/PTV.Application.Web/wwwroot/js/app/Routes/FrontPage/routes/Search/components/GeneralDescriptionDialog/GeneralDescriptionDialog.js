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
import { ModalContent } from 'sema-ui-components'
import ModalDialog from 'appComponents/ModalDialog'
import { injectIntl, defineMessages } from 'react-intl'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { getGeneralDescription } from 'Routes/GeneralDescription/actions'
import { getNameLocale,
  getAlternateNameLocale,
  getShortDescriptionLocale,
  getDescriptionLocale,
  getGeneralDescriptionServiceClasses,
  getGeneralDescriptionSubTargetGroups,
  getGeneralDescriptionTargetGroups,
  getGeneralDescriptionOntologyTerms
} from 'Routes/GeneralDescription/selectors'
import { getIsFetchingForId } from 'Containers/Common/Selectors'
import { LocalizedTextList } from 'Containers/Common/localizedData'
import { PTVPreloader, PTVTextEditorNotEmpty } from 'Components'

export const messages = defineMessages({
  detailHeaderTitle: {
    id: 'GeneralDescriptions.Detail.Dialog.Header.Title',
    defaultMessage: 'Pohjakuvauksen tiedot'
  },
  detailNameTitle: {
    id: 'GeneralDescriptions.Detail.Dialog.Name.Title',
    defaultMessage: 'Nimi'
  },
  detailAlternateNameTitle: {
    id: 'GeneralDescriptions.Detail.Dialog.AlternateName.Title',
    defaultMessage: 'Vaihtoehtoinen nimi'
  },
  detailShortDescriptionTitle: {
    id: 'GeneralDescriptions.Detail.Dialog.ShortDescription.Title',
    defaultMessage: 'Tiivistelmä'
  },
  detailDescriptionTitle: {
    id: 'GeneralDescriptions.Detail.Dialog.Description.Title',
    defaultMessage: 'Kuvaus'
  },
  detailTargetGroupTitle: {
    id: 'GeneralDescriptions.Detail.Dialog.TargetGroup.Title',
    defaultMessage: 'Pääkohderyhmä'
  },
  detailSubTargetGroupTitle: {
    id: 'GeneralDescriptions.Detail.Dialog.SubTargetGroup.Title',
    defaultMessage: 'Alakohderyhmä'
  },
  detailServiceClassTitle: {
    id: 'GeneralDescriptions.Detail.Dialog.ServiceClass.Title',
    defaultMessage: 'Palveluluokka'
  },
  detailOntologyTermTitle: {
    id: 'GeneralDescriptions.Detail.Dialog.OntologyTerm.Title',
    defaultMessage: 'Asiasanat'
  }
})

const GeneralDescriptionDialog = ({
  intl: { formatMessage },
  generalDescriptionId,
  isFetching,
  name,
  alternateName,
  shortDescription,
  description,
  getGeneralDescription,
  languageCode,
  ...rest
}) => {
  const handleOnAfterOpen = () =>
    getGeneralDescription(generalDescriptionId, languageCode)

  return (
    <ModalDialog onAfterOpen={handleOnAfterOpen}
      name={generalDescriptionId}
      title={formatMessage(messages.detailHeaderTitle)}
      contentLabel='General description' {...rest} >
      <ModalContent>
        { isFetching ? <PTVPreloader />
        : <div>
          <label><strong>{formatMessage(messages.detailNameTitle)}</strong></label>
          <p>{name}</p>
          <label><strong>{formatMessage(messages.detailAlternateNameTitle)}</strong></label>
          <p>{alternateName}</p>
          <label><strong>{formatMessage(messages.detailShortDescriptionTitle)}</strong></label>
          <p>{shortDescription}</p>
          <label><strong>{formatMessage(messages.detailDescriptionTitle)}</strong></label>
          <p>{<PTVTextEditorNotEmpty
            value={description}
            name='statutoryDescription'
            readOnly
            />}</p>
          <label><strong>{formatMessage(messages.detailTargetGroupTitle)}</strong></label>
          <p>{<LocalizedTextList
            getListSelector={getGeneralDescriptionTargetGroups}
            property="name"
            generalDescriptionId={generalDescriptionId}
            />}</p>
          <label><strong>{formatMessage(messages.detailSubTargetGroupTitle)}</strong></label>
          <p>{<LocalizedTextList
            getListSelector={getGeneralDescriptionSubTargetGroups}
            property="name"
            generalDescriptionId={generalDescriptionId}
            />}</p>
          <label><strong>{formatMessage(messages.detailServiceClassTitle)}</strong></label>
          <p>{<LocalizedTextList
            getListSelector={getGeneralDescriptionServiceClasses}
            property="name"
            generalDescriptionId={generalDescriptionId}
            />}</p>
          <label><strong>{formatMessage(messages.detailOntologyTermTitle)}</strong></label>
          <p>{<LocalizedTextList
            getListSelector={getGeneralDescriptionOntologyTerms}
            property="name"
            generalDescriptionId={generalDescriptionId}
            />}</p>
        </div>}
      </ModalContent>
    </ModalDialog>
  )
}

GeneralDescriptionDialog.propTypes = {
  intl: PropTypes.object.isRequired,
  getGeneralDescription: PropTypes.func.isRequired,
  actions: PropTypes.object.isRequired,
  generalDescriptionId: PropTypes.string.isRequired,
  name: PropTypes.string.isRequired,
  alternateName: PropTypes.string.isRequired,
  shortDescription: PropTypes.string.isRequired,
  description: PropTypes.string.isRequired,
  languageCode: PropTypes.string.isRequired,
  isFetching: PropTypes.bool.isRequired
}

export default compose(
  connect((state, ownProps) => ({
    isFetching: getIsFetchingForId(state, { keyToState: 'generalDescription', id: ownProps.generalDescriptionId }),
    name: getNameLocale(state, ownProps),
    alternateName: getAlternateNameLocale(state, ownProps),
    shortDescription: getShortDescriptionLocale(state, ownProps),
    description: getDescriptionLocale(state, ownProps)
  }), { getGeneralDescription }
), injectIntl)(GeneralDescriptionDialog)
