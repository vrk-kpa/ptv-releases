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
import React, { PureComponent, PropTypes } from 'react'
import { connect } from 'react-redux'
import { compose } from 'redux'
import NewLanguageSelection from './NewLanguageSelection'
import { injectFormName } from 'util/redux-form/HOC'
import LanguagesTable from 'util/redux-form/fields/LanguagesTable'
import { getIsAddingNewLanguage } from 'selectors/formStates'
import styles from './styles.scss'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import { isSubmitting } from 'redux-form/immutable'
import { Button, Spinner } from 'sema-ui-components'
import cx from 'classnames'
import withState from 'util/withState'
import LanguageLabel from './LanguageLabel'
import EntityLabel from './EntityLabel'
import VersionLabel from './VersionLabel'
import { getIsEntityArchived } from './selectors'
import { formEntityTypes } from 'enums'
import { EntitySelectors } from 'selectors'

export const messages = defineMessages({
  entityType: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityType.Title',
    defaultMessage: 'Sisältötyyppi'
  },
  languageVersionStatus: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.LanguageVersionStatus.Title',
    defaultMessage: 'Kieliversio / tila'
  },
  languageVersionToggleTitle: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.LanguageVersionToggle.Title',
    defaultMessage: 'Lisätiedot'
  },
  versionTitle: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.Version.Title',
    defaultMessage: 'Versio'
  }
})

const withEntityHeader = ({ language, getEndpoint, handleArchiveResponse }) => ComposedComponent => {
  class EntityHeader extends PureComponent {
    toggleLanguageVersions = () => {
      this.props.updateUI('areLanguageVersionsVisible', !this.props.areLanguageVersionsVisible)
    }

    render () {
      const {
        isAddingNewLanguage,
        areLanguageVersionsVisible,
        intl: { formatMessage },
        isArchived,
        isLockingUnlocking
      } = this.props
      const entityWrapClass = cx(
        styles.entityWrap,
        {
          [styles.expanded]: areLanguageVersionsVisible,
          [styles.collapsed]: !areLanguageVersionsVisible,
          [styles.newLanguage]: isAddingNewLanguage
        }
      )
      return (
        <div className={entityWrapClass}>
          <div className={styles.entityHeader}>
            <div className={styles.entityHeaderMain}>
              <div className={styles.entitySummary}>
                {!isAddingNewLanguage
                  ? <div className={styles.entityHeaderItemList}>
                    <div className={styles.entityHeaderItem}>
                      <div className='row'>
                        <div className='col-lg-4'>
                          <strong>{formatMessage(messages.entityType)} :</strong>
                        </div>
                        <div className='col-lg-20'>
                          <div style={{ display: 'inline-flex' }}>
                            <EntityLabel disableLinks={isLockingUnlocking} linksVisible={areLanguageVersionsVisible} getEndpoint={getEndpoint} handleArchiveResponse={handleArchiveResponse}/>
                            {isLockingUnlocking && <Spinner />}
                          </div>
                        </div>
                      </div>
                    </div>
                    <div className={styles.entityHeaderItem}>
                      <div className='row'>
                        <div className='col-md-4'>
                          <strong>{formatMessage(messages.languageVersionStatus)} :</strong>
                        </div>
                        <div className='col-md-20'>
                          <LanguageLabel disableLinks={isLockingUnlocking} linksVisible={areLanguageVersionsVisible} />
                        </div>
                      </div>
                    </div>
                  </div>
                : <NewLanguageSelection />
                }
              </div>
              <div
                className={styles.toggleButton}
                onClick={this.toggleLanguageVersions}
              >
                <Button small secondary={!areLanguageVersionsVisible}>
                  {formatMessage(messages.languageVersionToggleTitle)}
                </Button>
              </div>
            </div>
            {areLanguageVersionsVisible &&
              <div>
                <div className={styles.entityLanguageTable}>
                  <LanguagesTable isArchived={isArchived} />
                </div>
                <div className={styles.entityVersionLabel}>
                  <VersionLabel />
                </div>
              </div>
            }
          </div>
          <ComposedComponent {...this.props} />
        </div>
      )
    }
  }
  EntityHeader.propTypes = {
    isAddingNewLanguage: PropTypes.bool,
    isArchived: PropTypes.bool,
    isLockingUnlocking: PropTypes.bool,
    areLanguageVersionsVisible: PropTypes.bool.isRequired,
    intl: intlShape,
    updateUI: PropTypes.func.isRequired
  }
  return compose(
    injectFormName,
    injectIntl,
    withState({
      initialState: {
        areLanguageVersionsVisible: false
      }
    }),
    connect(
      (state, { formName }) => ({
        isLockingUnlocking: EntitySelectors[formEntityTypes[formName]].getEntityIsUnLocking(state) ||
          EntitySelectors[formEntityTypes[formName]].getEntityIsLocking(state),
        isLoading: isSubmitting(formName)(state),
        isAddingNewLanguage: getIsAddingNewLanguage(formName)(state),
        isArchived: getIsEntityArchived(state)
      })
    )
  )(EntityHeader)
}

export default withEntityHeader
