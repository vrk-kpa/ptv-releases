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
import { AreaInformationType } from 'util/redux-form/fields'
import { asContainer, asSection, injectFormName, withFormStates } from 'util/redux-form/HOC'
import { AreaTrees } from 'util/redux-form/sections'
import { getIsTreeActive } from './selectors'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { Label } from 'sema-ui-components'
import { formTypesEnum } from 'enums'
import { injectIntl, intlShape, defineMessages } from 'react-intl'

const messages = defineMessages({
  electronicChannelFormContainerTitle: {
    id: 'Containers.Channels.ElectronicChannel.InformationArea.Title',
    defaultMessage: 'Alue, joilla verkkoasiointi on käytettävissä'
  },
  electronicChannelFormContainerTooltip: {
    id: 'Containers.Channels.ElectronicChannel.InformationArea.Tooltip',
    defaultMessage: 'Alue, joilla verkkoasiointi on käytettävissä'
  },
  phoneChannelFormContainerTitle: {
    id: 'Containers.Channels.PhoneChannel.InformationArea.Title',
    defaultMessage: 'Alue, joilla puhelinasiointi on käytettävissä'
  },
  phoneChannelFormContainerTooltip: {
    id: 'Containers.Channels.PhoneChannel.InformationArea.Tooltip',
    defaultMessage: 'Alue, joilla puhelinasiointi on käytettävissä'
  },
  printableFormContainerTitle: {
    id: 'Containers.Channels.PrintableFormChannel.InformationArea.Title',
    defaultMessage: 'Alue, joilla tulostettava lomake on käytettävissä'
  },
  printableFormContainerTooltip: {
    id: 'Containers.Channels.PrintableFormChannel.InformationArea.Tooltip',
    defaultMessage: 'Alue, joilla tulostettava lomake on käytettävissä'
  },
  serviceLocationFormContainerTitle: {
    id: 'Containers.Channels.ServiceLocationChannel.InformationArea.Title',
    defaultMessage: 'Alue, joilla palvelupiste on käytettävissä'
  },
  serviceLocationFormContainerTooltip: {
    id: 'Containers.Channels.ServiceLocationChannel.InformationArea.Tooltip',
    defaultMessage: 'Alue, joilla palvelupiste on käytettävissä'
  },
  infoTextService: {
    id: 'Containers.Services.AddService.Step1.AreaInformation.Type.Title',
    defaultMessage: 'Aluetieto täytetty organisaation mukaan. Voit muuttaa valintaa.'
  },
  infoTextChannel: {
    id: 'Containers.Channels.Common.InformationArea.AreaInformationType.Title',
    defaultMessage: 'Täytä, jos alueen tiedot eroavat palvelun aluetiedoista.'
  }
})

const AreaInformation = ({
  isTreeActive,
  parentPath,
  isCompareMode,
  isReadOnly,
  intl: { formatMessage },
  formName
}) => {
  const basicCompareModeClass = isCompareMode ? 'col-md-24' : 'col-md-12'
  return (
    <div>
      <div className='form-row'>
        {!isReadOnly && !isCompareMode &&
          <Label
            infoLabel
            labelText={formatMessage(messages.infoTextChannel)}
          />
        }
        <div className='row'>
          <div className={basicCompareModeClass}>
            <AreaInformationType />
          </div>
        </div>
      </div>
      {isTreeActive &&
        <div className='form-row'>
          <AreaTrees parentPath={parentPath} />
        </div>}
    </div>
  )
}

AreaInformation.propTypes = {
  isTreeActive: PropTypes.bool.isRequired,
  parentPath: PropTypes.string,
  isCompareMode: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  intl: intlShape,
  formName: PropTypes.string
}

export default compose(
  injectFormName,
  injectIntl,
  withFormStates,
  connect((state, ownProps) => ({
    isTreeActive: getIsTreeActive(state, ownProps),
    title: messages[`${ownProps.formName}ContainerTitle`],
    tooltip: messages[`${ownProps.formName}ContainerTooltip`]
  })),
  asSection('areaInformation'),
  asContainer({
    title: messages.electronicChannelFormContainerTitle,
    tooltip: messages.electronicChannelFormContainerTitle,
    dataPaths: 'areaInformationType'
  })
)(AreaInformation)
