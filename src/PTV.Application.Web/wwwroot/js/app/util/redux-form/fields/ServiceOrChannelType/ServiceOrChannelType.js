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
import { RenderSelect } from 'util/redux-form/renders'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import { compose } from 'redux'
import { injectIntl } from 'util/react-intl'
import Tooltip from 'appComponents/Tooltip'
import { Field } from 'redux-form/immutable'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import { messages } from 'Routes/Search/components/SearchFilters/messages'
import { searchContentTypeEnum } from 'enums'

const ServiceOrChannelType = ({
  intl: { formatMessage },
  tooltip,
  ...rest
}) => {
  const labelChildren = () => (
    tooltip && <Tooltip tooltip={tooltip} />
  )

  const createOption = (value, isSubType) => ({
    value,
    label: formatMessage(messages[value]),
    isSubType
  })

  const options = [
    { value: 'all', label: formatMessage(messages.allSelectedLabel), isSubType: false },
    createOption(searchContentTypeEnum.SERVICE, false),
    createOption(searchContentTypeEnum.SERVICESERVICE, true),
    createOption(searchContentTypeEnum.SERVICEPROFESSIONAL, true),
    createOption(searchContentTypeEnum.SERVICEPERMIT, true),
    createOption(searchContentTypeEnum.CHANNEL, false),
    createOption(searchContentTypeEnum.ECHANNEL, true),
    createOption(searchContentTypeEnum.WEBPAGE, true),
    createOption(searchContentTypeEnum.PRINTABLEFORM, true),
    createOption(searchContentTypeEnum.PHONE, true),
    createOption(searchContentTypeEnum.SERVICELOCATION, true)
  ]

  return (
    <Field
      name='serviceOrChannelType'
      component={RenderSelect}
      size='full'
      label={formatMessage(messages.contentTypeFilterTitle)}
      labelChildren={rest.labelPosition !== 'inside' && labelChildren()}
      {...rest}
      optionRenderer={({ label, isSubType }) => isSubType
        ? <span style={{ paddingLeft: '10px' }}>{label}</span>
        : <strong>{label}</strong>}
      options={options}
    />
  )
}
ServiceOrChannelType.propTypes = {
  intl: PropTypes.any.isRequired,
  tooltip: PropTypes.string
}

export default compose(
  injectIntl,
  injectSelectPlaceholder(),
  asDisableable
)(ServiceOrChannelType)
