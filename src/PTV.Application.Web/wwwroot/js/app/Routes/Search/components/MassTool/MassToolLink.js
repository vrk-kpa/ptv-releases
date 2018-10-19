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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import ToggleButton from 'appComponents/Buttons/ToggleButton'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import withState from 'util/withState'
import { formTypesEnum } from 'enums'

const messages = defineMessages({
  massTool: {
    id: 'FrontPageSearchFrom.MassTool.Link',
    defaultMessage: 'Massatyökalut'
  }
})

class MassToolButton extends Component {
  toggleMassTools = () => {
    const { isVisible, updateUI } = this.props
    updateUI('isVisible', !isVisible)
  }

  render () {
    const { isVisible, intl: { formatMessage } } = this.props
    return (<ToggleButton
      onClick={this.toggleMassTools}
      isCollapsed={!isVisible}
      showIcon
    >{formatMessage(messages.massTool)}</ToggleButton>
    )
  }
}

MassToolButton.propTypes = {
  isVisible: PropTypes.bool.isRequired,
  updateUI: PropTypes.func.isRequired,
  intl: intlShape.isRequired
}

export default compose(
  injectIntl,
  withState({
    redux: true,
    key: formTypesEnum.MASSTOOLFORM,
    initialState: {
      isVisible: false
    }
  })
)(MassToolButton)
