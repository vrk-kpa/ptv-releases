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
import { compose } from 'redux'

// components
import { Areas, AreaType, AreaInformationType } from 'Containers/Common/Areas'
import { PTVAddItem } from '../../../../../Components'
import * as PTVValidatorTypes from '../../../../../Components/PTVValidators'

// actions
import * as organizationActions from '../../Common/Actions'
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps'

// selectors
import * as CommonSelectors from '../../../../Common/Selectors'
import * as CommonOrganizationSelectors from '../Selectors'

// enums
import { areaTypes } from '../../../../Common/Enums'

const OrganizationAreaInformation = ({
    messages,
    readOnly,
    language,
    translationMode,
    areaInformatioTypeId,
    areaTypeId,
    areaTypeVisible,
    actions,
    entityId,
    filteredCodes,
    selectedAreaCount,
    intl }) => {
  const onInputChange = (input, isSet = false) => value => {
    actions.onLocalizedOrganizationInputChange(input, entityId, value, isSet, language)
  }
  const onListChange = (input) => (value, isAdd) => {
    actions.onLocalizedOrganizationListChange(input, entityId, value, language, isAdd)
  }
  const onListRemove = (input) => (value) => {
    actions.onLocalizedOrganizationListChange(input, entityId, value, language, false)
  }

  const onAreaInformationTypeInputChange = (input, isSet = false) => value => {
    actions.onLocalizedOrganizationInputChange(input, entityId, value, isSet, language)
  }

  const validators = [PTVValidatorTypes.IS_REQUIRED]

  const configuration = {
    [areaTypes.MUNICIPALITY] : {
      listSelector:CommonOrganizationSelectors.getSelectedAreaMunicipality,
      getItemSelector:CommonSelectors.getMunicipality,
      onChange:onListChange('areaMunicipality'),
      onNodeRemove:onListRemove('areaMunicipality')
    },
    [areaTypes.PROVINCE] : {
      listSelector:CommonOrganizationSelectors.getSelectedAreaProvince,
      getItemSelector: CommonSelectors.getProvince,
      onChange:onListChange('areaProvince'),
      onNodeRemove:onListRemove('areaProvince')
    },
    [areaTypes.BUSINESS_REGION] : {
      listSelector:CommonOrganizationSelectors.getSelectedAreaBusinessRegions,
      getItemSelector: CommonSelectors.getBusinessRegion,
      onChange:onListChange('areaBusinessRegions'),
      onNodeRemove:onListRemove('areaBusinessRegions')
    },
    [areaTypes.HOSPITAL_REGION] : {
      listSelector:CommonOrganizationSelectors.getSelectedAreaHospitalRegions,
      getItemSelector: CommonSelectors.getHospitalRegion,
      onChange:onListChange('areaHospitalRegions'),
      onNodeRemove:onListRemove('areaHospitalRegions')
    }
  }

  return (
    <div>
      <div>
        <div className='row'>
          <div className='col-xs-12'>
            <AreaInformationType
              language={language}
              messages={messages}
              readOnly={readOnly || translationMode === 'view' || translationMode === 'edit'}
              changeCallback={onAreaInformationTypeInputChange('areaInformationTypeId')}
              areaInformationType={areaInformatioTypeId}
              filteredCodes={filteredCodes}
              radio
          />
          </div>
        </div>
        {areaTypeVisible &&
        <div>
          <div className='row'>
            <div className='col-xs-6'>
              <AreaType
                language={language}
                messages={messages}
                readOnly={readOnly || translationMode === 'view' || translationMode === 'edit'}
                changeCallback={onInputChange('areaTypeId')}
                areaType={areaTypeId}
                validators={validators}
                disabled={selectedAreaCount > 0}
                />
            </div>
          </div>
          <div className='row'>
            <Areas
              language={language}
              messages={messages}
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
  )
}

OrganizationAreaInformation.propTypes = {
  messages: PropTypes.any.isRequired,
  readOnly: PropTypes.bool.isRequired,
  language: PropTypes.string.isRequired,
  translationMode: PropTypes.string.isRequired,
  actions: PropTypes.object.isRequired,
  intl: PropTypes.object.isRequired,
  areaInformatioTypeId: PropTypes.string.isRequired,
  areaTypeId: PropTypes.string.isRequired,
  areaTypeVisible: PropTypes.bool.isRequired,
  entityId: PropTypes.string.isRequired,
  selectedAreaCount: PropTypes.number.isRequired,
  filteredCodes: PropTypes.array
}

function mapStateToProps (state, ownProps) {
  const filteredCodeSelected = CommonOrganizationSelectors.getIsFilteredCodeSelected(state, ownProps)

  const filteredCodes = !filteredCodeSelected && ['areatype', 'wholecountry', 'wholecountryexceptalandislands']
  const selectedInfoAreaTypeId = CommonOrganizationSelectors.getAreaInformationTypeId(state, ownProps)
  const defaultAreaTypeId = CommonOrganizationSelectors.getSelectedAreaType(state, ownProps)
  return {

    areaInformatioTypeId: selectedInfoAreaTypeId,
    areaTypeVisible: selectedInfoAreaTypeId === CommonSelectors.getAreaInformationTypeId(state, 'areatype'),
    areaTypeId: defaultAreaTypeId,
    entityId: CommonOrganizationSelectors.getOrganizationId(state, ownProps),
    selectedAreaCount: CommonOrganizationSelectors.getSelectedAreaCount(state, ownProps),
    filteredCodes
  }
}

const actions = [
  organizationActions
]

const composeAddItem = InnerComponent => {
  const AddItem = props => {
    const render = () => (<InnerComponent {...props} />)

    return (
      <PTVAddItem {...props}
        readOnly={props.readOnly && props.translationMode === 'none'}
        collapsible={false}
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
)(OrganizationAreaInformation)

