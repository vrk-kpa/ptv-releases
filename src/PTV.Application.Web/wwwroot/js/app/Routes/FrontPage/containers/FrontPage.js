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
