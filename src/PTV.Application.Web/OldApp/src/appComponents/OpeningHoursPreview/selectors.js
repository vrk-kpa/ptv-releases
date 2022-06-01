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
import { createSelector } from 'reselect'
import { formValueSelector } from 'redux-form/immutable'
import { List, Map } from 'immutable'
import { EnumsSelectors, EntitySelectors } from 'selectors'
import moment from 'moment'

export const getExceptionalOpeningHours = createSelector(
  (state, props) =>
    [formValueSelector(props.formName)(state, props.composedPath) || List(),
      formValueSelector(props.formName)(state, props.holidayPath) || List(),
      EnumsSelectors.holidays.getEntities(state),
      EnumsSelectors.holidayDates.getEntities(state),
      EntitySelectors.translatedItems.getEntities(state)
    ],
  (result) => {
    let exHours = result[0]
    const holidays = result[1].filter(x => x.get('active'))
    const now = moment.utc().startOf('day')
    const hEntities = result[2]
    const hDates = result[3].filter(date => moment.utc(date.get('date')) >= now)
    const translations = result[4]

    holidays.keySeq().forEach(holiday => {
      const hEntity = hEntities.find(x => x.get('code') === holiday) || Map()
      const hId = hEntity.get('id')
      const date = hDates.find(x => x.get('holidayId') === hId) || Map()
      const name = translations.getIn([hId, 'texts'])
      exHours = exHours.push(Map({
        title: name,
        dateFrom: moment.utc(date.get('date')).valueOf(),
        timeFrom: holidays.getIn([holiday, 'intervals', 0, 'from']),
        timeTo:holidays.getIn([holiday, 'intervals', 0, 'to']),
        closedForPeriod: !holidays.getIn([holiday, 'type']) || holidays.getIn([holiday, 'type']) === 'close'
      }))
    })

    return exHours.sort((a, b) => {
      const from1 = a && a.get('dateFrom')
      const from2 = b && b.get('dateFrom')
      return from1 < from2 ? -1 : from1 > from2 ? 1 : 0
    })
  }
)
