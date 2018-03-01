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
import React, { PropTypes, Component } from 'react'
import Organizations from './Organizations/Pages'
import GeneralDescriptions from './GeneralDescriptions/GeneralDescriptions/Pages'
import SearchGeneralDescriptionContainer from './GeneralDescriptions/Pages'
import SearchServicePackageContainer from './ServicePackages/Pages'
import SearchUserContainer from './Users/Pages'

const ManageContainer = (props) => {
  const renderContainer = (props) => {
    switch (props.params.id) {
      case 'organizations':
        return <Organizations {...props} />
      case 'generalDescriptions':
        return <SearchGeneralDescriptionContainer {...props} />
      case 'generalDescription':
        return <GeneralDescriptions {...props} />
      case 'servicePackages':
        return <SearchServicePackageContainer {...props} />
      case 'users':
        return <SearchUserContainer {...props} />
      default:
        return <Organizations {...props} />
    }
  }

  return renderContainer(props)
}

export default ManageContainer
