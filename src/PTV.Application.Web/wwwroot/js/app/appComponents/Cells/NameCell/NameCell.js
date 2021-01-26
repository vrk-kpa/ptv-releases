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
import cx from 'classnames'
import { PTVIcon } from 'Components'
import styles from './styles.scss'
import { connect } from 'react-redux'
import { getContentLanguageCode } from 'selectors/selections'
import Popup from 'appComponents/Popup'
import ClampText from 'appComponents/ClampText'

const getName = (name, languageCode, altName, useAltName) => {
  if (name && name.toJS) {
    name = name.toJS()
  }

  if (useAltName) {
    useAltName.forEach(lang => {
      if (altName && altName[lang]) {
        if (name[lang]) {
          name[lang] = altName[lang]
        }
      }
    })
  }

  const result = name && (typeof name === 'object' && (
    name[languageCode] ||
    name[Object.keys(name)[0]]
  ) || typeof name === 'string' && name)
  return result || ''
}

const NameCell = ({
  name,
  id,
  viewIcon,
  viewOnClick,
  languageCode,
  alternateName,
  isAlternateNameUsedAsDisplayName,
  bold,
  componentClass,
  textClass,
  ...rest
}) => {
  const handleViewClick = () => {
    viewIcon && viewOnClick && typeof viewOnClick === 'function' && viewOnClick(id, rest)
  }
  const cellNameClass = cx(
    'cell',
    styles.cellName,
    {
      [styles.withIcon]: viewIcon
    },
    bold ? styles.bold : null,
    componentClass
  )
  return (
    <div className={cellNameClass}>
      {viewIcon && <PTVIcon name='icon-eye' onClick={handleViewClick} />}
      <Popup
        trigger={
          <div className={styles.name}>
            <ClampText
              text={getName(name, languageCode, alternateName, isAlternateNameUsedAsDisplayName)}
              id={id}
              className={textClass}
            />
          </div>
        }
        position='top left'
        on='hover'
        iconClose={false}
        content={getName(name, languageCode)}
      />
    </div>
  )
}

NameCell.propTypes = {
  name: PropTypes.any,
  id: PropTypes.string,
  viewIcon: PropTypes.bool,
  viewOnClick: PropTypes.func,
  languageCode: PropTypes.string,
  componentClass: PropTypes.string,
  textClass: PropTypes.string
}
export default connect(
  (state, ownProps) => ({
    languageCode: ownProps.languageCode || getContentLanguageCode(state, ownProps),
    alternateName: ownProps.alternateName,
    isAlternateNameUsedAsDisplayName: ownProps.isAlternateNameUsedAsDisplayName
  })
)(NameCell)
