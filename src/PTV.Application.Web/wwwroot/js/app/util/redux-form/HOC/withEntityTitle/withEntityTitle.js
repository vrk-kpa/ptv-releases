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
import { connect } from 'react-redux'
import {
  getSelectedEntityId
} from 'selectors/entities/entities'
import {
  getFormNameFieldValue,
  getFormAlterNameFieldValue,
  getFormUseAlterNameFieldValue
} from './selectors'
import { getIsAddingNewLanguage } from 'selectors/formStates'
import GoBackLink from 'appComponents/GoBackLink'
import styles from './styles.scss'
import injectFormName from 'util/redux-form/HOC/injectFormName'

const withEntityTitle = ({
  newEntityTitle,
  newLanguageVersionTitle,
  description
}) => WrappedComponent => {
  const InnerComponent = props => {
    const {
      entityId,
      nameFieldValue,
      isAddingNewLanguage,
      ...rest
    } = props
    const entityTitle = !entityId
      ? nameFieldValue || newEntityTitle
      : isAddingNewLanguage
        ? newLanguageVersionTitle
        : nameFieldValue || newLanguageVersionTitle // should be label for exising servis but no name
    return (
      <div>
        <GoBackLink />
        <h1 className={styles.entityTitle}>{entityTitle}</h1>
        {description && <div>{description}</div>}
        <WrappedComponent {...rest} />
      </div>
    )
  }

  InnerComponent.propTypes = {
    entityId: PropTypes.string,
    nameFieldValue: PropTypes.string,
    isAddingNewLanguage: PropTypes.bool
  }

  return compose(
    injectFormName,
    connect((state, { formName }) => ({
      entityId: getSelectedEntityId(state),
      // entityName: getEntityUseAlternateName(state)
      // ? getEntityAlternateName(state)
      // : getEntityName(state),
      nameFieldValue: getFormUseAlterNameFieldValue(state, { formName })
        ? getFormAlterNameFieldValue(state, { formName })
        : getFormNameFieldValue(state, { formName }),
      isAddingNewLanguage: getIsAddingNewLanguage(formName)(state)
      // isContentLanguageInEntityLanguages: getIsContentLanguageInEntityLanguages(state)
    }))
  )(InnerComponent)
}

export default withEntityTitle
