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
import { FormSection } from 'redux-form/immutable'
import { Sticky, StickyContainer } from 'react-sticky'
import {
  NormalOpeningHours,
  SpecialOpeningHours,
  ExceptionalOpeningHours
} from 'util/redux-form/sections'
import { compose } from 'redux'
import { withFormStates, asCollection, asContainer, asComparable } from 'util/redux-form/HOC'
import styles from './styles.scss'
import { defineMessages, FormattedMessage, injectIntl } from 'react-intl'
import OpeningHoursPreview from 'appComponents/OpeningHoursPreview/OpeningHoursPreview'
import withState from 'util/withState'
import cx from 'classnames'

export const messages = defineMessages({
  add: {
    id: 'redux-form.Sections.OpeningHours.AddButton',
    defaultMessage: 'Lisää palveluaika'
  },
  containerTitle: {
    id: 'Containers.Channels.Common.OpeningHours.ShowOpeningHours',
    defaultMessage: 'Palveluajat'
  },
  normalTitle: {
    id: 'Containers.Channels.Common.OpeningHours.MainLabelNormal',
    defaultMessage: 'Normaalitaukioloajat'
  },
  specialTitle: {
    id: 'Containers.Channels.Common.OpeningHours.MainLabelSpecial',
    defaultMessage: 'Vuorokauden yli menevät aukioloajat'
  },
  exceptionalTitle: {
    id: 'Containers.Channels.Common.OpeningHours.MainLabelExceptional',
    defaultMessage: 'Poikkeusaukioloajat'
  },
  normalTooltip: {
    id: 'Containers.Channels.Common.OpeningHours.MainTooltipNormal',
    defaultMessage: 'This is a tooltip for NOH'
  },
  specialTooltip: {
    id: 'Containers.Channels.Common.OpeningHours.MainTooltipSpecial',
    defaultMessage: 'This is a tooltip for SOH'
  },
  exceptionalTooltip: {
    id: 'Containers.Channels.Common.OpeningHours.MainTooltipExceptional',
    defaultMessage: 'This is a tooltip for EOH'
  }
})
const NormalOpeningHoursCollection = compose(
  injectIntl,
  asContainer({
    withoutState: true,
    title: messages.normalTitle,
    tooltip: messages.normalTooltip,
    withCollection: true
  }),
  asCollection({
    name: 'normalOpeningHour',
    addBtnTitle: <FormattedMessage {...messages.add} />
  })
)(props => <NormalOpeningHours {...props} />)
const SpecialOpeningHoursCollection = compose(
  asContainer({
    withoutState: true,
    title: messages.specialTitle,
    tooltip: messages.specialTooltip,
    withCollection: true
  }),
  asCollection({
    name: 'specialOpeningHour',
    addBtnTitle: <FormattedMessage {...messages.add} />
  })
)(props => <SpecialOpeningHours {...props} />)
const ExceptionalOpeningHoursCollection = compose(
  asContainer({
    withoutState: true,
    title: messages.exceptionalTitle,
    tooltip: messages.exceptionalTooltip,
    withCollection: true
  }),
  asCollection({
    name: 'exceptionalOpeningHour',
    addBtnTitle: <FormattedMessage {...messages.add} />
  })
)(props => <ExceptionalOpeningHours {...props} />)

class OpeningHours extends FormSection {
  static defaultProps = {
    name: 'openingHours',
    showPreview: true
  }
  handleOnOpen = name => this.props.updateUI(
    'openedCollection',
    this.props.openedCollection === name
      ? null
      : name
  )
  normalOpeningHoursOnChange = () => this.handleOnOpen('normal')
  specialOpeningHoursOnChange = () => this.handleOnOpen('special')
  exceptionalOpeningHoursOnChange = () => this.handleOnOpen('exceptional')
  render () {
    const {
      openedCollection,
      compare,
      isCompareMode,
      showPreview,
      field
    } = this.props
    const previewCompareModeClass = isCompareMode ? 'col-lg-24' : 'col-lg-8'
    const openingHoursWrapClass = showPreview && !isCompareMode ? 'col-lg-16' : 'col-lg-24'
    const openingHoursClass = cx(
      styles.openingHours,
      {
        [styles.noPreview]: !showPreview || isCompareMode
      }
    )
    return (
      <StickyContainer>
        <div className='row'>
          <div className={openingHoursWrapClass}>
            <div className={openingHoursClass}>
              <NormalOpeningHoursCollection
                onChange={this.normalOpeningHoursOnChange}
                isCollapsed={openedCollection !== 'normal'}
                compare={compare}
              />
              <SpecialOpeningHoursCollection
                onChange={this.specialOpeningHoursOnChange}
                isCollapsed={openedCollection !== 'special'}
                compare={compare}
              />
              <ExceptionalOpeningHoursCollection
                onChange={this.exceptionalOpeningHoursOnChange}
                isCollapsed={openedCollection !== 'exceptional'}
                compare={compare}
              />
            </div>
          </div>
          {showPreview && !isCompareMode &&
            <div className={previewCompareModeClass}>
              <Sticky>
                <OpeningHoursPreview compare={compare} field={field} />
              </Sticky>
            </div>}
        </div>
      </StickyContainer>
    )
  }
}

export default compose(
  injectIntl,
  asComparable({ DisplayComponent: OpeningHoursPreview }),
  withFormStates,
  withState({
    initialState: {
      openedCollection: null
    }
  })
)(OpeningHours)
