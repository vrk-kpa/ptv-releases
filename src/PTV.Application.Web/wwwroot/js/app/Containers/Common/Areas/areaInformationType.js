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
import React, { PropTypes } from 'react'
import { connect } from 'react-redux'
import { injectIntl } from 'react-intl'

// components
// import { PTVRadioGroup } from '../../../../Components';
import { LocalizedRadioGroup, LocalizedComboBox } from 'Containers/Common/localizedData'

// actions
import mapDispatchToProps from 'Configuration/MapDispatchToProps'

// selectors
import * as CommonSelectors from 'Containers/Common/Selectors'

const AreaInformationType = ({
    messages,
    readOnly,
    language,
    radio,
    areaInformationType,
    areaInformationTypes,
    changeCallback,
    actions,
    intl }) => {
  return (
    radio
      ? <LocalizedRadioGroup
        language={language}
        radioGroupLegend={messages.areaInformationTypeTitle &&
          intl.formatMessage(messages.areaInformationTypeTitle)}
        name='AreaInformationType'
        value={areaInformationType}
        onChange={changeCallback}
        items={areaInformationTypes}
        verticalLayout
        readOnly={readOnly}
      />
      : <LocalizedComboBox
        language={language}
        value={areaInformationType}
        values={areaInformationTypes}
        label={messages.areaInformationTypeTitle &&
          intl.formatMessage(messages.areaInformationTypeTitle)}
        changeCallback={changeCallback}
        order={20}
        name='organizationName'
        autosize={false}
        readOnly={readOnly}
        className='limited w480'
        inputProps={{ 'maxLength': '100' }}
      />
  )
}

AreaInformationType.propTypes = {
  changeCallback: PropTypes.func.isRequired,
  messages: PropTypes.any.isRequired,
  readOnly: PropTypes.bool.isRequired,
  language: PropTypes.string.isRequired,
  areaInformationType: PropTypes.string.isRequired,
  areaInformationTypes: PropTypes.array.isRequired,
  actions: PropTypes.object.isRequired,
  intl: PropTypes.object.isRequired,
  radio: PropTypes.bool
}

function mapStateToProps (state, ownProps) {
  const filteredTypes = ownProps.filteredCodes
    ? CommonSelectors.getAreaInformationTypesObjectArray(state, ownProps)
      .filter(type => !ownProps.filteredCodes.includes(type.code.toLowerCase()))
    : CommonSelectors.getAreaInformationTypesObjectArray(state, ownProps)
  return {
    areaInformationTypes: filteredTypes
  }
}

const actions = []

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(AreaInformationType))

