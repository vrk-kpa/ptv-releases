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
import { FieldArray } from 'redux-form/immutable'
import AstiDescription from './AstiDescription'
import AstiType from './AstiType'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { compose } from 'redux'
import { connect } from 'react-redux'
import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'
import { getContentLanguageCode } from 'selectors/selections'
import { Map } from 'immutable'

const messages = defineMessages({
  asd: {
    id: 'appComponents.ConnectionsStep.AdditionalInformation.AstiInformation',
    defaultMessage : 'Asiointipisteiden sopimusjärjestelmässä (ASTI) on tälle palvelulle määritelty alla olevat palvelutasot. Näitä tietoja ei voi muokata Palvelutietovarannossa.'
  }
})

const RenderAstiInformation = ({ fields, languageCode }) => {
  return (
    <div>
      {fields.map((field, index) => {
        const astiInformation = fields.get(index)
        const astiTypeId = astiInformation && astiInformation.get('astiTypeId')
        const astiDescription = astiInformation && astiInformation.get('astiDescription') || Map()
        const isAvailableInLanguage = astiDescription.get(languageCode)
        return (
          <div key={index}>
            <AstiType astiTypeId={astiTypeId} />
            {isAvailableInLanguage &&
              <AstiDescription
                name={`${field}.astiDescription`}
              />
            }
          </div>
        )
      })}
    </div>
  )
}

const AstiInformation = ({
  field,
  intl: { formatMessage },
  languageCode
}) => {
  return (
    <div>
      <p>{formatMessage(messages.asd)}</p>
      <FieldArray
        name={`${field}.astiDetails.astiTypeInfos`}
        component={RenderAstiInformation}
        props={{ languageCode }}
      />
    </div>
  )
}

AstiInformation.propTypes = {
  field: PropTypes.string.isRequired,
  languageCode: PropTypes.string,
  intl: intlShape
}

export default compose(
  injectIntl,
  withLanguageKey,
  connect((state, { languageKey }) => ({
    languageCode: getContentLanguageCode(state, { languageKey }) || 'fi'
  }))
)(AstiInformation)
