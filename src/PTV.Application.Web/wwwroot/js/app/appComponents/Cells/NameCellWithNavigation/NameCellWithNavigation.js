/* eslint-disable no-trailing-spaces */
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
import Tag from 'appComponents/Tag'
import cx from 'classnames'
import styles from './styles.scss'
import { withRouter } from 'react-router'
import { getContentLanguageCode, getCodesOfTranslationLanguages } from 'selectors/selections'
import { getEntityInfo } from 'enums'
import { defineMessages } from 'util/react-intl'
import withReturnLink from 'util/redux-form/HOC/withReturnLink'
import IconEye from 'appComponents/IconEye'

const messages = defineMessages({
  copiedEntityTitle: {
    id: 'FrontPage.Search.CopiedEntityTag.Title',
    defaultMessage: 'KOPIOITU'
  }
})

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
  history,
  sortedCodes,
  viewIcon,
  viewOnClick,
  sub,
  id,
  linkProps = {},
  customName,
  getNavigationReturnLink,
  viewCopiedTag,
  formatMessage,
  ...rest
}) => {
  const cellNameClass = cx(
    'cell',
    styles.cellName,
    {
      [styles.withIcon]: viewIcon
    }
  )
  const nameValue = customName || name
  return (
    <div className={cellNameClass}>
      {viewIcon && <IconEye viewOnClick={viewOnClick} id={id} {...rest} />}
      <Popup
        trigger={
          <div className={cx(componentClass, 'cell', 'cell-name')}>
            <div>
              <Link
                className={styles.nameCell}
                to={getNavigationReturnLink(getEntityInfo(main, sub).path + '/' + id)}
                {...linkProps}
              >{
                  typeof nameValue === 'string'
                    ? nameValue
                    : getName(nameValue, languageCode, sortedCodes)
                }
              </Link>
            </div>
            {viewCopiedTag &&
              <div>
                <Tag
                  message={formatMessage(messages.copiedEntityTitle)}
                  isRemovable={false}
                  bgColor='#e6e6e6'
                  textColor='#000000'
                />
              </div>
            }
          </div>
        }
        position='top left'
        on='hover'
        iconClose={false}
        content={typeof name === 'string' ? name : getName(name, languageCode, sortedCodes)}
      />
    </div >
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
  viewIcon: PropTypes.bool,
  linkProps: PropTypes.object,
  customName: PropTypes.any,
  history: PropTypes.any,
  getNavigationReturnLink: PropTypes.func,
  viewCopiedTag: PropTypes.bool,
  formatMessage: PropTypes.func
}

NameCellWithNavigation.defaultProps = {
  viewCopiedTag: false
}

export default compose(
  withRouter,
  withReturnLink,
  connect(
    (state, ownProps) => ({
      main: camelCase(ownProps.mainEntityType) || camelCase(ownProps.entityType),
      sub: camelCase(ownProps.subEntityType),
      languageCode: ownProps.languageCode || getContentLanguageCode(state, ownProps),
      sortedCodes: getCodesOfTranslationLanguages(state)
    })
  ))(NameCellWithNavigation)
