export function looksLikeValidUrl(url: string): boolean {
  if (!url) {
    return false;
  }

  // TODO: we should validate that the url has somewhat valid domain part
  // e.g. foo.com, www.foo.com, wwww.foo.co.uk. Note that the url can point to a file.
  // Old code uess this lib https://github.com/validatorjs/validator.js
  const str = url.toLowerCase();
  return str.startsWith('http://') || str.startsWith('https://');
}
