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
import { compose } from 'redux'
import { injectIntl, intlShape } from 'util/react-intl'
import { TreeListDisplay } from 'util/redux-form/fields/TreeListDisplay'
import CommonMessages from 'util/redux-form/messages'
import * as FintoSelectors from 'util/redux-form/fields/DigitalAuthorizationTree/selectors'
import { connect } from 'react-redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import styles from './styles.scss'
import { Label } from 'sema-ui-components'
import { getFormValue } from 'selectors/base'

const ConnectionAuthorization = ({
  intl: { formatMessage },
  label,
  name,
  isVisible,
  ...rest
}) => {
  return (
    isVisible
      ? <div className={styles.previewBlock}>
        <Label labelText={label} />
        <div className={styles.previewItem}>
          <TreeListDisplay
            name={name}
            selector={FintoSelectors.getDigitalAuthorizationsForIdsJs}
            invalidItemTooltip={formatMessage(CommonMessages.invalidItemTooltip)}
            isReadOnly
            {...rest}
          />
        </div>
      </div>
      : null
  )
}
ConnectionAuthorization.propTypes = {
  intl: intlShape,
  label: PropTypes.string,
  name: PropTypes.string,
  isVisible: PropTypes.bool
}

export default compose(
  injectIntl,
  injectFormName,
  connect((state, ownProps) => {
    const name = `connections[${ownProps.parentIndex}].childs[${ownProps.index}].digitalAuthorization.digitalAuthorizations`
    const authorization = getFormValue(name)(state, ownProps)
    const isVisible = authorization && authorization.size
    return {
      isVisible,
      name
    }
  })
)(ConnectionAuthorization)
