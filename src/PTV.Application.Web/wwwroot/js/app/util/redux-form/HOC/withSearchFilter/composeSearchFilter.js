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
import { compose } from 'redux'
import SearchField from './SearchField'
import withState from 'util/withState'

export const getFilteredProps = (filterFunc, propertyName) => (props) => {
  if (!props.searchValue || props.searchValue === '') {
    return props
  }
  const list = props[propertyName]
  const newProps = {
      ...props,
      [propertyName]: list && list.filter(x => filterFunc(x, props))
    }
    return newProps
}

export const filterListComponent = (filterFunc, propertyName) => FilteredComponent => {
  const filterProps = getFilteredProps(filterFunc, propertyName)
  const Filter = props => {
    const newProps = filterProps(props)
    return <FilteredComponent {...newProps} />
  }
  return Filter
}

export const filterComponent = filterFunc => FilteredComponent => {
  const Filter = props => {
    if (filterFunc(props)) {
      return (
        <FilteredComponent {...props} />
      )
    }
    return null
  }
  return Filter
}

export const withSearchFilter = (options = {}) => FilteredComponent => {
  const searchOptions = {
    ...options,
    charCount: options.charCount || 3,
    FilterComponent: options.FilterComponent || SearchField,
    componentClass: options.componentClass || 'col-lg-24',
    isText: options.isText != null ? options.isText : true
  }

  const SearchFilter = ({
    searchValue,
    uiKey,
    uiPath,
    resetUI,
    updateUI,
    ...props
  }) => {
    const onChange = (value) => {
      updateUI('searchValue', value)
      if (typeof searchOptions.filterFunc === 'function') {
        if (searchOptions.isText) {
          if (value.length >= searchOptions.charCount) {
            searchOptions.filterFunc(props, value)
          } else if (value === '') {
            searchOptions.filterFunc(props, '')
          }
        } else {
          searchOptions.filterFunc(props, value)
        }
      }
    }

    const value = searchOptions.isText
       ? (searchValue.length >= searchOptions.charCount ? searchValue : '').toLowerCase()
       : searchValue

    return (
      <div>
        <div className='row'>
          <div className={searchOptions.componentClass}>
            <searchOptions.FilterComponent
              searchValue={searchValue}
              onChange={onChange}
              partOfTree={searchOptions.partOfTree}
              {...props}
            />
          </div>
        </div>
        <FilteredComponent
          searchValue={value}
          {...props}
          onSearchValueChange={onChange}
        />
      </div>
    )
  }

  SearchFilter.propTypes = {
    searchValue: PropTypes.string,
    uiKey: PropTypes.any,
    uiPath: PropTypes.any,
    resetUI: PropTypes.func,
    updateUI: PropTypes.func.isRequired
  }

  return compose(
    withState({
      initialState: {
        searchValue: ''
      }
    })
  )(SearchFilter)
}
