import { compose } from 'redux'
import FrontPage from 'Routes/FrontPage/components/FrontPage'
import { injectIntl } from 'util/react-intl'
// sarzijan:remove
import { testCallMassFunction } from 'reducers/signalR'
import withSignalRHub from 'util/redux-form/HOC/withSignalRHub'
import { API_ROOT } from 'Configuration/AppHelpers'

export default compose(
  injectIntl
  // withSignalRHub({ hubName: 'app',
  //   hubLink: new URL(API_ROOT).origin + '/massToolHub',
  //   actionDefinition: [{ type: 'ReceiveMessage',
  //     action: (dispatch) => (message) => {
  //       // sarzijan:remove
  //       dispatch(testCallMassFunction)
  //       console.log('SignalR Message: ', message)
  //     }
  //   }]
  // }),
)(FrontPage)
