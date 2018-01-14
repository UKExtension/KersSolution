import {    Component, Input, OnInit, EventEmitter, Output   } from '@angular/core';
import { ProfileService, Profile } from './profile.service';

@Component({
    selector: '[profileListDetail]',
    templateUrl: 'profile-list-detail.component.html'
})
export class ProfileListDetailComponent implements OnInit {

    @Input ('profileListDetail') profile: Profile;

    @Output() onProfileUpdated = new EventEmitter();

    viewType = 'row'

    constructor(private profileService: ProfileService){
        
    }

    ngOnInit(){
        
    }
    view(){
        this.viewType = "detail";
    }
    profileUpdated(){
        this.onProfileUpdated.emit();
        //console.log(this.profile)
        this.viewType = 'row';
    }
}
