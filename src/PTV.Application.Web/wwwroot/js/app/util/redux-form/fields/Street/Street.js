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
import { Field, change } from 'redux-form/immutable'
import { RenderTextField, RenderTextFieldDisplay } from 'util/redux-form/renders'
import { injectIntl, intlShape } from 'util/react-intl'
import CommonMessages from 'util/redux-form/messages'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { compose } from 'redux'
import messages from './messages'
import StreetMenu from './StreetMenu'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { combinePathAndField } from 'util/redux-form/util'
import { getContentLanguageCode } from 'selectors/selections'
import {
  getLanguageId,
  getFormValue,
  getDefaultStreetName
} from './selectors'
import styles from './styles.scss'
import debounce from 'debounce-promise'
import { asyncCallApi } from 'Middleware/Api'
import { normalize } from 'normalizr'
import { CodeListSchemas } from 'schemas'
import cx from 'classnames'
import { isAddressNumberInRange } from '../AddressNumber/selectors'
import withAccessibilityPrompt from 'util/redux-form/HOC/withAccessibilityPrompt'
import { PTVIcon } from 'Components'
import _ from 'lodash'
import { getSuitableStreet } from 'util/redux-form/sections/StreetAddress/actions'
import { EntitySelectors } from 'selectors'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'
import asLocalizable from 'util/redux-form/HOC/asLocalizable'
import { Map as ImmutableMap } from 'immutable'
import { isAddressRequired } from 'util/redux-form/validators'
import withValidation from 'util/redux-form/HOC/withValidation'
import { Spinner } from 'sema-ui-components'
import { SearchIcon } from 'appComponents/Icons'

class Street extends React.Component {
  // _timeoutId

  constructor (props) {
    super(props)

    // A map for storing menu item keys - it prevents duplicate key occurences
    // in case the backend pagination returns multiple similar results
    this.dataIndexMap = new Map()

    this.state = {
      menuVisible: false,
      menuOptions: [],
      focusedOption: {},
      moreAvailable: false,
      isLoadingData: false,
      lastRequestTime: 0,
      nextOffset: 0,
      totalCount: 0
    }
  }

  formatResponse = (data, append) => {
    const {
      postalCodeId,
      language
    } = this.props
    const streets = data && data.data || []
    const result = []

    if (!append) {
      // Reset map in case we are not appending additional results to existing menu options
      this.dataIndexMap = new Map()
    }

    for (const street of streets) {
      for (const streetNumber of street.streetNumbers) {
        const key = `${street.id}${streetNumber.postalCode.code}`
        if (!this.dataIndexMap.has(key) && (!postalCodeId || postalCodeId === streetNumber.postalCode.id)) {
          this.dataIndexMap.set(key, true)
          result.push({
            label: street.translation.texts[language] || street.defaultTranslation,
            value: street.id,
            entity: street,
            postalCode: streetNumber.postalCode,
            key
          })
        }
      }
    }

    const sortedResult = result.sort((a, b) => {
      const labelOrder = a.label.localeCompare(b.label)
      if (labelOrder !== 0) {
        return labelOrder
      }
      const postOfficeOrder = a.postalCode.postOffice.localeCompare(b.postalCode.postOffice)
      if (postOfficeOrder === 0) {
        return a.postalCode.code.localeCompare(b.postalCode.code)
      }
      return postOfficeOrder
    })

    return sortedResult
  }

