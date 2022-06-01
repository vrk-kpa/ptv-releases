import { BuildEnvironment } from 'types/buildTypes';

export const warn = (msg: string): void => {
  if (process.env.NODE_ENV === BuildEnvironment.DEVELOPMENT) {
    console.warn(msg);
  }
};
