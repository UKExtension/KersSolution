import { Component, OnInit, Input } from '@angular/core';
import { LadderApplication } from './ladder';
import { LadderService } from './ladder.service';
import { TrainingService } from '../training/training.service';
import { Observable } from 'rxjs';
import { FiscalyearService } from '../admin/fiscalyear/fiscalyear.service';
import { saveAs } from 'file-saver';

@Component({
  selector: 'ladder-application-details',
  templateUrl: './ladder-application-details.component.html',
  styles: []
})
export class LadderApplicationDetailsComponent implements OnInit {
  @Input() application:LadderApplication;
  @Input() applicationId:number;

  loading = true;
  firstOfTheYear:Date;
  lastPromotionDate:Date;
  hoursAttended:Observable<number>;
  pdfLoading = false;

  constructor(
    private service:LadderService,
    private fiscalYearService:FiscalyearService,
    private trainingService:TrainingService
  ) { 
  }

  ngOnInit() {
    if(this.application != null){
      this.loading = false;
      this.lastPromotionDate = new Date(this.application.lastPromotion);
      this.fiscalYearService.forDate(new Date(this.application.created)).subscribe(
        res => {
          var end = new Date(res.end);
          this.firstOfTheYear = new Date(end.getFullYear(), 0, 1);
          this.hoursAttended = this.trainingService.hoursByUser(this.application.kersUserId,this.lastPromotionDate, this.firstOfTheYear);
        }
      );
    }else if( this.applicationId != null){
      this.service.getApplication( this.applicationId).subscribe(
        res => {
          this.application = res;
          this.loading = false;
          this.lastPromotionDate = new Date(this.application.lastPromotion);
          this.fiscalYearService.forDate(new Date(this.application.created)).subscribe(
            res => {
              var end = new Date(res.end);
              this.firstOfTheYear = new Date(end.getFullYear(), 0, 1);
              this.hoursAttended = this.trainingService.hoursByUser(this.application.kersUserId,this.lastPromotionDate, this.firstOfTheYear);
            }
          );
        }
      )
    }
  }

  print(){
    this.pdfLoading = true;
    this.service.pdf(this.application.id).subscribe(
      data => {
          var blob = new Blob([data], {type: 'application/pdf'});
          saveAs(blob, "LadderApplication_" + this.application.kersUser.rprtngProfile.name + "_" + this.application.created + ".pdf");
          this.pdfLoading = false;
      },
      err => console.error(err)
  )
  }

}
