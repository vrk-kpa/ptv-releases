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
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { Label } from 'sema-ui-components'
import Paragraph from 'appComponents/Paragraph'
import { compose } from 'redux'
import styles from '../styles.scss'

const messages = defineMessages({
  title: {
    id: 'ExceptionalOpeningHours.HolidayInfo.Title',
    defaultMessage: 'Juhlapyhät'
  },
  description: {
    id: 'ExceptionalOpeningHours.HolidayInfo.Description',
    defaultMessage: 'Jos syötät listassa olevalle juhlapyhälle poikkeavan palveluajan, se on voimassa toistaiseksi. Muista tarkistaa joka vuosi, että tieto pitää yhä paikkansa.' // eslint-disable-line
  }
})

const HolidayInfo = ({
  intl: { formatMessage }
}) => (
  <React.Fragment>
    <Label labelText={formatMessage(messages.title)} className={styles.holidayInfoTitle} />
    <Paragraph className={styles.holidayInfoDescription}>
      {formatMessage(messages.description)}
    </Paragraph>
  </React.Fragment>
)

HolidayInfo.propTypes = {
  intl: intlShape.isRequired
}

export default compose(
  injectIntl
)(HolidayInfo)
