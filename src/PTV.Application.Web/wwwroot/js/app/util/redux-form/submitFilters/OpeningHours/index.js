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
import * as Mappers from '../Mappers'
import composeDecorators from 'draft-js-plugins-editor/lib/utils/composeDecorators'
import { Map } from 'immutable'

const {
  getFilteredList
} = Mappers

const getHolidayList = list => {
  const holidayCodes = list.filter(h => h.get('active')).keySeq()
  return holidayCodes.map(code => Map({
    isClosed: !((list.getIn([code, 'type']) && list.getIn([code, 'type']) === 'open') || false),
    code: code,
    active: list.getIn([code, 'active']),
    intervals: list.getIn([code, 'intervals'])
  }))
}

export default (path = []) => values => {
  values = values
    .updateIn([...path, 'normalOpeningHours'], getFilteredList)
    .updateIn([...path, 'specialOpeningHours'], getFilteredList)
    .updateIn([...path, 'exceptionalOpeningHours'], getFilteredList)
    .updateIn([...path, 'holidayHours'], getHolidayList)
  // values = values.updateIn(
  //    [...path, 'normalOpeningHours'],
  //    normalOpeningHours => {
  //      normalOpeningHours = normalOpeningHours.updateIn('intervals', intervals => )
  //      return normalOpeningHour
  //    }
  // )
  return values
}

