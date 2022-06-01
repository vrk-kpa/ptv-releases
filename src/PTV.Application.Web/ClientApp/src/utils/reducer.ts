// See https://stackoverflow.com/questions/39419170/how-do-i-check-that-a-switch-block-is-exhaustive-in-typescript
export function assertUnreachable(x: never): never {
  throw new Error(`Unsupported reducer type: ${JSON.stringify(x)}`);
}
