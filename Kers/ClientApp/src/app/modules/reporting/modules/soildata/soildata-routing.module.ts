import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SoildataHomeComponent } from './soildata-home.component';
import { SoildataFarmerAddressComponent } from './addresses/soildata-farmer-address.component';
import { SoildataNotesComponent } from './notes/soildata-notes.component';


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
       }
        
        
        
    ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SoildataRoutingModule { }
