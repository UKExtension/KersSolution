import { Component, OnInit, Input } from '@angular/core';
import { LadderApplication } from './ladder';
import { LadderService } from './ladder.service';
import { TrainingService } from '../training/training.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'ladder-application-details',
  templateUrl: './ladder-application-details.component.html',
  styles: []
})
export class LadderApplicationDetailsComponent implements OnInit {
  @Input() application:LadderApplication;
  @Input() applicationId:number;

  loading = true;
  today:Date;
  firstOfTheYear:Date;
  lastPromotionDate:Date;
  hoursAttended:Observable<number>;

  constructor(
    private service:LadderService,
    private trainingService:TrainingService
  ) { 
    this.today = new Date();
    this.firstOfTheYear = new Date(this.today.getFullYear(), 0, 1)
  }

  ngOnInit() {
    if(this.application != null){
      this.loading = false;
      this.lastPromotionDate = new Date(this.application.lastPromotion);
      this.hoursAttended = this.trainingService.hoursByUser(this.application.kersUserId,this.lastPromotionDate, this.firstOfTheYear);
    }else if( this.applicationId != null){
      this.service.getApplication( this.applicationId).subscribe(
        res => {
          this.application = res;
          this.loading = false;
          this.lastPromotionDate = new Date(this.application.lastPromotion);
          this.hoursAttended = this.trainingService.hoursByUser(this.application.kersUserId,this.lastPromotionDate, this.firstOfTheYear);
        }
      )
    }
  }

}
