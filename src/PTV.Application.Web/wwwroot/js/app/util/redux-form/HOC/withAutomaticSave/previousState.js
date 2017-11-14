import { fromJS } from 'immutable'
import { EditorState } from 'draft-js'

let previousState = fromJS({
  basicInfo: {
    description: null,
    authenticationSign: false,
    attachments:  [null],
    name: '( ͡° ͜ʖ ͡°)'
  },
  openingHours: {
    normalOpeningHours: [null],
    specialOpeningHours: [null],
    exceptionalOpeningHours: [null]
  }
})

previousState = previousState.setIn(['basicInfo', 'description'], EditorState.createEmpty())

export default previousState
