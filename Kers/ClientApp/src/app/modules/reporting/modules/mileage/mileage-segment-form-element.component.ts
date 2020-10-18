import { Component, Input, forwardRef, OnInit, Output, EventEmitter } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormBuilder, FormGroup, Validators, NG_VALIDATORS, AbstractControl, ValidationErrors } from '@angular/forms';
import { BaseControlValueAccessor } from '../../core/BaseControlValueAccessor';
import { MileageSegment } from './mileage';



@Component({
  selector: 'mileage-segment',
  template: `
<div class="form-group" [formGroup]="sectionGroup">
    <div class="row">
        <div class="form-group">
            <label class="control-label col-md-3 col-sm-3 col-xs-12" for="comment">Business Purpose:</label>
            <div class="col-md-9 col-sm-9 col-xs-12">
                <input type="text" name="businessPurpose" id="businessPurpose" formControlName="businessPurpose" class="form-control">
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-3 col-sm-3 col-xs-12" for="comment">Mileage:</label>
            <div class="col-md-9 col-sm-9 col-xs-12">
                    <input type="text" name="mileage" id="mileage" formControlName="mileage" class="form-control">
        </div>
    </div>
    <div class="col-sm-2">
            <div class="col-xs-1 ng-star-inserted"><span><a class="close-link" (click)="onRemove()" style="position:relative; cursor:pointer; top: -18px;"><i class="fa fa-close"></i></a></span></div>
            <br>
        <small>&nbsp;</small>
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
    @Input('index') index:number;
    @Output() removeMe = new EventEmitter<number>();
    
    constructor( 
      private formBuilder: FormBuilder
    )   
    {
      super();
      this.sectionGroup = formBuilder.group({
        locationId: '',
        programCategoryId:'',
        businessPurpose: ['', Validators.required],
        mileage:['', Validators.required]
      });
  
      this.sectionGroup.valueChanges.subscribe(val => {
        this.value = <MileageSegment>val;
        console.log( this.value );
        this.onChange(this.value);
      });
    }
    onRemove(){
        this.removeMe.emit(this.value.order);
    } 
    ngOnInit(){
    }
    writeValue(session: MileageSegment) {
      this.value = session;
      this.sectionGroup.patchValue(this.value);
    }


    validate(c: AbstractControl): ValidationErrors | null{
      return this.sectionGroup.valid ? null : { invalidForm: {valid: false, message: "Mileage segment fields are invalid"}};
    }


}