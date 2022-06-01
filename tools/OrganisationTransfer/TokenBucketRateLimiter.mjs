class TokenBucketRateLimiter {
  constructor ({ maxRequests, maxRequestWindowMS }) {
    this.maxRequests = maxRequests
    this.maxRequestWindowMS = maxRequestWindowMS
    this.reset()
  }

  reset () {
    this.count = 0
    this.resetTimeout = null
  }

  scheduleReset () {
    // Only the first token in the set triggers the resetTimeout
    if (!this.resetTimeout) {
      this.resetTimeout = setTimeout(() => (
        this.reset()
      ), this.maxRequestWindowMS)
    }
  }

  async acquireToken (fn) {
    this.scheduleReset()

    if (this.count === this.maxRequests) {
      await sleep(this.maxRequestWindowMS)
      return this.acquireToken(fn)
    }

    this.count += 1
    await nextTick()
    return fn()
  }
}

function sleep (milliseconds) {
  return new Promise((resolve) => setTimeout(resolve, milliseconds))
}

function nextTick () {
  return sleep(0)
}

export default TokenBucketRateLimiter;