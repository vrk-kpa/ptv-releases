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

import React, { Component, createElement } from 'react'
import PropTypes from 'prop-types'
import shortId from 'shortid'
import cx from 'classnames'
import styles from './styles.scss'

class BodyRow extends Component {
  renderRow = (columns, rowIndex) => {
    const { data } = this.props
    return columns.map((column, index) => {
      const { property, header = {}, cell = {}, customClass } = column
      const bodyCellClass = cx(
        customClass || undefined,
        styles.bodyCell
      )
      const newProperty = property || header.property
      if (cell.formatters && Array.isArray(cell.formatters) && cell.formatters[0]) {
        const formatter = cell.formatters[0]
        if (typeof formatter !== 'function') {
          console.error('Table Row: formatter is not a function')
          return <div>Undefined Formatter</div>
        }
        const bodyRowItemKey = `table-body-row-${rowIndex}-item-${index}}`
        return (
          <div className={bodyCellClass} key={bodyRowItemKey}>
            {formatter(data[newProperty], {
              rowData: data,
              props: { ...this.props }
            })}
          </div>
        )
      }

      return typeof data[newProperty] === 'string' && <div className={bodyCellClass}>{data[newProperty]}</div>
    })
  }
  render () {
    const {
      rowIndex,
      columns,
      customRowComponent,
      data,
      bodyRowClassName
    } = this.props
    const bodyRowClass = cx(
      styles.bodyRow,
      bodyRowClassName
    )
    return (
      <div className={bodyRowClass}>
        {this.renderRow(columns, rowIndex)}
        {customRowComponent && createElement(customRowComponent,
          { ...data })
        }
      </div>
    )
  }
}

BodyRow.propTypes = {
  bodyRowClassName: PropTypes.string,
  data: PropTypes.object.isRequired,
  rowIndex: PropTypes.number,
  columns: PropTypes.array,
  customRowComponent: PropTypes.any
}

export default BodyRow
