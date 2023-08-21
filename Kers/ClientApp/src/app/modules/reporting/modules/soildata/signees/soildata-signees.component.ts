import { Component, OnInit } from '@angular/core';
import { SoildataService } from '../soildata.service';
import { Observable } from 'rxjs';
import { FormTypeSignees } from '../soildata.report';
import { FormArray, FormGroup, FormBuilder } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ReportingService } from '../../../components/reporting/reporting.service';

@Component({
  selector: 'soildata-signees',
  template: `<br>
  <h3>Agent Information</h3><br>
  <p>The signature that appears on reports will come from the KERS personal profile of the person completing the review step.<br>If the review step is skipped, the information on this page will be used.</p>
  <loading *ngIf="loading"></loading>
  <div class="row" *ngIf="!loading">
      <form novalidate class="form-horizontal form-label-left" (ngSubmit)="onSubmit()" [formGroup]="signeesForm">
      <div formArrayName="signees" *ngFor="let item of signeesForm.get('signees').controls; let i = index;">
        <div [formGroupName]="i">
          
          <div class="col-sm-offset-3" style="padding-left: 10px;"><br>
            <h3 style="margin-bottom: 0;"><small class="green">Form</small> {{ signeesForm.controls.signees.controls[i].controls.typeFormDisplay.value.code }}</h3>
            <small>({{ signeesForm.controls.signees.controls[i].controls.typeFormDisplay.value.name }})</small>
          </div>
          <div class="form-group">
              <label for="signee" class="control-label col-md-3 col-sm-3 col-xs-12">Signee:</label>           
              <div class="col-md-9 col-sm-9 col-xs-12">
                  <input type="text" name="signee" formControlName="signee" id="signee" class="form-control col-xs-12" />
              </div>
          </div>
          <div class="form-group">
              <label for="title" class="control-label col-md-3 col-sm-3 col-xs-12">Signee Title:</label>           
              <div class="col-md-9 col-sm-9 col-xs-12">
                  <input type="text"  name="title" formControlName="title" id="title" class="form-control col-xs-12" />
              </div>
          </div>
        </div>
      </div>
      <div class="ln_solid"></div>
      <div class="form-group">
        <label for="invoiceEmail" class="control-label col-md-3 col-sm-3 col-xs-12">Invoice Email:</label>           
        <div class="col-md-9 col-sm-9 col-xs-12">
            <input type="text"  name="invoiceEmail" formControlName="invoiceEmail" id="invoiceEmail" class="form-control col-xs-12" />
        </div>
      </div>
      <div class="form-group">
        <label for="reportEmail" class="control-label col-md-3 col-sm-3 col-xs-12">Report Email:</label>           
        <div class="col-md-9 col-sm-9 col-xs-12">
            <input type="text"  name="reportEmail" formControlName="reportEmail" id="reportEmail" class="form-control col-xs-12" />
        </div>
      </div>
          <div class="ln_solid"></div>
          <div class="form-group">
              <div class="col-md-6 col-sm-6 col-xs-12 col-sm-offset-3">
                  <a class="btn btn-primary" (click)="onCancel()">Cancel</a>
                  <button type="submit" [disabled]="signeesForm.invalid"  class="btn btn-success">Sign & Submit</button>
              </div>
          </div>
      </form>
  </div>
  `,
  styles: []
})
export class SoildataSigneesComponent implements OnInit {
  signeesForm: FormGroup;
  signeesFormArray: FormArray;
  loading = true;

  signees:Observable<FormTypeSignees[]>;

  constructor(
    private reportingService: ReportingService,
    private service:SoildataService,
    private formBuilder: FormBuilder,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit() {
    this.signeesForm = this.formBuilder.group({
      signees: this.formBuilder.array([]),
      invoiceEmail: "",
      reportEmail: ""
    });
    this.service.countyInfo().subscribe(
      res => {
        this.signeesForm.patchValue({
          invoiceEmail:res.invoiceEmail, reportEmail:res.reportEmail
        })
      }
    )
    this.service.signeesByCounty().subscribe(
      res => {
        for( let item of res ) this.addItem(item);
        this.loading = false;
      }
    );
  }

  addItem(item:FormTypeSignees): void {
    this.signeesFormArray = this.signeesForm.get('signees') as FormArray;
    this.signeesFormArray.push(this.createItem(item));
  }

  createItem(item:FormTypeSignees): FormGroup {
    return this.formBuilder.group({
      title: item.title,
      signee: item.signee,
      typeFormId: item.typeForm.id,
      typeFormDisplay: item.typeForm,
      planningUnitId: item.planningUnit.id
    });
  }

  onSubmit(){
    var si = this.signeesForm.value;
    this.service.updateSignees(this.signeesForm.value).subscribe(
          _ => {
            this.reportingService.setAlert("Report Signees Updated");
            this.router.navigate(['../reports'], {relativeTo: this.route});
          }
      );
  }

  onCancel(){
    this.router.navigate(['../reports'], {relativeTo: this.route});
  }

}
