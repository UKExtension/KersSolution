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




@NgModule({
  declarations: [
  SoildataHomeComponent,
  SoildataFarmerAddressComponent,
  SoildataNotesComponent,
  SoildataFarmerAddressFormComponent,
  SoildataFarmerAddressDetailComponent,
  SoildataNotesDetailComponent,
  SoildataNotesFormComponent],
  imports: [
    SharedModule,
    CommonModule,
    SoildataRoutingModule
  ]
})
export class SoildataModule { }
