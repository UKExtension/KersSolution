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
import { AngularMyDatePickerModule } from 'angular-mydatepicker';
import { SoildataReportsCatalogDetailsComponent } from './reports/soildata-reports-catalog-details.component';
import { SoildataReportFormComponent } from './reports/soildata-report-form.component';
import { SoildataReportCropComponent } from './reports/soildata-report-crop.component';
import { SoildataReportLabResulsComponent } from './reports/soildata-report-lab-resuls.component';
import { SoildataAddressBrowserComponent } from './addresses/soildata-address-browser.component';
import { SampleFormComponent } from './sample/sample-form.component';
import { SoilCropFormElementComponent } from './sample/soil-crop-form-element.component';
import { SoildataListAddressComponent } from './addresses/soildata-list-address.component';
import { SoilCropattributeFormElementComponent } from './sample/soil-cropattribute-form-element.component';
import { AddressBrowserFormElementComponent } from './sample/address-browser-form-element.component';
import { NgSelectModule } from '@ng-select/ng-select';
import { FormsModule } from '@angular/forms';



@NgModule({
  imports: [
    SharedModule,
    AngularMyDatePickerModule,
    CommonModule,
    SoildataRoutingModule,
    NgSelectModule,
    FormsModule
  ],
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
    SoildataReportFormComponent,
    SoildataReportCropComponent,
    SoildataReportLabResulsComponent,
    SoildataAddressBrowserComponent,
    SampleFormComponent,
    SoilCropFormElementComponent,
    SoilCropattributeFormElementComponent,
    AddressBrowserFormElementComponent,
    SoildataListAddressComponent]
})
export class SoildataModule { }
