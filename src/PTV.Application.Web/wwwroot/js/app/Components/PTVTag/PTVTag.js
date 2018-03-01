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
import './styles.scss'
import PTVIcon from '../PTVIcon'
import PTVTooltip from '../PTVTooltip'
import { renderOptions } from '../PTVComponent'
import { injectIntl, intlShape } from 'react-intl'
import cx from 'classnames'

const PTVTag = (props) => {
  const {
    onTagRemove,
    id,
    name,
    isDisabled,
    readOnly,
    className,
    tagColor,
    tagType,
    tagPostfix,
    tagTooltipContent,
    tagTooltipCallback
  } = props

  const onClickTagRemove = () => {
    if (!isDisabled) {
      onTagRemove(id)
    }
  }

  const renderOption = () => {
    const option = { id, name }

    const data = { ...props, option }
    return renderOptions(data)
  }

  const wrapClass = cx('tag-wrap', className, tagColor, tagType, { 'with-suffix': tagPostfix })
  const itemClass = cx('tag-item', { 'disabled': isDisabled })
  return (
    <li className={wrapClass} key={id} >
      <span className={itemClass}>
        <span className='wrapped'>
          <span className='tag-item-body'>
            { renderOption() }
            { tagTooltipContent ? <PTVTooltip  className='tree' tooltip = { tagTooltipContent() } onOpenCallback = { () => tagTooltipCallback(id) }/>:null}
          </span>
          { !readOnly &&
            <PTVIcon
              name='icon-cross'
              onClick={onClickTagRemove}
            />
          }
        </span>
      </span>
      { tagPostfix && <span className='suffix'> {tagPostfix} </span> }
    </li>
  )
}

PTVTag.propTypes = {
  intl: intlShape.isRequired,
  onTagRemove: PropTypes.func,
  id: PropTypes.string,
  name: PropTypes.string,
  isDisabled: PropTypes.bool,
  readOnly: PropTypes.bool,
  className: PropTypes.string,
  tagColor: PropTypes.string,
  tagType: PropTypes.string,
  tagPostfix: PropTypes.string,
  tagTooltipContent: PropTypes.func,
}

PTVTag.defaultProps = {
  className: ''
}

export default injectIntl(PTVTag)
