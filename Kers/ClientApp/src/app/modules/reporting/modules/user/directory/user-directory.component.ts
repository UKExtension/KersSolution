import { Component, Input } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';
import { ProfileService } from '../../../components/reporting-profile/profile.service';
import { UserService, User } from '../user.service';
import "rxjs/add/operator/debounceTime";
import "rxjs/add/operator/switchMap";
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';




@Component({
  templateUrl: 'user-directory.component.html'
})
export class UserDirectoryComponent {


        users: Observable<User[]>;
        errorMessage: string;
        planningUnits = null;
        positions = null;
        numResults = 0;
        numProfiles = 0;
        criteria = {
            search: '',
            position: 0,
            unit: 0,
            amount: 30
        }
        condition = false;
        private searchTermStream = new Subject<string>();




        constructor(    
                    private service: UserService, 
                    private reportingService: ReportingService
                ){
                    this.users = this.searchTermStream
                        .debounceTime(300)
                        .switchMap((term:string) => {
                            return this.performSearch(term);
                        });
                }
   
    ngOnInit(){


        this.planningUnits = this.service.units();
        this.positions = this.service.extensionPositions();


        this.reportingService.setTitle("Kentucky Extension Directory");
        
        
        
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
    onPositionChange(event){
        this.criteria.amount = 30;
        this.criteria.position = event.target.value;
        this.searchTermStream.next(this.criteria.search);
    }
    updateNumResults(){
        this.service.getCustomCount(this.criteria).subscribe(
            num => {
                this.numResults = num;
                this.numProfiles = Math.min(this.numResults, this.criteria.amount);
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