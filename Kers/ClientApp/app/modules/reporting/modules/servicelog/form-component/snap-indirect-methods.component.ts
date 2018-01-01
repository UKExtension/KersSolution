import { Component, Input, forwardRef, OnInit } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormBuilder } from '@angular/forms';
import { ServicelogService, SnapIndirectMethod } from "../servicelog.service";
import { Observable } from "rxjs/Observable";

@Component({
  selector: 'snap-indirect-methods',
  template: `
  <loading *ngIf="loading"></loading>
  <table class="table table-striped table-bordered" *ngIf="!loading" [formGroup]="methodForm">
      <tbody formArrayName="methodSelections">
          <tr *ngFor="let method of methods; let i=index" [formGroupName]="i">
              <td>{{method.name}}</td>
              <td><input type="checkbox" formControlName="selected" (change)="changed($event)"></td>
              
          </tr>
      </tbody>
  </table>

  `,
    providers:[  { 
                    provide: NG_VALUE_ACCESSOR,
                    useExisting: forwardRef(() => SnapIndirectMethodsComponent),
                    multi: true
                  } 
                ]
})
export class SnapIndirectMethodsComponent implements ControlValueAccessor, OnInit {

  
  methodForm = null;
  loading = true;
  methods:SnapIndirectMethod[] = [];
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
  snapindirectmethod:Observable<SnapIndirectMethod[]>;
  

  constructor(
      private fb: FormBuilder,
      private service: ServicelogService,
  ){
    
    this.propagateChange = () => {};
    this.snapindirectmethod = this.service.snapindirectmethod().share();
  }

  ngOnInit(){
    let methodArray = [];
    this.service.snapindirectmethod().subscribe(
      res => {
        this.methods = <SnapIndirectMethod[]> res;
        for(let met of this.methods){
          methodArray.push(this.fb.group({
            snapIndirectMethodId: met.id,
            selected: false
          }))
        }
        this.methodForm = this.fb.group({
          methodSelections: this.fb.array(methodArray)
        });
        this.patch();
        this.loading = false;
      },
      err => this.errorMessage = <any> err
    );
      
  }

  patch(){
    var toPatch = [];
    for( let sel of this.methodForm.value.methodSelections){
      
      var isInSelection = this.selections.filter(s => s.snapIndirectMethodId == sel.snapIndirectMethodId);
      if(isInSelection.length > 0){
        toPatch.push({
          snapIndirectMethodId: sel.snapIndirectMethodId,
          selected: true
        })
      }else{
        toPatch.push({
          snapIndirectMethodId: sel.snapIndirectMethodId,
          selected: false
        })
      }
    }
    this.methodForm.patchValue({methodSelections:toPatch});
  }

  writeValue(value: any) {
    if (value !== []) {
      this.selections = value;
      if(this.methodForm != null){
        this.patch();
      }
    }
  }


  registerOnChange(fn) {
    this.propagateChange = fn;
  }

  registerOnTouched() {}


  changed(event){
    this.selections = [];
    for( let sel of this.methodForm.value.methodSelections){
      if(sel.selected){
        this.selections.push(sel);
      }
    }
  }
  
}

