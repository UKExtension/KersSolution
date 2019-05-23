import { Component, OnInit, Input } from '@angular/core';
import { TrainingService } from './training.service';
import { UserService, User } from '../user/user.service';
import { Observable } from 'rxjs';
import { Training } from './training';

@Component({
  selector: 'training-upcomming',
  templateUrl: './training-upcomming.component.html',
  styles: []
})
export class TrainingUpcommingComponent implements OnInit {
  @Input() user:User;
  trainings:Observable<Training[]>;
  constructor(
    private service:TrainingService,
    private userService:UserService
  ) { }

  ngOnInit() {
    if( this.user == null){
      this.userService.current().subscribe(
        res =>{
          this.user = res;
          this.loadData();
        } 
      );
    }else{
      this.loadData();
    }
  }
  loadData(){
      this.trainings = this.service.upcommingByUser(this.user.id);
  }
}
