import { Component, ChangeDetectionStrategy } from '@angular/core';


@Component({
    template: `
  <div class="text-right"><a class="btn btn-default btn-xs" routerLink="/reporting/state">State Admin Dashboard</a></div>
    
  <county-list></county-list>
    
  `,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class NotCountiesListComponent { 

    errorMessage:string;

    constructor( 

    )   
    {}

    ngOnInit(){
        
    }

}