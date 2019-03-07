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
import { formValues } from 'redux-form/immutable'
import { Checkbox } from 'util/redux-form/fields'
import asSection from 'util/redux-form/HOC/asSection'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { Button } from 'sema-ui-components'
import Arrow from 'appComponents/Arrow'
import {
  getIsBottomReached,
  getCurrentItem,
  getTotalCount
} from './selectors'
import withState from 'util/withState'
import withPath from 'util/redux-form/HOC/withPath'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import styles from './styles.scss'
import { withProps } from 'recompose'
import { localizeProps, languageTranslationTypes } from 'appComponents/Localize'
import { ShowDate } from 'appComponents/Cells/ModifiedAtCell/ModifiedAtCell'
import {
  moveToStep,
  switchToEditableVersion
} from './actions'
import Language from 'appComponents/LanguageBar/Language'
import cx from 'classnames'

const messages = defineMessages({
  entityLabel: {
    id: 'Util.ReduxForm.HOC.WithMassToolForm.Form.EntityLabel.Title',
    defaultMessage: 'Hyväksyttävä sisältö'
  },
  expiryLabel: {
    id: 'Util.ReduxForm.HOC.WithMassToolForm.Form.ExpiryLabel.Title',
    defaultMessage: 'Vanhenee'
  },
  errorsLabel: {
    id: 'Util.ReduxForm.HOC.WithMassToolForm.Form.ErrorsLabel.Title',
    defaultMessage: 'Virheilmoitukset'
  },
  approveErrorsText: {
    id: 'Util.ReduxForm.HOC.WithMassToolForm.Form.ErrorsLabel.Message',
    defaultMessage: 'Korjaa virheet muokkaustilassa, tai siirry hyväksymättä seuraavaan sisältöön.',
    description: {
      en: 'Mandatory information missing. Please check them to be able to approve the content for publishing.'
    }
  },
  approveLabel: {
    id: 'Util.ReduxForm.HOC.WithMassToolForm.Form.ApproveLabel.Title',
    defaultMessage: 'Hyväksyminen'
  },
  approveCheckboxLabel: {
    id: 'Util.ReduxForm.HOC.WithMassToolForm.Form.ApproveCheckboxLabel.Title',
    defaultMessage: 'Hyväksy julkaistavaksi'
  },
  nextButton: {
    id: 'Components.Buttons.NextButton',
    defaultMessage: 'Seuraava'
  },
  publishButton: {
    id: 'Components.Buttons.PublishButton',
    defaultMessage: 'Julkaise'
  },
  editableVersionLink: {
    id: 'Util.ReduxForm.HOC.WithMassToolForm.Form.UseMoreRecentVersion.Link',
    defaultMessage: 'Version is not valid for review, use more recent version'
  }
})

const LanguageText = compose(
  localizeProps({
    languageTranslationType: languageTranslationTypes.locale
  })
)(({ name }) => name)

