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
import moment from 'moment'
import cx from 'classnames'
import styles from './styles.scss'

export const ShowDate = ({ value }) => value &&
  <div className={styles.date}>{moment(value).format('DD.MM.YYYY')}</div> || null

export const ShowTime = ({ value, timeParens }) => {
  const time = value && moment(value).format('HH:mm') || null
  return time && <div className={styles.time}>{timeParens ? `(${time})` : time}</div> || null
}

ShowDate.propTypes = {
  value: PropTypes.any
}

ShowTime.propTypes = {
  value: PropTypes.any,
  timeParens: PropTypes.bool
}

const ModifiedAtCell = ({
  modifiedAt,
  componentClass,
  inline,
  timeParens,
  hideTime
}) => {
  const modifiedAtClass = cx(
    componentClass,
    'cell',
    styles.modifiedAt,
    {
      [styles.inline]: inline
    }
  )
  return <div className={modifiedAtClass}>
    <ShowDate value={modifiedAt} />
    {!hideTime && <ShowTime value={modifiedAt} timeParens={timeParens} />}
  </div>
}
ModifiedAtCell.propTypes = {
  modifiedAt: PropTypes.any,
  componentClass: PropTypes.string,
  inline: PropTypes.bool,
  timeParens: PropTypes.bool,
  hideTime: PropTypes.bool
}

export default ModifiedAtCell
