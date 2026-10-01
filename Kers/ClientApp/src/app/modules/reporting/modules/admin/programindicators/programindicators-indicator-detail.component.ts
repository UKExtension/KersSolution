import {Component, Input, Output, EventEmitter, ChangeDetectionStrategy} from '@angular/core';
import {IndicatorsService, Indicator} from '../../indicators/indicators.service';



@Component({
    selector: 'programindicators-indicator-detail',
    template: `
<div class="row" style="padding-bottom: 20px;">
  <div class="col-xs-9">@if (rowDefault) {
    <span [innerHTML]="indicator.question"></span>
  }
  @if (rowEdit) {
    <div class="col-xs-12">
      <programindicators-form-admin [indicator]="indicator" (onFormCancel)="default()" (onFormSubmit)="editSubmit($event)"></programindicators-form-admin>
      <div class="ln_solid"></div>
    </div>
  }
  @if (rowDelete) {
    <div class="col-xs-11">
      Do you really want to delete indicator:<br> <small>{{indicator.question}}</small>?<br><button (click)="confirmDelete()" class="btn btn-info btn-xs">Yes</button> <button (click)="default()" class="btn btn-info btn-xs">No</button>
    </div>
  }
</div>
<div class="col-xs-3 text-right">
  @if (rowDefault) {
    <a class="btn btn-info btn-xs" (click)="edit()">edit</a>
  }
  @if (rowDefault) {
    <a class="btn btn-info btn-xs" (click)="delete()">delete</a>
  }
  @if (!rowDefault) {
    <a class="btn btn-info btn-xs" (click)="default()">close</a>
  }
</div>
</div>
`,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class ProgramindicatorsIndicatorDetailComponent{
    
    @Input()indicator:Indicator; 

    rowDefault =true;
    rowEdit = false;
    rowDelete = false;

    @Output() onDeleted = new EventEmitter<Indicator>();


    constructor(
       private service: IndicatorsService
    ){}

    confirmDelete(){
        this.service.deleteIndicator(this.indicator.id).subscribe(
            res => {
                this.onDeleted.emit(this.indicator);
                return res;
            }
        );
    }

    editSubmit(indicator:Indicator){
        this.indicator=indicator;
        this.default();
    }

    edit(){
        this.rowDefault = false;
        this.rowEdit = true;
        this.rowDelete = false;
    }
    delete(){
        this.rowDefault = false;
        this.rowEdit = false;
        this.rowDelete = true;
    }
    default(){
        this.rowDefault = true;
        this.rowEdit = false;
        this.rowDelete = false;
    }
    
    

}