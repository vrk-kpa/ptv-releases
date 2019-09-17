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
import { connect } from 'react-redux'
import cx from 'classnames'
import styles from './styles.scss'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { Map } from 'immutable'
import ConnectionDescription from './ConnectionDescription'
import ConnectionChargeType from './ConnectionChargeType'
import ConnectionContactDetails from './ConnectionContactDetails'
import ConnectionAuthorization from './ConnectionAuthorization'
import ConnectionOpeningHours from './ConnectionOpeningHours'

const messages = defineMessages({
  descriptionTitle: {
    id: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.Description.Title',
    defaultMessage: 'Kuvaus'
  },
  chargeTypeAdditionalInfoTitle: {
    id: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.ChargeTypeAdditionalInfo.Title',
    defaultMessage: 'Maksullisuuden lisätieto'
  },
  chargeTypeTitle: {
    id: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.ChargeType.Title',
    defaultMessage: 'Maksullisuuden tiedot'
  },
  authorizationTitle: {
    id: 'Containers.Relations.ServiceAndChannel.ChannelRelation.DigitalAuthorization.Title',
    defaultMessage: 'Digitaalinen lupa'
  },
  openingHoursTitle: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.Header.Title',
    defaultMessage: 'Vaihe 2/2: Aukioloajat'
  }
})

const ConnectionDescriptionCell = ({
  basicInformation,
  contactDetails,
  index,
  parentIndex,
  intl: { formatMessage }
}) => {
  return (
    <div className={cx('cell', styles.connectionDescriptionCell)}>
      <ConnectionDescription
        description={basicInformation.get('description')}
        label={formatMessage(messages.descriptionTitle)}
      />
      <ConnectionChargeType
        chargeTypeId={basicInformation.get('chargeType')}
        label={formatMessage(messages.chargeTypeTitle)}
      />
      <ConnectionDescription
        description={basicInformation.get('additionalInformation')}
        label={formatMessage(messages.chargeTypeAdditionalInfoTitle)}
      />
      <ConnectionAuthorization
        index={index}
        parentIndex={parentIndex}
        label={formatMessage(messages.authorizationTitle)}
      />
      <ConnectionOpeningHours
        index={index}
        parentIndex={parentIndex}
        label={formatMessage(messages.openingHoursTitle)}
      />
      <ConnectionContactDetails
        contactDetails={contactDetails}
      />
    </div>
  )
}

ConnectionDescriptionCell.propTypes = {
  basicInformation: PropTypes.any,
  contactDetails: PropTypes.any,
  intl: intlShape,
  index: PropTypes.any,
  parentIndex: PropTypes.any
}

export default compose(
  injectIntl,
  connect((state, ownProps) => ({
    basicInformation: ownProps.connection.get('basicInformation') || Map(),
    contactDetails: ownProps.connection.get('contactDetails') || Map()
  }))
)(ConnectionDescriptionCell)
