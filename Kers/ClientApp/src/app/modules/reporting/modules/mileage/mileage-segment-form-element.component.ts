import { Component, Input, forwardRef, OnInit, Output, EventEmitter } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormBuilder, FormGroup, Validators, NG_VALIDATORS, AbstractControl, ValidationErrors } from '@angular/forms';
import { BaseControlValueAccessor } from '../../core/BaseControlValueAccessor';
import { ProgramCategory } from '../admin/programs/programs.service';
import { ExtensionEventLocation } from '../events/extension-event';
import { ExpenseFundingSource } from '../expense/expense.service';
import { User } from '../user/user.service';
import { MileageSegment } from './mileage';



@Component({
  selector: 'mileage-segment',
  template: `
<div class="form-group" [formGroup]="sectionGroup">
  
    <div class="col-xs-12 ng-star-inserted text-right"><span><a class="close-link" (click)="onRemove()" style="position:relative; cursor:pointer;"><i class="fa fa-close"></i></a></span></div>

        <div class="row">
            <div class="form-group">
            <label [ngClass]="(locationBrowser)?'col-xs-12':'control-label col-md-3 col-sm-3 col-xs-12'" [ngStyle]="{'margin-left': (locationBrowser)?'10px;':'0px'}">Location:</label>
            
                <div class="col-xs-12" *ngIf="locationBrowser" style="padding: 10px;">
                    <location-browser *ngIf="currentUser" [purpose]="'Mileage'" [user]="currentUser" [includeCountyOffice]="true" (onSelected)="locationSelected($event)"></location-browser>
                </div>
                <div  class="col-md-9 col-sm-9 col-xs-12" *ngIf="!locationBrowser" style="padding: 10px;">
                    <h4>{{loc.displayName}}</h4>
                    <h5>{{loc.address.building}}</h5>
                    <h5>{{loc.address.street}}</h5>
                    <h5>{{loc.address.city}}{{loc.address.state != ""?", "+loc.address.state:""}}</h5>
                    <a (click)="locationBrowser = true" class="btn btn-info btn-xs">change</a>
                </div>
                
            
            
        </div>
        <div class="form-group">
            <label class="control-label col-md-3 col-sm-3 col-xs-12" for="programCategory">Program Category:</label>
            <div class="col-md-6 col-sm-6 col-xs-12">
                <select name="programCategoryId" formControlName="programCategoryId" class="form-control col-md-7 col-xs-12">
                    <option value="">--- select ---</option>
                    <option *ngFor="let category of programCategories" [value]="category.id">{{category.name}}</option>
                </select>
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-3 col-sm-3 col-xs-12" for="comment">Business Purpose:</label>
            <div class="col-md-9 col-sm-9 col-xs-12">
                <input type="text" name="businessPurpose" id="businessPurpose" formControlName="businessPurpose" class="form-control">
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-3 col-sm-3 col-xs-12" for="fundingSourceMileageId">Mileage Funding Source: </label>
            <div class="col-md-6 col-sm-6 col-xs-12">
                <select name="fundingSourceId" id="fundingSourceId" formControlName="fundingSourceId" class="form-control col-md-7 col-xs-12" >
                    <option value="">--- select ---</option>
                    <option *ngFor="let source of fundingSources" [value]="source.id">{{source.name}}</option>
                </select>
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-3 col-sm-3 col-xs-12" for="comment">Mileage:</label>
            <div class="col-md-5 col-sm-5 col-xs-10">
                    <input type="number" name="mileage" id="mileage" formControlName="mileage" class="form-control">
        </div>
    </div>
  </div>
</div>
  `,
  providers:[  { 
                  provide: NG_VALUE_ACCESSOR,
                  useExisting: forwardRef(() => MileageSegmentFormElementComponent),
                  multi: true
                } ,
                {
                  provide: NG_VALIDATORS,
                  useExisting: forwardRef(() => MileageSegmentFormElementComponent),
                  multi: true
                }
                ]
})
export class MileageSegmentFormElementComponent extends BaseControlValueAccessor<MileageSegment> implements ControlValueAccessor, OnInit { 
    sectionGroup: FormGroup;
    @Input() programCategories: ProgramCategory[];
    @Input() fundingSources:ExpenseFundingSource[];
    @Input() currentUser:User;
    @Input() index:number;

    @Output() removeMe = new EventEmitter<number>();

    locationBrowser:boolean = true;
  

    get loc(){
      return this.sectionGroup.get('location').value as ExtensionEventLocation;
    }
    set loc(lc: ExtensionEventLocation){
      this.sectionGroup.patchValue({location:lc});
    }
    
    constructor( 
      private formBuilder: FormBuilder
    )   
    {
      super();
      this.sectionGroup = formBuilder.group({
        location: [null, Validators.required],
        programCategoryId:['', Validators.required],
        fundingSourceId: ['', Validators.required],
        businessPurpose: ['', Validators.required],
        mileage:['', Validators.required]
      });
  
      this.sectionGroup.valueChanges.subscribe(val => {
        this.value = <MileageSegment>val;
        this.onChange(this.value);
      });
    }
    onRemove(){
        this.removeMe.emit(this.index);
    } 
    ngOnInit(){
    }
    writeValue(session: MileageSegment) {
      this.value = session;
      this.sectionGroup.patchValue(this.value);
      if(session.location != null) this.locationBrowser = false;
    }
    locationSelected(event){
      this.loc = event.extensionEventLocation;
      this.locationBrowser = false;
    }

    validate(c: AbstractControl): ValidationErrors | null{
      return this.sectionGroup.valid ? null : { invalidForm: {valid: false, message: "Mileage segment fields are invalid"}};
    }
    //[class.ng-invalid]="expenseForm.hasError('noMileageSource')"


}