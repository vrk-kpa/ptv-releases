
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
import { connect } from 'react-redux'
import { compose } from 'redux'
import RenderOrderTableHeader from './RenderOrderTableHeader'
import RenderOrderTableRow from './RenderOrderTableRow'
import withState from 'util/withState'
import styles from './styles.scss'
import { getTranslationStatesWithOrderEntities } from './selectors'

const TranslationOrderTable = ({ openedIndex, updateUI, translationData }) => {
  return (
    <div>
      <div className={styles.table}>
        <div className={styles.tableHead}>
          <div className={styles.tableRow}>
            <RenderOrderTableHeader />
          </div>
        </div>
        <div className={styles.tableBody}>
          {translationData && translationData.map((rowData) => {
            const rowId = rowData.has('id') && rowData.get('id')
            const isOpen = openedIndex === rowId
            const handleOnOpen = () => updateUI({
              openedIndex: isOpen
                ? null
                : rowId
            })
            return (
              <div>
                <RenderOrderTableRow
                  isOpen={isOpen}
                  onOpen={handleOnOpen}
                  inputData={rowData}
                  stateTypeId={rowData && rowData.get('translationStateTypeId')}
                />
              </div>
            )
          })}
        </div>
      </div>
    </div>
  )
}

TranslationOrderTable.propTypes = {
  rows: PropTypes.array,
  openedIndex: PropTypes.string,
  updateUI: PropTypes.func.isRequired,
  translationData:  PropTypes.array
}

export default compose(
  connect((state, ownProps) => ({
    translationData : getTranslationStatesWithOrderEntities(state)
  })
  ),
  withState({
    initialState: {
      openedIndex: null
    }
  })
)(TranslationOrderTable)

