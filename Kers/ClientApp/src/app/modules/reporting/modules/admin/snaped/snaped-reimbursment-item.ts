import {    Component, Input, OnInit, EventEmitter, Output   } from '@angular/core';
import { SnapBudgetReimbursementsCounty, SnapedAdminService } from './snaped-admin.service';


@Component({
    selector: '[snapedReimbursmentItem]',
    template: `
    <td *ngIf="rowOppened">{{reimbursment.notes}}</td>
    <td *ngIf="rowOppened" class="text-right">{{reimbursment.amount | currency:'USD':'symbol'}}</td>
    <td class="text-right"  *ngIf="rowOppened">
        
        <button (click)="edit()" class="btn btn-info btn-xs">edit</button>
        <button (click)="delete()" class="btn btn-info btn-xs">delete</button>
        <span *ngIf="errorMessage">{{errorMessage}}</span>
    </td>


    <td *ngIf="editOppened" colspan="2">
        <snaped-reimbursment-form [reimbursment]="reimbursment" [countyId]="reimbursment.planningUnitId" (onFormCancel)="close()" (onFormSubmit)="editSubmit($event)"></snaped-reimbursment-form>
    </td>
    <td class="text-right"  *ngIf="editOppened">
        <button (click)="close()" class="btn btn-info btn-xs">close</button>
    </td>
    <td *ngIf="deleteOppened" colspan="2">Do you really want to delete County Reimbursement <strong>{{reimbursment.notes}}</strong>?<br><button (click)="confirmDelete()" class="btn btn-info btn-xs">Yes</button> <button (click)="close()" class="btn btn-info btn-xs">No</button></td>
    <td class="text-right"  *ngIf="deleteOppened">
        <button (click)="close()" class="btn btn-info btn-xs">close</button>
    </td>

    `
})
export class SnapedReimbursmentItem implements OnInit {

    @Input('snapedReimbursmentItem') reimbursment;

    @Output() onReimbursementUpdated = new EventEmitter<SnapBudgetReimbursementsCounty>();
    @Output() onReimbursementDeleted = new EventEmitter<SnapBudgetReimbursementsCounty>();

    editOppened = false;
    deleteOppened = false;
    programsOppened = false;
    rowOppened = true;
    errorMessage: string;


    constructor(
        private service:SnapedAdminService
    ){
        
    }

    ngOnInit(){
        
    }
    programs(){
        this.rowOppened = false;
        this.programsOppened = true;
    }
    edit(){
        this.rowOppened = false;
        this.editOppened = true;
    }
    delete(){
        this.rowOppened = false;
        this.deleteOppened = true;
    }
    confirmDelete(){
        if(this.reimbursment.toId == null){
            this.service.deleteCountyReimbursment(this.reimbursment.id).subscribe(
                res => {
                    this.onReimbursementDeleted.emit(this.reimbursment);
                },
                err => this.errorMessage = <any> err
            )
        }else{
            this.service.deleteAssistantReimbursment(this.reimbursment.id).subscribe(
                res => {
                    this.onReimbursementDeleted.emit(this.reimbursment);
                },
                err => this.errorMessage = <any> err
            )
        }
        
        
    }

    editSubmit(event:SnapBudgetReimbursementsCounty){
        this.reimbursment.amount = event.amount;
        this.reimbursment.notes = event.notes;
        this.close();
        this.onReimbursementUpdated.emit(this.reimbursment);
    }
    close(){
        this.rowOppened = true;
        this.editOppened = false;
        this.deleteOppened = false;
        this.programsOppened = false;
    }
    

}