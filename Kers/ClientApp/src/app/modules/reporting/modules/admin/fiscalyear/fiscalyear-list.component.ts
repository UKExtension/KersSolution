import {Component, OnInit, ChangeDetectionStrategy} from '@angular/core';
import {Router} from '@angular/router';
import { Observable } from 'rxjs';
import {FiscalyearFormComponent} from './fiscalyear-form.component';
import {FiscalyearService, FiscalYear} from './fiscalyear.service';

@Component({
    template: `
<div>
  <div class="text-right">
    @if (!newYear) {
      <a class="btn btn-info btn-xs" (click)="newFiscalYearOpen()">+ new fiscal year</a>
    }
  </div>
  @if (newYear) {
    <fiscalyear-form (onFormCancel)="newFiscalYearCancelled()" (onFormSubmit)="newFiscalYearSubmitted()"></fiscalyear-form>
  }
</div>
@if (fiscalyears) {
  <div>
    <table class="table table-striped">
      <thead>
        <tr>
          <th>Name</th>
          <th>Type</th>
          <th></th>
        </tr>
      </thead>
      <tbody>
        @for (fiscalyear of fiscalyears; track fiscalyear) {
          <tr [fiscalyearListDetail]="fiscalyear" (onFiscalyearUpdated)="onFiscalYearUpdate()" (onFisclyearDeleted)="onFiscalYearUpdate()"></tr>
        }
      </tbody>
    </table>
  </div>
}

`,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class FiscalyearListComponent implements OnInit{


    errorMessage: string;
    newYear = false;

    fiscalyears: FiscalYear[];

    constructor(
        private router: Router,
        private service: FiscalyearService
    ){}

    ngOnInit(){
        this.getList();
    }

    getList(){
        this.service.listFiscalYears(true).subscribe(
            fiscal => {
                this.fiscalyears = null;
                this.fiscalyears = fiscal;
            },
            error =>  this.errorMessage = <any>error
        );
    }

    onFiscalYearUpdate(){
        this.getList();
    }

    newFiscalYearOpen(){
        this.newYear = true;
    }

    newFiscalYearCancelled(){
        this.newYear=false;
    }
    newFiscalYearSubmitted(){
        this.newYear=false;
        this.getList();
    }
}
