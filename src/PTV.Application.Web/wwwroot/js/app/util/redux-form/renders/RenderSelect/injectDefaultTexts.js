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
import { injectIntl, defineMessages } from 'react-intl'
import { compose } from 'redux'

const messages = defineMessages({
  searching: {
    id: 'Components.AutoCombobox.Searching',
    defaultMessage: 'Haetaan...'
  },
  noResults: {
    id: 'Components.AutoCombobox.NoResults',
    defaultMessage: 'Ei hakutuloksia'
  },
  typeToSearch: {
    id: 'Components.AutoCombobox.TypeToSearch',
    defaultMessage: 'Kirjoita hakusana'
  },
  clearValue: {
    id: 'Components.AutoCombobox.ClearValue',
    defaultMessage: 'Selkeää arvo'
  },
  clearAll: {
    description: 'Components.AutoCombobox.ClearValue',
    id: 'Components.AutoCombobox.ClearAll',
    defaultMessage: 'Selkeää arvo'
  }
})

const injectDefaultTexts = ComponentToInject => {
  const TextInjector = props => {
    return <ComponentToInject
      {...props}
      searchingText={props.intl.formatMessage(messages.searching)}
      noResultsText={props.intl.formatMessage(messages.noResults)}
      clearValueText={props.intl.formatMessage(messages.clearValue)}
      clearAllText={props.intl.formatMessage(messages.clearAll)}
      searchPromptText={props.intl.formatMessage(messages.typeToSearch)}
      loadingPlaceholder={props.intl.formatMessage(messages.searching)}
     />
  }

  TextInjector.propTypes = {
    intl: PropTypes.object.isRequired
  }

  return compose(injectIntl)(TextInjector)
}

export default injectDefaultTexts
