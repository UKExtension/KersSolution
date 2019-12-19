import { Component, OnInit, Input } from '@angular/core';
import { trainingValidator } from './training-form.component';
import { Training, TrainingEnrollment } from './training';
import { TrainingService } from './training.service';

@Component({
  selector: '[training-post-attendance-detail]',
  templateUrl: './training-post-attendance-detail.component.html',
  styles: []
})
export class TrainingPostAttendanceDetailComponent implements OnInit {

  @Input('training-post-attendance-detail') training:Training;

  default = true;
  post = false;
  loading = false;
  enrolledFolks: TrainingEnrollment[];
  moreInfo = false;
  coppied = false;

  constructor(
    private service:TrainingService
  ) { }

  ngOnInit() {
    this.enrolledFolks = this.training.enrollment.filter( e => e.eStatus == "E").sort((a, b) => a.attendie.rprtngProfile.name.localeCompare(b.attendie.rprtngProfile.name));
  }

  defaultView(){
    this.default = true;
    this.post = false;
  }
  postView(){
    this.default = false;
    this.post = true;
  }
  checked(event:any, enrolled:TrainingEnrollment){
    if(event.currentTarget.checked){
      enrolled.attended = true;
    }else{
      enrolled.attended = false;
    }
  }
  public notify(payload: string) {
    // Might want to notify the user that something has been pushed to the clipboard
    //console.info(`'${payload}' has been copied to clipboard`);
    this.coppied = true;
  }
  submit(){
    this.loading = true;
    this.service.updateAttendance(this.training.id, this.training).subscribe(
      res => {
        this.loading = false;
        this.training = res;
        this.defaultView();
      }
    );
  }
}
