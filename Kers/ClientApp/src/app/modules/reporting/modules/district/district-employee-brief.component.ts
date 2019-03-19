import { Component, OnInit, Input } from '@angular/core';
import { EmployeeNumActivities } from './district.service';
import {Location} from '@angular/common';

@Component({
  selector: 'district-employee-brief',
  templateUrl: './district-employee-brief.component.html',
  styles: []
})
export class DistrictEmployeeBriefComponent implements OnInit {
  @Input() data:EmployeeNumActivities;
  profilePicSrc:string;

  constructor(
    private location:Location
  ) {

    this.profilePicSrc = location.prepareExternalUrl('/assets/images/user.png');
   }

  ngOnInit() {
    if(this.data.user.personalProfile && this.data.user.personalProfile.uploadImage){
      this.profilePicSrc = this.location.prepareExternalUrl('/image/crop/250/250/' + this.data.user.personalProfile.uploadImage.name);
  }
  }

}
