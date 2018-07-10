import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { PlansofworkService, Map } from './plansofwork.service';
import { FormBuilder, Validators }   from '@angular/forms';
import { FiscalYear } from '../admin/fiscalyear/fiscalyear.service';

@Component({
    selector: 'plansofwork-map-form',
    template: `
    <loading *ngIf="loading"></loading>
    <div *ngIf="!loading">
        <div class="row">
            <div class="col-sm-offset-3 col-sm-9">
                <h2 *ngIf="!map">&nbsp;&nbsp;New MAP</h2>
            </div>
        </div>
        <form class="form-horizontal form-label-left col-sm-12" novalidate (ngSubmit)="onSubmit()" [formGroup]="mapForm">
            <div class="form-group">
                <label class="control-label col-md-3 col-sm-3 col-xs-12" for="title">Title: </label>
                <div class="col-md-6 col-sm-6 col-xs-12">
                    <input type="text" name="title" id="title" formControlName="title" class="form-control col-md-7 col-xs-12" >
                </div>
            </div>
            <div class="col-md-6 col-sm-6 col-xs-12 col-md-offset-3">
                <a class="btn btn-primary" (click)="OnCancel()">Cancel</a>
                <button type="submit" [disabled]="mapForm.invalid"  class="btn btn-success">Submit</button>
            </div>
        </form>
    </div>
    ` 
})
export class PlansofworkMapFormComponent implements OnInit{

    mapForm = null;
    @Input() map = null;
    @Input() fiscalYear:FiscalYear;
    errorMessage: string;
    loading = false;

    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<Map>();

    constructor( 
        private plansofworkService: PlansofworkService,
        private fb: FormBuilder
    ){


        this.mapForm = fb.group(
            {
              title: ['', Validators.required]
            }
        );
    }
   
    ngOnInit(){
       if(this.map){
           this.mapForm.patchValue(this.map);
       }
       

    }

    onSubmit(){ 
        this.loading = true;     
        if(this.map){
            this.plansofworkService.updateMap(this.map.id, this.mapForm.value).
            subscribe(
                res => {
                    this.map = <Map> res;
                    this.onFormSubmit.emit(res);
                    this.loading = false;
                    //console.log(res);
                }
            );
        }else{
            let m:Map = <Map> this.mapForm.value;
            m.fiscalYearId = this.fiscalYear.id;
            this.plansofworkService.addMap(this.mapForm.value)
                .subscribe(
                    res => {
                        this.onFormSubmit.emit(res);
                        this.loading = false;
                        //console.log(res.json());
                    }
                );
        }

    }

    OnCancel(){
        this.onFormCancel.emit();
    }   
}