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
import { isValid } from './isValid'
import { isFunction } from 'lodash'
import messages from './messages'
import { Map } from 'immutable'

const UL_TYPE = 'unordered-list-item'
const OL_TYPE = 'ordered-list-item'

const hasMaxBulletCountDraftJS = (limit) => isValid(
  ({ value }) => {
    const currentContent = value &&
      isFunction(value.getCurrentContent) &&
      value.getCurrentContent()
    const blockMap = currentContent && currentContent.getBlockMap() || Map()
    // count items in bullet/numbered lists created with draftJS
    const draftJSBulletListCount = blockMap.filter(block => block.get('type') === UL_TYPE).size
    const draftJSNumberedListCount = blockMap.filter(block => block.get('type') === OL_TYPE).size
    // check if text starts with bullet/numbered list created with draftJS
    const firstBlockType = currentContent && currentContent.getFirstBlock().get('type')
    const startsWithDraftJSList = firstBlockType === UL_TYPE || firstBlockType === OL_TYPE
    // check for user defined lists
    const text = currentContent && currentContent.getPlainText()
    // RE to match defined bullet groups: 1), 1., a), a., -, *
    let bulletRE = /^(?:(\d+\))|(\d+\.)|(\w\))|(\w\.)|(-)|(\*))\s*/mg
    // check if text starts with one of bullet groups
    const firstLine = text.split('\n')[0]
    const startWithList = firstLine.match(bulletRE)
    // count individual bullet group
    let output
    let bulletCounters = [] // count occurrence of each found bullet group
    while ((output = bulletRE.exec(text)) != null) {
      // remove first item from array which represents whole match
      // strip also additional properties like index etc.
      const captureGroups = output.slice(1)
      // prefill bulletCounters array with zeros based on output from exec method
      if (!bulletCounters.length) bulletCounters = Array(captureGroups.length).fill(0)
      // find index of bullet group which was matched in the current run
      const index = captureGroups.findIndex(o => o)
      // increment appropriate counter
      bulletCounters[index]++
    }
    return (
      startsWithDraftJSList ||
      draftJSBulletListCount > limit ||
      draftJSNumberedListCount > limit ||
      !!startWithList ||
      bulletCounters.some(counter => counter > limit)
    )
  }, {
    validationMessage: messages.draftJSbulletMaxCount,
    messageOptions: {
      limit
    }
  }
)

export default hasMaxBulletCountDraftJS
