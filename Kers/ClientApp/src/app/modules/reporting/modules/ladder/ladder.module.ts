import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedModule } from '../../shared/shared.module';
import { LadderRoutingModule } from './ladder-routing.module';
import { LadderComponent } from './ladder.component';
import { LadderApplicationFormComponent } from './ladder-application-form.component';
import { AngularMyDatePickerModule } from 'angular-mydatepicker';
import { TrainingModule } from '../training/training.module';
import { LadderApplicantComponent } from './ladder-applicant.component';
import { LadderApplicationsListComponent } from './ladder-applications-list.component';
import { LadderApplicantListDetailComponent } from './ladder-applicant-list-detail.component';
import { LadderReviewComponent } from './ladder-review.component';
import { LadderApplicationDetailsComponent } from './ladder-application-details.component';
import { LadderDetailImageComponent } from './ladder-detail-image.component';
import { LadderReviewApplicationDetailComponent } from './ladder-review-application-detail.component';
import { LadderFilterComponent } from './ladder-filter.component';
import { LadderFilterDetailComponent } from './ladder-filter-detail.component';

@NgModule({
  declarations: [LadderComponent, LadderApplicationFormComponent, LadderApplicantComponent, LadderApplicationsListComponent, LadderApplicantListDetailComponent, LadderReviewComponent, LadderApplicationDetailsComponent, LadderDetailImageComponent, LadderReviewApplicationDetailComponent, LadderFilterComponent, LadderFilterDetailComponent],
  imports: [
    CommonModule,
    AngularMyDatePickerModule,
    SharedModule,
    LadderRoutingModule,
    TrainingModule
  ]
})
export class LadderModule { }
