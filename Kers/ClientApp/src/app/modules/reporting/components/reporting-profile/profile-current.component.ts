import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import {ProfileService, Profile} from './profile.service';

@Component({
    selector: 'current-user',
    template: `
    <h2>Current User</h2>
    <div class="row">
      @if (profile) {
        <div class="col-lg-6 col-md-12">
          {{profile.personName}}
        </div>
      }
    
    </div>
    `,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class ProfileCurrentComponent implements OnInit {
    profile:Profile;
    errorMessage: string;

    constructor( private profileService : ProfileService ){}

    ngOnInit(){
        this.profileService.currentUser().subscribe(
                profile => this.profile = profile,
                error => this.errorMessage = <any> error
            );
            
    }
 }