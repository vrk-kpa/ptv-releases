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
import PropTypes from 'prop-types'
import { reduxForm } from 'redux-form/immutable'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { getElectronicChannel } from 'Routes/Channels/routes/Electronic/selectors'
import { getWebPageChannel } from 'Routes/Channels/routes/WebPage/selectors'
import { getPrintableForm } from 'Routes/Channels/routes/PrintableForm/selectors'
import { getPhoneChannel } from 'Routes/Channels/routes/Phone/selectors'
import { getServiceLocationChannel } from 'Routes/Channels/routes/ServiceLocation/selectors'
import { getService } from 'Routes/Service/selectors'
import { getGeneralDescription } from 'Routes/GeneralDescription/selectors'
import { getOrganizationFormInitialValues } from 'Routes/Organization/selectors'
import { getServiceCollection } from 'Routes/ServiceCollection/selectors'
import { formTypesEnum, entityConcreteTypesEnum } from 'enums'
import { getAttachedGDId } from './selectors'
import AvailableLanguages from 'appComponents/AvailableLanguages'
import {
  ModalTitle,
  ModalContent,
  Spinner
} from 'sema-ui-components'

import PreviewDialogContent from './PreviewDialogContent'

const PreviewDialogForm = ({
  isLoading,
  languageAvailabilities,
  titleText,
  publishingStatusId,
  ...props
}) => (
  <div>
    {!isLoading &&
      <ModalTitle title={titleText}>
        <AvailableLanguages availableLanguages={languageAvailabilities} publishingStatusId={publishingStatusId} />
      </ModalTitle>
    }
    <ModalContent>
      {isLoading &&
        <Spinner /> ||
        <PreviewDialogContent
          {...props}
        />
      }
    </ModalContent>
  </div>
)

PreviewDialogForm.propTypes = {
  isLoading: PropTypes.bool,
  languageAvailabilities: PropTypes.any,
  titleText: PropTypes.string,
  publishingStatusId: PropTypes.string
}

export default compose(
  connect((state, ownProps) => {
    let initialValues = {}
    let attachedGDId = null
    switch (ownProps.concreteType) {
      case entityConcreteTypesEnum.ELECTRONICCHANNEL:
        initialValues = getElectronicChannel(state, ownProps)
        break
      case entityConcreteTypesEnum.WEBPAGECHANNEL:
        initialValues = getWebPageChannel(state, ownProps)
        break
      case entityConcreteTypesEnum.PRINTABLEFORMCHANNEL:
        initialValues = getPrintableForm(state, ownProps)
        break
      case entityConcreteTypesEnum.PHONECHANNEL:
        initialValues = getPhoneChannel(state, ownProps)
        break
      case entityConcreteTypesEnum.SERVICELOCATIONCHANNEL:
        initialValues = getServiceLocationChannel(state, ownProps)
        break
      case entityConcreteTypesEnum.SERVICE:
        initialValues = getService(state, ownProps)
        attachedGDId = getAttachedGDId(state, ownProps)
        break
      case entityConcreteTypesEnum.GENERALDESCRIPTION:
        initialValues = getGeneralDescription(state, ownProps)
        break
      case entityConcreteTypesEnum.ORGANIZATION:
        initialValues = getOrganizationFormInitialValues(state, ownProps)
        break
      case entityConcreteTypesEnum.SERVICECOLLECTION:
        initialValues = getServiceCollection(state, ownProps)
        break
      default:
        initialValues
        break
    }
    return {
      initialValues,
      attachedGDId
    }
  }),
  reduxForm({
    form: formTypesEnum.PREVIEW,
    enableReinitialize: true
  })
)(PreviewDialogForm)
