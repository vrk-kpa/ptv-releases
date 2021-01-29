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
import ReactPaginate from 'react-paginate'
import Arrow from 'appComponents/Arrow'
import cx from 'classnames'
import styles from './styles.scss'

const Pagination = ({
  containerClassName,
  activeClassName,
  disabledClassName,
  marginPagesDisplayed = 2,
  pageRangeDisplayed = 5,
  breakLabel = '...',
  nextLabel = <Arrow direction='east' secondary height={12} width={12} />,
  previousLabel = <Arrow direction='west' secondary height={12} width={12} />,
  ...rest
}) => {
  const containerCSS = cx(styles.pagination, containerClassName)
  const activeCSS = cx(styles.active, activeClassName)
  const disabledCSS = cx(styles.disabled, disabledClassName)

  return (
    <ReactPaginate
      previousLabel={previousLabel}
      nextLabel={nextLabel}
      breakLabel={breakLabel}
      marginPagesDisplayed={marginPagesDisplayed}
      pageRangeDisplayed={pageRangeDisplayed}
      containerClassName={containerCSS}
      activeClassName={activeCSS}
      disabledClassName={disabledCSS}
      {...rest}
    />
  )
}

Pagination.propTypes = {
  containerClassName: PropTypes.string,
  activeClassName: PropTypes.string,
  disabledClassName: PropTypes.string,
  marginPagesDisplayed: PropTypes.number,
  pageRangeDisplayed: PropTypes.number,
  previousLabel: PropTypes.string,
  nextLabel: PropTypes.string,
  breakLabel: PropTypes.string
}

export default Pagination
