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
import ImmutablePropTypes from 'react-immutable-proptypes'
import { List } from 'immutable'
import Popup from 'appComponents/Popup'
import ShowMoreButton from 'appComponents/ShowMoreButton'

const withShowMore = ({
  countOfVisible,
  MoreContent,
  triggerCSS
}) => WrappedComponent => {
  const InnerComponent = props => {
    const {
      items,
      className,
      popupProps = {}
    } = props

    let isMoreContentAvailable = false
    let visibleItems = items
    let hiddenItems = null
    if (List.isList(items)) {
      isMoreContentAvailable = countOfVisible < items.size
    } else if (Array.isArray(items)) {
      isMoreContentAvailable = countOfVisible < items.length
    } else return null
    if (isMoreContentAvailable) {
      hiddenItems = items.slice(countOfVisible)
      visibleItems = items.slice(0, countOfVisible)
    }

    return (
      <div className={className}>
        <WrappedComponent {...props} countOfVisible={countOfVisible} items={visibleItems} />
        {hiddenItems &&
          <Popup {...popupProps} trigger={<div className={triggerCSS}><ShowMoreButton /></div>}>
            <MoreContent {...props} items={hiddenItems} visibleItems={visibleItems} />
          </Popup>
        }
      </div>
    )
  }
  InnerComponent.propTypes = {
    items: PropTypes.oneOfType([
      PropTypes.array,
      ImmutablePropTypes.list
    ]),
    className: PropTypes.string,
    popupProps: PropTypes.object
  }
  return InnerComponent
}

export default withShowMore
