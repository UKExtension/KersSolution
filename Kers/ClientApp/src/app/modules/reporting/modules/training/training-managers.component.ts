import { Component, OnInit } from '@angular/core';
import { UserService, User } from '../user/user.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'training-managers',
  templateUrl: './training-managers.component.html',
  styles: []
})
export class TrainingManagersComponent implements OnInit {

  trainiers:Observable<User[]>;

  constructor(
    private userService:UserService
  ) {
    
   }

  ngOnInit() {
    this.trainiers = this.userService.usersWithRole('SRVCTRNR');
  }

}
