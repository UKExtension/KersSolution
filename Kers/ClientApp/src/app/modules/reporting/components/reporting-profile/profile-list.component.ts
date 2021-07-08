import { Component, OnInit } from '@angular/core';
import { ProfileService, Profile } from './profile.service';
import { ReportingService } from '../reporting/reporting.service';
import { debounceTime, switchMap } from 'rxjs/operators';
import { Observable, pipe, Subject } from 'rxjs';

@Component({
    selector: 'profile-list',
    templateUrl: 'profile-list.component.html' 
})
export class ProfileListComponent implements OnInit{

    condition = false;
    profiles: Observable<Profile[]>;
    errorMessage: string;
    planningUnits = null;
    positions = null;
    numResults = 0;
    numProfiles = 0;
    criteria = {
        search: '',
        position: 0,
        unit: 0,
        amount: 40
    }
    private searchTermStream = new Subject<string>();

    constructor(    private profileService: ProfileService, 
                    private reportingService: ReportingService
                ){
                    this.profiles = this.searchTermStream
                        .pipe(
                                debounceTime(300),
                                switchMap((term:string) => {
                                    return this.performSearch(term);
                                }
                            )
                        );
                }
   
    ngOnInit(){

        this.profileService.planningUnits().subscribe(
                units => {
                    this.planningUnits = units;
                },
                error => this.errorMessage = <any> error
            );

        this.profileService.positions().subscribe(
                pos => {
                    this.positions = pos;
                },
                error => this.errorMessage = <any> error
            );



        this.reportingService.setTitle("Employees Management");
        
        
        
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
        return this.profileService.getCustom(this.criteria);
    }

    onPlaningUnitChange(event){
        this.criteria.amount = 40;
        this.criteria.unit = event.target.value;
        this.searchTermStream.next(this.criteria.search);
    }
    onPositionChange(event){
        this.criteria.amount = 40;
        this.criteria.position = event.target.value;
        this.searchTermStream.next(this.criteria.search);
    }
    updateNumResults(){
        this.profileService.getCustomCount(this.criteria).subscribe(
            num => {
                this.numResults = num;
                this.numProfiles = Math.min(this.numResults, this.criteria.amount);
                return this.numResults;
            },
            error => this.errorMessage = <any> error
        );
        
    }
    loadMore(){
        this.criteria.amount = this.criteria.amount + 20;
        this.searchTermStream.next(this.criteria.search);
    }

    
    onProfileUpdate(){
        this.searchTermStream.next(this.criteria.search);
    }

}