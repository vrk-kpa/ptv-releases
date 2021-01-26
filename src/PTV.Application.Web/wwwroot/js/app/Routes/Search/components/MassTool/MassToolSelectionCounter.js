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
import { branch, withProps } from 'recompose'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { getShowReviewBar } from 'util/redux-form/HOC/withMassToolForm/selectors'
import {
  getSelectedEntitiesForMassOperation,
  getSelectedCount,
  getSelectedFilteredCount,
  getSelectedLanguageVersionsCount,
  getSelectedFilteredLanguageVersionsCount
} from './selectors'
import { formTypesEnum } from 'enums'

const messages = defineMessages({
  massToolEntityCount: {
    id: 'MassTool.SelectionCounter.EntityCount.Title',
    defaultMessage: '{entityCount} sisältöä'
  },
  massToolLanguageVersionCount: {
    id: 'MassTool.SelectionCounter.LanguageVersionCount.Title',
    defaultMessage: '{languageVersionCount} kieliversiota'
  }
})

const MassToolSelectionCounter = ({ intl: { formatMessage }, entityCount, languageVersionCount, className }) => (
  <span className={className}>
    {`${formatMessage(messages.massToolEntityCount, { entityCount })},
      ${formatMessage(messages.massToolLanguageVersionCount, { languageVersionCount })}`}
  </span>
)

MassToolSelectionCounter.propTypes = {
  entityCount: PropTypes.number,
  languageVersionCount: PropTypes.number,
  className: PropTypes.string,
  intl: intlShape.isRequired
}

// export default compose(
//   injectIntl,
//   // branch(({ formName }) => !formName, injectFormName),
//   withProps(props => ({ formName: formTypesEnum.MASSTOOLSELECTIONFORM })),
//   connect((state, { formName }) => {
//     const inReview = getShowReviewBar(state)
//     const selected = getSelectedEntitiesForMassOperation(state, { formName })
//     return {
//       entityCount: inReview
//         ? getSelectedFilteredCount(state, { selected, formName })
//         : getSelectedCount(state, { selected }),
//       languageVersionCount: inReview
//         ? getSelectedFilteredLanguageVersionsCount(state, { selected, formName })
//         : getSelectedLanguageVersionsCount(state, { selected, formName })
//     }
//   })
// )(MassToolSelectionCounter)

export default compose(
  injectIntl,
  // branch(({ formName }) => !formName, injectFormName),
  connect((state) => {
    const formName = formTypesEnum.MASSTOOLSELECTIONFORM
    const selected = getSelectedEntitiesForMassOperation(state, { formName })
    return {
      entityCount: getSelectedCount(state, { selected, formName }),
      languageVersionCount: getSelectedLanguageVersionsCount(state, { selected, formName })
    }
  })
)(MassToolSelectionCounter)
