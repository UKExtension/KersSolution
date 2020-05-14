import { Component, OnInit, Input } from '@angular/core';
import { ReportingService } from '../../components/reporting/reporting.service';
import { LadderService } from './ladder.service';
import { LadderApplication } from './ladder';
import { Router } from '@angular/router';

@Component({
  selector: 'ladder-applicant',
  templateUrl: './ladder-applicant.component.html',
  styles: []
})
export class LadderApplicantComponent implements OnInit {
  @Input() userId:number;

  application:LadderApplication;
  
  newApplication = false;
  loading = true;
  
  constructor(
    private reportingService: ReportingService,
    private service: LadderService,
    private router: Router
  ) {
    
  }

  ngOnInit() {
    this.defaultTitle();
    this.service.applicationByUserByFiscalYear().subscribe(
      res => {
        if( res == null ){
          this.newApplication = true;
        }else{
          this.application = res;
        }
        this.loading = false;
      }
    )
  }

  defaultTitle(){
    this.reportingService.setTitle("Professional Promotion Application");
    this.reportingService.setSubtitle("For Outstanding Job Performance and Experiences Gained Through Program Development");
  }

  canceled(){
    this.router.navigate(['']);
  }
  submitted(){
    this.router.navigate(['']);
  }

}