  loadData = async (queryData, append) => {
    try {
      const timestamp = Date.now()
      this.setState({ isLoadingData: true, lastRequestTime: timestamp })

      const result = await asyncCallApi('common/GetStreets', queryData, this.props.dispatch).then(({ data }) => {
        const formattedData = this.formatResponse(data, append)
        const menuOptions = append ? this.state.menuOptions.concat(formattedData) : formattedData
        const entities = menuOptions.map(option => option.entity)
        const focusedOption = append ? this.state.focusedOption : menuOptions[0]
        const nextOffset = data && data.skip || 0
        const totalCount = data && data.count || 0
        const moreAvailable = data && data.moreAvailable || false

        if (timestamp >= this.state.lastRequestTime) {
          this.setState({ menuOptions, focusedOption, moreAvailable, isLoadingData: false, nextOffset, totalCount })
          this.props.dispatch({
            type: 'STREETS',
            response: normalize(entities, CodeListSchemas.STREET_ARRAY)
          })
        }
      })

      return result
    } catch (err) {
      console.log('Async select err:\n', err)
    }
  }

  debouncedLoadData = debounce(this.loadData, 300)

  getLocalizedName = (nameMap, language) => {
    if (!nameMap || !language) {
      return null
    }

    return nameMap.get(language) || nameMap.get('fi') || nameMap.first()
  }

  handleOnChange = (_, newName) => {
    const {
      streetId,
      language,
      postalCodeId,
      contentLanguageId
    } = this.props

    if (streetId) {
      this.callDispatchChange('streetName', null)
      this.callDispatchChange('street', null)
      this.callDispatchChange('streetNumberRange', null)
    }

    if (!newName || newName.length < 3) {
      return
    }

    const localizedName = this.getLocalizedName(newName, language)
    const queryData = {
      searchedStreetName: localizedName,
      postalCodeId,
      languageId: contentLanguageId,
      offset: 0
    }

    this.debouncedLoadData(queryData, false)
  }

  handleLoadMore = () => {
    if (this.state.isLoadingData || this.state.nextOffset > this.state.totalCount) {
      return
    }

    const {
      streetName,
      language,
      postalCodeId,
      contentLanguageId
    } = this.props

    const localizedName = this.getLocalizedName(streetName, language)
    const queryData = {
      searchedStreetName: localizedName,
      postalCodeId: postalCodeId || null,
      languageId: contentLanguageId,
      offset: this.state.nextOffset
    }

    this.debouncedLoadData(queryData, true)
  }

  getStreetNumberRangeId = (ranges, streetNumber, postalCodeId) => {
    const filteredRanges = ranges.filter((range, key) => {
      return range.postalCode.id === postalCodeId &&
        isAddressNumberInRange(streetNumber, range.startNumber, range.endNumber, range.isEven)
    })

    if (!filteredRanges || filteredRanges.length === 0) {
      return null
    }

    if (filteredRanges.length > 1) {
      console.error('Multiple street ranges detected!')
    }

    return filteredRanges[0].id
  }

  callDispatchChange = (fieldName, value) => {
    const {
      dispatch,
      formName,
      path
    } = this.props
    dispatch(change(formName, combinePathAndField(path, fieldName), value))
  }

  polyfillStreetName = entity => {
    const texts = entity && entity.translation && entity.translation.texts
    if (!texts) {
      return null
    }

    const defaultLanguageKey = Object.keys(texts)[0]
    // For Finnish use Finnish or Åland Swedish
    const finnish = texts['fi'] || texts[defaultLanguageKey]

    return {
      'fi': finnish,
      // For Swedish use Swedish or default Finnish
      'sv': texts['sv'] || finnish,
      // For English and Sami try their respective translations or default Finnish or Åland Swedish
      'en': texts['en'] || finnish,
      'se': texts['se'] || finnish,
      'smn': texts['smn'] || finnish,
      'sms': texts['sms'] || finnish
    }
  }

  handleSelectOption = option => {
    const {
      streetNumber,
      onChange
    } = this.props
    const entity = option && option.entity
    const streetName = this.polyfillStreetName(entity)
    const postalCodeId = option && option.postalCode && option.postalCode.id

    this.callDispatchChange('streetName', ImmutableMap(streetName))
    this.callDispatchChange('street', option.value)
    this.callDispatchChange('postalCode', postalCodeId)
    this.callDispatchChange('postOffice', postalCodeId)
    this.callDispatchChange('municipality', entity && entity.municipalityId)
    this.callDispatchChange('postalCodeOutsideChange', null)

    if (entity && streetNumber) {
      this.callDispatchChange('streetNumberRange',
        this.getStreetNumberRangeId(entity.streetNumbers, streetNumber, postalCodeId))
    }

    this.setState({ menuVisible: false })
    onChange && onChange(option)
  }

