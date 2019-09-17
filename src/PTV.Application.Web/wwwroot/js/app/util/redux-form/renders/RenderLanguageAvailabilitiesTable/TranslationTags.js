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
import React, { Fragment } from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { compose } from 'redux'
import Tag from 'appComponents/Tag'
import { injectIntl, intlShape } from 'util/react-intl'
import { EnumsSelectors } from 'selectors'
import { Map } from 'immutable'
import { getIsInTranslation, getIsTranslationDelivered } from 'selectors/entities/entities'
import { commonAppMessages } from 'util/redux-form/messages'

const TranslationTags = ({
  intl: { formatMessage },
  languageId,
  isInTranslation,
  isTranslationDelivered
}) => {
  return <Fragment>
    {isInTranslation && (
      <Tag
        message={formatMessage(commonAppMessages.translationOrdered)}
        isRemovable={false}
        bgColor='#00B38A'
        textColor='#FFFFFF'
      />
    )}
    {isTranslationDelivered && (
      <Tag
        message={formatMessage(commonAppMessages.translationArrived)}
        isRemovable={false}
        bgColor='#00B38A'
        textColor='#FFFFFF'
      />
    )}
  </Fragment>
}
TranslationTags.propTypes = {
  intl: intlShape,
  languageId: PropTypes.string.isRequired,
  isInTranslation: PropTypes.bool.isRequired,
  isTranslationDelivered: PropTypes.bool.isRequired
}

export default compose(
  injectIntl,
  connect((state, { languageId }) => {
    const tLanguages = EnumsSelectors.translationLanguages.getEntitiesMap(state)
    const languageCode = (tLanguages.get(languageId) || Map()).get('code')
    return {
      isInTranslation: getIsInTranslation(state, { languageCode }),
      isTranslationDelivered: getIsTranslationDelivered(state, { languageCode })
    }
  })
)(TranslationTags)
