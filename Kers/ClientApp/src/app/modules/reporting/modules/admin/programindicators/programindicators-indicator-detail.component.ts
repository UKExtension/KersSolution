import {Component, Input, Output, EventEmitter} from '@angular/core';
import {IndicatorsService, Indicator} from '../../indicators/indicators.service';



@Component({
    selector: 'programindicators-indicator-detail',
    template: `
<div class="row" style="padding-bottom: 20px;">
    <div class="col-xs-9"><span *ngIf="rowDefault" [innerHTML]="indicator.question"></span>
        <div class="col-xs-12" *ngIf="rowEdit">
            <programindicators-form-admin [indicator]="indicator" (onFormCancel)="default()" (onFormSubmit)="editSubmit($event)"></programindicators-form-admin>
            <div class="ln_solid"></div>
        </div>
        <div class="col-xs-11" *ngIf="rowDelete">
            Do you really want to delete indicator:<br> <small>{{indicator.question}}</small>?<br><button (click)="confirmDelete()" class="btn btn-info btn-xs">Yes</button> <button (click)="default()" class="btn btn-info btn-xs">No</button>
        </div>
    </div>
    <div class="col-xs-3 text-right">
        <a class="btn btn-info btn-xs" (click)="edit()" *ngIf="rowDefault">edit</a>
        <a class="btn btn-info btn-xs" (click)="delete()" *ngIf="rowDefault">delete</a>
        <a class="btn btn-info btn-xs" (click)="default()" *ngIf="!rowDefault">close</a>
    </div>
</div>
    `
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