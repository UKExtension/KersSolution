import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SoildataHomeComponent } from './soildata-home.component';
import { SoildataFarmerAddressComponent } from './addresses/soildata-farmer-address.component';
import { SoildataNotesComponent } from './notes/soildata-notes.component';
import { SoildataSigneesComponent } from './signees/soildata-signees.component';
import { SoildataReportsComponent } from './reports/soildata-reports.component';
import { SoildataReportsCatalogComponent } from './reports/soildata-reports-catalog.component';
import { SampleFormComponent } from './sample/sample-form.component';


const routes: Routes = [{
  path: '',
  component: SoildataHomeComponent,
  children: 
    [
       {
         path: 'addresses',
         component: SoildataFarmerAddressComponent
       },
       {
         path: 'notes',
         component: SoildataNotesComponent
       },
       {
         path: 'signees',
         component: SoildataSigneesComponent
       },
       {
         path: 'reports',
         component: SoildataReportsCatalogComponent
       }
       ,
       {
         path: 'sample',
         component: SampleFormComponent
       }
        
        
        
    ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SoildataRoutingModule { }
