import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SnapEdRoutingModule } from './snap-ed-routing.module';
import { SharedModule } from '../../shared/shared.module';
import { CommitmentHomeComponent } from './commitment/commitment-home.component';
import { SnapEdCommitmentService } from './snap-ed-commitment.service';
import { SnapEdHomeComponent } from './snap-ed-home.component';
import { CommitmentFormComponent } from './commitment/commitment-form.component';
import { SnapedService } from '../servicelog/snaped.service';
import { CommitmentViewComponent } from './commitment/commitment-view.component';
import { SnapEdCommitmentComponent } from './commitment/snap-ed-commitment.component';

@NgModule({
  imports: [
    CommonModule,
    SnapEdRoutingModule,
    SharedModule
  ],
  declarations: [
    CommitmentHomeComponent,
    SnapEdHomeComponent,
    CommitmentFormComponent,
    CommitmentViewComponent,
    SnapEdCommitmentComponent
  ],
  providers: [
    SnapEdCommitmentService,
    SnapedService
  ],
  exports: [
    SnapEdHomeComponent,
    CommitmentHomeComponent,
    SnapEdCommitmentComponent
  ]
})
export class SnapEdModule { }