const ReviewBar = ({
  languageAvailability,
  entityName,
  language,
  languageId,
  statusId,
  expirationDate,
  hasError,
  currentStep,
  totalSteps,
  intl: { formatMessage },
  isBottomReached,
  setReviewCurrentStep,
  updateUI,
  mergeInUIState,
  moveToStep,
  switchToEditableVersion,
  isItemApproved,
  forwardedRef,
  isEditable,
  id,
  editableVersion
  // ...rest
}) => {
  const handleOnNext = () => {
    const el = forwardedRef.current
    el.scrollTop = 0
    moveToStep(currentStep + 1)
  }
  const handleOnPublish = () => {
    updateUI({ isOpen: true })
  }

  const editableVersionLinkOnClick = () => {
    switchToEditableVersion({ id, editableVersion })
  }

  const isLastStep = currentStep === totalSteps - 1
  const approvalRowHeadClass = cx(
    styles.approvalRowHead,
    {
      [styles.approved]: isItemApproved
    }
  )
  const errorCellClass = cx(
    styles.errorCell,
    {
      [styles.hasError]: hasError
    }
  )
  return (
    <div>
      <div className={approvalRowHeadClass}>
        <div className='row'>
          <div className='col-lg-6'>
            {formatMessage(messages.entityLabel)}
          </div>
          <div className='col-lg-2'>
            {formatMessage(messages.expiryLabel)}
          </div>
          <div className='col-lg-7'>
            {formatMessage(messages.errorsLabel)}
          </div>
          <div className='col-lg-4'>
            {formatMessage(messages.approveLabel)}
          </div>
          <div className='col-lg-5' />
        </div>
      </div>
      <div className={styles.approvalRowBody}>
        <div className='row align-items-center'>
          <div className='col-lg-6'>
            <div className={styles.contentColumn}>
              <Language
                componentClass={styles.languageIcon}
                languageId={languageId}
                statusId={statusId}
              /> {entityName} / <LanguageText id={languageId} name={languageId} />
            </div>
          </div>
          <div className='col-lg-2'>
            <ShowDate value={expirationDate} />
          </div>
          <div className='col-lg-7'>
            <div className={errorCellClass}>
              {hasError && formatMessage(messages.approveErrorsText) || null}
              {!isEditable && editableVersion &&
                <Button link onClick={editableVersionLinkOnClick}>
                  {formatMessage(messages.editableVersionLink)}
                </Button> ||
                null
              }
            </div>
          </div>
          <div className='col-lg-4'>
            <div className={styles.approvalCheckbox}>
              <Checkbox
                name='approved'
                label={formatMessage(messages.approveCheckboxLabel)}
                small
                centered
                disabled={(!isBottomReached || hasError || !isEditable) && !isItemApproved}
                archived={!isItemApproved}
                success={isItemApproved}
              />
            </div>
          </div>
          <div className='col-lg-5'>
            <div className={styles.reviewBarButtonGroup}>
              {totalSteps > 1 && <Button
                // children={formatMessage(messages.nextButton)}
                onClick={handleOnNext}
                small
                disabled={isLastStep}
              >
                {formatMessage(messages.nextButton)}
                <Arrow direction='east' gap='g5' gapSide='left' />
              </Button>
              }
              <Button
                children={formatMessage(messages.publishButton)}
                onClick={handleOnPublish}
                small
                secondary
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}

ReviewBar.propTypes = {
  languageAvailability: PropTypes.string,
  entityName: PropTypes.string,
  language: PropTypes.string,
  languageId: PropTypes.string,
  statusId: PropTypes.string,
  expirationDate: PropTypes.number,
  hasError: PropTypes.bool,
  currentStep: PropTypes.number,
  totalSteps: PropTypes.number,
  isBottomReached: PropTypes.bool,
  setReviewCurrentStep: PropTypes.func,
  updateUI: PropTypes.func,
  mergeInUIState: PropTypes.func,
  switchToEditableVersion: PropTypes.func.isRequired,
  moveToStep: PropTypes.func.isRequired,
  isItemApproved: PropTypes.bool,
  forwardedRef: PropTypes.oneOfType([PropTypes.func, PropTypes.object]),
  isEditable: PropTypes.bool,
  id: PropTypes.string.isRequired,
  editableVersion: PropTypes.string,
  intl: intlShape
}

export default compose(
  injectIntl,
  withProps(
    ({ currentStep }) => ({
      name: `review[${currentStep}]`
    })
  ),
  asSection(),
  injectFormName,
  formValues({
    type: 'meta.type',
    id: 'id',
    language: 'language',
    languageId: 'languageId',
    meta: 'meta',
    isItemApproved: 'approved'
  }),
  withPath,
  connect((state, ownProps) => {
    const currentItem = getCurrentItem(state, { id: ownProps.id, type: ownProps.type, language: ownProps.language })
    const expirationDate = currentItem.get('expireOn')
    return {
      entityName: currentItem.get('name'),
      languageId: ownProps.languageId,
      statusId: currentItem.get('statusId'),
      expirationDate,
      hasError: currentItem.get('hasError'),
      totalSteps: getTotalCount(state, ownProps),
      isBottomReached: getIsBottomReached(state),
      isEditable: currentItem.get('isEditable'),
      editableVersion: currentItem.get('editableVersion')
    }
  }, {
    moveToStep,
    switchToEditableVersion
  }),
  withState({
    redux: true,
    key: 'MassToolDialog',
    initialState: {
      isOpen: false
    }
  }),
)(ReviewBar)
