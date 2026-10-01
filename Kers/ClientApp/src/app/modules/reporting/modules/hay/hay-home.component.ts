import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { User, UserService } from '../user/user.service';
import { PlanningUnit } from '../plansofwork/plansofwork.service';
import { CountyCode, SoildataService } from '../soildata/soildata.service';

@Component({
    selector: 'hay-home',
    template: `
  @if (countyCode) {
    <div>
      <hay-sample-form [countyCode]="countyCode"></hay-sample-form>
    </div>
  }
  `,
    styles: [],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class HayHomeComponent implements OnInit {

  currentUser!: User;
  countyCode!: CountyCode;

  constructor(
    private userService: UserService,
    private soilService:SoildataService
  ) { }

  ngOnInit(): void {
    this.userService.current().subscribe(
      res => {
        this.currentUser = res;
        this.soilService.countyInfo(this.currentUser.rprtngProfile.planningUnitId).subscribe(
          res => this.countyCode = res
        )
      }
    )
  }

}
