import { Component } from '@angular/core';


@Component({
  template: `
  <div class="text-right"><a class="btn btn-default btn-xs" routerLink="/reporting/state">State Admin Dashboard</a></div>
    
  <county-list></county-list>
    
  `
})
export class NotCountiesListComponent { 

    errorMessage:string;

    constructor( 

    )   
    {}

    ngOnInit(){
        
    }

}