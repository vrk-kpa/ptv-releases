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
import { injectIntl } from 'react-intl'
import { connect } from 'react-redux'
import { areaTypes } from 'Containers/Common/Enums'

// selectors
import { getAreaTypeCodeForId, getAreaMunicipalities } from 'Containers/Common/Selectors'

// components
import { LocalizedTagSelect, LocalizedText } from 'Containers/Common/localizedData'
import PTVTreeList from 'Components/PTVTreeViewComponent/PTVTreeList'
import { getDefaultTranslationText } from 'Containers/Common/FintoTree'
import { PTVLabel, PTVLabelCustomComponent } from 'Components'
import AreaTree from './areaTree'

const connectionAreaMunicipalities = ({ municipalities }) => {
  return (
    <div className='list-inline'>
      {municipalities.map(municipality => {
        return (<LocalizedText id={municipality.id} name={municipality.name} key={municipality.id} />)
      })}
    </div>
  )
}

connectionAreaMunicipalities.propTypes = {
  municipalities: PropTypes.array
}

const mapStateToPropsConnection = (state, ownProps) => {
  return {
    municipalities:ownProps.selector(state, ownProps)
  }
}
const ConnectAreaTag = connect(mapStateToPropsConnection)(connectionAreaMunicipalities)

const Areas = ({
  intl,
  configuration,
  selectedType,
  componentClass,
  id,
  messages,
  changeCallback,
  validators,
  order,
  readOnly,
  selectedCount,
  ...rest }) => {
  const { formatMessage } = intl
  const renderOption = (option) => {
    return (
      <div className='tag-item-content' key={option.id}>
        <strong>{option.name}</strong>
        <ConnectAreaTag
          id={option.id}
          selector={getAreaMunicipalities} />
      </div>
    )
  }
  return (
    <div>
      {selectedType && !readOnly &&
      <div className='col-xs-6'>
        <AreaTree
          {...rest}
          selectedType={selectedType}
          componentClass={componentClass}
          id={id}
          label={messages[`${selectedType}AreasTitle`] && formatMessage(messages[`${selectedType}AreasTitle`])}
          validatedField={messages[`${selectedType}AreasTitle`]}
          {...configuration[selectedType]}
          validators={validators}
          order={order}
          readOnly={readOnly}
        />
      </div>}
      <div className={readOnly ? 'col-xs-12' : 'col-xs-6'}>
        {selectedCount > 0 &&
        <PTVLabel labelClass='main'>
          {formatMessage(messages.selectedRegionsAreasTitle, { count: selectedCount })}
        </PTVLabel>
        }
        <PTVTreeList
          {...rest}
          getDefaultTranslationText={getDefaultTranslationText(intl)}
          {...configuration[areaTypes.MUNICIPALITY]}
          readOnly={readOnly}
          tagType='full-size'
        />
        <PTVTreeList
          {...rest}
          getDefaultTranslationText={getDefaultTranslationText(intl)}
          {...configuration[areaTypes.PROVINCE]}
          readOnly={readOnly}
          optionRenderer={renderOption}
          tagType='full-size'
        />
        <PTVTreeList
          {...rest}
          getDefaultTranslationText={getDefaultTranslationText(intl)}
          {...configuration[areaTypes.BUSINESS_REGION]}
          readOnly={readOnly}
          optionRenderer={renderOption}
          tagType='full-size'
        />
        <PTVTreeList
          {...rest}
          getDefaultTranslationText={getDefaultTranslationText(intl)}
          {...configuration[areaTypes.HOSPITAL_REGION]}
          readOnly={readOnly}
          optionRenderer={renderOption}
          tagType='full-size'
        />
      </div>
    </div>
  )
}

Areas.propTypes = {
  readOnly: PropTypes.bool.isRequired,
  order: PropTypes.number,
  validators: PropTypes.any,
  changeCallback: PropTypes.func,
  messages: PropTypes.object,
  id: PropTypes.string,
  componentClass: PropTypes.string,
  configuration: PropTypes.object.isRequired,
  selectedType: PropTypes.string,
  intl: PropTypes.object.isRequired,
  selectedCount: PropTypes.number.isRequired
}

function mapStateToProps (state, ownProps) {
  const selectedType = getAreaTypeCodeForId(state, { id:ownProps.areaType })
  return {
    selectedType
  }
}

export default connect(mapStateToProps)(injectIntl(Areas))
