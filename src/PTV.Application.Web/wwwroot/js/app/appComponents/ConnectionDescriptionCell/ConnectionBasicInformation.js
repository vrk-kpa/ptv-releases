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
import { connect } from 'react-redux'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'
import {
  getBasicInformationDescription,
  getBasicInformationChargeType,
  getBasicInformationAdditionalInformation
} from './selectors'
import { Label } from 'sema-ui-components'
import NoDataLabel from 'appComponents/NoDataLabel'
import ConnectionDescription from 'appComponents/ConnectionDescriptionCell/ConnectionDescription'
import ConnectionChargeType from 'appComponents/ConnectionDescriptionCell/ConnectionChargeType'
import styles from './styles.scss'

const messages = defineMessages({
  basicInformationHeading: {
    id: 'ConnectionDetail.BasicInformation.Title',
    defaultMessage: 'Perustiedot',
    description: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.Tabs.GeneralTab.Title'
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

const ConnectionBasicInformation = ({
  description,
  chargeTypeId,
  additionalInformation,
  intl: { formatMessage }
}) => {
  const isAnyBasicInformationProvided = description || chargeTypeId || additionalInformation
  return (
    <div className={styles.previewBlock}>
      <Label className={styles.previewHeading} labelText={formatMessage(messages.basicInformationHeading)} />
      <div className={styles.previewItem}>
        {isAnyBasicInformationProvided && (
          <React.Fragment>
            {!!description && <ConnectionDescription
              description={description}
              label={formatMessage(messages.basicInformationDescriptionTitle)}
            />}
            {!!chargeTypeId && <ConnectionChargeType chargeTypeId={chargeTypeId} />}
            {!!additionalInformation && <ConnectionDescription
              description={additionalInformation}
              label={formatMessage(messages.basicInformationChargeTypeAdditionalInfoTitle)}
            />}
          </React.Fragment>
        ) || <NoDataLabel />}
      </div>
    </div>
  )
}

ConnectionBasicInformation.propTypes = {
  description: PropTypes.string,
  chargeTypeId: PropTypes.string,
  additionalInformation: PropTypes.string,
  intl: intlShape
}
export default compose(
  injectIntl,
  injectFormName,
  withLanguageKey,
  connect((state, { formName, field, languageKey }) => ({
    description: getBasicInformationDescription(state, { formName, field, languageKey }),
    chargeTypeId: getBasicInformationChargeType(state, { formName, field, languageKey }),
    additionalInformation: getBasicInformationAdditionalInformation(state, { formName, field, languageKey })
  }))
)(ConnectionBasicInformation)
