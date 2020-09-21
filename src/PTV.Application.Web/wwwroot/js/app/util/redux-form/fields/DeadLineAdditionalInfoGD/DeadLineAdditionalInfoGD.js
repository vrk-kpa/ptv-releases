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
import { TextField, Label } from 'sema-ui-components'
import { compose } from 'redux'
import { connect } from 'react-redux'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asLocalizable from 'util/redux-form/HOC/asLocalizable'
import asComparable from 'util/redux-form/HOC/asComparable'
import { injectIntl, intlShape } from 'util/react-intl'
import { serviceDescriptionMessages } from 'Routes/Service/components/Messages'
import {
  getIsGDAvailableInContentLanguage,
  getIsGDAvailableInCompareLanguage,
  getGeneralDescriptionName,
  getGeneralDescriptionCompareName,
  getGDDeadLineAdditionalInfoValue,
  getGDDeadLineAdditionalInfoCompareValue
} from 'Routes/Service/components/ServiceComponents/selectors'
import Tooltip from 'appComponents/Tooltip'

const renderLabel = (formatMessage) => (
  <Label
    labelText={formatMessage(serviceDescriptionMessages.optionConnectedDescriptionTitle)}
    labelPosition='top'
  >
    <Tooltip tooltip={formatMessage(serviceDescriptionMessages.connectedGDFieldTooltip)} />
  </Label>
)
const DeadLineAdditionalInfoGD = ({
  name,
  value,
  isGDAvailable,
  intl: { formatMessage },
  labelPosition,
  disabled,
  multiline,
  rows,
  size,
  maxLength,
  counter
}) => <TextField
  value={isGDAvailable ? value && value.length > 0 ? value : '' : ''}
  label={renderLabel(formatMessage)}
  placeholder={isGDAvailable
    ? formatMessage(serviceDescriptionMessages.dataNotAvailable)
    : formatMessage(serviceDescriptionMessages.languageVersionNotAvailable)}
  labelPosition={labelPosition}
  disabled={disabled}
  multiline={multiline}
  rows={rows}
  size={size}
  maxLength={maxLength}
  counter={counter}
/>

DeadLineAdditionalInfoGD.propTypes = {
  name: PropTypes.string,
  value: PropTypes.string,
  isGDAvailable: PropTypes.bool,
  intl: intlShape,
  labelPosition: PropTypes.string,
  disabled: PropTypes.bool,
  multiline: PropTypes.bool,
  rows: PropTypes.number,
  size: PropTypes.string,
  maxLength: PropTypes.number,
  counter: PropTypes.bool
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: TextField }),
  connect((state, ownProps) => {
    const name = ownProps.compare
      ? getGeneralDescriptionCompareName(state, ownProps)
      : getGeneralDescriptionName(state, ownProps)
    const value = ownProps.compare
      ? getGDDeadLineAdditionalInfoCompareValue(state, ownProps)
      : getGDDeadLineAdditionalInfoValue(state, ownProps)
    const isGDAvailable = ownProps.compare
      ? getIsGDAvailableInCompareLanguage(state, ownProps)
      : getIsGDAvailableInContentLanguage(state, ownProps)
    return {
      name,
      value,
      isGDAvailable
    }
  }),
  asDisableable,
  asLocalizable
)(DeadLineAdditionalInfoGD)
