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
import BasicInformation from './BasicInformation'
import DigitalAuthorization from './DigitalAuthorization'
import AstiInformation from './AstiInformation'
import UserSupport from './UserSupport'
import { Tabs, Tab } from 'sema-ui-components'
import { connect } from 'react-redux'
import { compose } from 'redux'
import withState from 'util/withState'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import {
  getFormNameByEntityConcreteType
} from 'appComponents/ConnectionsStep/selectors'
import { OpeningHours } from 'util/redux-form/sections'
import styles from 'appComponents/ConnectionsStep/styles.scss'
import LanguageSwitcher from './LanguageSwitcher'
import { getSelectedEntityConcreteType } from 'selectors/entities/entities'
import { getMainNavigationIndex } from 'selectors/common'
import { formTypesEnum } from 'enums'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { Set } from 'immutable'
import ConnectionBasicInformation from 'appComponents/ConnectionDescriptionCell/ConnectionBasicInformation'
import ConnectionAstiInformation from 'appComponents/ConnectionDescriptionCell/ConnectionAstiInformation'
import ConnectionOpeningHours from 'appComponents/ConnectionDescriptionCell/ConnectionOpeningHours'
import ConnectionAuthorization from 'appComponents/ConnectionDescriptionCell/ConnectionAuthorization'
import ConnectionContactDetails from 'appComponents/ConnectionDescriptionCell/ConnectionContactDetails'

const messages = defineMessages({
  title: {
    id: 'AppComponents.ConnectionStep.ConnectionsForm.AdditionalInformation.Title',
    defaultMessage: 'Liitostiedot'
  },
  description: {
    id: 'AppComponents.ConnectionStep.ConnectionsForm.AdditionalInformation.Description',
    defaultMessage: 'Lisää tarvittaessa lisätiedot, jotka pätevät tässä liitoksessa.'
  },
  generalTab: {
    id: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.Tabs.GeneralTab.Title',
    defaultMessage: 'Perustiedot'
  },
  digitalAuthorizationTab: {
    id: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.Tabs.DigitalAuthorizationTab.Title',
    defaultMessage: 'Asiointivaltuudet'
  },
  ASTITab: {
    id: 'AppComponents.ConnectionStep.ConnectionsForm.AdditionalInformation.Tabs.ASTI.Title',
    defaultMessage: 'Asti-palvelutasot'
  },
  openingHoursTab: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.Header.Title',
    defaultMessage: 'Vaihe 2/2: Aukioloajat'
  },
  contactDetails: {
    id: 'AppComponents.ConnectionStep.ConnectionsForm.AdditionalInformation.Tabs.UserSupport.Title',
    defaultMessage: 'Yhteystiedot'
  },
  tooltip: {
    id: 'AppComponents.ConnectionStep.ConnectionsForm.AdditionalInformation.Title.Tooltip',
    defaultMessage: 'Liitoksen lisätiedot voi lisätä niillä kielillä, joilla on tehty kuvaus molemmille palvelulle ja asiointikanavalle. Jos et löydä haluamaasi kieltä, tarkista että molemmilla, sekä palvelulla ja asiointikanavalla on kyseinen kieliversio kuvattu.' // eslint-disable-line
  },
  basicInformationDescriptionTitle: {
    id: 'ConnectionStep.BasicInformation.Description.Title',
    defaultMessage: 'Kuvaus',
    description: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.Description.Title'
  },
  basicInformationChargeTypeAdditionalInfoTitle: {
    id: 'ConnectionStep.BasicInformation.ChargeTypeAdditionalInformation.Title',
    defaultMessage: 'Maksullisuuden lisätieto',
    description: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.ChargeTypeAdditionalInfo.Title'
  }
})

const tabVisibility = {
  contactDetails: Set([formTypesEnum.ELECTRONICCHANNELFORM,
    formTypesEnum.PHONECHANNELFORM,
    formTypesEnum.SERVICELOCATIONFORM]
  ),
  openingHours: Set([formTypesEnum.ELECTRONICCHANNELFORM,
    formTypesEnum.PHONECHANNELFORM,
    formTypesEnum.SERVICELOCATIONFORM]
  ),
  authorization: Set([formTypesEnum.ELECTRONICCHANNELFORM,
    formTypesEnum.PHONECHANNELFORM,
    formTypesEnum.WEBPAGEFORM,
    formTypesEnum.PRINTABLEFORM,
    formTypesEnum.SERVICELOCATIONFORM]
  )
}

