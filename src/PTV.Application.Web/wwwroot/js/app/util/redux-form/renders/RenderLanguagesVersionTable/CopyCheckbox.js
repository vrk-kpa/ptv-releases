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
import { Checkbox } from 'sema-ui-components'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { getLanguagesToCopy } from 'selectors/copyEntity'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import PropTypes from 'prop-types'
import ImmutablePropTypes from 'react-immutable-proptypes'
import styles from './styles.scss'
import { fieldPropTypes } from 'redux-form/immutable'

const CopyCheckbox = ({
  copyLanguages,
  languageCode,
  languageId,
  input
}) => {
  const checked = copyLanguages.includes(languageId)
  const handleOnChange = () => {
    let newValue = checked
      ? input.value.filter(lang => lang !== languageId)
      : input.value.set(input.value.size, languageId)
    input.onChange(newValue)
  }

  return (
    <div className={styles.copyCheckbox}>
      <Checkbox
        checked={checked}
        onChange={handleOnChange}
      />
    </div>
  )
}

CopyCheckbox.propTypes = {
  copyLanguages: ImmutablePropTypes.list,
  languageCode: PropTypes.string,
  languageId: PropTypes.string,
  input: fieldPropTypes.input
}

export default compose(
  injectFormName,
  connect((state, { languageId, formName }) => {
    const copyLanguages = getLanguagesToCopy(state, { formName })
    return {
      copyLanguages
    }
  })
)(CopyCheckbox)
