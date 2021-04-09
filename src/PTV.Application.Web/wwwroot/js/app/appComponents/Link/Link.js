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
import { injectIntl, intlShape } from 'util/react-intl'
import ClampText from 'appComponents/ClampText'
import { ExternalLinkIcon } from 'appComponents/Icons'
import styles from './styles.scss'
import cx from 'classnames'

const Link = ({
  href,
  className,
  text,
  target = '_blank',
  external = true,
  id,
  intl: { formatMessage }
}) => {
  const linkClass = cx(
    styles.link,
    className
  )
  const formatText = text => text && typeof text === 'object' && text.id && formatMessage(text) || text
  return (
    <a className={linkClass} href={href} target={target}>
      <ClampText text={formatText(text) || href} id={id} />
      {external && <ExternalLinkIcon />}
    </a>
  )
}
Link.propTypes = {
  text: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.object
  ]),
  href: PropTypes.string,
  id: PropTypes.string,
  target: PropTypes.string,
  external: PropTypes.bool,
  className: PropTypes.string,
  intl: intlShape
}

export default compose(
  injectIntl
)(Link)
