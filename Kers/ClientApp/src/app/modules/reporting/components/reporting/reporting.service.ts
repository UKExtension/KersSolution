import {Injectable} from "@angular/core";
import { Title }     from '@angular/platform-browser';

@Injectable()
export class ReportingService{
    public title = { name: "Reporting"};
    public subtitle = {name: ""};
    public alert = { name: ""};
    public statsHtml = {name:''};

    constructor( 
        private titleService: Title 
    )   
    {}

    getTitle():string{
        return this.title.name;
    }

    getSubtitle():string{
        return this.subtitle.name;
    }
    
    setTitle(title:string){
        setTimeout(()=>{
            this.title.name = title;
        },0);
        this.titleService.setTitle( title + ' - Kentucky Extension Reporting System' );
    }

    setDefaultTitle(){
        this.title.name = 'Kentucky Extension Reporting System';
        this.subtitle.name = '';
        this.titleService.setTitle( 'Kentucky Extension Reporting System' );
    }

    setSubtitle(subtitle:string){
        this.subtitle.name = subtitle;
    }

    setAlert(alrt){
        this.alert.name = alrt;
        setTimeout(() => { 
            this.alert.name = ""; 
        }, 3000);
    }

    addStats(st:string){
        this.statsHtml.name = st;
    }
    hideStats(){
        this.statsHtml.name = '';
    }
    stats(){
        return this.statsHtml;
    }

}