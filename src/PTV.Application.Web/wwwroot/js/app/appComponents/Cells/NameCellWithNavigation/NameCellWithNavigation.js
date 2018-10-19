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
import { camelCase } from 'lodash'
import { Link } from 'react-router-dom'
import Popup from 'appComponents/Popup'
import cx from 'classnames'
import styles from './styles.scss'
import { withRouter } from 'react-router'
import { PTVIcon } from 'Components'
import { getContentLanguageCode, getCodesOfTranslationLanguages } from 'selectors/selections'
import { getEntityInfo } from 'enums'
const getName = (name, languageCode, sortedCodes) => {
  if (name.toJS) {
    name = name.toJS()
  }

  let result = null
  name && (
    sortedCodes.insert(0, languageCode).forEach(
      l => {
        result = name[l]
        return !result
      }
    )
  )
  return result
}

const NameCellWithNavigation = ({
  name = '',
  componentClass,
  main,
  languageCode,
  sortedCodes,
  viewIcon,
  viewOnClick,
  sub,
  id,
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
    }
  )
  return (
    <div className={cellNameClass}>
      {viewIcon && <PTVIcon name='icon-eye' onClick={handleViewClick} />}
      <Popup
        trigger={
          <div className={cx(componentClass, 'cell', 'cell-name')}>
            <Link
              className={styles.nameCell}
              to={getEntityInfo(main, sub).path + '/' + id}
            >{
                typeof name === 'string'
                  ? name
                  : getName(name, languageCode, sortedCodes)
              }
            </Link>
          </div>
        }
        position='top left'
        on='hover'
        iconClose={false}
        content={getName(name, languageCode, sortedCodes)}
      />
    </div>
  )
}

NameCellWithNavigation.propTypes = {
  name: PropTypes.any, // currently is returned as string for old impl, in new as object (fi,sv...)
  componentClass: PropTypes.string,
  main: PropTypes.string,
  languageCode: PropTypes.string,
  sortedCodes: ImmutablePropTypes.list.isRequired,
  sub: PropTypes.string,
  id: PropTypes.string,
  viewOnClick: PropTypes.func,
  viewIcon: PropTypes.bool
}

export default compose(
  withRouter,
  connect(
    (state, ownProps) => ({
      main: camelCase(ownProps.mainEntityType),
      sub: camelCase(ownProps.subEntityType),
      languageCode: ownProps.languageCode || getContentLanguageCode(state, ownProps),
      sortedCodes: getCodesOfTranslationLanguages(state)
    })
  ))(NameCellWithNavigation)
