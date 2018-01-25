import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { ProfileService, Profile } from './profile.service';
import { ReportingService } from '../reporting/reporting.service';
import { FormBuilder, Validators }   from '@angular/forms';
import {Router} from '@angular/router';

@Component({
    selector: 'reporting-profile-edit',
    templateUrl: 'reporting-profile-edit.component.html' 
})
export class ReportingProfileEditComponent implements OnInit{

    profile: Profile;
    errorMessage: string;
    editForm = null;
    planningUnits = null;
    positions = null;
    locations = null;

    @Input ('profile') rptProfile: Profile = null;
    @Input () admin: boolean = false;

    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<void>();

    constructor( 
        private profileService: ProfileService,
        private reportingService: ReportingService,
        private fb: FormBuilder,
        private router: Router
    ){

        this.editForm = fb.group(
            {
              rDT: [''],
              enabled: [''],
              sapLeaveRequestPilotGroup: [''],
              extensionIntern: [''],
              instID: [''],
              planningUnitID:['', Validators.required],
              planningUnitName: ['', Validators.required],
              positionID: ['', Validators.required],
              positionID_BAK: [''],
              linkBlueID: ['', Validators.required],
              personID: ['', Validators.required],
              personName: ['', Validators.required], 
              progANR: [''],
              progHORT: [''],
              progFCS: [''],
              prog4HYD: [''],
              progFACLD: [''],
              progNEP: [''],
              progOther: [''], 
              locationID: [''],
              isDD: [''],
              isCesInServiceTrainer: [''],
              isCesInServiceAdmin: [''],
              emailDeliveryAddress: ['', Validators.required],
              emailUEA: ['']
            })


    }
   
    ngOnInit(){
        this.reportingService.setTitle("Edit Reporting Profile");
        
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

        this.profileService.locations().subscribe(
                loc => {
                    this.locations = loc;
                },
                error => this.errorMessage = <any> error
            );

            if(this.rptProfile == null){
                this.profileService.currentUser().subscribe(
                        profile => {
                            this.profile = profile;
                            this.editForm.patchValue(this.profile);
                        },
                        error => this.errorMessage = <any> error
                    );
            }else{
                this.profile = this.rptProfile;
                this.editForm.patchValue(this.profile);
            }

        }

        onSubmit(){            
            if(this.admin){
                this.profileService.update(this.profile.id, this.editForm.value, true).subscribe(
                    response => {
                        this.profile = response;
                        this.rptProfile = response;
                        this.onFormSubmit.emit();
                    }
                );
                
            }else{
                    this.profileService.update(this.profile.id, this.editForm.value).subscribe(
                    response => {
                        this.profile = <Profile> response.json();
                    }
                );
                this.reportingService.setAlert("Profile Updated");
                this.router.navigate(['/reporting']);
            }
        }

        OnCancel(){
            if(this.admin){
                this.onFormCancel.emit();
            }else{
                this.router.navigate(['/reporting']);
            }
        }   
}