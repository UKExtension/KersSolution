import { Component, Input } from '@angular/core';
import {Location} from '@angular/common';
import { UserService, User } from '../user.service';





@Component({
    selector: 'user-directory-profile',
    template: `<div class="col-md-4 col-sm-4 col-xs-12 profile_details" *ngIf="user">
            <div class="well profile_view">
                <div class="col-sm-12 top">
                    <h4 class="brief"><i *ngIf="user.personalProfile">{{user.personalProfile.professionalTitle}}</i>&nbsp;</h4>
                    <div class="left col-xs-7">
                        <h2 *ngIf="user.personalProfile">{{user.personalProfile.firstName}} {{user.personalProfile.lastName}}</h2>
                        <p><strong>{{user.extensionPosition.title}}</strong></p>
                        <ul class="list-unstyled">
                            <li *ngIf="user.rprtngProfile"><i class="fa fa-building"></i> {{user.rprtngProfile.planningUnit.name}}</li> 
                            <li *ngIf="user.personalProfile"><i class="fa fa-phone"></i> 
                                <span *ngIf="user.personalProfile.officePhone != ''">{{user.personalProfile.officePhone}}</span>
                                <span *ngIf="user.personalProfile.officePhone == null || user.personalProfile.officePhone == '' || !user.personalProfile">{{user.rprtngProfile.planningUnit.phone}}</span> 
                            </li>
                        </ul>
                    </div>
                    <div class="right col-xs-5 text-center">
                        <img src="{{profilePicSrc}}" alt="" class="img-circle img-responsive">
                    </div>
                </div>
                <div class="col-xs-12 bottom text-center">

                    <div class="col-lg-12 emphasis text-left">
                        <button type="button" class="btn btn-primary btn-xs" [routerLink]="['/reporting/user', user.id]">
                        <i class="fa fa-user"> </i> View Profile
                        </button>
                        <button *ngIf="showEmployeeSummaryButton" type="button" class="btn btn-success btn-xs" [routerLink]="['/reporting/user/summary', user.id]"> <i class="fa fa-cog">
                                </i> Employee Summary </button>
                        <button *ngIf="showSnapButton" type="button" class="btn btn-success btn-xs" [routerLink]="['/reporting/admin/snaped/user', user.id]"> <i class="fa fa-cog">
                                </i> Snap-Ed </button>
                    </div>
                </div>
            </div>
            </div>`,
    styles: [`
            .profile_view > div.top{
                min-height: 180px;
            }
            
            `]
})
export class UserDirectoryProfileComponent {
    
    @Input() user:User;
    @Input() showEmployeeSummaryButton:boolean = false;
    @Input() showSnapButton = false;
    
    public profilePicSrc;
    constructor(
        private location:Location
    ){
        this.profilePicSrc = location.prepareExternalUrl('/dist/assets/images/user.png');
    }

    ngOnInit(){
       if(this.user.personalProfile && this.user.personalProfile.uploadImage){
            this.profilePicSrc = this.location.prepareExternalUrl('/image/crop/250/250/' + this.user.personalProfile.uploadImage.uploadFile.name);
        }
    }

}




