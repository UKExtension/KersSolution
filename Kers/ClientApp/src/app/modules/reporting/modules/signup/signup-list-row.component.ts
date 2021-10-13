import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { ActivitySignUpEntry, SignupService } from './signup.service';

@Component({
  selector: '[signup-list-row]',
  template: `
  <td *ngIf="defaultView">{{attendie.name}}</td>
  <td *ngIf="defaultView">{{attendie.address}}</td>
  <td *ngIf="defaultView">{{attendie.email}}</td>
  <td class="text-right" *ngIf="defaultView">
  
    <a class="btn btn-info btn-xs" (click)="edit()" ><i class="fa fa-pencil"></i></a>
    <a class="btn btn-info btn-xs" (click)="delete()"><i class="fa fa-trash-o"></i></a>
  
  </td>
  <td colspan="4" *ngIf="editView">
    
      <signup-form [dalayConfirm]="false" [entry]="attendie" (Submit)="edited($event);" (Cancel)="canceled();"></signup-form>
  </td>
  <td colspan="4" *ngIf="deleteView">
    <div class="text-right">
      <a class="btn btn-primary btn-xs" (click)="default()" *ngIf="!rowDefault"><i class="fa fa-close"></i> Close</a>
    </div>
    <div *ngIf="!loading">
      Are you sure you want to delete atendie {{attendie.name}}?<br>
      <a (click)="confirmDelete()" style="cursor:pointer">Yes</a> | <a (click)="default()" style="cursor:pointer">No</a><br><br>
    </div>
    <loading *ngIf="loading"></loading>
  </td>
  `,
  styles: [
  ]
})
export class SignupListRowComponent implements OnInit {
  @Input('signup-list-row') attendie:ActivitySignUpEntry = null;
  @Output() deleted = new EventEmitter<void>();
  defaultView = true;
  editView = false;
  deleteView = false;
  loading = false;

  constructor(
    private service:SignupService
  ) { }

  ngOnInit(): void {
  }
  default(){
    this.defaultView = true;
    this.editView = false;
    this.deleteView = false;
  }

  edit(){
    this.defaultView = false;
    this.editView = true;
    this.deleteView = false;

  }
  delete(){
    this.defaultView = false;
    this.editView = false;
    this.deleteView = true;
  }
  edited(event:ActivitySignUpEntry){
    this.attendie = event;
    this.default();

  }
  canceled(){
    this.default();
  }
  confirmDelete(){
    this.loading = true;
    this.service.delete(this.attendie.id).subscribe(
      _ => {
        this.deleted.emit();
      }
    )
    
  }

}
