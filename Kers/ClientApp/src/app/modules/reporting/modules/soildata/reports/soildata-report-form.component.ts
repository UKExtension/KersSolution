import { Component, OnInit, Input } from '@angular/core';
import { SoilReportBundle } from '../soildata.report';
import { FarmerAddress, SoildataService } from '../soildata.service';

@Component({
  selector: 'soildata-report-form',
  templateUrl: './soildata-report-form.component.html',
  styles: []
})
export class SoildataReportFormComponent implements OnInit {
  @Input() report: SoilReportBundle;
  addressBrowserOpen = false;
  loading = false;
  
  constructor(
    private service:SoildataService
  ) { }

  ngOnInit() {
    console.log(this.report);
  }

  addressSelected(event:FarmerAddress){
    this.loading = true;
    this.service.updateBundleFarmer(this.report.id, event).subscribe(
      res => {
        this.report.farmerForReport = res.farmerForReport;
        this.addressBrowserOpen = false;
        this.loading = false;
      }
    )
    console.log(event);
    
  }
  addressSelectionCanceled(){
    this.addressBrowserOpen = false;
  }

}
