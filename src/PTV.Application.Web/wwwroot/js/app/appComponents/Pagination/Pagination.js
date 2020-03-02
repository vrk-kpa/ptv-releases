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
import PaginationItem from './PaginationItem'
import Arrow from 'appComponents/Arrow'
import cx from 'classnames'
import styles from './styles.scss'

const itemTypesEnum = {
  PREVELLIPSIS: 'prevEllipsis',
  NEXTELLIPSIS: 'nextEllipsis',
  ITEM: 'item'
}

const generatePaginationItems = ({ count, active, offset = 0 }) => {
  return Array.from({ length: count }, (v, k) => ({
    isActive: active === k + offset,
    pageIndex: k + offset
  }))
}

const createPaginationItems = ({ count, active, boundaryPages, visibleNeighbours }) => {
  const pageRadius = boundaryPages + visibleNeighbours + 1
  const startIndex = active <= pageRadius
    ? boundaryPages
    : active >= (count - pageRadius)
      ? count - 1 - pageRadius - visibleNeighbours
      : active - visibleNeighbours
  const endIndex = active < pageRadius
    ? pageRadius + visibleNeighbours
    : active >= (count - 1 - pageRadius)
      ? count - 1 - boundaryPages
      : active + visibleNeighbours

  const startItems = generatePaginationItems({ count: boundaryPages, active })
  const prevEllipsisItem = active > pageRadius ? [{ type: itemTypesEnum.PREVELLIPSIS }] : []
  const middleItems = generatePaginationItems({ count: endIndex - startIndex + 1, active, offset: startIndex })
  const nextEllipsisItem = active < (count - 1 - pageRadius) ? [{ type: itemTypesEnum.NEXTELLIPSIS }] : []
  const endItems = generatePaginationItems({ count: boundaryPages, active, offset: count - boundaryPages })

  return [
    ...startItems,
    ...prevEllipsisItem,
    ...middleItems,
    ...nextEllipsisItem,
    ...endItems
  ]
}

const PaginationHooks = ({
  onChange,
  active,
  count,
  visibleNeighbours,
  boundaryPages,
  showPreviousAndNext,
  disabled,
  className
}) => {
  const paginationClass = cx(
    styles.pagination,
    {
      [styles.disabled]: disabled
    },
    className
  )

  const handleOnPreviousClick = () => onChange(active - 1)
  const handleOnNextClick = () => onChange(active + 1)

  const pageJump = boundaryPages + visibleNeighbours
  const handleOnPreviousEllipsisClick = () => onChange(active - pageJump)
  const handleOnNextEllipsisClick = () => onChange(active + pageJump)

  const visibleItems = 2 * boundaryPages + 2 * visibleNeighbours + 3
  const items = count < visibleItems
    ? generatePaginationItems({ count, active })
    : createPaginationItems({ count, active, boundaryPages, visibleNeighbours })

  const previousDisabled = disabled || (active === 0)
  const nextDisabled = disabled || (active === count - 1)
  return (
    <ul className={paginationClass}>
      {showPreviousAndNext && (
        <PaginationItem
          disabled={previousDisabled}
          onClick={handleOnPreviousClick}
          children={<Arrow direction='west' secondary height={12} width={12} />}
        />
      )}
      {items.map(item => {
        if (item.type === itemTypesEnum.PREVELLIPSIS) {
          return <PaginationItem
            disabled={disabled}
            onClick={handleOnPreviousEllipsisClick}
            key={`prevEllipsis`}
            children={<span>...</span>}
          />
        }
        if (item.type === itemTypesEnum.NEXTELLIPSIS) {
          return <PaginationItem
            disabled={disabled}
            onClick={handleOnNextEllipsisClick}
            key={`nextEllipsis`}
            children={<span>...</span>}
          />
        }
        return <PaginationItem
          disabled={disabled}
          onClick={() => onChange(item.pageIndex)}
          key={`page${item.pageIndex}`}
          children={item.pageIndex + 1}
          {...item}
        />
      })}
      {showPreviousAndNext && (
        <PaginationItem
          disabled={nextDisabled}
          onClick={handleOnNextClick}
          children={<Arrow direction='east' secondary height={12} width={12} />}
        />
      )}
    </ul>
  )
}

PaginationHooks.propTypes = {
  onChange: PropTypes.func.isRequired,
  active: PropTypes.number.isRequired,
  count: PropTypes.number.isRequired,
  visibleNeighbours: PropTypes.number,
  boundaryPages: PropTypes.number,
  disabled: PropTypes.bool,
  showPreviousAndNext: PropTypes.bool,
  className: PropTypes.string
}

PaginationHooks.defaultProps = {
  showPreviousAndNext: true,
  visibleNeighbours: 2,
  boundaryPages: 2
}

export default PaginationHooks
