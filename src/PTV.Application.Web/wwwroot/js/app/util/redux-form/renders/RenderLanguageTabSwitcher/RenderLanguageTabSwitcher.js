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
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { injectIntl, intlShape } from 'util/react-intl'
import { changeContentLanguage } from './actions'
import { getShowReviewBar } from 'util/redux-form/HOC/withMassToolForm/selectors'
import { getIsAddingNewLanguage } from 'selectors/formStates'
import { getContentLanguageId } from 'selectors/selections'
import { Map, List, fromJS } from 'immutable'
import LanguageOption from './LanguageOption'
import AddNewLanguage from './AddNewLanguage'
import { Tabs, Tab } from 'sema-ui-components'
import { messages as commonMessages } from 'Routes/messages'
import cx from 'classnames'
import styles from './styles.scss'

const languageTabOptionRenderer = option => <LanguageOption {...option} />
const languageTabValueRenderer = option => <LanguageOption {...option} />

const LanguageTabSwitcher = ({
  activeIndex,
  compact,
  changeContentLanguage,
  hasError,
  input,
  intl: { formatMessage },
  isAddingNewLanguage,
  isInReview,
  isReadOnly
}) => {
  const newLanguage = Map({
    code: '?',
    name: commonMessages.languageVersionTitleNew,
    languageId: 'temp_lang_id'
  })

  const handleOnSelectOptionChange = option => {
    changeContentLanguage(fromJS(option))
  }

  const tabSelectionDisabled = !isReadOnly && hasError || isInReview || isAddingNewLanguage
  const languageTabSwitcherClass = cx(
    styles.languageTabSwitcher,
    {
      [styles.compact]: compact
    }
  )
  return (
    <div className={languageTabSwitcherClass}>
      {input.value.size > 0 &&
        <Tabs
          index={activeIndex}
          compact={compact}
          selectProps={{
            options: input.value.toJS(),
            value: input.value.toJS()[activeIndex],
            valueKey: 'languageId',
            onChange: handleOnSelectOptionChange,
            disabled: tabSelectionDisabled,
            optionRenderer: languageTabOptionRenderer,
            valueRenderer: languageTabValueRenderer,
            size: isAddingNewLanguage ? 'w280' : 'w200',
            className: styles.languageSelect
          }}
        >
          {input.value.map((language, index) => {
            const handleOnTabOptionClick = () => {
              changeContentLanguage(language)
            }

            const tabDisabled = !isReadOnly && hasError ||
              (isInReview || isAddingNewLanguage) && activeIndex !== index
            return (
              <Tab
                disabled={tabDisabled}
                hidden={isInReview && tabDisabled}
                key={index}
                label={
                  <span className={styles.languageTab}>
                    <LanguageOption
                      {...language.toJS()}
                      disabled={tabDisabled}
                      active={activeIndex === index}
                      inTab
                    />
                  </span>
                }
                onClick={handleOnTabOptionClick}
              />
            )
          })}
        </Tabs> ||
        <Tabs
          index={0}
          compact={compact}
          selectProps={{
            options: [newLanguage.toJS()],
            value: [newLanguage.toJS()][0],
            valueKey: 'languageId',
            disabled: tabSelectionDisabled,
            optionRenderer: languageTabOptionRenderer,
            valueRenderer: languageTabValueRenderer,
            size: 'w280',
            className: styles.languageSelect
          }}
        >
          <Tab
            key={0}
            label={
              <span className={styles.languageTab}>
                <LanguageOption
                  {...newLanguage.toJS()}
                  active
                  inTab
                />
              </span>
            }
          />
        </Tabs>
      }
      <AddNewLanguage
        newLanguage={newLanguage}
        hasError={hasError}
        input={input}
      />
    </div>
  )
}

LanguageTabSwitcher.propTypes = {
  activeIndex: PropTypes.number.isRequired,
  compact: PropTypes.bool,
  changeContentLanguage: PropTypes.func.isRequired,
  hasError: PropTypes.bool,
  input: PropTypes.any,
  intl: intlShape,
  isAddingNewLanguage: PropTypes.bool,
  isInReview: PropTypes.bool,
  isReadOnly: PropTypes.bool
}

export default compose(
  injectIntl,
  injectFormName,
  connect(
    (state, ownProps) => {
      const contentLanguageId = getContentLanguageId(state, ownProps)
      const isAddingNewLanguage = getIsAddingNewLanguage(ownProps.formName)(state)
      const aIndex = (ownProps.input.value || List()).findIndex(language => isAddingNewLanguage
        ? language.get('languageId') === 'temp_lang_id'
        : language.get('languageId') === contentLanguageId
      )
      const hasError = ownProps.getHasErrorSelector && ownProps.getHasErrorSelector(state, ownProps)
      const isInReview = getShowReviewBar(state)
      return {
        isAddingNewLanguage,
        activeIndex: aIndex > 0 ? aIndex : (contentLanguageId == null && ownProps.input.value.size || 0),
        hasError,
        isInReview
      }
    }, {
      changeContentLanguage
    }
  ),
  withFormStates
)(LanguageTabSwitcher)
