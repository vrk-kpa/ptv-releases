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
import { defineMessages } from 'util/react-intl'

export default defineMessages({
  entityNotSelectedPlaceholder: {
    id: 'Routes.Connections.Components.SearchResult.EntityNotSelected.Title',
    defaultMessage: 'Valitse sisältötyyppi yläpuolella olevista napeista'
  },
  noResultsPlaceholder: {
    id: 'Routes.Connections.Components.SearchResult.NoResults.Title',
    defaultMessage: 'Ei hakutuloksia.'
  },
  editConnectionsTabName: {
    id: 'Routes.Connections.Components.Tab.Edit.Title',
    defaultMessage: 'Muokkaa liitoksia'
  },
  organizationConnectionsTabName: {
    id: 'Routes.Connections.Components.Tab.MyOrganization.Title',
    defaultMessage: 'Organisaationi liitokset'
  },
  pageTitle: {
    id: 'Routes.Connections.Components.Page.Title',
    defaultMessage: 'Liitokset osiossa voit hallinnoida organisaatiosi palvelujen ja asiointikanavien liitoksia.'
  },
  pageTooltip: {
    id: 'Routes.Connections.Components.Page.Tooltip',
    defaultMessage: 'Liitokset osiossa voit hallinnoida organisaatiosi palvelujen ja asiointikanavien liitoksia.'
  },
  loadingWorkbenchData: {
    id: 'Connections.Workbench.LoadingData.Title',
    defaultMessage: 'Loading workbench data'
  },
  resultCountInfo: {
    id: 'Routes.Connections.Components.SearchResult.Filter.Title',
    defaultMessage: 'Specify search by using search keyword'
  }
})
