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
import { get, isUndefined } from 'lodash'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { fieldPropTypes } from 'redux-form/immutable'

const withLazyLoading = ({
  loadItemByIdAction,
  getIsItemInStoreAction
}) => WrappedComponent => {
  class LazyRender extends Component {
    loadMissingItem = postalCodeId => {
      const id = postalCodeId || get(this.props, 'input.value')
      if (id) {
        this.props.loadItemById(id)
      }
    }
    componentDidMount () {
      if (!this.props.isPostalCodeInStore) {
        this.loadMissingItem()
      }
    }
    componentDidUpdate (previousProps) {
      const currentPostalCodeId = get(this.props, 'input.value')
      const previousPostalCodeId = get(previousProps, 'input.value')
      if (
        !isUndefined(currentPostalCodeId) &&
        !isUndefined(previousPostalCodeId) &&
        currentPostalCodeId !== previousPostalCodeId &&
        !this.props.isPostalCodeInStore
      ) {
        this.loadMissingItem(currentPostalCodeId)
      }
    }
    render () {
      return <WrappedComponent {...this.props} />
    }
  }
  LazyRender.propTypes = {
    loadItemById: PropTypes.func,
    isPostalCodeInStore: PropTypes.bool,
    input: fieldPropTypes.input,
    options: PropTypes.object
  }
  return compose(
    connect(
      (state, { input: value }) => ({
        isItemInStore: getIsItemInStoreAction(state, { id: value })
      }), {
        loadItemById: loadItemByIdAction
      }
    )
  )(LazyRender)
}

export default withLazyLoading
