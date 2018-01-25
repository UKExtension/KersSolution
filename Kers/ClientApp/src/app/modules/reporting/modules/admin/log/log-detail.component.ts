import { Component, OnInit, Input } from '@angular/core';
import {Location} from '@angular/common';
import { Log, LogService } from './log.service';



@Component({
    selector: '[log-detail]',
    templateUrl: 'log-detail.component.html'
})
export class LogDetailComponent implements OnInit { 
    
    @Input('log-detail')log:Log;
    rowDefault =true;
    rowDetail = false;
    profilePicSrc:string;
    logTime:Date;
    JSON;

    constructor( 
        private service:LogService,
        private location:Location
    )   
    {
        this.profilePicSrc = location.prepareExternalUrl('/dist/assets/images/user.png');
        this.JSON = JSON;
    }

    ngOnInit(){
        if(this.log.user.personalProfile.uploadImage){
            this.profilePicSrc = this.location.prepareExternalUrl('/image/crop/60/60/' + this.log.user.personalProfile.uploadImage.uploadFile.name);
        }
        var a = this.log.time.toString().split(/[^0-9]/);
        this.logTime = new Date (+a[0],+a[1]-1,+a[2],+a[3],+a[4],+a[5] );
    }

    default(){
        this.rowDefault =true;
        this.rowDetail = false;
    }
    details(){
        this.rowDefault =false;
        this.rowDetail = true;
    }



}