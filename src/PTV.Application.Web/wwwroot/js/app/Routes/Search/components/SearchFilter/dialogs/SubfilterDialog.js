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
import { change } from 'redux-form/immutable'
import { OrderedSet } from 'immutable'
import { searchInTree, clearTreeSearch } from 'Containers/Common/Actions/Nodes'
import ModalDialog from 'appComponents/ModalDialog'
import styles from '../styles.scss'
import { Checkbox } from 'sema-ui-components'
import { messages } from '../messages'
import Spacer from 'appComponents/Spacer'
import { ClearableFulltext } from 'util/redux-form/fields'
import PropTypes from 'prop-types'
import { injectIntl, intlShape } from 'util/react-intl'
import { compose } from 'redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { connect } from 'react-redux'
import { getFormValueWithPath } from 'selectors/base'

const SubfilterDialog = props => {
  const {
    // Props from connect
    intl: { formatMessage },
    dispatch,
    formName,
    searchValue,
    // Props related to the ALL option
    hasAllOption,
    isAllSelected,
    storeValueName,
    // Props related to search
    hasSearch,
    searchFieldName,
    treeTypeName,
    searchSchema,
    // Common dialog props
    dialogName,
    dialogTitleMessage,
    // Props related to inner tree component
    TreeComponent
  } = props

  const handleAllChange = () => {
    if (!isAllSelected) {
      dispatch(change(formName, storeValueName, OrderedSet()))
    }
  }

  const handleSearchChange = (input, value) => {
    if (!value || value.length < 3) {
      dispatch(clearTreeSearch(treeTypeName))
      return
    }

    dispatch(searchInTree({
      searchSchema,
      treeType: treeTypeName,
      value,
      contextId: value
    }))
  }

  return (
    <ModalDialog
      className={styles.searchFilterDialog}
      name={dialogName}
      title={`${formatMessage(messages.commonSearchFilterTitle)}: ${formatMessage(dialogTitleMessage)}`}
    >
      <div className={styles.dialogBody}>
        {hasAllOption &&
          <Checkbox
            label={formatMessage(messages.allSelectedLabel)}
            onChange={handleAllChange}
            checked={isAllSelected}
            disabled={isAllSelected}
            className={styles.node}
          />
        }
        <Spacer />
        {hasSearch &&
          <ClearableFulltext
            name={searchFieldName}
            placeholder={dialogTitleMessage}
            onChange={handleSearchChange}
            labelMessage={messages.commonSearchLabel}
            componentClass={styles.searchWrap}
            inputClass={styles.searchInput}
            iconClass={styles.searchIcon}
          />
        }
        <TreeComponent
          searchValue={searchValue}
          filterTree
          simple
          nodeClass={styles.node}
          containerClass={styles.nodeContainer}
        />
      </div>
    </ModalDialog>
  )
}

SubfilterDialog.propTypes = {
  intl: intlShape,
  dispatch: PropTypes.func,
  formName: PropTypes.string,
  hasAllOption: PropTypes.bool,
  isAllSelected: PropTypes.bool,
  hasSearch: PropTypes.bool,
  searchValue: PropTypes.string,
  searchFieldName: PropTypes.string,
  storeValueName: PropTypes.string,
  treeTypeName: PropTypes.string,
  searchSchema: PropTypes.object,
  dialogName: PropTypes.string,
  dialogTitleMessage: PropTypes.object,
  TreeComponent: PropTypes.func
}

export default compose(
  injectIntl,
  injectFormName,
  connect((state, ownProps) => {
    const searchValue = ownProps.searchFieldName
      ? getFormValueWithPath(_ => ownProps.searchFieldName)(state, ownProps)
      : undefined

    return {
      searchValue
    }
  })
)(SubfilterDialog)
