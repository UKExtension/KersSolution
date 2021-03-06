import { Component, Input, Output, EventEmitter } from '@angular/core';
import { PlanningUnit, User } from '../../user/user.service';
import { SnapedService } from '../snaped.service';
import { FiscalYear } from '../../admin/fiscalyear/fiscalyear.service';


@Component({
  selector: 'snape-ed-stats',
  templateUrl: 'servicelog-snaped-stats.component.html'
})
export class ServicelogSnapedStatsComponent { 
    @Input() planningUnit: PlanningUnit;
    @Input() user:User | null = null;
    @Input() fiscalYear:FiscalYear;

    @Output() onCalculated = new EventEmitter<number>();

    stats;
    loading = false;
    directPerMounth = [];
    indirectPerMounth = [];
    directTotal = 0;
    indirectTotal = 0;
    hoursTotal = 0;

    errorMessage:string;

    constructor( 
      private service:SnapedService,
    )   
    {}

    ngOnInit(){
      this.loading = true;
      if(this.user != null){
          this.service.statsPerIndividual(this.user.id, this.fiscalYear.name).subscribe(
            res => {
              this.stats = res;
              this.processStats();
              this.loading = false;
            },
            err => this.errorMessage = <any>err
          );
      }else if(this.planningUnit != null){
        this.service.statsPerCounty(this.planningUnit.id).subscribe(
          res => {
            this.stats = res;
            this.processStats();
            this.loading = false;
          },
          err => this.errorMessage = <any>err
        );
      }
    }

    processStats(){
      for(let stat of this.stats){
        var direct = 0;
        var indirect = 0;
        for(let rev of stat.revisions){
          var indrct = rev.activityOptionNumbers.filter( o => o.activityOptionNumber.name == 'Number of Indirect Contacts');
          if(indrct.length != 0){
            indirect += indrct[0].value;
          }
          if( (rev.male + rev.female)>0 && !rev.isAdmin && !rev.isPolicy){
            direct += rev.male + rev.female;
          }
        }
        this.indirectPerMounth.push(indirect);
        this.indirectTotal += indirect;
        this.directPerMounth.push(direct);
        this.directTotal += direct;
        this.hoursTotal += stat.hours;
      }
      this.onCalculated.emit(this.hoursTotal);
    }

}