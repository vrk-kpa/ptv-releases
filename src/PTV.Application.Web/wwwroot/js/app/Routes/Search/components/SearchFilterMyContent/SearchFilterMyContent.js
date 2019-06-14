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
import { compose } from 'redux'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import Checkbox from 'util/redux-form/fields/Checkbox'
import Tooltip from 'appComponents/Tooltip'
import cx from 'classnames'
import styles from '../styles.scss'

const messages = defineMessages({
  title: {
    id: 'Routes.Search.SearchFilterMyContent.Title',
    defaultMessage: 'Hae vain omista sisällöistä'
  },
  tooltip: {
    id: 'Routes.Search.SearchFilterMyContent.Tooltip',
    defaultMessage: 'Voit hakea sisällön nimen lisäksi myös sisällön id-numeron ja muokkaajan sähköpostiosoitteen perusteella.' // eslint-disable-line
  }
})

const SearchFilterMyContent = ({
  intl: { formatMessage },
  componentClass,
  ...rest
}) => {
  const myContentClass = cx(
    styles.myContent,
    componentClass
  )
  return (
    <div className={myContentClass}>
      <Checkbox
        name='myContent'
        label={formatMessage(messages.title)}
        small
        centered
        className={styles.searchRowCheckbox}
        {...rest}
      />
      <Tooltip tooltip={formatMessage(messages.tooltip)} indent='i5' />
    </div>
  )
}

SearchFilterMyContent.propTypes = {
  intl: intlShape,
  componentClass: PropTypes.string
}

export default compose(
  injectIntl
)(SearchFilterMyContent)