  handleFocusOption = option => {
    this.setState({ focusedOption: option })
  }

  handleWrapperFocus = () => {
    if (!this.state.menuVisible) {
      this.setState({
        menuVisible: true
      })
    }
  }

  handleClickOutsideMenu = () => {
    if (this.state.menuVisible) {
      this.setState({
        menuVisible: false
      })
    }
  }

  handleWrapperBlur = () => {
    const suitableStreet = getSuitableStreet(this.props)
    if (suitableStreet) {
      this.callDispatchChange('street', suitableStreet.id)
      this.callDispatchChange('streetName', suitableStreet.streetName)
    }

    this.props.onStreetBlur && this.props.onStreetBlur(this.props.streetName, this.props)
  }

  handleCrossClicked = () => {
    this.callDispatchChange('streetName', null)
    this.callDispatchChange('street', null)
  }

  moveFocusedOption = indexOffset => {
    const {
      menuOptions,
      focusedOption
    } = this.state

    if (!menuOptions || menuOptions.length === 0) {
      return
    }

    if (!focusedOption || _.isEmpty(focusedOption)) {
      this.setState({ focusedOption: menuOptions[0] })
      return
    }

    const maxIndex = menuOptions.length - 1
    const currentIndex = menuOptions.findIndex(option => option.key === focusedOption.key)
    let nextIndex = currentIndex + indexOffset

    if (nextIndex < 0) {
      nextIndex = maxIndex
    } else if (nextIndex > maxIndex) {
      nextIndex = 0
    }

    this.setState({ focusedOption: menuOptions[nextIndex] })
    this.refs['streetMenu'].scrollToIndex(menuOptions[nextIndex])
  }

  handleKeyDown = e => {
    const event = e || window.event

    switch (event.keyCode) {
      // Enter
      case 13:
        this.handleSelectOption(this.state.focusedOption)
        e.preventDefault()
        e.stopPropagation()
        break
      // Arrow up
      case 38:
        this.moveFocusedOption(-1)
        e.preventDefault()
        e.stopPropagation()
        break
      // Arrow down
      case 40:
        this.moveFocusedOption(1)
        e.preventDefault()
        e.stopPropagation()
        break
    }

    return false
  }

  isStreetNameEmpty = streetName => {
    return !streetName || streetName.size === 0 || !streetName.first()
  }

  handleCustomOnChange = (input, isCompareField = false) => newValue => {
    const { streetId, customOnChange } = this.props
    // Resetting street names when selecting a new street
    if (streetId) {
      input.value = ImmutableMap()
    }
    return customOnChange(input, isCompareField)(newValue)
  }

