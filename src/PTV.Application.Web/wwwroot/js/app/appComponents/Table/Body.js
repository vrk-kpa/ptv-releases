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

import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { isFunction } from 'util/helpers'
import { Iterable } from 'immutable'
import BodyRow from './BodyRow'
import Placeholder from 'appComponents/Placeholder'
import PlaceholderLabel from 'appComponents/PlaceholderLabel'
import cx from 'classnames'
import styles from './styles.scss'
import { defaultEmptyMessage } from 'appComponents/EmptyTableMessage/EmptyTableMessage'
import { compose } from 'redux'
import { injectIntl } from 'util/react-intl'
const { isIterable: isImmutable } = Iterable

class Body extends Component {
  handleColumnHeaderClick = () => {
    this.props.sortOnClick && isFunction(this.props.sortOnClick) && this.props.sortOnClick()
  }
  columns = this.props.columnsDefinition(this.handleColumnHeaderClick)
  renderBody = () => {
    const { rows } = this.props
    if (!rows) return null
    if (Array.isArray(rows) && rows.length === 0) {
      return (
        <Placeholder className={styles.empty}>
          <PlaceholderLabel placeholder={defaultEmptyMessage} />
        </Placeholder>
      )
    }
    const evaluatedRows = isImmutable(rows) && rows.toJS() || !Array.isArray(rows) && Object.values(rows) || rows

    return evaluatedRows.map((row, rowIndex) => {
      const rowId = row.id || rowIndex
      return (
        <BodyRow
          key={`table-body-row-${rowId}`}
          rowIndex={rowIndex}
          data={row}
          columns={this.columns}
          {...this.props}
        />
      )
    })
  }

  render () {
    const { bodyClassName } = this.props
    const bodyClass = cx(
      styles.body,
      bodyClassName
    )
    return (
      <div className={bodyClass}>
        {this.renderBody()}
      </div>
    )
  }
}

Body.propTypes = {
  bodyClassName: PropTypes.string,
  columnsDefinition: PropTypes.func.isRequired,
  columnWidths: PropTypes.array,
  sortOnClick: PropTypes.func,
  rows: PropTypes.any
}

export default compose(
  injectIntl
)(Body)
