import { Component, Input, forwardRef, OnInit } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormBuilder } from '@angular/forms';
import { ServicelogService, SnapIndirectMethod, SnapPolicyAimed } from "../servicelog.service";
import { Observable } from "rxjs/Observable";

@Component({
  selector: 'snap-policy-aimed',
  template: `
  <loading *ngIf="loading"></loading>
  <table class="table table-striped table-bordered" *ngIf="!loading" [formGroup]="aimedForm">
      <tbody formArrayName="aimedSelections">
          <tr *ngFor="let aim of aimed; let i=index" [formGroupName]="i">
              <td>{{aim.name}}</td>
              <td><input type="checkbox" formControlName="selected" (change)="changed($event)"></td>
          </tr>
      </tbody>
  </table>

  `,
    providers:[  { 
                    provide: NG_VALUE_ACCESSOR,
                    useExisting: forwardRef(() => SnapPolicyAimedComponent),
                    multi: true
                  } 
                ]
})
export class SnapPolicyAimedComponent implements ControlValueAccessor, OnInit {



  aimedForm = null;
  loading = true;
  aimed:SnapPolicyAimed[] = [];
  errorMessage;

  @Input() _value = [];

  get selections() {
      return this._value;
  }
  set selections(val) {
      this._value = val;     
      this.propagateChange(val);
  }


  public propagateChange: any;
  

  constructor(
      private fb: FormBuilder,
      private service: ServicelogService,
  ){
    
    this.propagateChange = () => {};
  }

  ngOnInit(){
    let aimedArray = [];
    this.service.snappolicyaimed().subscribe(
      res => {
        this.aimed = <SnapPolicyAimed[]> res;
        for(let aim of this.aimed){
          aimedArray.push(this.fb.group({
            snapPolicyAimedId: aim.id,
            selected: false
          }))
        }
        this.aimedForm = this.fb.group({
          aimedSelections: this.fb.array(aimedArray)
        });
        this.patch();
        this.loading = false;
      },
      err => this.errorMessage = <any> err
    )
  }

  writeValue(value: any) {
      if (value !== []) {
        this.selections = value;
        if(this.aimedForm != null){
          this.patch();
        }
      }
      
  }

  patch(){
    var toPatch = [];
    for( let sel of this.aimedForm.value.aimedSelections){
      
      var isInSelection = this.selections.filter(s => s.snapPolicyAimedId == sel.snapPolicyAimedId);
      if(isInSelection.length > 0){
        toPatch.push({
          snapPolicyAimedId: sel.snapPolicyAimedId,
          selected: true
        })
      }else{
        toPatch.push({
          snapPolicyAimedId: sel.snapPolicyAimedId,
          selected: false
        })
      }
    }
    this.aimedForm.patchValue({aimedSelections:toPatch});
  }


  registerOnChange(fn) {
    this.propagateChange = fn;
  }

  registerOnTouched() {}


  changed(event){
    this.selections = [];
    for( let sel of this.aimedForm.value.aimedSelections){
      if(sel.selected){
        this.selections.push(sel);
      }
    }
    
  }
  
}

