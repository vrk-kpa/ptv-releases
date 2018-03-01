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
import { injectIntl, intlShape } from 'react-intl'

import { composePTVComponent, ValidatePTVComponent } from '../../../../../Components/PTVComponent'
import { PTVCheckBox } from '../../../../../Components'
import OpeningHoursDaily from './OpeningHoursDaily'

// selectors
import * as CommonChannelSelectors from '../../Selectors'

const OpeningHoursDailySet = props => {
  const {
    intl: { formatMessage },
    id,
    readOnly,
    language,
    messages,
    weekdays,
    nonstop,
    onNonstopChange,
    onToggleAdditional,
    onTimeChange,
    onTimeChangeExtra,
    onDayChange,
    valueToValidate
  } = props
  return (
    <div className={'row form-group'}>
      <div className='col-xs-12'>
        <PTVCheckBox
          id={id}
          onClick={onNonstopChange}
          isSelectedSelector={CommonChannelSelectors.getOpeningHourNonstop}
          language={language}
        >
          { formatMessage(messages.nonstopOpeningHours) }
        </PTVCheckBox>
      </div>

      <div className='col-xs-12'>
        {
          weekdays.map((weekdayId) => {
            const dailyId = CommonChannelSelectors.getDailyId(id, weekdayId)
            return (
              <OpeningHoursDaily
                key={dailyId}
                id={dailyId}
                weekdayId={weekdayId}
                openingHoursId={id}
                readOnly={readOnly}
                disabled={nonstop}
                onToggleAdditional={onToggleAdditional(dailyId)}
                onTimeChange={onTimeChange(dailyId)}
                onTimeChangeExtra={onTimeChangeExtra(dailyId)}
                onDayChange={onDayChange(dailyId)}
                openingHoursMessages={messages}
                language={language}
              />
            )
          })
        }
      </div>
      <div className='col-xs-12'>
        <ValidatePTVComponent {...props}
          valueToValidate={valueToValidate}
        />
      </div>
    </div>
  )
}

OpeningHoursDailySet.propTypes = {
  intl: intlShape.isRequired,
  id: PropTypes.string.isRequired,
  readOnly: PropTypes.bool,
  messages: PropTypes.object,
  weekdays: PropTypes.object,
  nonstop: PropTypes.bool,
  onNonstopChange: PropTypes.func,
  onToggleAdditional: PropTypes.func,
  onTimeChange: PropTypes.func,
  onTimeChangeExtra: PropTypes.func,
  onDayChange: PropTypes.func,
  isNormalEmpty: PropTypes.bool,
  valueEntered: PropTypes.bool,
  valueToValidate: PropTypes.string
}

export default injectIntl(composePTVComponent(OpeningHoursDailySet))
