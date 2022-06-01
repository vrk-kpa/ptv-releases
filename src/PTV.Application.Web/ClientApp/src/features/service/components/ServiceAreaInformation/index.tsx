import React from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Box from '@mui/material/Box';
import { Municipality, Region, cAreaInformation } from 'types/areaTypes';
import { AreaInformationType, AreaType, Language, Mode } from 'types/enumTypes';
import { ServiceModel } from 'types/forms/serviceFormTypes';
import { OptionalFieldByWatch } from 'components/OptionalFieldByWatch';
import { useFormMetaContext } from 'context/formMeta';
import { withNamespace } from 'utils/fieldIds';
import {
  isAreaInformationTypeAreaType,
  isAreaTypeBusinessRegions,
  isAreaTypeHospitalRegions,
  isAreaTypeMunicipality,
  isAreaTypeProvince,
} from 'features/service/utils';
import { AreaMunicipalitySelector } from './AreaMunicipalitySelector';
import { AreaReferenceSelector } from './AreaReferenceSelector';
import { ServiceAreaInformationType } from './ServiceAreaInformationType';
import { ServiceAreaType } from './ServiceAreaType';

interface ServiceAreaInformatinInterface {
  name: string;
  tabLanguage: Language;
  allMunicipalities: Municipality[];
  allProvinces: Region[];
  allBusinessRegions: Region[];
  allHospitalRegions: Region[];
  mode: Mode;
  control: Control<ServiceModel>;
}

export function ServiceAreaInformation(props: ServiceAreaInformatinInterface): React.ReactElement {
  const { t } = useTranslation();
  const { mode } = useFormMetaContext();

  return (
    <Box>
      <Box mb={2} tabIndex={0} id={`languageVersions.${props.tabLanguage}.areaType`}>
        <ServiceAreaInformationType
          control={props.control}
          mode={mode}
          name={withNamespace(props.name, cAreaInformation.areaInformationType)}
          tabLanguage={props.tabLanguage}
        />
      </Box>

      <OptionalFieldByWatch<AreaInformationType>
        fieldName={`${props.name}.${cAreaInformation.areaInformationType}`}
        control={props.control}
        shouldRender={isAreaInformationTypeAreaType}
      >
        <Box>
          <Box mb={2}>
            <ServiceAreaType
              name={withNamespace(props.name, cAreaInformation.areaTypes)}
              tabLanguage={props.tabLanguage}
              mode={props.mode}
              control={props.control}
            />
          </Box>
          <OptionalFieldByWatch<AreaType[]>
            control={props.control}
            fieldName={`${props.name}.${cAreaInformation.areaTypes}`}
            shouldRender={isAreaTypeMunicipality}
          >
            <AreaMunicipalitySelector
              name={`${props.name}.${cAreaInformation.municipalities}`}
              tabLanguage={props.tabLanguage}
              allMunicipalities={props.allMunicipalities}
              mode={props.mode}
              control={props.control}
            />
          </OptionalFieldByWatch>

          <OptionalFieldByWatch<AreaType[]>
            control={props.control}
            fieldName={`${props.name}.${cAreaInformation.areaTypes}`}
            shouldRender={isAreaTypeProvince}
          >
            <Box>
              <AreaReferenceSelector
                control={props.control}
                name={`${props.name}.${cAreaInformation.provinces}`}
                tabLanguage={props.tabLanguage}
                mode={props.mode}
                allItems={props.allProvinces}
                label={t('Ptv.Service.Form.Field.AreaInformation.Provinces.Label')}
                placeholder={t('Ptv.Service.Form.Field.AreaInformation.Provinces.PlaceHolder')}
                areaOverviewLabel={t('Ptv.Service.Form.Field.AreaOverview.Provinces.Label')}
                areaOverviewButtonOpenText={t('Ptv.Service.Form.Field.AreaOverview.Provinces.Button.Open.Text')}
              />
            </Box>
          </OptionalFieldByWatch>
          <OptionalFieldByWatch<AreaType[]>
            control={props.control}
            fieldName={`${props.name}.${cAreaInformation.areaTypes}`}
            shouldRender={isAreaTypeBusinessRegions}
          >
            <Box>
              <AreaReferenceSelector
                control={props.control}
                name={`${props.name}.${cAreaInformation.businessRegions}`}
                mode={props.mode}
                tabLanguage={props.tabLanguage}
                allItems={props.allBusinessRegions}
                label={t('Ptv.Service.Form.Field.AreaInformation.BusinessRegions.Label')}
                placeholder={t('Ptv.Service.Form.Field.AreaInformation.BusinessRegions.PlaceHolder')}
                areaOverviewLabel={t('Ptv.Service.Form.Field.AreaOverview.BusinessRegions.Label')}
                areaOverviewButtonOpenText={t('Ptv.Service.Form.Field.AreaOverview.BusinessRegions.Button.Open.Text')}
              />
            </Box>
          </OptionalFieldByWatch>
          <OptionalFieldByWatch<AreaType[]>
            control={props.control}
            fieldName={`${props.name}.${cAreaInformation.areaTypes}`}
            shouldRender={isAreaTypeHospitalRegions}
          >
            <Box>
              <AreaReferenceSelector
                control={props.control}
                name={`${props.name}.${cAreaInformation.hospitalRegions}`}
                mode={props.mode}
                tabLanguage={props.tabLanguage}
                allItems={props.allHospitalRegions}
                label={t('Ptv.Service.Form.Field.AreaInformation.HospitalRegions.Label')}
                placeholder={t('Ptv.Service.Form.Field.AreaInformation.HospitalRegions.PlaceHolder')}
                areaOverviewLabel={t('Ptv.Service.Form.Field.AreaOverview.HospitalRegions.Label')}
                areaOverviewButtonOpenText={t('Ptv.Service.Form.Field.AreaOverview.HospitalRegions.Button.Open.Text')}
              />
            </Box>
          </OptionalFieldByWatch>
        </Box>
      </OptionalFieldByWatch>
    </Box>
  );
}
