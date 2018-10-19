// import { createSelector } from 'reselect'

// export const getEntityType = createSelector(
//   getPageModeState,
//   pageModeState => {
//     const result = {
//       services: 'eChannel',
//       eChannel: 'eChannel'
//     }[pageModeState.get('searchDomain')] || null
//     return result
//   }
// )
// export const getEntityId = createSelector(
//   [getPageModeState, getEntityType],
//   (pageModeState, entityType) => {
//     const result = pageModeState.getIn([entityType, 'id'])
//     console.log('result:\n', result)
//     return result
//   }
// )
// export const getLanguageId = createSelector(
//   [getPageModeState, getEntityType],
//   (pageModeState, entityType) => {
//     return pageModeState.getIn([entityType, 'languageTo']) || '77f76576-cd77-43dd-b13f-6aff0f84a4b4'
//   }
// )
