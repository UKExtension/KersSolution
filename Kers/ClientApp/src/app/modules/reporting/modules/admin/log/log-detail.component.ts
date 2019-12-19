import { Component, OnInit, Input } from '@angular/core';
import {Location} from '@angular/common';
import { Log } from './log.service';
import * as ua_parser from "ua-parser-js"


@Component({
    selector: '[log-detail]',
    templateUrl: 'log-detail.component.html'
})
export class LogDetailComponent implements OnInit { 
    
    @Input('log-detail')log:Log;
    rowDefault =true;
    rowDetail = false;
    rowProgress = false;
    profilePicSrc:string;
    logTime:Date;
    JSON;
    

    constructor( 
        private location:Location
    )   
    {
        this.profilePicSrc = location.prepareExternalUrl('/assets/images/user.png');
        this.JSON = JSON;
        
    }

    ngOnInit(){
        if(this.log.user && this.log.user.personalProfile.uploadImage){
            this.profilePicSrc = this.location.prepareExternalUrl('/image/crop/60/60/' + this.log.user.personalProfile.uploadImage.uploadFile.name);
        }
        var a = this.log.time.toString().split(/[^0-9]/);
        this.logTime = new Date (+a[0],+a[1]-1,+a[2],+a[3],+a[4],+a[5] );
    }

    default(){
        this.rowDefault =true;
        this.rowDetail = false;
        this.rowProgress = false;
    }
    details(){
        this.rowDefault =false;
        this.rowDetail = true;
    }
    progress(){
        this.rowDefault =false;
        this.rowProgress = true;
    }

    browser():string{
        var parser = new ua_parser();
        parser.setUA(this.log.agent)
        var result = parser.getResult();
        return result.browser.name + ' (' + result.browser.version + ')'+', '+ result.os.name + ' (' + result.os.version + ')'
    }



}