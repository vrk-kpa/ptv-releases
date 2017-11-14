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
import BasicInformation from './BasicInformation'
import DigitalAuthorization from './DigitalAuthorization'
import { Tabs, Tab } from 'sema-ui-components'
import { connect } from 'react-redux'
import { compose } from 'redux'
import withState from 'util/withState'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import ConnectionButtons from './ConnectionButtons'
import { isConnectionRowReadOnly } from 'appComponents/ConnectionsStep/ConnectionsForm/selectors'
import styles from '../styles.scss'

const messages = defineMessages({
  title : {
    id : 'AppComponents.ConnectionStep.ConnectionsForm.AdditionalInformation.Title',
    defaultMessage: 'Liitostiedot'
  },
  description: {
    id : 'AppComponents.ConnectionStep.ConnectionsForm.AdditionalInformation.Description',
    defaultMessage: 'Lisää tarvittaessa lisätiedot, jotka pätevät tässä liitoksessa.'
  },
  generalTab: {
    id: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.Tabs.GeneralTab.Title',
    defaultMessage: 'Perustiedot'
  },
  digitalAuthorizationTab: {
    id: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.Tabs.DigitalAuthorizationTab.Title',
    defaultMessage: 'Asiointivaltuudet'
  }
})

const AdditionalInformation = ({
  field,
  index,
  activeIndex,
  updateUI,
  isReadOnly,
  intl: { formatMessage }
}) => {
  const handleOnChange = activeIndex => updateUI('activeIndex', activeIndex)
  return (
    <div className={styles.connectionDetail}>
      <header className={styles.header}>
        <div>
          <h1>{formatMessage(messages.title)}</h1>
          <p>{formatMessage(messages.description)}</p>
        </div>
        <ConnectionButtons
          index={index}
          field={field}
        />
      </header>
      <Tabs index={activeIndex} onChange={handleOnChange}>
        <Tab label={formatMessage(messages.generalTab)}>
          <div className={styles.connectionDetailTab}>
            <BasicInformation
              field={field}
              isReadOnly={isReadOnly}
            />
          </div>
        </Tab>
        <Tab label={formatMessage(messages.digitalAuthorizationTab)}>
          <div className={styles.connectionDetailTab}>
            <DigitalAuthorization
              field={field}
              isReadOnly={isReadOnly}
            />
          </div>
        </Tab>
      </Tabs>
    </div>
  )
}
AdditionalInformation.propTypes = {
  field: PropTypes.string,
  index: PropTypes.number,
  activeIndex: PropTypes.number,
  updateUI: PropTypes.func,
  isReadOnly: PropTypes.bool,
  intl: intlShape
}

export default compose(
  injectIntl,
  connect((state, { index }) => ({
    isReadOnly: isConnectionRowReadOnly(index)(state)
  })),
  withState({
    initialState: {
      activeIndex: 0
    }
  })
)(AdditionalInformation)
