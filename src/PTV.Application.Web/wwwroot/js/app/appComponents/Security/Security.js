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
import { connect } from 'react-redux'
import { compose } from 'redux'
import { getIsAnyAccessible, getIsAccessible } from './selectors'
import { formEntityTypes } from 'enums'

const renderChildren = (children) => {
  if (children && !Array.isArray(children)) {
    return children
  }
  if (children) {
    return (
      <div>{children}</div>
    )
  }
  return null
}

const SecurityComponent = ({
  children,
  accessible,
  skipSecurityCheck,
  isNotAccessibleComponent
}) => (accessible || skipSecurityCheck ? renderChildren(children) : renderChildren(isNotAccessibleComponent))

SecurityComponent.propTypes = {
  accesible: PropTypes.bool,
  isNotAccessibleComponent: PropTypes.any,
  skipSecurityCheck: PropTypes.bool
}

export const Security = compose(
  connect(
    (state, ownProps) => {
      const securityDomain = ownProps.domain || formEntityTypes[ownProps.formName]
      return {
        accessible : getIsAccessible(state, { ...ownProps, formName: ownProps.formName || 'not defined', domain: securityDomain })
      }
    }
  )
)(SecurityComponent)

export const SecurityAnyDomain = compose(
  connect(
    (state, ownProps) => ({
      accessible : getIsAnyAccessible(state, { ...ownProps, formName: ownProps.formName || 'not defined' })
    })
  )
)(SecurityComponent)
