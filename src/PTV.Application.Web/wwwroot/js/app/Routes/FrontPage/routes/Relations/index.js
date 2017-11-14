import Relations from './containers/Relations'

export default store => ({
  path: 'relations',
  getComponent (nextState, cb) {
    cb(null, Relations)
  }
})
