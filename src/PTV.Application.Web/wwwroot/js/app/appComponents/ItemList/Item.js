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
import styles from './styles.scss'
import { injectIntl, intlShape } from 'util/react-intl'
import { Button } from 'sema-ui-components'
import { compose } from 'redux'
import Languages from './Languages'

const getPath = path => path && path.reduce((prev, curr) => {
  const delimiter = prev && '.' || ''
  return prev + (Number.isInteger(curr) ? `[${curr}]` : delimiter + curr)
})

const renderLanguages = languages => languages && languages.length &&
  <Languages languages={languages} /> || null

const renderPathLabel = (path, formatMessage) => {
  // console.log(path)
  if (path && path.length > 1) {
    return `${localize(path[0], formatMessage)} / `
  }
  return null
}

const renderLabel = (pathLabel, label, languages, formatMessage, showForLanguage) => (
  <span className={styles.itemTitle}>
    {renderPathLabel(pathLabel, formatMessage)}{localize(label, formatMessage)}
    {!showForLanguage && renderLanguages(languages)}
  </span>
)

const renderFieldDescription = (path, pathLabel, label, languages,
  formatMessage, onClickAction, showForLanguage) => {
  const link = getPath(path)

  return label && (
    link && <Button
      type='button'
      link
      onClick={() => onClickAction(link) || (() => {})}
      className={styles.itemLink}
    >
      {renderLabel(pathLabel, label, languages, formatMessage, showForLanguage)}
    </Button> ||
    renderLabel(pathLabel, label, languages, formatMessage, showForLanguage)
  ) || null
}

const formatOptions = (options, formatMessage) => {
  return Object.keys(options).reduce((acc, curr) => {
    if (Array.isArray(options[curr])) {
      acc[curr] = options[curr]
        .map(item => localize(item, formatMessage))
        .join(', ')
    } else if (typeof options[curr] === 'object') {
      acc[curr] = localize(options[curr], formatMessage)
    } else {
      acc[curr] = options[curr]
    }
    return acc
  }, {})
}

const localize = (value, formatMessage, options) =>
  value && value.id && (
    options
      ? formatMessage(value, formatOptions(options, formatMessage))
      : formatMessage(value)
  ) || value

const Item = ({
  label,
  message,
  onClick,
  options,
  languages,
  path,
  pathLabel,
  showForLanguage,
  intl: { formatMessage }
}) => {
  return (
    <li className={styles.item}>
      <div>
        {renderFieldDescription(path, pathLabel, label, languages, formatMessage, onClick, showForLanguage)}
        {localize(message, formatMessage, options)}
      </div>
    </li>
  )
}

Item.propTypes = {
  label: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.object
  ]),
  languages: PropTypes.array,
  onClick: PropTypes.func,
  options: PropTypes.object,
  message: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.object
  ]),
  path: PropTypes.array,
  pathLabel: PropTypes.array,
  intl: intlShape.isRequired,
  showForLanguage: PropTypes.string
}

export default compose(
  injectIntl
)(Item)
