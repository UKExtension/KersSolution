import { Component } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';
import { ActivatedRoute, Router, Params } from "@angular/router";
import {StoryService, Story} from '../story.service';
import { switchMap } from 'rxjs/operators';

@Component({
  template: `
    <success-story-full [story]="story" *ngIf="story"></success-story-full>
    
  `
})
export class StoryReportsDetailComponent { 

    story: Story;
    errorMessage: string;

    constructor( 
        private reportingService: ReportingService,
        private route: ActivatedRoute,
        private router: Router, 
        private service: StoryService
    )   
    {}

    ngOnInit(){ 
        this.route.params.pipe(
            switchMap((params: Params) => this.service.byId(params['id']))
                ).subscribe((story: Story) => 
                {
                    this.story = story;
                },
                err => this.errorMessage = <any> err);

        this.defaultTitle();
    }

    defaultTitle(){
        this.reportingService.setTitle("Success Story");
    }
}