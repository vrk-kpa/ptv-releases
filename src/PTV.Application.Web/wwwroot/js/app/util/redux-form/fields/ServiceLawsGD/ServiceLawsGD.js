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
import ImmutablePropTypes from 'react-immutable-proptypes'
import { Map } from 'immutable'
import { TextField, Label } from 'sema-ui-components'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { asDisableable, asLocalizable, asComparable } from 'util/redux-form/HOC'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'
import { injectIntl, intlShape } from 'react-intl'
import { serviceDescriptionMessages } from 'Routes/Service/components/Messages'
import {
  getIsGDAvailableInContentLanguage,
  getIsGDAvailableInCompareLanguage,
  getGeneralDescriptionName,
  getGeneralDescriptionCompareName,
  getGDBackgroundDescriptionCompareValue,
  getGDBackgroundDescriptionValue,
  getGeneralDescriptionLaws
} from 'Routes/Service/components/ServiceComponents/selectors'
import {
  getContentLanguageCode,
  getSelectedComparisionLanguageCode
} from 'selectors/selections'
import styles from 'Routes/Service/components/ServiceComponents/styles.scss'
import cx from 'classnames'
import { EditorState, Editor } from 'draft-js'
import { RenderTextField, RenderUrlChecker } from 'util/redux-form/renders'

const ServiceLawsGD = ({
  name,
  value,
  compareValue,
  isGDAvailable,
  gdLaws,
  intl: { formatMessage },
  language,
  isCompareMode,
  ...rest
}) => {
  const textEditorBodyClass = cx(
        styles.textEditorBody,
        styles.disabled
      )
  const renderLinks = () => (
    <div>
      <div className='form-row'>
        <Label labelText={isGDAvailable
              ? formatMessage(serviceDescriptionMessages.optionConnectedDescriptionTitle) + ': ' + name
              : formatMessage(serviceDescriptionMessages.optionConnectedDescriptionTitle) + ': ' + formatMessage(serviceDescriptionMessages.languageVersionNotAvailable)}
          labelPosition='top' />
      </div>
      {gdLaws.map((law, index) => {
        const name = isGDAvailable ? law.get('name') : Map()
        const nameInLanguage = name.get(language)
        const url = isGDAvailable ? law.get('urlAddress') : Map()
        const urlInLanguage = url.get(language)
        const shouldBeDisplayed = isCompareMode || isGDAvailable && (nameInLanguage || urlInLanguage)
        return shouldBeDisplayed && (
          <div key={index}>
            <div className='form-row'>
              <RenderTextField label={formatMessage(serviceDescriptionMessages.lawNameTitle)}
                input={{ value: nameInLanguage || formatMessage(serviceDescriptionMessages.dataNotAvailable) }}
                disabled
                meta={{ dirty: false }} />
            </div>
            <div className='form-row'>
              <RenderUrlChecker label={formatMessage(serviceDescriptionMessages.lawUrlLabel)}
                input={{ value: urlInLanguage || formatMessage(serviceDescriptionMessages.dataNotAvailable) }}
                disabled
                meta={{ dirty: false }} />
            </div>
          </div>
        )
      })}
    </div>
      )
  return (<div>
    {(isCompareMode && (value || compareValue) || isGDAvailable && value) &&
      <div className='form-row'>
        <Label labelText={
            formatMessage(serviceDescriptionMessages.backgroundDescriptionTitle)}
          labelPosition='top' />
        <div className={textEditorBodyClass}>
          <Editor
            editorState={isGDAvailable
                    ? value || EditorState.createEmpty()
                    : EditorState.createEmpty()}
            placeholder={formatMessage(serviceDescriptionMessages.dataNotAvailable)}
            readOnly
                />
        </div>
      </div>
    }
    {renderLinks()}
  </div>
  )
}
ServiceLawsGD.propTypes = {
  name: PropTypes.string,
  value: PropTypes.object,
  compareValue: PropTypes.object,
  isGDAvailable: PropTypes.bool,
  intl: intlShape,
  language: PropTypes.string,
  gdLaws: ImmutablePropTypes.list,
  isCompareMode: PropTypes.bool
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: TextField }),
  withFormStates,
  withLanguageKey,
  connect((state, ownProps) => {
    const name = ownProps.compare
      ? getGeneralDescriptionCompareName(state, ownProps)
      : getGeneralDescriptionName(state, ownProps)
    const value = ownProps.compare
      ? getGDBackgroundDescriptionCompareValue(state, ownProps)
      : getGDBackgroundDescriptionValue(state, ownProps)
    const compareValue = !ownProps.compare
      ? getGDBackgroundDescriptionCompareValue(state, ownProps)
      : getGDBackgroundDescriptionValue(state, ownProps)
    const isGDAvailable = ownProps.compare
      ? getIsGDAvailableInCompareLanguage(state, ownProps)
      : getIsGDAvailableInContentLanguage(state, ownProps)
    const language = ownProps.compare
      ? getSelectedComparisionLanguageCode(state, ownProps)
      : getContentLanguageCode(state, ownProps)
    return {
      name,
      value,
      compareValue,
      isGDAvailable,
      gdLaws: getGeneralDescriptionLaws(state, ownProps),
      language
    }
  }),
  asDisableable,
  asLocalizable
)(ServiceLawsGD)
