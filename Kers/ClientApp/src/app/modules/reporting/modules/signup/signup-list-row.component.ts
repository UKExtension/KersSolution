import { Component, Input, OnInit, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { ActivitySignUpEntry, SignupService } from './signup.service';

@Component({
    selector: '[signup-list-row]',
    template: `
  @if (defaultView) {
    <td>{{attendie.name}}</td>
  }
  @if (defaultView) {
    <td>{{attendie.address}}</td>
  }
  @if (defaultView) {
    <td>{{attendie.email}}</td>
  }
  @if (defaultView) {
    <td class="text-right">
      <a class="btn btn-info btn-xs" (click)="edit()" ><i class="fa fa-pencil"></i></a>
      <a class="btn btn-info btn-xs" (click)="delete()"><i class="fa fa-trash-o"></i></a>
    </td>
  }
  @if (editView) {
    <td colspan="4">
      <signup-form [dalayConfirm]="false" [entry]="attendie" (Submit)="edited($event);" (Cancel)="canceled();"></signup-form>
    </td>
  }
  @if (deleteView) {
    <td colspan="4">
      <div class="text-right">
        @if (!rowDefault) {
          <a class="btn btn-primary btn-xs" (click)="default()"><i class="fa fa-close"></i> Close</a>
        }
      </div>
      @if (!loading) {
        <div>
          Are you sure you want to delete atendie {{attendie.name}}?<br>
          <a (click)="confirmDelete()" style="cursor:pointer">Yes</a> | <a (click)="default()" style="cursor:pointer">No</a><br><br>
        </div>
      }
      @if (loading) {
        <loading></loading>
      }
    </td>
  }
  `,
    styles: [],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
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
