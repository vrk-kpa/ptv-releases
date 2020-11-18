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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { reduxForm, formValueSelector } from 'redux-form/immutable'
import { compose } from 'redux'
import { connect } from 'react-redux'
import {
  massToolTypes,
  formTypesEnum,
  massToolSectionTypesEnum
} from 'enums'
import cx from 'classnames'
import styles from './styles.scss'
import withMassToolActive from './withMassToolActive'
import { injectIntl } from 'util/react-intl'
import { getPublishingStatusDeletedId } from 'selectors/common'
import { Map } from 'immutable'

class MassToolSelectionForm extends Component {
  render () {
    const { className } = this.props
    const massToolFormClass = cx(
      styles.massToolForm,
      className
    )
    return (
      <div className={massToolFormClass}>
        <this.props.content />
      </div>
    )
  }
}

MassToolSelectionForm.propTypes = {
  className: PropTypes.string
}

export default compose(
  injectIntl,
  withMassToolActive,
  connect((state, { initialMassType,  massSectionType}) => {
    const publishingStatuses = formValueSelector(formTypesEnum.FRONTPAGESEARCH)(state, 'selectedPublishingStatuses') || Map()
    const selectedPublishingStatuses = publishingStatuses.filter(isSelected => isSelected)
    const publishingStatusDelete = getPublishingStatusDeletedId(state)
    const initialType = initialMassType || ((selectedPublishingStatuses.size === 1 && selectedPublishingStatuses.keySeq().first() === publishingStatusDelete)
      ? massToolTypes.RESTORE
      : massToolTypes.PUBLISH) 
    const initialSectionType =  massSectionType || massToolSectionTypesEnum.SEARCH
    
    const initialValues = {
      type: initialType,
      section: initialSectionType
    }
    return {
      initialValues
    }
  }),
  reduxForm({
    form: formTypesEnum.MASSTOOLSELECTIONFORM,
    destroyOnUnmount : false
  })
)(MassToolSelectionForm)
