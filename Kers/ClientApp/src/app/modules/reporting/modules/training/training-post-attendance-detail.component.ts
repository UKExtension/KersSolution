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

  constructor(
    private service:TrainingService
  ) { }

  ngOnInit() {
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
      enrolled.eStatus = "A";
    }else{
      enrolled.eStatus = "E";
    }
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
