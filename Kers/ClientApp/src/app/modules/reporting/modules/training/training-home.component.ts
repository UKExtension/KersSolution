import { Component, OnInit } from '@angular/core';
import {TrainingService} from './training.service';
import { Training } from './training';
import { Observable } from 'rxjs';
import { ReportingService } from '../../components/reporting/reporting.service';
import { UserService, User } from '../user/user.service';

@Component({
  selector: 'training-home',
  templateUrl: './training-home.component.html',
  styleUrls: ['./training-home.component.css']
})
export class TrainingHomeComponent implements OnInit {

  currentUser:User;
  constructor(
    private reportingService: ReportingService,
    private userService:UserService
  ) { }

  ngOnInit() {
    this.defaultTitle();
    this.userService.current().subscribe(
      res =>{
        this.currentUser = res;
      } 
    ) 
  }

  isTrainer():boolean{
    var filteredRoles = this.currentUser.roles.filter( r => r.zEmpRoleType.shortTitle == "SRVCTRNR");
    if(filteredRoles.length > 0 ) return true;
    return false;
  }
  isAdmin():boolean{
    var filteredRoles = this.currentUser.roles.filter( r => r.zEmpRoleType.shortTitle == "SRVCADM");
    if(filteredRoles.length > 0 ) return true;
    return false;
  }

  defaultTitle(){
    this.reportingService.setTitle("In-Service Training");
    //this.reportingService.setSubtitle("For specific In-Service related questions or assistance, please email: agpsd@lsv.uky.edu");
  }
  ngOnDestroy(){
    this.reportingService.setTitle("Kentucky Extension Reporting System");
    this.reportingService.setSubtitle("");
  }
}