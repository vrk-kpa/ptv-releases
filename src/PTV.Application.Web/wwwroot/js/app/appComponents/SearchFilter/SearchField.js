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
import React, { Component, PropTypes} from 'react'
import classnames from 'classnames'
import PTVIcon from 'Components/PTVIcon'

class SearchInput extends Component {
  constructor(props) {
    super(props)
  }

  shouldComponentUpdate() {
    return false;
  }

  render() {
    return (
      <input
        type='text'
        key='treeViewSearchInput'
        className='ptv-textinput full with-icon'
        placeholder={this.props.placeholder}
        onChange={this.props.onChange}
        disabled={this.props.disabled}
      />
    )
  }
}

export default ({ isSearching, searchPlaceholder, onChange }) => (
  <div key='treeViewSearchDiv' className='list-search-field'>
    {isSearching
      ? <div className='loading' />
      : <PTVIcon componentClass='magnifying-glass' name='icon-search' />
    }
    <SearchInput
      placeholder={searchPlaceholder}
      onChange={onChange}
      disabled={isSearching} />
  </div>
)
