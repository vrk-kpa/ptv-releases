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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { compose } from 'redux'
import NewLanguageSelection from './NewLanguageSelection'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import LanguagesTable from 'util/redux-form/fields/LanguagesTable'
import { getIsAddingNewLanguage } from 'selectors/formStates'
import styles from './styles.scss'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { isSubmitting } from 'redux-form/immutable'
import { Button, Tabs, Tab } from 'sema-ui-components'
import ArrowDown from 'appComponents/ArrowDown'
import ArrowUp from 'appComponents/ArrowUp'
import cx from 'classnames'
import withState from 'util/withState'
import LanguageLabel from './LanguageLabel'
import EntityLabel from './EntityLabel'
import VersionLabel from './VersionLabel'
import ConnectionHistory from './ConnectionHistory'
import EntityHistory from './EntityHistory'
import { getIsEntityArchived, getIsEntityWithConnections } from './selectors'
import { formEntityTypes } from 'enums'
import { EntitySelectors } from 'selectors'
import EntityActions from './EntityActions'

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
  },
  languageVersionsTabTitle: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.Tab.LanguageVersions.Title',
    defaultMessage: 'Language versions'
  },
  languagePublishTabTitle: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.Tab.LanguagePublish.Title',
    defaultMessage: 'Julkaisutiedot'
  },
  entityHistoryTabTitle: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.Tab.EntityHistory.Title',
    defaultMessage: 'Entity history'
  },
  connectionHistoryTabTitle: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.Tab.ConnectionHistory.Title',
    defaultMessage: 'Connection history'
  },
  versionInformationTabTitle: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.VersionLabel.Title',
    defaultMessage: 'Tunnistetiedot'
  }
})

const withEntityHeader = ({
  language,
  handleArchiveResponse,
  handleNewLanguageResponse,
  successArchiveAction,
  handleActionCancel
}) => ComposedComponent => {
  class EntityHeader extends Component {
    state = { tabIndex: 0 }
    handleTabOnChange = tabIndex => this.setState({ tabIndex })
    toggleLanguageVersions = () => {
      this.props.updateUI(
        'areLanguageVersionsVisible',
        !this.props.areLanguageVersionsVisible
      )
    }
    render () {
      const {
        isAddingNewLanguage,
        areLanguageVersionsVisible,
        intl: { formatMessage },
        isArchived,
        isLockingUnlocking,
        isEntityWithConnections
      } = this.props
      const entityWrapClass = cx(styles.entityWrap, {
        [styles.expanded]: areLanguageVersionsVisible,
        [styles.collapsed]: !areLanguageVersionsVisible,
        [styles.newLanguage]: isAddingNewLanguage
      })
      return (
        <div className={entityWrapClass}>
          <div className={styles.entityHeader}>
            <div className={styles.entityHeaderMain}>
              <div className={styles.entitySummary}>
                {!isAddingNewLanguage ? (
                  <div className={styles.entityHeaderItemList}>
                    <div className={styles.entityHeaderItem}>
                      <div className={styles.entityHeaderItemLabel}>
                        <strong>{formatMessage(messages.entityType)}: </strong>
                      </div>
                      <div>
                        <EntityLabel isLockingUnlocking={isLockingUnlocking} />
                      </div>
                    </div>
                    <div className={styles.entityHeaderItem}>
                      <div className={styles.entityHeaderItemLabel}>
                        <strong>
                          {formatMessage(messages.languageVersionStatus)}:{' '}
                        </strong>
                      </div>
                      <div>
                        <LanguageLabel />
                      </div>
                    </div>
                  </div>
                ) : (
                  <NewLanguageSelection handleNewLanguageResponse={handleNewLanguageResponse} />
                )}
              </div>
              <EntityActions
                handleArchiveResponse={handleArchiveResponse}
                successArchiveAction={successArchiveAction}
                handleActionCancel={handleActionCancel}
              />
              <div
                className={styles.toggleButton}
                onClick={this.toggleLanguageVersions}
              >
                <Button small secondary={!areLanguageVersionsVisible}>
                  <div>
                    {formatMessage(messages.languageVersionToggleTitle)}
                  </div>
                  {areLanguageVersionsVisible ? <ArrowUp /> : <ArrowDown />}
                </Button>
              </div>
            </div>
            {areLanguageVersionsVisible && (
              <div>
                <Tabs
                  index={this.state.tabIndex}
                  onChange={this.handleTabOnChange}
                  className={styles.entityHeaderTabs}
                >
                  <Tab label={formatMessage(messages.languageVersionsTabTitle)}>
                    <div className={styles.entityLanguageTable}>
                      <LanguagesTable isArchived={isArchived} borderless />
                    </div>
                  </Tab>
                  <Tab label={formatMessage(messages.entityHistoryTabTitle)}>
                    <EntityHistory />
                  </Tab>
                  <Tab
                    label={formatMessage(messages.connectionHistoryTabTitle)}
                    hidden={!isEntityWithConnections}
                  >
                    <ConnectionHistory />
                  </Tab>
                  <Tab label={formatMessage(messages.versionInformationTabTitle)}>
                    <VersionLabel />
                  </Tab>
                </Tabs>
              </div>
            )}
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
    updateUI: PropTypes.func.isRequired,
    isEntityWithConnections: PropTypes.bool
  }

  return compose(
    injectIntl,
    injectFormName,
    withState({
      initialState: {
        areLanguageVersionsVisible: false
      },
      key:'languageVersionsVisible',
      redux: true
    }),
    connect((state, { formName }) => ({
      isLockingUnlocking: EntitySelectors[formEntityTypes[formName]].getEntityIsLoading(state),
      isLoading: isSubmitting(formName)(state),
      isAddingNewLanguage: getIsAddingNewLanguage(formName)(state),
      isArchived: getIsEntityArchived(state),
      isEntityWithConnections: getIsEntityWithConnections(state)
    }))
  )(EntityHeader)
}

export default withEntityHeader
