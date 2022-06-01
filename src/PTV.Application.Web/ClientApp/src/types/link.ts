export type LinkModel = {
  name: string;
  url: string;
};

export enum cLink {
  name = 'name',
  url = 'url',
}

export const Link = (): LinkModel => ({
  name: '',
  url: '',
});