  render () {
    const {
      intl: { formatMessage },
      addressUseCase,
      isFirst,
      wrapperClass,
      menuClass,
      crossClass,
      streetName,
      defaultStreetName,
      // Prevent onChange overriding
      // eslint-disable-next-line no-unused-vars
      onChange,
      // Prevent customOnChange overriding from asLocalizable
      // eslint-disable-next-line no-unused-vars
      customOnChange,
      ...rest
    } = this.props

    const streetWrapperClass = cx(
      styles.streetWrapper,
      {
        [styles.isOpen]: this.state.menuVisible
      },
      wrapperClass
    )
    const streetMenuClass = cx(styles.streetMenu, menuClass)
    const streetCrossClass = cx(styles.streetCross, crossClass)

    return (
      <div
        className={streetWrapperClass}
        onFocus={this.handleWrapperFocus}
        onBlur={this.handleWrapperBlur}
        onKeyDown={this.handleKeyDown}
      >
        <Field
          name='streetName'
          label={formatMessage(CommonMessages.street)}
          tooltip={isFirst
            ? formatMessage(messages[`${addressUseCase}Tooltip1`])
            : formatMessage(messages[`${addressUseCase}Tooltip`])
          }
          placeholder={formatMessage(messages.placeholder)}
          maxLength={100}
          defaultValue={defaultStreetName}
          component={RenderTextField}
          onChange={this.handleOnChange}
          fieldClass={styles.streetField}
          inputClass={styles.streetInput}
          autocomplete={`${addressUseCase}Address`}
          customOnChange={this.handleCustomOnChange}
          {...rest}
        >
          {!this.state.isLoadingData && !this.isStreetNameEmpty(streetName) && <PTVIcon
            componentClass={streetCrossClass}
            name='icon-cross'
            onClick={this.handleCrossClicked} />}
          {this.state.isLoadingData && <Spinner className={styles.spinner} />}
          {this.isStreetNameEmpty(streetName) && <SearchIcon componentClass={styles.searchIcon} size={16} />}
          {this.state.menuVisible && <StreetMenu
            options={this.state.menuOptions || []}
            focusedOption={this.state.focusedOption}
            formatMessage={formatMessage}
            selectValue={this.handleSelectOption}
            focusOption={this.handleFocusOption}
            menuClass={streetMenuClass}
            ref={'streetMenu'}
            moreAvailable={this.state.moreAvailable}
            onLoadMore={this.handleLoadMore}
            onClickOutsideMenu={this.handleClickOutsideMenu}
          />}
        </Field>
      </div>
    )
  }
}

Street.propTypes = {
  intl: intlShape,
  addressUseCase: PropTypes.string,
  isFirst: PropTypes.bool,
  formName: PropTypes.string,
  language: PropTypes.string,
  contentLanguageId: PropTypes.string,
  postalCodeId: PropTypes.string,
  dispatch: PropTypes.func,
  streetNumber: PropTypes.string,
  wrapperClass: PropTypes.string,
  menuClass: PropTypes.string,
  onChange: PropTypes.func,
  path: PropTypes.string,
  streetName: PropTypes.object,
  crossClass: PropTypes.string,
  defaultStreetName: PropTypes.string,
  streetId: PropTypes.string,
  customOnChange: PropTypes.func,
  onStreetBlur: PropTypes.func
}

export default compose(
  withValidation({
    // validation is not localized, it has to be before asLocalize hoc
    label: CommonMessages.street,
    validate: isAddressRequired((props) => messages[`${props.addressUseCase}StreetNotFoundError`])
  }),
  injectIntl,
  injectFormName,
  asComparable({ DisplayRender: RenderTextFieldDisplay }),
  asDisableable,
  asLocalizable,
  withAccessibilityPrompt,
  connect((state, ownProps) => {
    const languageCode = ownProps.customLanguage || getContentLanguageCode(state, ownProps) || 'fi'

    return {
      language: languageCode,
      contentLanguageId: getLanguageId(state, { languageCode }),
      postalCodeId: getFormValue(state, { ...ownProps, fieldName: 'postalCode' }),
      streetNumber: getFormValue(state, { ...ownProps, fieldName: 'streetNumber' }),
      streetName: getFormValue(state, { ...ownProps, fieldName: 'streetName' }),
      streetId: getFormValue(state, { ...ownProps, fieldName: 'street' }),
      streets: EntitySelectors.streets.getEntities(state, ownProps),
      streetNumbers: EntitySelectors.streetNumbers.getEntities(state, ownProps),
      translatedItems: EntitySelectors.translatedItems.getEntities(state, ownProps),
      defaultStreetName: getDefaultStreetName(state, { ...ownProps, languageCode, fieldName: 'street' })
    }
  })
)(Street)
