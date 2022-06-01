import { Municipality } from 'types/areaTypes';

export function hasPostalCode(municipalityCode: string, municipalities: Municipality[], postalCode: string): boolean {
  const municipality = municipalities.find((x) => x.code === municipalityCode);
  if (!municipality) return false;

  return municipality.postalCodes.find((x) => x.code === postalCode) !== undefined;
}
