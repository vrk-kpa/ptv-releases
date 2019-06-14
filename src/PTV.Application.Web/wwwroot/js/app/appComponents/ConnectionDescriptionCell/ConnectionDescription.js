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
import { getSelectedLanguage } from '../../Intl/Selectors'
import { Label } from 'sema-ui-components'
import styles from './styles.scss'

const getName = (name, languageCode) => {
  if (name.toJS) {
    name = name.toJS()
  }
  const result = name && (
    name[languageCode] ||
    name[Object.keys(name)[0]]
  )
  return result
}

const ConnectionDescription = ({
    description,
    label,
    languageCode
}) => {
  return (
    description && description.size
      ? <div className={styles.previewBlock}>
        <Label labelText={label} />
        <div className={styles.previewItem}>
          {getName(description, languageCode)}
        </div>
      </div>
      : null
  )
}

ConnectionDescription.propTypes = {
  description: PropTypes.any,
  languageCode: PropTypes.string,
  label: PropTypes.any
}
export default connect(
  (state, ownProps) => ({
    languageCode:  getSelectedLanguage(state)
  })
)(ConnectionDescription)
