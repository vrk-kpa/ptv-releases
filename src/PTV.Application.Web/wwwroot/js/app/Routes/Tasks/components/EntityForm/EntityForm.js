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
import React, { PureComponent } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { injectIntl, intlShape } from 'util/react-intl'
import { getKey, formAllTypes } from 'enums'
import {
  DataTable
} from 'util/redux-form/fields'
import { reduxForm } from 'redux-form/immutable'
import { mergeInUIState } from 'reducers/ui'
import { createGetEntityAction } from 'actions'
import { buttonMessages } from 'Routes/messages'
import { Button, Spinner } from 'sema-ui-components'
import styles from './styles.scss'
import withState from 'util/withState'

class EntityForm extends PureComponent {
  handlePreviewClick = (entityId, { subEntityType }) => {
    const previewEntityformName = subEntityType && getKey(formAllTypes, subEntityType.toLowerCase()) || ''
    this.props.mergeInUIState({
      key: 'entityPreviewDialog',
      value: {
        sourceForm: previewEntityformName,
        isOpen: true,
        entityId: null
      }
    })
    this.props.loadPreviewEntity(entityId, previewEntityformName)
  }

  handleLoadMore = () => {
    this.props.loadMore(this.props)
  }

  componentWillReceiveProps (nextProps, prevState) {
    if (this.props.isActive !== nextProps.isActive && nextProps.isActive) {
      nextProps.submit()
    }
  }
  componentDidMount () {
    if (this.props.isActivated) {
      this.props.submit()
      this.props.updateUI('isActivated', false)
    }
  }
  componentWillUnmount () {
    if (this.props.isActive) {
      this.props.updateUI('isActivated', true)
    }
  }

  getColumnsDefinition = this.props.getColumnsDefinition({
    previewOnClick: this.handlePreviewClick,
    formName: this.props.form,
    formatMessage: this.props.intl.formatMessage
  })

  render () {
    const { intl: { formatMessage },
      searchResults,
      form,
      isMoreAvailable,
      isLoadMoreFetching,
      submit,
      isLoading,
      columnWidths = ['20%', '40%', '25%', '15%']
    } = this.props

    return (
      <div>
        {isLoading &&
          <div className={styles.spinner}><Spinner /></div> ||
          <div>
            <div>
              <DataTable
                name={`dataTable${form}`}
                rows={searchResults}
                columnsDefinition={this.getColumnsDefinition}
                // borderless
                scrollable
                columnWidths={columnWidths}
                sortOnClick={submit}
                tight
              />
            </div>
            {isMoreAvailable &&
            <div className={styles.buttonGroup}>
              <Button
                children={isLoadMoreFetching && <Spinner /> || formatMessage(buttonMessages.showMore)}
                onClick={this.handleLoadMore}
                disabled={isLoadMoreFetching}
                small secondary
              />
            </div>}
          </div>}
      </div>
    )
  }
}

const loadMore = (props) => ({ dispatch, getState }) => {
  dispatch(props.loadEntities(getState(), dispatch, { ...props, loadMoreEntities: true }))
}

const onSubmit = (values, dispatch, props) => {
  dispatch(({ dispatch, getState }) => {
    dispatch(
      props.loadEntities(getState(), dispatch, props)
    )
  })
}

EntityForm.propTypes = {
  form: PropTypes.string.isRequired,
  searchResults: PropTypes.string.isRequired,
  intl: intlShape.isRequired,
  loadPreviewEntity: PropTypes.func,
  getColumnsDefinition: PropTypes.func.isRequired,
  mergeInUIState: PropTypes.func,
  submit: PropTypes.func,
  loadMore: PropTypes.func,
  isMoreAvailable: PropTypes.bool,
  isActive: PropTypes.bool.isRequired,
  isLoadMoreFetching: PropTypes.bool,
  columnWidths: PropTypes.array,
  isActivated: PropTypes.bool,
  isLoading: PropTypes.bool,
  updateUI: PropTypes.func
}

export default compose(
  injectIntl,
  withState({
    key: ({ form }) => form,
    initialState: () => ({
      isActivated:false
    }),
    redux: true
  }),
  connect(
    null
    , {
      mergeInUIState,
      loadMore,
      loadPreviewEntity: createGetEntityAction
    }),
  reduxForm({
    onSubmit
  })
)(EntityForm)
