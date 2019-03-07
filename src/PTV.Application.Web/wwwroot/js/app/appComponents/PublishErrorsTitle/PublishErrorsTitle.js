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
import { compose } from 'redux'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { Set } from 'immutable'
import ImmutablePropTypes from 'react-immutable-proptypes'
import moment from 'moment'
import { getEntityAvailableLanguages } from 'selectors/entities/entities'
import { connect } from 'react-redux'

const messages = defineMessages({
  publishErrorTitle: {
    id: 'PublishingDialog.Language.CannotPublish.Title',
    defaultMessage: 'Kieliversiota ei voi julkaista. Tarkistathan seuraavat kohdat: '
  },
  timedPublishErrorTitle: {
    id: 'PublishingDialog.Language.CannotTimedPublish.Title',
    defaultMessage: 'Timed publishing {time} set for the content. Please check the following: '
  }
})

const PublishErrorsTitle = ({ intl, timedPublish }) =>
  <span>{
    timedPublish && timedPublish.size > 0 &&
      intl.formatMessage(messages.timedPublishErrorTitle, {
        time: timedPublish.map(t => moment(t).format('DD.MM.YYYY')).join(', ')
      }) ||
      intl.formatMessage(messages.publishErrorTitle)
  }
  </span>

PublishErrorsTitle.propTypes = {
  intl: intlShape.isRequired,
  timedPublish: ImmutablePropTypes.list
}

export default compose(
  injectIntl,
  connect(state => ({
    timedPublish: getEntityAvailableLanguages(state).reduce(
      (times, la) => la.get('validFrom') && times.add(la.get('validFrom')) || times,
      Set()
    ) || null
  }))
)(PublishErrorsTitle)
