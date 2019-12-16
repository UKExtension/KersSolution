import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedModule } from '../../shared/shared.module';
import { SoildataRoutingModule } from './soildata-routing.module';
import { SoildataHomeComponent } from './soildata-home.component';
import { SoildataFarmerAddressComponent } from './addresses/soildata-farmer-address.component';
import { SoildataNotesComponent } from './notes/soildata-notes.component';
import { SoildataFarmerAddressFormComponent } from './addresses/soildata-farmer-address-form.component';
import { SoildataFarmerAddressDetailComponent } from './addresses/soildata-farmer-address-detail.component';
import { SoildataNotesDetailComponent } from './notes/soildata-notes-detail.component';
import { SoildataNotesFormComponent } from './notes/soildata-notes-form.component';
import { SoildataSigneesComponent } from './signees/soildata-signees.component';
import { SoildataReportsComponent } from './reports/soildata-reports.component';
import { SoildataReportsCatalogComponent } from './reports/soildata-reports-catalog.component';
import { MyDatePickerModule } from 'mydatepicker';
import { MyDateRangePickerModule } from 'mydaterangepicker';
import { SoildataReportsCatalogDetailsComponent } from './reports/soildata-reports-catalog-details.component';
import { SoildataReportFormComponent } from './reports/soildata-report-form.component';




@NgModule({
  declarations: [
  SoildataHomeComponent,
  SoildataFarmerAddressComponent,
  SoildataNotesComponent,
  SoildataFarmerAddressFormComponent,
  SoildataFarmerAddressDetailComponent,
  SoildataNotesDetailComponent,
  SoildataNotesFormComponent,
  SoildataSigneesComponent,
  SoildataReportsComponent,
  SoildataReportsCatalogComponent,
  SoildataReportsCatalogDetailsComponent,
  SoildataReportFormComponent],
  imports: [
    SharedModule,
    MyDatePickerModule,
    MyDateRangePickerModule,
    CommonModule,
    SoildataRoutingModule
  ]
})
export class SoildataModule { }
