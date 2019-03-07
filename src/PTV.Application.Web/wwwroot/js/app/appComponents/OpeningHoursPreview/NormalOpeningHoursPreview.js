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
import ImmutablePropTypes from 'react-immutable-proptypes'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { formValueSelector } from 'redux-form/immutable'
import OpeningHoursWeekPreview from './OpeningHoursWeekPreview'
import withPath from 'util/redux-form/HOC/withPath'
import { List } from 'immutable'
import styles from './styles.scss'

const NormalOpeningHoursPreview = ({
  openingHours,
  composedPath,
  ...rest
}) => (
  openingHours.size > 0 &&
    <div className={styles.previewBlock}>
      {openingHours.map((openingHours, index) => (
        <OpeningHoursWeekPreview
          key={index}
          openingHours={openingHours}
          index={index}
          composedPath={composedPath}
          {...rest}
        />
      ))}
    </div>
)
NormalOpeningHoursPreview.propTypes = {
  openingHours: ImmutablePropTypes.list.isRequired,
  composedPath: PropTypes.string
}

export default compose(
  withPath,
  connect(
    (state, { formName, path, field }) => {
      const getFormValues = formValueSelector(formName)
      const pathParts = [path, field, 'openingHours.normalOpeningHours']
      const fromIndex = pathParts.lastIndexOf(undefined)
      const composedPath = fromIndex !== -1 && pathParts.slice(fromIndex + 1, pathParts.length).join('.') || pathParts.join('.')
      // const composedPath = [path, field, 'openingHours.normalOpeningHours'].filter(val => val).join('.')
      return {
        openingHours: getFormValues(state, composedPath) || List(),
        composedPath
      }
    }
  )
)(NormalOpeningHoursPreview)
