export const RedirectRoot = () => {
  const redirectUrl = window.location.href
  window.location.replace(redirectUrl)
  return null
}
