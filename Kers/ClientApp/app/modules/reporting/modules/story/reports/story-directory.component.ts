import { Component, Input } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';
import { ProfileService } from '../../../components/reporting-profile/profile.service';
import "rxjs/add/operator/debounceTime";
import "rxjs/add/operator/switchMap";
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import { Story, StoryService } from '../story.service';
import { UserService, PlanningUnit } from '../../user/user.service';
import { MajorProgram, ProgramsService, StrategicInitiative } from '../../admin/programs/programs.service';




@Component({
  templateUrl: 'story-directory.component.html'
})
export class StoryDirectoryComponent {


        stories: Observable<Story[]>;
        errorMessage: string;
        planningUnits: Observable<PlanningUnit[]>;
        initiatives:Observable<StrategicInitiative[]>;
        positions = null;
        condition = false;
        numResults = 0;
        numProfiles = 0;
        criteria = {
            search: '',
            unit: 0,
            program: 0,
            snap: 0,
            withImage: 0,
            amount: 10
        }
        private searchTermStream = new Subject<string>();

        loading = false;


        constructor(     
                    private reportingService: ReportingService,
                    private service:StoryService,
                    private userService: UserService,
                    private programsService: ProgramsService
                ){
                    this.stories = this.searchTermStream
                        .debounceTime(300)
                        .switchMap((term:string) => {
                            this.loading = true;
                            return this.performSearch(term);
                        });
                }
   
    ngOnInit(){


        this.planningUnits = this.userService.units();
        this.initiatives = this.programsService.listInitiatives();

        this.reportingService.setTitle("Success Stories Search");
        
        
        
    }

    ngAfterViewInit(){
        this.searchTermStream.next("");
    }

    

    onSearch(event){
         this.searchTermStream.next(event.target.value);
    }

    performSearch(term:string){
        this.criteria.search = term;
        this.updateNumResults();
        return this.service.getCustom(this.criteria);
    }

    onPlaningUnitChange(event){
        this.criteria.amount = 30;
        this.criteria.unit = event.target.value;
        this.searchTermStream.next(this.criteria.search);
    }
    onProgramChange(event){
        this.criteria.amount = 30;
        this.criteria.program = event.target.value;
        this.searchTermStream.next(this.criteria.search);
    }
    onSnapChange(event){
        this.criteria.amount = 30;
        if(event.target.checked){
            this.criteria.snap = 1;
        }else{
            this.criteria.snap = 0;
        }
        this.searchTermStream.next(this.criteria.search);
    }
    onWithImageChange(event){
        this.criteria.amount = 30;
        if(event.target.checked){
            this.criteria.withImage = 1;
        }else{
            this.criteria.withImage = 0;
        }
        this.searchTermStream.next(this.criteria.search);
    }

    updateNumResults(){
        this.service.getCustomCount(this.criteria).subscribe(
            num => {
                this.numResults = num;
                this.numProfiles = Math.min(this.numResults, this.criteria.amount);
                this.loading = false;
                return this.numResults;
            },
            error => this.errorMessage = <any> error
        );
        
    }
    loadMore(){
        this.criteria.amount = this.criteria.amount + 15;
        this.searchTermStream.next(this.criteria.search);
    }


}