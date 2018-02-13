import { Component, OnInit, Input } from '@angular/core';
import { User } from '../../user/user.service';

@Component({
  selector: '[servicelog-snaped-stats-row]',
  templateUrl: './servicelog-snaped-stats-row.component.html',
  styles: []
})
export class ServicelogSnapedStatsRowComponent implements OnInit {

  @Input('servicelog-snaped-stats-row') stat;
  @Input() user:User;

  fullstats = false;
  direct = 0;
  indirect = 0;
  constructor() { }

  ngOnInit() {
    for(let rev of this.stat.revisions){
      var indrct = rev.activityOptionNumbers.filter( o => o.activityOptionNumber.name == 'Number of Indirect Contacts');
      if(indrct.length != 0){
        this.indirect += indrct[0].value;
      }
      if( (rev.male + rev.female)>0 && !rev.isAdmin && !rev.isPolicy){
        this.direct += rev.male + rev.female;
      }
    }
  }
  open(){
    this.fullstats = true;
  }
  close(){
    this.fullstats = false;
  }

}
