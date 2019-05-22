import { Component, OnInit, Input } from '@angular/core';
import { User, UserService } from '../user/user.service';
import { TrainingService } from './training.service';
import { Observable } from 'rxjs';
import { Training } from './training';

@Component({
  selector: 'training-transcript',
  templateUrl: './training-transcript.component.html',
  styles: []
})
export class TrainingTranscriptComponent implements OnInit {

  @Input() user:User;
  @Input() year:number = 0;


  trainings:Observable<Training[]>;

  constructor(
    private service:TrainingService,
    private userService:UserService
  ) { }

  ngOnInit() {
    if( this.user == null){
      this.trainings = this.service.enrolledByUser(0,this.year);
      this.userService.current().subscribe(
        res => this.user = res
      );
    }else{
      this.trainings = this.service.enrolledByUser(this.user.id,this.year);
    }
  }
  attendance(training:Training){
    var attended = "NO"
    var enrollment = training.enrollment.filter( e => e.attendie);
    if( enrollment.length != 0){
      if( enrollment[0].attended == true ) attended = "YES";
    }
    return attended;
  }

}
