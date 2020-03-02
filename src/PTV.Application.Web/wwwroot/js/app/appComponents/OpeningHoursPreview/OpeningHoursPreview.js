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
import { Label } from 'sema-ui-components'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import NormalOpeningHoursPreview from './NormalOpeningHoursPreview'
import SpecialOpeningHoursPreview from './SpecialOpeningHoursPreview'
import ExceptionalOpeningHoursPreview from './ExceptionalOpeningHoursPreview'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import styles from './styles.scss'
import cx from 'classnames'

const messages = defineMessages({
  title:  {
    id: 'AppComponents.OpeningHoursPreview.OpeningHoursPreview.Title',
    defaultMessage: 'Esikatselu'
  }
})

const OpeningHoursPreview = ({
  formName,
  isReadOnly,
  intl: { formatMessage },
  compare,
  preview,
  field,
  className
}) => {
  const previewClass = cx(
    styles.preview,
    {
      [styles.notReadOnly]: !isReadOnly
    },
    className
  )
  return (
    <div className={previewClass}>
      { !isReadOnly &&
        <div className={styles.title}>
          <Label labelText={formatMessage(messages.title)} />
        </div>
      }
      <NormalOpeningHoursPreview
        formName={formName}
        compare={compare}
        preview={preview}
        field={field}
      />
      <SpecialOpeningHoursPreview
        formName={formName}
        compare={compare}
        preview={preview}
        field={field}
      />
      <ExceptionalOpeningHoursPreview
        formName={formName}
        compare={compare}
        preview={preview}
        field={field}
      />
    </div>
  )
}
OpeningHoursPreview.propTypes = {
  formName: PropTypes.string.isRequired,
  isReadOnly: PropTypes.bool,
  compare: PropTypes.bool,
  preview: PropTypes.bool,
  intl: intlShape,
  field: PropTypes.string,
  className: PropTypes.string
}

export default compose(
  injectIntl,
  injectFormName,
  withFormStates,
)(OpeningHoursPreview)
