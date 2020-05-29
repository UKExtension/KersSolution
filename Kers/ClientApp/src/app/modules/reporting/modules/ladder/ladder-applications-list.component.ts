import { Component, OnInit, Input } from '@angular/core';
import { LadderApplication } from './ladder';
import { Observable } from 'rxjs';
import { LadderService } from './ladder.service';

@Component({
  selector: 'ladder-applications-list',
  templateUrl: './ladder-applications-list.component.html',
  styles: []
})
export class LadderApplicationsListComponent implements OnInit {
  @Input() userId:number = 0;

  applications:Observable<LadderApplication[]>;


  constructor(
    private service:LadderService
  ) { }

  ngOnInit() {
    this.applications = this.service.applicationsByUser(this.userId);
  }

}
