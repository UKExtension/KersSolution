import { Component, Input, OnInit, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { UserService, User, SocialConnection, SocialConnectionType } from "../user.service";
import { UntypedFormGroup } from "@angular/forms";


@Component({
    selector: 'social-connection',
    template: `
<div class="row">
  <div class="col-xs-11">
    <div class="input-group" [formGroup]="connectionForm">


        <social-picker [types]="connectionTypes" class="input-group-btn" formControlName="socialConnectionTypeId"></social-picker>


        <input type="text" class="form-control" formControlName="identifier" placeholder="Your Identifier">
     </div>
  </div>
  <div class="col-xs-1" *ngIf="canDelete" style="padding: 6px 0;">  
     <span><a class="close-link" (click)="onRemove()"><i class="fa fa-close"></i></a></span>
  </div>
</div>

  `,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class UserPersonalConnectionComponent implements OnInit { 

    @Input('group') 
        public connectionForm:UntypedFormGroup;
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