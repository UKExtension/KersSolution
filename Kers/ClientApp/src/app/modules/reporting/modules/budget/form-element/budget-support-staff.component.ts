import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { UserService, User, SocialConnection, SocialConnectionType } from "../../user/user.service";
import { FormGroup } from "@angular/forms";


@Component({
  selector: 'budget-support-staff',
  template: `
<div class="row">
  <div class="col-xs-11">
    <div class="input-group" [formGroup]="connectionForm">
        <input type="text" class="form-control" formControlName="identifier" placeholder="Your Identifier">
     </div>
  </div>
  <div class="col-xs-1" *ngIf="canDelete" style="padding: 6px 0;">  
     <span><a class="close-link" (click)="onRemove()"><i class="fa fa-close"></i></a></span>
  </div>
</div>

  `
})
export class BudgetSupportStaffComponent implements OnInit { 

    @Input('group') public connectionForm:FormGroup;
    @Input('canDelete') canDelete:boolean;
    @Input('index') index:number;
    @Input('connectionTypes') connectionTypes:SocialConnectionType[];

    @Output() removeMe = new EventEmitter<number>();

    public selectedLabel = "Select Social Media ";



    constructor( 
        private userService: UserService
    )   
    {}


    selectedConnection(type){
        this.selectedLabel = type.name;
    }

    onRemove(){
        this.removeMe.emit(this.index);
    }

    ngOnInit(){
        
        
    }

}