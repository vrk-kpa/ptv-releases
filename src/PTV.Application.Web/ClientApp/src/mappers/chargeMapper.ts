import { valueOrDefault } from 'utils';
import { ChargeApiModel } from 'types/api/chargeApiModel';
import { ChargeModel } from 'types/forms/chargeType';

export function toChargeUiModel(input: ChargeApiModel): ChargeModel {
  return {
    info: valueOrDefault(input.info, ''),
  };
}

export function toChargeApiModel(input: ChargeModel): ChargeApiModel {
  return {
    info: input.info,
  };
}
