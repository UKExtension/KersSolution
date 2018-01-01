import { Injectable} from '@angular/core';
import {Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import {Location} from '@angular/common';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {AuthHttp} from '../../../../authentication/auth.http';
import {Profile} from '../../../components/reporting-profile/profile.service';
import {Role} from '../roles/roles.service';
import { NavigationService, NavSection, NavGroup, NavItem } from '../../../components/reporting-navigation/navigation.service';

@Injectable()
export class AdminNavigationService {

    private baseUrl = '/api/nav/';

    private navSections:NavSection[];

    constructor( 
        private http:AuthHttp, 
        private location:Location,
        private navigationService: NavigationService
        )
    {

    }

    nav(){
        //if(this.navSections == null){
            var url = this.baseUrl + 'all/';
            return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => {
                    var navSections = <NavSection[]>res.json();
                    this.navSections = navSections;
                    return navSections;
                })
                .catch(this.handleError);
    /*    
    }else{
            return Observable.of(this.navSections);
        }
        */
    }

    // Navigation SECTION crud
    addSection(section:NavSection){
        var url = this.baseUrl + 'section/';
        return this.http.post(this.location.prepareExternalUrl(url), JSON.stringify(section), this.getRequestOptions())
                    .map( res => {
                        this.navSections.push(<NavSection> res.json());
                        return res.json();
                    })
                    .catch(this.handleError);
                    
    }
    updateSection(id:number, section:NavSection){
        console.log(section);
        var url = this.baseUrl + 'section/' + id;
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(section), this.getRequestOptions())
                    .map( res => {
                        this.navSections = null;
                        return res.json();
                    })
                    .catch(this.handleError);
    }

    deleteSection(section:NavSection){
        var url = this.baseUrl + 'section/' + section.id;
        return this.http.delete(this.location.prepareExternalUrl(url), this.getRequestOptions())
                    .map( res => {
                        return res;
                    })
                    .catch(this.handleError);
    }

    // Navigation GROUP crud

    addGroup(group:NavGroup, section:NavSection){
        var url = this.baseUrl + 'group/'+section.id;
        return this.http.post(this.location.prepareExternalUrl(url), JSON.stringify(group), this.getRequestOptions())
                    .map( res => {
                        return res.json();
                    })
                    .catch(this.handleError);
                    
    }
    updateGroup(id:number, group:NavGroup){
        var url = this.baseUrl + 'group/' + id;
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(group), this.getRequestOptions())
                    .map( res => {
                        return res.json();
                    })
                    .catch(this.handleError);
    }

    deleteGroup(group:NavGroup){
        var url = this.baseUrl + 'group/' + group.id;
        return this.http.delete(this.location.prepareExternalUrl(url), this.getRequestOptions())
                    .map( res => {
                        return res;
                    })
                    .catch(this.handleError);
    }


    // Navigation ITEM crud

    addItem(item:NavItem, group:NavGroup){
        var url = this.baseUrl + 'item/'+group.id;
        return this.http.post(this.location.prepareExternalUrl(url), JSON.stringify(item), this.getRequestOptions())
                    .map( res => {
                        return res.json();
                    })
                    .catch(this.handleError);
                    
    }
    updateItem(id:number, item:NavItem){
        var url = this.baseUrl + 'item/' + id;
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(item), this.getRequestOptions())
                    .map( res => {
                        return res.json();
                    })
                    .catch(this.handleError);
    }

    deleteItem(item:NavItem){
        var url = this.baseUrl + 'item/' + item.id;
        return this.http.delete(this.location.prepareExternalUrl(url), this.getRequestOptions())
                    .map( res => {
                        return res;
                    })
                    .catch(this.handleError);
    }




    handleError(err:Response){
        console.error(err);
        return Observable.throw(err.json().error || 'Server error');
    }

    getRequestOptions(){
        return new RequestOptions(
            {
                headers: new Headers({
                    "Content-Type": "application/json; charset=utf-8"
                })
            }
        )
    }
}