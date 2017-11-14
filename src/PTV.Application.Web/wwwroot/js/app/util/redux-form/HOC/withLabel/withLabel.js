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
import { injectIntl, intlShape } from 'react-intl'
import { Label } from 'sema-ui-components'
import { PTVIcon } from 'Components'
import { Popup } from 'appComponents'

const withLabel = (label, tooltip, required) => InnerComponent => {
  const WrappedComponent = props => (
    <div>
      <Label
        labelText={props.intl.formatMessage(label)}
        labelPosition='top'
        required={required && !props.isReadOnly}
      >
        {!props.isReadOnly && tooltip &&
          <Popup
            trigger={<PTVIcon name='icon-tip2' width={30} height={30} />}
            content={props.intl.formatMessage(tooltip)}
          />
        }
      </Label>
      <InnerComponent {...props} />
    </div>
  )

  WrappedComponent.propTypes = {
    intl: intlShape.isRequired,
    isReadOnly: PropTypes.bool
  }
  return compose(
    injectIntl
  )(WrappedComponent)
}

export default withLabel
