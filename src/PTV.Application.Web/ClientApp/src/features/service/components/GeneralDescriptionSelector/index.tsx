import React, { useState } from 'react';
import { Control, UseFormSetValue, UseFormTrigger, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Grid from '@mui/material/Grid';
import { makeStyles } from '@mui/styles';
import { Button, Expander, ExpanderContent, ExpanderTitleButton, Paragraph } from 'suomifi-ui-components';
import { valueOrDefault } from 'utils';
import { GeneralDescriptionType, Language, ServiceType } from 'types/enumTypes';
import { ServiceFormValues, ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { GeneralDescriptionModel } from 'types/generalDescriptionTypes';
import Paginator from 'components/Paginator';
import { GdSearchItem, useSearchGeneralDescriptions } from 'hooks/queries/useSearchGeneralDescriptions';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { getEnabledLanguages } from 'utils/service';
import AreaTypeSelector from './AreaTypeSelector';
import { Footer } from './Footer';
import SearchBox from './SearchBox';
import SearchCount from './SearchCount';
import SearchResults from './SearchResults';
import SelectedGd from './SelectedGd';
import ServiceTypeSelector from './ServiceTypeSelector';
import * as utils from './utils';
import { SearchParameterState, selectGeneralDescription, unselectGeneralDescription } from './utils';

const useStyles = makeStyles(() => ({
  description: {
    marginBottom: '17px',
  },
  item: {
    marginBottom: '8px',
    maxWidth: '250px',
  },
  searchParameters: {
    paddingLeft: '20px',
    paddingRight: '20px',
    paddingTop: '13px',
    paddingBottom: '17px',
    backgroundColor: 'rgb(247, 247, 248)',
    borderTop: '1px solid rgb(200, 205, 208)',
    borderLeft: '1px solid rgb(200, 205, 208)',
    borderRight: '1px solid rgb(200, 205, 208)',
  },
  searchFunctions: {
    paddingBottom: '18px',
    paddingLeft: '20px',
    backgroundColor: 'rgb(247, 247, 248)',
    borderLeft: '1px solid rgb(200, 205, 208)',
    borderRight: '1px solid rgb(200, 205, 208)',
  },
  searchButton: {
    marginRight: '15px',
  },
  searchCount: {
    paddingLeft: '20px',
    paddingTop: '10px',
    paddingBottom: '10px',
    backgroundColor: 'rgb(247, 247, 248)',
    border: '1px solid rgb(200, 205, 208)',
  },
  selectedGd: {
    marginTop: '20px',
  },
  paginator: {
    marginTop: '20px',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
  },
  footer: {
    marginTop: '20px',
  },
}));

export type GeneralDescriptionSelectorProps = {
  selectedGeneralDescription: GeneralDescriptionModel | null | undefined;
  tabLanguage: Language;
  control: Control<ServiceModel>;
  setValue: UseFormSetValue<ServiceModel>;
  getFormValues: () => ServiceFormValues;
  trigger: UseFormTrigger<ServiceModel>;
};

export function GeneralDescriptionSelector(props: GeneralDescriptionSelectorProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();
  const lang = useGetUiLanguage();

  const serviceType = useWatch({ control: props.control, name: `${cService.serviceType}` });

  const [searchParameters, setSearchParameters] = useState<SearchParameterState>({
    enabled: false,
    searchText: '',
    serviceType: undefined,
    areaType: undefined,
    result: undefined,
    pageNumber: 0,
  });

  const [selectedSearchItem, setSelectedSearchItem] = useState<GdSearchItem | null | undefined>(undefined);
  const query = useSearchGeneralDescriptions(utils.getSearchParameters(searchParameters, lang), {
    enabled: searchParameters.enabled,
  });

  function onServiceTypeChange(newValue: ServiceType | undefined) {
    setSearchParameters({ ...searchParameters, serviceType: newValue });
  }

  function onAreaTypeChange(newValue: GeneralDescriptionType | undefined) {
    setSearchParameters({ ...searchParameters, areaType: newValue });
  }

  function onResetSearchState() {
    setSearchParameters({
      ...searchParameters,
      searchText: '',
      serviceType: undefined,
      areaType: undefined,
      pageNumber: 0,
    });
  }

  function onSearchTextChange(value: string) {
    setSearchParameters({ ...searchParameters, searchText: value });
  }

  function onSearch(pageNumber = 0) {
    setSearchParameters({
      ...searchParameters,
      enabled: true,
      pageNumber: pageNumber,
    });
  }

  function setSelectedGeneralDescription(gd: GeneralDescriptionModel, useGdName: boolean) {
    const formModel = props.getFormValues();
    const languages = getEnabledLanguages(formModel.languageVersions);

    selectGeneralDescription({
      gd: gd,
      useGdName: useGdName,
      enabledLanguages: languages,
      serviceLifeEvents: formModel.lifeEvents,
      serviceType: serviceType,
      serviceClassIds: formModel.serviceClasses,
      ontologyTerms: formModel.ontologyTerms,
      setValue: props.setValue,
      trigger: props.trigger,
    });
  }

  function onRemoveSelectedGd() {
    const formModel = props.getFormValues();
    const languages = getEnabledLanguages(formModel.languageVersions);

    unselectGeneralDescription({
      enabledLanguages: languages,
      setValue: props.setValue,
      trigger: props.trigger,
    });
  }

  function onSelectSearchItem(item: GdSearchItem) {
    setSelectedSearchItem(item);
  }

  function onClearSelectedSearchItem() {
    setSelectedSearchItem(undefined);
  }

  function goToPage(page: number) {
    // Pagination works with pages 1...n but api with pages from 0...n
    onSearch(page - 1);
  }

  if (query.data && !query.error && searchParameters.enabled) {
    // Store the result in local state. Otherwise when we disabled the query,
    // react-query sets query state to idle and data is gone.
    setSearchParameters({
      ...searchParameters,
      enabled: false,
      result: query.data,
    });
  }

  const pageCount = utils.getNumberOfPages(searchParameters.result);
  const currentPage = utils.getCurrentPageNumber(searchParameters.result);
  const count = valueOrDefault(searchParameters.result?.count, 0);
  const items = valueOrDefault(searchParameters.result?.items, []);

  return (
    <div>
      <Expander id='generalDescriptionExpander'>
        <ExpanderTitleButton>{t('Ptv.Service.Form.GdSearch.Title')}</ExpanderTitleButton>
        <ExpanderContent>
          <div>
            <div className={classes.description}>
              <Paragraph>{t('Ptv.Service.Form.GdSearch.Description')}</Paragraph>
            </div>
            <Grid container className={classes.searchParameters}>
              <Grid item sm={12} md={12} lg={4}>
                <div className={classes.item}>
                  <SearchBox value={searchParameters.searchText} onChange={onSearchTextChange} />
                </div>
              </Grid>
              <Grid item sm={12} md={12} lg={4}>
                <div className={classes.item}>
                  <ServiceTypeSelector serviceType={searchParameters.serviceType} onChange={onServiceTypeChange} />
                </div>
              </Grid>
              <Grid item sm={12} md={12} lg={4}>
                <div className={classes.item}>
                  <AreaTypeSelector areaType={searchParameters.areaType} onChange={onAreaTypeChange} />
                </div>
              </Grid>
            </Grid>
            <Grid className={classes.searchFunctions} container>
              <Grid item className={classes.searchButton}>
                <Button onClick={() => onSearch()} key='search'>
                  {t('Ptv.Service.Form.GdSearch.Search.Button.Label')}
                </Button>
              </Grid>
              <Grid item>
                <Button onClick={onResetSearchState} icon='remove' key='remove' variant='secondaryNoBorder'>
                  {t('Ptv.Service.Form.GdSearch.Reset.Button.Label')}
                </Button>
              </Grid>
            </Grid>
            <Grid container direction='column'>
              <Grid item className={classes.searchCount}>
                <SearchCount count={count} />
              </Grid>
              <Grid item>
                <SearchResults isLoading={query.isLoading} items={items} selected={selectedSearchItem} onSelect={onSelectSearchItem} />
              </Grid>
            </Grid>
          </div>
          <div className={classes.paginator}>
            <Paginator currentPage={currentPage} pageCount={pageCount} onChange={goToPage} />
          </div>
          <div className={classes.footer}>
            <Footer
              clearSelection={onClearSelectedSearchItem}
              select={setSelectedGeneralDescription}
              selectedItem={selectedSearchItem}
              getFormValues={props.getFormValues}
            />
          </div>
        </ExpanderContent>
      </Expander>
      <div className={classes.selectedGd}>
        <SelectedGd tabLanguage={props.tabLanguage} remove={onRemoveSelectedGd} gd={props.selectedGeneralDescription} />
      </div>
    </div>
  );
}
