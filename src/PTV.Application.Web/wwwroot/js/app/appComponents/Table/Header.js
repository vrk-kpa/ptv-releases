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

import React, { Component, Fragment } from 'react'
import PropTypes from 'prop-types'
import { isFunction } from 'util/helpers'
import shortId from 'shortid'
import cx from 'classnames'
import styles from './styles.scss'
import { compose } from 'redux'
import { injectIntl } from 'util/react-intl'

class Header extends Component {
  handleColumnHeaderClick = (column) => {
    this.props.sortOnClick && isFunction(this.props.sortOnClick) && this.props.sortOnClick(column)
  }
  columns = this.props.columnsDefinition(this.handleColumnHeaderClick)
  renderHeader = () => {
    // TODO: add needed code for html tag below
    return (
      <Fragment>
        {this.columns.map((column, columnIndex) => {
          const { property, onColumn, header = {}, props = {}, customClass } = column
          isFunction(onColumn) && onColumn(column, columnIndex)
          const newProperty = property || header.property
          const {
            label,
            formatters = []
          } = header
          const headerCellClass = cx(
            customClass || undefined,
            styles.headerCell
          )
          const headerColumn = (label ||
              (formatters &&
                formatters.map((f, index) => isFunction(f) && (
                  <div
                    // className={headerCellClass}
                    key={`table-header-column-${columnIndex}-item-${index}`}>
                    {f(label, {
                      column,
                      props: {
                        ...this.props,
                        ...props,
                        property: newProperty
                        // key: `table-header-column-${columnIndex}-item-${index}-${shortId.generate()}`
                      }
                    })}
                  </div>
                ) ||
                console.error('TableHeader: formatter has to be a function'))) ||
                console.error('TableHeader: no lable or formatters provided')) ||
                console.error('Table: no header provided')
          /* TODO: add needed code for html tag below */
          return (
            <div
              className={headerCellClass}
              key={`table-header-column-${columnIndex}-${shortId.generate()}`}>{headerColumn}
            </div>
          )
        })}
      </Fragment>
    )
  }

  render () {
    const { headerClassName } = this.props
    const headerClass = cx(
      styles.header,
      headerClassName
    )
    return (<div className={headerClass}>
      {this.renderHeader()}
    </div>)
  }
}

Header.propTypes = {
  headerClassName: PropTypes.string,
  columnsDefinition: PropTypes.func.isRequired,
  sortOnClick: PropTypes.func
}

export default compose(
  injectIntl
)(Header)
