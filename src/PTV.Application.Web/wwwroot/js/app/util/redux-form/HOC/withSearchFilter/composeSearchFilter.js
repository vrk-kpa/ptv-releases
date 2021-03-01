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
import React from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import SearchField from './SearchField'
import withState from 'util/withState'
import styles from './styles.scss'

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
    searchValue = '',
    updateUI,
    filterClass,
    componentClass,
    ...props
  }) => {
    const onChange = (value) => {
      updateUI('searchValue', value)
      if (typeof searchOptions.filterFunc === 'function') {
        if (searchOptions.isText) {
          if (value.length >= searchOptions.charCount) {
            searchOptions.filterFunc(props, value)
            updateUI('isOpen', true)
          } else if (value === '') {
            searchOptions.filterFunc(props, '')
            updateUI('isOpen', false)
          }
        } else {
          searchOptions.filterFunc(props, value)
        }
      }
    }
    const onFocus = () => {
      updateUI('isFocused', true)
    }
    const onBlur = () => {
      updateUI('isFocused', false)
    }

    const value = searchOptions.isText
      ? (searchValue.length >= searchOptions.charCount ? searchValue : '').toLowerCase()
      : searchValue

    return (
      <div className={styles.withSearchFilter}>
        <searchOptions.FilterComponent
          searchValue={searchValue}
          onChange={onChange}
          onFocus={onFocus}
          onBlur={onBlur}
          partOfTree={searchOptions.partOfTree}
          className={filterClass}
          searchPlaceholder={searchOptions.searchPlaceholder}
          {...props}
        />
        <FilteredComponent
          searchValue={value}
          className={componentClass}
          {...props}
          onSearchValueChange={onChange}
        />
      </div>
    )
  }

  SearchFilter.propTypes = {
    searchValue: PropTypes.string,
    uiPath: PropTypes.any,
    resetUI: PropTypes.func,
    updateUI: PropTypes.func.isRequired,
    filterClass: PropTypes.string,
    componentClass: PropTypes.string
  }
  return (
    compose(
      withState({
        redux: searchOptions.withReduxState,
        key: searchOptions.withReduxState ? searchOptions.key : null,
        initialState: {
          searchValue: '',
          isFocused: false
        }
      })
    )
  )(SearchFilter)
}
