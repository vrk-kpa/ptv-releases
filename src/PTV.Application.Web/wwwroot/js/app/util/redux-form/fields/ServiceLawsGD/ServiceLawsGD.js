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
import { Map } from 'immutable'
import { TextField, Label } from 'sema-ui-components'
import { compose } from 'redux'
import { connect } from 'react-redux'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asLocalizable from 'util/redux-form/HOC/asLocalizable'
import asComparable from 'util/redux-form/HOC/asComparable'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'
import { injectIntl, intlShape } from 'util/react-intl'
import { serviceDescriptionMessages } from 'Routes/Service/components/Messages'
import Tooltip from 'appComponents/Tooltip'
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
  const renderLinks = (laws) => {
    return (
      <div>
        <div className='form-row'>
          <Label
            labelText={formatMessage(serviceDescriptionMessages.optionConnectedDescriptionTitle)}
            labelPosition='top'>
            <Tooltip tooltip={formatMessage(serviceDescriptionMessages.connectedGDFieldTooltip)} />
          </Label>
        </div>
        {laws.map(law => {
          const name = law.get('name') || Map()
          const nameInLanguage = name.get(language)
          const webPage = law.get('webPage') || Map()
          const webPageInLanguage = webPage.get(language) || Map()
          const urlInLanguage = webPageInLanguage.get('urlAddress')
          const shouldBeDisplayed = isCompareMode || isGDAvailable && (nameInLanguage || urlInLanguage)
          const key = `${law.get('id')}_${language}`
          return shouldBeDisplayed && (
            <div key={key}>
              <div className='form-row'>
                <div className='row'>
                  <div className={isCompareMode ? 'col-lg-24' : 'col-lg-12'}>
                    <RenderTextField label={formatMessage(serviceDescriptionMessages.lawNameTitle)}
                      input={{ value: nameInLanguage }}
                      disabled
                      placeholder={isGDAvailable
                        ? formatMessage(serviceDescriptionMessages.dataNotAvailable)
                        : formatMessage(serviceDescriptionMessages.languageVersionNotAvailable)}
                      meta={{ dirty: false }} />
                  </div>
                </div>
              </div>
              <div className='form-row'>
                <RenderUrlChecker label={formatMessage(serviceDescriptionMessages.lawUrlLabel)}
                  input={{ value: urlInLanguage }}
                  disabled
                  placeholder={isGDAvailable
                    ? formatMessage(serviceDescriptionMessages.dataNotAvailable)
                    : formatMessage(serviceDescriptionMessages.languageVersionNotAvailable)}
                  meta={{ dirty: false }} />
              </div>
            </div>
          )
        })}
      </div>
    )
  }
  const laws = gdLaws.map(law => {
    const name = isGDAvailable ? law.get('name') : Map()
    const nameInLanguage = name.get(language)
    const webPage = law.get('webPage') || Map()
    const webPageInLanguage = webPage.get(language) || Map()
    const urlInLanguage = webPageInLanguage.get('urlAddress')
    return (nameInLanguage || urlInLanguage) ? law : Map()
  })
  return (<div>
    {(isCompareMode && (value || compareValue) || isGDAvailable && value) &&
      <div className='form-row'>
        <Label
          labelText={formatMessage(serviceDescriptionMessages.backgroundDescriptionTitle)}
          labelPosition='top' />
        <div className={textEditorBodyClass}>
          <Editor
            editorState={isGDAvailable
              ? value || EditorState.createEmpty()
              : EditorState.createEmpty()}
            placeholder={isGDAvailable
              ? formatMessage(serviceDescriptionMessages.dataNotAvailable)
              : formatMessage(serviceDescriptionMessages.languageVersionNotAvailable)}
            readOnly
          />
        </div>
      </div>
    }
    {renderLinks(laws)}
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
