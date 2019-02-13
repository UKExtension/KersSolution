import { Component, OnInit, Input } from '@angular/core';
import { SnapedAdminService } from './snaped-admin.service';
import { FiscalYear } from '../fiscalyear/fiscalyear.service';
import { saveAs } from 'file-saver';

@Component({
  selector: 'snaped-download-button',
  template: `<button class="btn btn-info btn-xs" (click)="csvDownload()" *ngIf="!loading">{{label}}</button><loading [type]="'bars'" *ngIf="loading"></loading>`,
  styles: []
})
export class SnapedDownloadButtonComponent implements OnInit {

  @Input() filename:string;
  @Input() label:string;
  @Input() location:string;
  @Input() fiscalYear:FiscalYear;




  loading = false;

  constructor(
    private service:SnapedAdminService
  ) { }

  ngOnInit() {
  }

  csvDownload(){
    this.loading = true;
    this.service.csv(this.location + "/" + this.fiscalYear.name ).subscribe(
        data => {
            if(data["size"] == undefined){
              var blob = new Blob(["An Error Occured"], {type: 'text/csv'});
              this.loading = false;
              saveAs(blob, this.filename + "_" + this.fiscalYear.name + ".csv");
            }else{
              var blob = new Blob([data], {type: 'text/csv'});
              this.loading = false;
              saveAs(blob, this.filename + "_" + this.fiscalYear.name + ".csv");
            }
        },
        err => console.error(err)
    )
  }

}
