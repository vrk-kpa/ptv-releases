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
import { connect } from 'react-redux'
import { compose } from 'redux'
import { Tag } from 'appComponents'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import { EnumsSelectors } from 'selectors'
import { Map } from 'immutable'
import { getIsInTranslation } from 'selectors/entities/entities'

const messages = defineMessages({
  translationOrdered: {
    id: 'Util.ReduxForm.Renders.RenderLanguageAvailabilitiesTable.TranslationOrdered.Title',
    defaultMessage: 'Käännös tilattu'
  }
})

const TranslationTags = ({
  intl: { formatMessage },
  languageId,
  isInTranslation
}) => {
  return isInTranslation && <Tag
    message={formatMessage(messages.translationOrdered)}
    isRemovable={false}
    bgColor='#00B38A'
    textColor='#FFFFFF'
  />
}
TranslationTags.propTypes = {
  intl: intlShape,
  languageId: PropTypes.string.isRequired,
  isInTranslation: PropTypes.bool.isRequired
}

export default compose(
  injectIntl,
  connect((state, { languageId }) => {
    const tLanguages = EnumsSelectors.translationLanguages.getEntitiesMap(state)
    const languageCode = (tLanguages.get(languageId) || Map()).get('code')
    return {
      isInTranslation: getIsInTranslation(state, { languageCode })
    }
  })
)(TranslationTags)
