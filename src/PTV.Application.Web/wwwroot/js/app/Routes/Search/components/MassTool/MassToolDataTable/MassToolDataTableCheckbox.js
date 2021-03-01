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
import CheckboxField from 'util/redux-form/fields/Checkbox'
import { compose } from 'redux'
import { connect } from 'react-redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { getErrors, getErrorTitle, getApprovedId } from '../selectors'
import { ErrorIcon } from 'appComponents/Icons'
import Popup from 'appComponents/Popup'
import { injectIntl, intlShape } from 'util/react-intl'
import ItemList from 'appComponents/ItemList'
import styles from '../styles.scss'

const MassToolDataTableCheckbox = ({
  id,
  massToolType,
  canBeRestore,
  hideOnScroll,
  errors,
  errorTitle,
  intl: { formatMessage }
}) => {
  return (
    <div>{
      errors && errors.size
        ? <div>
          <Popup
            trigger={<span><ErrorIcon size={20} /></span>}
            content={<ItemList
              type='error'
              title={errorTitle && formatMessage(errorTitle)}
              items={errors}
            />}
            hideOnScroll={hideOnScroll}
            maxWidth='mW350'
          />
        </div>
        : <div >
          <CheckboxField
            name={`selected.${id}.approved`}
            className={styles.summaryCheckbox}
          />
        </div>
    }
    </div>
  )
}

MassToolDataTableCheckbox.propTypes = {
  hideOnScroll: PropTypes.bool,
  id: PropTypes.string.isRequired,
  canBeRestore: PropTypes.bool,
  massToolType: PropTypes.string.isRequired,
  errors: ImmutablePropTypes.list,
  errorTitle: ImmutablePropTypes.map,
  intl: intlShape
}

export default compose(
  injectIntl,
  injectFormName,
  connect((state, { formName, canBeRestore, id, unificRootId }) => {
    return {
      id: getApprovedId(state, { formName, id, unificRootId }),
      errors: getErrors(state, { canBeRestore, formName }),
      errorTitle: getErrorTitle(state, { formName })
    }
  })
)(MassToolDataTableCheckbox)
