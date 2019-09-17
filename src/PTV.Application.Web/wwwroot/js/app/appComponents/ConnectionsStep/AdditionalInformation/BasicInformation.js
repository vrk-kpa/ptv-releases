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
import { ShortDescription, ChargeType } from 'util/redux-form/fields'
import LanguageSwitcher from './LanguageSwitcher'
import { injectIntl, defineMessages, intlShape } from 'util/react-intl'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { getKey, formEntityConcreteTypes } from 'enums'
import { getSelectedEntityConcreteType } from 'selectors/entities/entities'

const messages = defineMessages({
  descriptionTitle: {
    id: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.Description.Title',
    defaultMessage: 'Kuvaus'
  },
  descriptionTooltip: {
    id: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.Description.Tooltip',
    defaultMessage: 'Kuvaus'
  },
  descriptionPlaceholder: {
    id: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.Description.Placeholder',
    defaultMessage: 'Kirjoita liitosta kuvaava kuvaus.'
  },
  chargeTypeAdditionalInfoTitle: {
    id: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.ChargeTypeAdditionalInfo.Title',
    defaultMessage: 'Maksullisuuden lisätieto'
  },
  chargeTypeAdditionalInfoPlaceholder: {
    id: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.ChargeTypeAdditionalInfo.Placeholder',
    defaultMessage: 'paikkamerkkiteksti'
  },
  chargeTypeTitle: {
    id: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.ChargeType.Title',
    defaultMessage: 'Maksullisuuden tiedot'
  },
  chargeTypeTooltip: {
    id: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.ChargeType.Tooltip',
    defaultMessage: 'Missing'
  }
})

const BasicInformation = ({ field, isReadOnly, formName, intl: { formatMessage } }) => {
  const basicInformationFieldName = `${field}.basicInformation`
  return (
    <div>
      <div className='form-row'>
        <ShortDescription
          name={`${basicInformationFieldName}.description`}
          label={formatMessage(messages.descriptionTitle)}
          tooltip={formatMessage(messages.descriptionTooltip)}
          placeholder={formatMessage(messages.descriptionPlaceholder)}
          isReadOnly={isReadOnly}
          multiline
          rows={5}
          counter
          maxLength={500}
          noTranslationLock
        />
      </div>
      <div className='form-row'>
        <div className='row'>
          <div className='col-lg-12'>
            <ChargeType
              label={formatMessage(messages.chargeTypeTitle)}
              tooltip={formatMessage(messages.chargeTypeTooltip)}
              radio={false}
              name={`${basicInformationFieldName}.chargeType`}
              isReadOnly={isReadOnly}
              useDefaultValue={false}
            />
          </div>
        </div>
      </div>
      <div className='form-row'>
        <div className='row'>
          <div className='col-lg-12'>
            <ShortDescription
              label={formatMessage(messages.chargeTypeAdditionalInfoTitle)}
              placeholder={formatMessage(messages.chargeTypeAdditionalInfoPlaceholder)}
              name={`${basicInformationFieldName}.additionalInformation`}
              isReadOnly={isReadOnly}
              multiline
              rows={2}
              counter
              maxLength={500}
              noTranslationLock
            />
          </div>
        </div>
      </div>
    </div>
  )
}
BasicInformation.propTypes = {
  field: PropTypes.string,
  isReadOnly: PropTypes.bool,
  formName: PropTypes.string,
  intl: intlShape
}

export default compose(
  injectIntl,
  connect((state) => {
    const entityConcreteType = getSelectedEntityConcreteType(state)
    return {
      formName: entityConcreteType && getKey(formEntityConcreteTypes, entityConcreteType.toLowerCase()) || ''
    }
  })
)(BasicInformation)
