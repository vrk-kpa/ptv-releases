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
import { connect } from 'react-redux'
import { injectIntl } from 'react-intl'
import { compose } from 'redux'

// components
import { Areas, AreaInformationType, AreaType } from '../../../Common/Areas'
import { PTVAddItem } from '../../../../Components'
import * as PTVValidatorTypes from '../../../../Components/PTVValidators'

// actions
import * as channelActions from '../../Common/Actions'
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps'

// selectors
import * as ChannelSelectors from '../../Common/Selectors'
import * as CommonSelectors from '../../../Common/Selectors'

// enums
import { areaTypes } from '../../../Common/Enums'

const ChannelAreaInformation = ({
    commonMessages,
    readOnly,
    language,
    translationMode,
    areaInformatioTypeId,
    areaTypeId,
    areaTypeVisible,
    actions,
    selectedAreaCount,
    channelId,
    intl,
    ...rest }) => {
  const onInputChange = (input, isSet = false) => value => {
    actions.onChannelInputChange(input, channelId, value, isSet, language)
  }
  const onListChange = (input) => (value, isAdd) => {
    actions.onChannelListChange(input, channelId, value, language, isAdd)
  }
  const onListRemove = (input) => (value) => {
    actions.onChannelListChange(input, channelId, value, language, false)
  }
  const validators = [PTVValidatorTypes.IS_REQUIRED]
  const configuration = {
    [areaTypes.MUNICIPALITY] : {
      listSelector:ChannelSelectors.getSelectedAreaMunicipality,
      getItemSelector:CommonSelectors.getMunicipality,
      onChange:onListChange('areaMunicipality'),
      onNodeRemove:onListRemove('areaMunicipality')
    },
    [areaTypes.PROVINCE] : {
      listSelector:ChannelSelectors.getSelectedAreaProvince,
      getItemSelector:CommonSelectors.getProvince,
      onChange:onListChange('areaProvince'),
      onNodeRemove:onListRemove('areaProvince')
    },
    [areaTypes.BUSINESS_REGION] : {
      listSelector:ChannelSelectors.getSelectedAreaBusinessRegions,
      getItemSelector:CommonSelectors.getBusinessRegion,
      onChange:onListChange('areaBusinessRegions'),
      onNodeRemove:onListRemove('areaBusinessRegions')
    },
    [areaTypes.HOSPITAL_REGION] : {
      listSelector:ChannelSelectors.getSelectedAreaHospitalRegions,
      getItemSelector:CommonSelectors.getHospitalRegion,
      onChange:onListChange('areaHospitalRegions'),
      onNodeRemove:onListRemove('areaHospitalRegions')
    }
  }
  return (
    <div>
      <div>
        <div>
          <div className='row'>
            <div className='col-xs-6'>
              <AreaInformationType
                language={language}
                messages={commonMessages}
                readOnly={readOnly || translationMode === 'view' || translationMode === 'edit'}
                changeCallback={onInputChange('areaInformationTypeId')}
                areaInformationType={areaInformatioTypeId}
              />
            </div>
          </div>
          {areaTypeVisible &&
          <div>
            {(!readOnly && translationMode === 'none')
            ? <div className='row'>
              <div className='col-xs-6'>
                <AreaType
                  language={language}
                  messages={commonMessages}
                  readOnly={readOnly || translationMode === 'view' || translationMode === 'edit'}
                  changeCallback={onInputChange('areaTypeId')}
                  areaType={areaTypeId}
                  validators={selectedAreaCount || validators} />
              </div>
            </div> : null}
            <div className='row'>
              <Areas
                {...rest}
                language={language}
                messages={commonMessages}
                readOnly={readOnly || translationMode === 'view' || translationMode === 'edit'}
                configuration={configuration}
                areaType={areaTypeId}
                validators={selectedAreaCount || validators}
                selectedCount={selectedAreaCount} />
            </div>
          </div>
        }
        </div>
      </div>
    </div>
  )
}

ChannelAreaInformation.propTypes = {
  commonMessages: PropTypes.any.isRequired,
  readOnly: PropTypes.bool.isRequired,
  language: PropTypes.string.isRequired,
  translationMode: PropTypes.string.isRequired,
  actions: PropTypes.object.isRequired,
  intl: PropTypes.object.isRequired,
  areaInformatioTypeId: PropTypes.string.isRequired,
  areaTypeId: PropTypes.string,
  areaTypeVisible: PropTypes.bool.isRequired,
  selectedAreaCount: PropTypes.number.isRequired,
  channelId: PropTypes.string.isRequired
}

function mapStateToProps (state, ownProps) {
  const defaultAreaInformatioTypeId = ChannelSelectors.getAreaInformationTypeId(state, ownProps)

  const defaultAreaTypeId = ChannelSelectors.getAreaTypeId(state, ownProps)
  return {
    areaInformatioTypeId: defaultAreaInformatioTypeId,
    areaTypeVisible: defaultAreaInformatioTypeId === CommonSelectors.getAreaInformationTypeId(state, 'areatype'),
    areaTypeId: defaultAreaTypeId,
    selectedAreaCount: ChannelSelectors.getSelectedAreaCount(state, ownProps),
    channelId: ChannelSelectors.getChannelId(state, ownProps)
  }
}

const actions = [
  channelActions
]
const composeAddItem = InnerComponent => {
  const AddItem = props => {
    const render = () => (<InnerComponent {...props} />)

    return (
      <PTVAddItem {...props}
        readOnly={props.readOnly && props.translationMode === 'none'}
        renderItemContent={render}
        messages={{
          'label': props.intl.formatMessage(props.messages.areaInformationTitle),
          'tooltip': props.intl.formatMessage(props.messages.areaInformationTooltip) }}
            />
    )
  }
  return AddItem
}

export default compose(
  connect(mapStateToProps, mapDispatchToProps(actions)),
  injectIntl,
  composeAddItem
)(ChannelAreaInformation)

