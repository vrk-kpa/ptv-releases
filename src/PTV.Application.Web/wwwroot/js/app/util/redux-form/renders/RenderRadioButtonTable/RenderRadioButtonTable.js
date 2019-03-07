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
import { SimpleTable, Spinner } from 'sema-ui-components'
import { compose } from 'redux'
import withFormFieldAnchor from 'util/redux-form/HOC/withFormFieldAnchor'
import { fieldPropTypes } from 'redux-form/immutable'
import EmptyTableMessage from 'appComponents/EmptyTableMessage'

// const rows = [
//   { id: 0, name: 'John' },
//   { id: 1, name: 'Jack' }
// ]
// const columns = [
//   {
//     property: 'id',
//     header: { label: 'id' }
//   },
//   {
//     property: 'name',
//     header: { label: 'Name' }
//   }
// ]
const RenderRadioButtonTable = ({
  input,
  rows,
  columns,
  className,
  isLoading,
  ...rest
}) => {
  const onRow = ({ id }, { rowIndex }) => ({
    // onClick: () => input.onClick(id)
  })
  return (
    <SimpleTable columns={columns} className={className} {...rest}>
      <SimpleTable.Header />
      {isLoading && <EmptyTableMessage children={<Spinner />} colSpan={4} />}
      {!rows.length && !isLoading
        ? <EmptyTableMessage colSpan={4} />
        : <SimpleTable.Body
          onRow={onRow}
          rows={rows}
          rowKey='id'
        />}
    </SimpleTable>
  )
}
RenderRadioButtonTable.propTypes = {
  input: fieldPropTypes.input,
  rows: PropTypes.array.isRequired,
  columns: PropTypes.array.isRequired,
  className: PropTypes.string,
  isLoading: PropTypes.bool
}

export default compose(
  withFormFieldAnchor
)(RenderRadioButtonTable)
