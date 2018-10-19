module.exports = {
  port: 4000,
  add: (app, middleware, options) => {
    app.use(async (ctx, next) => {
      ctx.set('Access-Control-Allow-Origin', '*')
      await next()
    })
  }
}
