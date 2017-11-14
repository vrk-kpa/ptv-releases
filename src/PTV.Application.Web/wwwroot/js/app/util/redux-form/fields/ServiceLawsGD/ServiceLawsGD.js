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
import { Map } from 'immutable'
import { TextField, Label } from 'sema-ui-components'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { asDisableable, asLocalizable, asComparable } from 'util/redux-form/HOC'
import { injectIntl, intlShape } from 'react-intl'
import {
  getGDBackgroundDescriptionCompareValue,
  getGDBackgroundDescriptionValue,
  getGeneralDescriptionLaws
} from './selectors'
import { serviceDescriptionMessages } from 'Routes/Service/components/Messages'
import {
  getIsGDAvailableInContentLanguage,
  getIsGDAvailableInCompareLanguage,
  getGeneralDescriptionName,
  getGeneralDescriptionCompareName
} from 'Routes/Service/components/ServiceComponents/selectors'
import {
  getContentLanguageCode,
  getSelectedComparisionLanguageCode
} from 'selectors/selections'
import styles from 'Routes/Service/components/ServiceComponents/styles.scss'
import cx from 'classnames'
import { EditorState, Editor } from 'draft-js'
import { Laws } from 'util/redux-form/sections'
import { RenderTextField, RenderUrlChecker } from 'util/redux-form/renders'

const ServiceLawsGD = ({
  name,
  value,
  isGDAvailable,
  GDlaws,
  intl: { formatMessage },
  contentLanguage,
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
      {GDlaws.map(law => {
        const name = isGDAvailable ? law.get('name') : Map()
        const url = isGDAvailable ? law.get('urlAddress') : Map()
        return (
          <div>
            <div className='form-row'>
              <RenderTextField label={formatMessage(serviceDescriptionMessages.lawNameTitle)}
                input={{ value: name.get(contentLanguage) || formatMessage(serviceDescriptionMessages.dataNotAvailable) }}
                disabled
                meta={{ dirty: false }} />
            </div>
            <div className='form-row'>
              <RenderUrlChecker label={formatMessage(serviceDescriptionMessages.lawUrlLabel)}
                input={{ value: url.get(contentLanguage) || formatMessage(serviceDescriptionMessages.dataNotAvailable) }}
                disabled
                meta={{ dirty: false }} />
            </div>
          </div>
        )
      })}
    </div>
      )
  return (<div>
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
    {renderLinks()}
  </div>
  )
}
ServiceLawsGD.propTypes = {
  name: PropTypes.string,
  value: PropTypes.string,
  isGDAvailable: PropTypes.bool,
  intl: intlShape
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: TextField }),
  connect((state, ownProps) => {
    const name = ownProps.compare
      ? getGeneralDescriptionCompareName(state, ownProps)
      : getGeneralDescriptionName(state, ownProps)
    const value = ownProps.compare
      ? getGDBackgroundDescriptionCompareValue(state, ownProps)
      : getGDBackgroundDescriptionValue(state, ownProps)
    const isGDAvailable = ownProps.compare
      ? getIsGDAvailableInCompareLanguage(state, ownProps)
      : getIsGDAvailableInContentLanguage(state, ownProps)
    const contentLanguage = ownProps.compare
      ? getSelectedComparisionLanguageCode(state, ownProps)
      : getContentLanguageCode(state, ownProps)
    return {
      name,
      value,
      isGDAvailable,
      GDlaws: getGeneralDescriptionLaws(state, ownProps),
      contentLanguage
    }
  }),
  asDisableable,
  asLocalizable
)(ServiceLawsGD)
