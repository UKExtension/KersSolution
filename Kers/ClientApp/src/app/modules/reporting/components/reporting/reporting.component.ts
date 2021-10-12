import { Component, OnInit } from '@angular/core';
import {Location} from '@angular/common';
import {NavigationService, NavSection} from '../reporting-navigation/navigation.service';
import {ProfileService, Profile} from '../reporting-profile/profile.service';
import {ReportingService} from './reporting.service';
import {GoogleAnalyticsEventsService} from "../../core/google-analytics-events.service";
import {Router, NavigationEnd} from "@angular/router";
declare let ga:Function;

if (typeof window != 'undefined') {
    //require("font-awesome-webpack");
  //require('style-loader!css-loader!font-awesome/css/font-awesome.css?');
}



@Component({
  templateUrl: './reporting.component.html'
})
export class ReportingComponent implements OnInit { 
    public navigation;
    public ukLogoSrc:string;
    public profilePicSrc:string;
    profile:Profile;
    errorMessage: string;
    title: string;
    layoutClasses = {};

    constructor( 
                private navService: NavigationService, 
                private profileService : ProfileService,
                private reportingService: ReportingService,
                private location:Location,
                public router: Router,
                public googleAnalyticsEventsService: GoogleAnalyticsEventsService
                ) 
    {
        //navService.getNavigation().then( navigation => this.navigation = navigation );
        
        this.title = this.reportingService.getTitle();
        this.ukLogoSrc = location.prepareExternalUrl('/assets/images/UK.svg');
        this.profilePicSrc = location.prepareExternalUrl('/assets/images/user.png');
        this.router.events.subscribe(event => {
        if (event instanceof NavigationEnd) {
            ga('set', 'page', event.urlAfterRedirects);
            ga('send', 'pageview');
        }
        }); 
        this.layoutClasses['nav-md'] = true;
        this.layoutClasses['nav-sm'] = false;
    }

    ngOnInit(){
        this.navService.nav().subscribe(
            res => {
                    
                    this.navigation =  res;
                    
                },
            error =>  this.errorMessage = <any>error
        );
        /*
        this.profileService.currentUser().subscribe(
            profile => this.profile = profile,
            error => this.errorMessage = <any> error
        );
        */
    }

    
    
    onLeftToggle(){
        if(this.layoutClasses['nav-md']){
            this.layoutClasses['nav-md'] = false;
            this.layoutClasses['nav-sm'] = true;
            this.closeGroups();
        }else{
            this.layoutClasses['nav-md'] = true;
            this.layoutClasses['nav-sm'] = false;
        }
    }

    closeGroups(){
        for(var section of this.navigation){
            this.closeInSection(section);
        } 
    }
    closeInSection(section:NavSection){
        for(var key in section.groups){ 
                section.groups[key].isOpen = 'inactive';
        }
    }
    itemSelected(event){
        if(this.layoutClasses['nav-sm']){
            this.closeGroups();
        }
    }
}