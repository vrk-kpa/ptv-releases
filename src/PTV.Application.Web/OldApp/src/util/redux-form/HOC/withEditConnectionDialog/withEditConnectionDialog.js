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
import React, { PureComponent } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import withState from 'util/withState'
import EditConnectionDialog from 'appComponents/EditConnectionDialog'

const withEditConnectionDialog = ({ reduxKey } = {}) => WrappedComponent => {
  class InnerComponent extends PureComponent {
    onCancelSuccess = () => {

    }
    render () {
      const {
        isOpen,
        groupIndex,
        parentIndex,
        childIndex,
        isAsti,
        isEdit,
        security
      } = this.props
      return (
        <div>
          <EditConnectionDialog
            isOpen={isOpen}
            groupIndex={groupIndex}
            parentIndex={parentIndex}
            childIndex={childIndex}
            isAsti={isAsti}
            reduxKey={reduxKey}
            isEdit={isEdit}
            security={security}
          />
          <WrappedComponent {...this.props} />
        </div>
      )
    }
  }
  InnerComponent.propTypes = {
    isOpen: PropTypes.bool,
    groupIndex: PropTypes.number,
    isEdit: PropTypes.bool,
    parentIndex: PropTypes.number,
    childIndex: PropTypes.number,
    isAsti: PropTypes.bool,
    onCancelSuccess: PropTypes.func
  }
  return compose(
    withState({
      redux: true,
      key: reduxKey,
      initialState: {
        isOpen: false,
        groupIndex: null,
        parentIndex: null,
        childIndex: null,
        isAsti:false,
        isEdit: false,
        security: null
      }
    })
  )(InnerComponent)
}

export default withEditConnectionDialog
