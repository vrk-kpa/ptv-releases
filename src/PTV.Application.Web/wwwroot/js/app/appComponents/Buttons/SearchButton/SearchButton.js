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
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { Button, Spinner } from 'sema-ui-components'

const messages = defineMessages({
  defaultText: {
    id: 'Components.Buttons.SearchButton',
    defaultMessage: 'Hae'
  }
})

const SearchButton = ({
  className,
  spinnerClass,
  type,
  children,
  disabled,
  onClick,
  isSearching,
  intl: { formatMessage }
}) => {
  return (
    <Button
      type={type}
      small
      onClick={onClick}
      disabled={disabled}
      className={className}
    >
      {isSearching && <Spinner inverse className={spinnerClass} /> || children || formatMessage(messages.defaultText)}
    </Button>
  )
}

SearchButton.propTypes = {
  className: PropTypes.string,
  spinnerClass: PropTypes.string,
  type: PropTypes.string,
  children: PropTypes.node,
  disabled: PropTypes.bool,
  onClick: PropTypes.func,
  isSearching: PropTypes.bool,
  intl: intlShape
}

export default compose(
  injectIntl
)(SearchButton)