const AdditionalInformation = ({
  field,
  index,
  activeIndex,
  updateUI,
  isReadOnly,
  isAsti,
  formName,
  entitiesType,
  parentId,
  connectedChannelformName,
  security,
  hasError,
  mainNavigationIndex,
  intl: { formatMessage }
  // customValidationLanguages //TODO is need to fix with new validation
}) => {
  const handleOnChange = activeIndex => updateUI('activeIndex', activeIndex)
  return (
    <div className={styles.connectionDetail}>
      <header className={styles.header}>
        <div>
          <h1>{formatMessage(messages.title)}</h1>
          <p>{formatMessage(messages.description)}</p>
          <p>{formatMessage(messages.tooltip)}</p>
        </div>
      </header>
      <LanguageSwitcher
        name={`${field}.languagesAvailabilities`}
        formName={formName}
        entitiesType={entitiesType}
        entityId={parentId}
        disabled={hasError}
      />
      {isReadOnly && (
        <div className={styles.connectionDetailBody}>
          <ConnectionBasicInformation field={`${field}.basicInformation`} />
          {isAsti && (
            <ConnectionAstiInformation field={`${field}.astiDetails`} />
          )}
          {tabVisibility.openingHours.has(connectedChannelformName) && (
            <ConnectionOpeningHours field={`${field}`} />
          )}
          {tabVisibility.contactDetails.has(connectedChannelformName) && (
            <ConnectionContactDetails field={`${field}.contactDetails`} />
          )}
          {tabVisibility.authorization.has(connectedChannelformName) && (
            <ConnectionAuthorization field={`${field}.digitalAuthorization`} />
          )}
        </div>
      ) ||
      (
        <Tabs index={activeIndex} onChange={handleOnChange}>
          <Tab label={formatMessage(messages.generalTab)} disabled={hasError && activeIndex !== 0}>
            <div className={styles.connectionDetailTab}>
              <BasicInformation
                field={`${field}.basicInformation`}
                isReadOnly={isReadOnly}
              />
            </div>
          </Tab>
          <Tab hidden={!isAsti} label={formatMessage(messages.ASTITab)} disabled={hasError && activeIndex !== 2}>
            <div className={styles.connectionDetailTab}>
              <AstiInformation
                field={`${field}.astiDetails`}
                isReadOnly={isReadOnly}
              />
            </div>
          </Tab>
          <Tab hidden={!tabVisibility.openingHours.has(connectedChannelformName)}
            label={formatMessage(messages.openingHoursTab)}
            disabled={hasError && activeIndex !== 3}>
            <div className={styles.connectionDetailTab}>
              <OpeningHours
                name={`${field}.openingHours`}
                field={field}
                isReadOnly={isReadOnly}
                showPreview={false}
              />
            </div>
          </Tab>
          <Tab hidden={!tabVisibility.contactDetails.has(connectedChannelformName)}
            label={formatMessage(messages.contactDetails)}
            disabled={hasError && activeIndex !== 4}>
            <div className={styles.connectionDetailTab}>
              <UserSupport
                name={`${field}.contactDetails`}
                isReadOnly={isReadOnly}
              />
            </div>
          </Tab>
          <Tab hidden={!tabVisibility.authorization.has(connectedChannelformName)}
            label={formatMessage(messages.digitalAuthorizationTab)} disabled={hasError && activeIndex !== 1}>
            <div className={styles.connectionDetailTab}>
              <DigitalAuthorization
                field={field}
                isReadOnly={isReadOnly}
              />
            </div>
          </Tab>
        </Tabs>
      )}
    </div>
  )
}
AdditionalInformation.propTypes = {
  field: PropTypes.string,
  index: PropTypes.number,
  activeIndex: PropTypes.number,
  updateUI: PropTypes.func,
  isReadOnly: PropTypes.bool,
  intl: intlShape,
  isAsti: PropTypes.bool,
  connectedChannelformName: PropTypes.string,
  security: PropTypes.any.isRequired,
  hasError: PropTypes.bool,
  formName: PropTypes.string,
  entitiesType: PropTypes.string,
  parentId: PropTypes.string,
  mainNavigationIndex: PropTypes.number
}

export default compose(
  injectFormName,
  injectIntl,
  connect((state, ownProps) => {
    const { parentType, entityConcreteType } = ownProps
    const entitiesType = getSelectedEntityConcreteType(state) || parentType
    return {
      entitiesType,
      connectedChannelformName: getFormNameByEntityConcreteType(state, { entityConcreteType }),
      mainNavigationIndex: getMainNavigationIndex(state)
    }
  }),
  withState({
    initialState: {
      activeIndex: 0
    }
  })
)(AdditionalInformation)
