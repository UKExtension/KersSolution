import { Component, Input } from '@angular/core';
import {Location} from '@angular/common';
import { UserService, User } from '../user.service';





@Component({
    selector: 'user-directory-profile',
    template: `<div class="col-md-4 col-sm-4 col-xs-12 profile_details" *ngIf="user">
            <div class="well profile_view">
                <div class="ribbon" *ngIf="!user.rprtngProfile.enabled"><span>Former Employee</span></div>
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


            .ribbon {
                position: absolute;
                right: 5px; top: -5px;
                z-index: 1;
                overflow: hidden;
                width: 75px; height: 75px;
                text-align: right;
              }
              .ribbon span {
                font-size: 7px;
                font-weight: bold;
                color: #FFF;
                text-transform: uppercase;
                text-align: center;
                line-height: 20px;
                transform: rotate(45deg);
                -webkit-transform: rotate(45deg);
                width: 100px;
                display: block;
                background: #79A70A;
                background: linear-gradient(#2989d8 0%, #1e5799 100%);
                box-shadow: 0 3px 10px -5px rgba(0, 0, 0, 1);
                position: absolute;
                top: 19px; right: -21px;
              }
              .ribbon span::before {
                content: "";
                position: absolute; left: 0px; top: 100%;
                z-index: -1;
                border-left: 3px solid #1e5799;
                border-right: 3px solid transparent;
                border-bottom: 3px solid transparent;
                border-top: 3px solid #1e5799;
              }
              .ribbon span::after {
                content: "";
                position: absolute; right: 0px; top: 100%;
                z-index: -1;
                border-left: 3px solid transparent;
                border-right: 3px solid #1e5799;
                border-bottom: 3px solid transparent;
                border-top: 3px solid #1e5799;
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
        this.profilePicSrc = location.prepareExternalUrl('/assets/images/user.png');
    }

    ngOnInit(){
       if(this.user.personalProfile && this.user.personalProfile.uploadImage){
            this.profilePicSrc = this.location.prepareExternalUrl('/image/crop/250/250/' + this.user.personalProfile.uploadImage.uploadFile.name);
        }
    }

}




