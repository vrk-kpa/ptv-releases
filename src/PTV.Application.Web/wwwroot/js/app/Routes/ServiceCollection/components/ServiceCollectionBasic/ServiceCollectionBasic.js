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
import {
  serviceCollectionMessages
} from '../Messages'
import {
  Name,
  Organization,
  Description,
  Summary
} from 'util/redux-form/fields'
import { injectIntl, intlShape } from 'util/react-intl'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { messages } from 'Routes/messages'

const ServiceCollectionBasic = ({
  intl: { formatMessage },
  isCompareMode,
  ...rest
}) => {
  const basicCompareModeClass = isCompareMode ? 'col-lg-24' : 'col-lg-12'
  return (
    <div>
      <div className='form-row'>
        <div className='row'>
          <div className={basicCompareModeClass}>
            <Name label={formatMessage(serviceCollectionMessages.nameTitle)}
              placeholder={formatMessage(serviceCollectionMessages.namePlaceholder)}
              tooltip={formatMessage(serviceCollectionMessages.nameTooltip)}
              required />
          </div>
        </div>
      </div>
      <div className='form-row'>
        <div className='row'>
          <div className={basicCompareModeClass}>
            <Summary
              label={formatMessage(serviceCollectionMessages.shortDescriptionTitle)}
              placeholder={formatMessage(serviceCollectionMessages.shortDescriptionPlaceholder)}
              tooltip={formatMessage(serviceCollectionMessages.shortDescriptionTooltip)}
            />
          </div>
        </div>
      </div>
      <div className='form-row'>
            <Description
              label={formatMessage(serviceCollectionMessages.descriptionTitle)}
              placeholder={formatMessage(serviceCollectionMessages.descriptionPlaceholder)}
              tooltip={formatMessage(serviceCollectionMessages.descriptionTooltip)}
              charLimit={2500}
              limit={2500}
            />
      </div>
      <div className='form-row'>
        <div className='row'>
          <div className={basicCompareModeClass}>
            <Organization
              label={formatMessage(serviceCollectionMessages.organizationTitle)}
              placeholder={formatMessage(messages.organizationPlaceholder)}
              tooltip={formatMessage(serviceCollectionMessages.organizationTooltip)}
              required
              {...rest}
            />
          </div>
        </div>
      </div>
    </div>
  )
}

ServiceCollectionBasic.propTypes = {
  intl: intlShape.isRequired,
  isCompareMode: PropTypes.bool

}

export default compose(
  injectIntl,
  injectFormName,
  withFormStates
)(ServiceCollectionBasic)
