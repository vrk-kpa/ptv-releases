import ElectronicChannelRoute from 'Routes/Channels/routes/Electronic'
import WebPageChannelRoute from 'Routes/Channels/routes/WebPage'
import PrintableFormRoute from 'Routes/Channels/routes/PrintableForm'
import PhoneChannelRoute from 'Routes/Channels/routes/Phone'
import ServiceLocationRoute from 'Routes/Channels/routes/ServiceLocation'
import { Channels } from 'Routes/Channels/components'

export default store => ({
  path: '/channels',
  childRoutes: [
    ElectronicChannelRoute(store),
    WebPageChannelRoute(store),
    PrintableFormRoute(store),
    PhoneChannelRoute(store),
    ServiceLocationRoute(store)
  ],
  getComponent (nextState, cb) {
    cb(null, Channels)
  }
})
