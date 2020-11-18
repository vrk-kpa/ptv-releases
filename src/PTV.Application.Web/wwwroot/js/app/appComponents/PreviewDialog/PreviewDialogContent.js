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
import { compose } from 'redux'
import { injectIntl, intlShape } from 'util/react-intl'
import ElectronicChannelBasic from 'Routes/Channels/routes/Electronic/components/ElectronicChannelBasic'
import WebPageBasic from 'Routes/Channels/routes/WebPage/components/WebPageBasic'
import PrintableFormBasic from 'Routes/Channels/routes/PrintableForm/components/PrintableFormBasic'
import PhoneChannelBasic from 'Routes/Channels/routes/Phone/components/PhoneChannelBasic'
import ServiceLocationBasic from 'Routes/Channels/routes/ServiceLocation/components/ServiceLocationBasic'
import ServiceBasic from 'Routes/Service/components/ServiceBasic'
import ServiceCollectionBasic from 'Routes/ServiceCollection/components/ServiceCollectionBasic'
import GeneralDescriptionBasic from 'Routes/GeneralDescription/components/GeneralDescriptionBasic'
import OrganizationBasic from 'Routes/Organization/components/OrganizationFormBasic'
import OpeningHoursPreview from 'appComponents/OpeningHoursPreview/OpeningHoursPreview'
import OpeningHoursTitle from 'appComponents/OpeningHoursPreview/OpeningHoursTitle'
import ConnectedEntities from 'appComponents/ConnectedEntities'
import { entityConcreteTypesEnum, entityTypesEnum } from 'enums'

const PreviewDialogContent = ({
  concreteType,
  attachedGDId,
  intl: { formatMessage },
  ...rest
}) => {
  switch (concreteType) {
    case entityConcreteTypesEnum.ELECTRONICCHANNEL:
      return <div>
        <ElectronicChannelBasic />
        <OpeningHoursTitle />
        <OpeningHoursPreview preview />
        <ConnectedEntities
          type={entityTypesEnum.CHANNELS}
          id={rest.id}
        />
      </div>
    case entityConcreteTypesEnum.WEBPAGECHANNEL:
      return <div>
        <WebPageBasic />
        <ConnectedEntities
          type={entityTypesEnum.CHANNELS}
          id={rest.id}
        />
      </div>
    case entityConcreteTypesEnum.PRINTABLEFORMCHANNEL:
      return <div>
        <PrintableFormBasic />
        <ConnectedEntities
          type={entityTypesEnum.CHANNELS}
          id={rest.id}
        />
      </div>
    case entityConcreteTypesEnum.PHONECHANNEL:
      return <div>
        <PhoneChannelBasic />
        <OpeningHoursTitle />
        <OpeningHoursPreview preview />
        <ConnectedEntities
          type={entityTypesEnum.CHANNELS}
          id={rest.id}
        />
      </div>
    case entityConcreteTypesEnum.SERVICELOCATIONCHANNEL:
      return <div>
        <ServiceLocationBasic mapDisabled />
        <OpeningHoursTitle />
        <OpeningHoursPreview preview />
        <ConnectedEntities
          type={entityTypesEnum.CHANNELS}
          id={rest.id}
        />
      </div>
    case entityConcreteTypesEnum.SERVICE:
      return <div>
        <ServiceBasic generalDescriptionId={attachedGDId} />
        <ConnectedEntities
          type={entityTypesEnum.SERVICES}
          id={rest.id}
        />
      </div>
    case entityConcreteTypesEnum.GENERALDESCRIPTION:
      return <div>
        <GeneralDescriptionBasic />
        <ConnectedEntities
          type={entityTypesEnum.GENERALDESCRIPTIONS}
          id={rest.id}
        />
      </div>
    case entityConcreteTypesEnum.ORGANIZATION:
      return <OrganizationBasic />
    case entityConcreteTypesEnum.SERVICECOLLECTION:
      return <div>
        <ServiceCollectionBasic />
        <ConnectedEntities
          type={entityTypesEnum.SERVICECOLLECTIONS}
          id={rest.id}
        />
      </div>
    default:
      return <div>Entity not defined</div>
  }
}
PreviewDialogContent.propTypes = {
  concreteType: PropTypes.string,
  attachedGDId: PropTypes.string,
  intl: intlShape
}

export default compose(
  injectIntl
)(PreviewDialogContent)
