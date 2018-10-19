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
import WarningMessage from 'appComponents/WarningMessage'
import { ConnectionType } from 'util/redux-form/fields'
import { injectIntl, defineMessages, intlShape } from 'util/react-intl'
import { compose } from 'redux'
import { connect } from 'react-redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { getFormValues, getFormInitialValues } from 'redux-form/immutable'
import asContainer from 'util/redux-form/HOC/asContainer'
import { getConnectionTypeCommonForAllId } from './selectors'
import { getSelectedEntityId } from 'selectors/entities/entities'

const messages = defineMessages({
  warningTitle: {
    id: 'AppComponents.ChannelConnectionType.Warning',
    defaultMessage: 'Olet muuttamassa yhteiskäyttöisen kanavan vain oman organisaatiosi käyttöön.'
  },
  title: {
    id: 'Containers.Channels.ConnectionType.Header.Title',
    defaultMessage: 'Asiointikanavan yhteiskäyttöisyystieto'
  }
})

const ChannelConnectionType = ({
  showWarning,
  ...rest
}) => {
  return (
    <div>
      {showWarning &&
      <WarningMessage
        warningText={messages.warningTitle}
        {...rest}
      />}
      <ConnectionType />
    </div>
  )
}

ChannelConnectionType.propTypes = {
  showWarning: PropTypes.bool,
  intl: intlShape
}

export default compose(
  injectFormName,
  injectIntl,
  asContainer({
    title: messages.title,
    dataPaths: 'connectionType'
  }),
  connect((state, { formName }) => {
    const values = getFormValues(formName)(state)
    const initial = getFormInitialValues(formName)(state)
    const commonId = getConnectionTypeCommonForAllId(state)
    const entityId = getSelectedEntityId(state)
    const showWarning = entityId &&
      commonId === initial.get('connectionType') &&
      values.get('connectionType') !== initial.get('connectionType')
    return {
      showWarning
    }
  })
)(ChannelConnectionType)
